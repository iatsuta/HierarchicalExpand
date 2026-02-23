using CommonFramework;
using CommonFramework.GenericRepository;

using GenericQueryable;
using GenericQueryable.Fetching;

namespace HierarchicalExpand.AncestorDenormalization;

public class AncestorLinkExtractor<TDomainObject, TDirectAncestorLink>(
    IQueryableSource queryableSource,
    IDomainObjectExpanderFactory<TDomainObject> domainObjectExpanderFactory,
    FullAncestorLinkInfo<TDomainObject, TDirectAncestorLink> fullAncestorLinkInfo)
    : IAncestorLinkExtractor<TDomainObject, TDirectAncestorLink>
    where TDirectAncestorLink : class
    where TDomainObject : class
{
    private readonly AncestorLinkInfo<TDomainObject, TDirectAncestorLink> ancestorLinkInfo = fullAncestorLinkInfo.Directed;

    private readonly FetchRule<TDirectAncestorLink> linkFetchRule =
        FetchRule<TDirectAncestorLink>.Create(fullAncestorLinkInfo.Directed.From.Path).Fetch(fullAncestorLinkInfo.Directed.To.Path);

    public async Task<SyncResult<TDomainObject, TDirectAncestorLink>> GetSyncAllResult(CancellationToken cancellationToken)
    {
        var existsDomainObjects = await queryableSource.GetQueryable<TDomainObject>().GenericToListAsync(cancellationToken);

        var existsLinks = await queryableSource.GetQueryable<TDirectAncestorLink>().WithFetch(linkFetchRule).GenericToListAsync(cancellationToken);

        var nonExistsDomainObjects = existsLinks.Select(this.ToInfo).SelectMany(link => new[] { link.Ancestor, link.Child }).Except(existsDomainObjects);

        return await this.GetSyncResult(existsDomainObjects, nonExistsDomainObjects, cancellationToken);
    }

    public async Task<SyncResult<TDomainObject, TDirectAncestorLink>> GetSyncResult(
        IEnumerable<TDomainObject> updatedDomainObjectsBase,
        IEnumerable<TDomainObject> removedDomainObjects,
        CancellationToken cancellationToken)
    {
        var domainObjectExpander = domainObjectExpanderFactory.Create();

        var existsLinkInfos =
            await updatedDomainObjectsBase.SyncWhenAll(domainObject => this.GetSyncResult(domainObject, domainObjectExpander, cancellationToken));

        var forceRemovedLinks = await this.GetExistsLinks(removedDomainObjects, cancellationToken);

        var forceRemovedLinksSyncResult = new SyncResult<TDomainObject, TDirectAncestorLink>([], forceRemovedLinks);

        return existsLinkInfos.Union([forceRemovedLinksSyncResult]).Aggregate();
    }

    public Task<SyncResult<TDomainObject, TDirectAncestorLink>> GetSyncResult(TDomainObject domainObject, CancellationToken cancellationToken) =>
        this.GetSyncResult([domainObject], [], cancellationToken);

    private async Task<SyncResult<TDomainObject, TDirectAncestorLink>> GetSyncResult(TDomainObject domainObject,
        IDomainObjectExpander<TDomainObject> domainObjectExpander, CancellationToken cancellationToken)
    {
        var expectedParents = await domainObjectExpander.GetAllParents([domainObject], cancellationToken);

        var existsParentLinks = await queryableSource
            .GetQueryable<TDirectAncestorLink>()
            .Where(ancestorLinkInfo.To.Path.Select(toObj => toObj == domainObject))
            .WithFetch(linkFetchRule)
            .GenericToListAsync(cancellationToken);

        var mergeResult = existsParentLinks.Select(ancestorLinkInfo.From.Getter).GetMergeResult(expectedParents);

        if (mergeResult.IsEmpty)
        {
            return SyncResult<TDomainObject, TDirectAncestorLink>.Empty;
        }
        else
        {
            var children = await domainObjectExpander.GetAllChildren([domainObject], cancellationToken);

            var addedLinks =

                from newParent in mergeResult.AddingItems

                from child in children

                select new AncestorLinkData<TDomainObject>(newParent, child);

            var removedLinks = await queryableSource
                .GetQueryable<TDirectAncestorLink>()
                .Where(ancestorLinkInfo.To.Path.Select(toObj => children.Contains(toObj))
                    .BuildAnd(ancestorLinkInfo.From.Path.Select(toObj => mergeResult.RemovingItems.Contains(toObj))))
                .GenericToListAsync(cancellationToken);

            return new(addedLinks, removedLinks);
        }
    }

    private async Task<IReadOnlyList<TDirectAncestorLink>> GetExistsLinks(IEnumerable<TDomainObject> domainObjects, CancellationToken cancellationToken)
    {
        var filter = ancestorLinkInfo.From.Path.Select(fromObj => domainObjects.Contains(fromObj))
            .BuildOr(ancestorLinkInfo.To.Path.Select(toObj => domainObjects.Contains(toObj)));

        return await queryableSource.GetQueryable<TDirectAncestorLink>().Where(filter).WithFetch(linkFetchRule).GenericToListAsync(cancellationToken);
    }

    private AncestorLinkData<TDomainObject> ToInfo(TDirectAncestorLink link)
    {
        return new AncestorLinkData<TDomainObject>(
            ancestorLinkInfo.From.Getter(link),
            ancestorLinkInfo.To.Getter(link));
    }
}