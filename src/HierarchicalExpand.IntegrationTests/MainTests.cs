using CommonFramework.GenericRepository;

using GenericQueryable;

using HierarchicalExpand.IntegrationTests.Domain;

using Microsoft.Extensions.DependencyInjection;

namespace HierarchicalExpand.IntegrationTests;

public class MainTests : TestBase
{
    [Fact]
    public async Task InvokeExpandWithParents_ForRootBu_DataCorrected()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var scope = this.RootServiceProvider.CreateAsyncScope();

        var queryableSource = scope.ServiceProvider.GetRequiredService<IQueryableSource>();
        var hierarchicalObjectExpanderFactory = scope.ServiceProvider.GetRequiredService<IHierarchicalObjectExpanderFactory>();
        var hierarchicalObjectExpander = hierarchicalObjectExpanderFactory.Create<Guid>(typeof(BusinessUnit));

        var rootBuId = await queryableSource.GetQueryable<BusinessUnit>().Where(bu => bu.Parent == null).Select(bu => bu.Id)
            .GenericSingleAsync(cancellationToken);

        // Act
        var dict = hierarchicalObjectExpander.ExpandWithParents([rootBuId], HierarchicalExpandType.Parents);

        // Assert
        dict.Count.Should().Be(1);
        dict.Should().BeEquivalentTo(new Dictionary<Guid, Guid> { { rootBuId, Guid.Empty } });
    }

    [Fact]
    public async Task InvokeChildrenExpand_ForRootBu_DataCorrected()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var scope = this.RootServiceProvider.CreateAsyncScope();

        var queryableSource = scope.ServiceProvider.GetRequiredService<IQueryableSource>();
        var hierarchicalObjectExpanderFactory = scope.ServiceProvider.GetRequiredService<IHierarchicalObjectExpanderFactory>();
        var hierarchicalObjectExpander = hierarchicalObjectExpanderFactory.Create<Guid>(typeof(BusinessUnit));

        var rootBuId = await queryableSource.GetQueryable<BusinessUnit>().Where(bu => bu.Parent == null).Select(bu => bu.Id)
            .GenericSingleAsync(cancellationToken);

        var expectedBuIdents = await queryableSource.GetQueryable<BusinessUnit>().Select(bu => bu.Id).OrderBy(v => v).GenericToListAsync(cancellationToken);

        // Act
        var result = hierarchicalObjectExpander.Expand([rootBuId], HierarchicalExpandType.Children).ToList();

        // Assert
        result.OrderBy(v => v).Should().BeEquivalentTo(expectedBuIdents);
    }


    [Fact]
    public async Task InvokeAllExpand_ForMiddleBu_DataCorrected()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var scope = this.RootServiceProvider.CreateAsyncScope();

        var queryableSource = scope.ServiceProvider.GetRequiredService<IQueryableSource>();
        var hierarchicalObjectExpanderFactory = scope.ServiceProvider.GetRequiredService<IHierarchicalObjectExpanderFactory>();
        var hierarchicalObjectExpander = hierarchicalObjectExpanderFactory.Create<Guid>(typeof(BusinessUnit));

        var middleBuId = await queryableSource.GetQueryable<BusinessUnit>().Where(bu => bu.Parent != null && bu.Parent.Parent == null).Select(bu => bu.Id)
            .GenericFirstAsync(cancellationToken);

        var parentsIdents = hierarchicalObjectExpander.Expand([middleBuId], HierarchicalExpandType.Parents).ToList();
        var childrenIdents = hierarchicalObjectExpander.Expand([middleBuId], HierarchicalExpandType.Children).ToList();

        var expectedBuIdents = parentsIdents.Concat(childrenIdents).Distinct().OrderBy(v => v).ToList();

        // Act
        var result = hierarchicalObjectExpander.Expand([middleBuId], HierarchicalExpandType.All).ToList();

        // Assert
        result.OrderBy(v => v).Should().BeEquivalentTo(expectedBuIdents);
    }
}