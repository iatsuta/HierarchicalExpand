using CommonFramework;
using CommonFramework.GenericRepository;

using Microsoft.Extensions.DependencyInjection;

namespace HierarchicalExpand.Denormalization;

public class DeepLevelDenormalizer(IServiceProvider serviceProvider, IEnumerable<DeepLevelInfo> deepLevelInfoList)
    : IDeepLevelDenormalizer
{
    public async Task Initialize(CancellationToken cancellationToken)
    {
        foreach (var deepLevelInfo in deepLevelInfoList)
        {
            var initializer =
                (IDeepLevelDenormalizer)serviceProvider.GetRequiredService(
                    typeof(IDeepLevelDenormalizer<>).MakeGenericType(deepLevelInfo.DomainObjectType));

            await initializer.Initialize(cancellationToken);
        }
    }
}

public class DeepLevelDenormalizer<TDomainObject>(
    IQueryableSource queryableSource,
    IGenericRepository genericRepository,
    IDomainObjectExpanderFactory<TDomainObject> domainObjectExpanderFactory,
    HierarchicalInfo<TDomainObject> hierarchicalInfo,
    DeepLevelInfo<TDomainObject> deepLevelInfo) : IDeepLevelDenormalizer<TDomainObject>
    where TDomainObject : class
{
    public async Task UpdateDeepLevels(IEnumerable<TDomainObject> domainObjects, CancellationToken cancellationToken)
    {
        var updateDict = domainObjects.Select(domainObject => new
            {
                DomainObject = domainObject,
                ActualLevel = domainObject.GetAllElements(hierarchicalInfo.ParentFunc, true).Count()
            }).Where(pair => deepLevelInfo.DeepLevel.Getter(pair.DomainObject) != pair.ActualLevel)
            .ToDictionary(pair => pair.DomainObject, pair => pair.ActualLevel);

        foreach (var domainObject in await domainObjectExpanderFactory
                     .Create()
                     .GetAllChildren(updateDict.Keys, cancellationToken))
        {
            deepLevelInfo.DeepLevel.Setter.Invoke(domainObject, updateDict[domainObject]);

            await genericRepository.SaveAsync(domainObject, cancellationToken);
        }
    }

    public Task Initialize(CancellationToken cancellationToken) =>
        this.UpdateDeepLevels(queryableSource.GetQueryable<TDomainObject>(), cancellationToken);
}