using CommonFramework.GenericRepository;

using GenericQueryable;

using HierarchicalExpand.AncestorDenormalization;
using HierarchicalExpand.IntegrationTests.Domain;

using Microsoft.Extensions.DependencyInjection;

namespace HierarchicalExpand.IntegrationTests;

public class DenormalizeTests : TestBase
{
    [Fact]
    public async Task Initialize_Should_DenormalizeAncestors_ForLargeTree()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;

        await this.InitTree(cancellationToken);

        var before = await this.GetSyncState(cancellationToken);

        // Act
        await this.Denormalize(cancellationToken);

        // Assert
        var after = await this.GetSyncState(cancellationToken);

        before.Adding.Count.Should().Be(7108);
        after.Should().Be(SyncResult<TestHierarchicalObject, TestHierarchicalObjectDirectAncestorLink>.Empty);
    }

    [Fact]
    public async Task Initialize_Should_DenormalizeAncestors_AfterNodeMove_ForLargeTree()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;

        await this.InitTree(cancellationToken);
        await this.Denormalize(cancellationToken);
        await this.MoveNode(cancellationToken);

        var before = await this.GetSyncState(cancellationToken);

        // Act
        await this.Denormalize(cancellationToken);

        // Assert
        var after = await this.GetSyncState(cancellationToken);

        before.Adding.Count.Should().Be(364);
        after.Should().Be(SyncResult<TestHierarchicalObject, TestHierarchicalObjectDirectAncestorLink>.Empty);
    }

    private Task InitTree(CancellationToken cancellationToken) =>
        this.Evaluate<IGenericRepository>(async genericRepository =>
        {
            var tree = CreateTree(new TestHierarchicalObject()).ToList();

            foreach (var node in tree)
            {
                await genericRepository.SaveAsync(node, cancellationToken);
            }
        });

    private Task MoveNode(CancellationToken cancellationToken) =>
        this.Evaluate<IServiceProvider>(async sp =>
        {
            var queryableSource = sp.GetRequiredService<IQueryableSource>();
            var genericRepository = sp.GetRequiredService<IGenericRepository>();

            var q = queryableSource.GetQueryable<TestHierarchicalObject>();

            var root = await q.Where(v => v.Parent == null).GenericSingleAsync(cancellationToken);

            var rootChildren = await q.Where(v => v.Parent == root).GenericToListAsync(cancellationToken);

            rootChildren[0].Parent = rootChildren[1];

            await genericRepository.SaveAsync(rootChildren[0], cancellationToken);
        });

    private Task Denormalize(CancellationToken cancellationToken) =>
        this.Evaluate<IDenormalizedAncestorsService<TestHierarchicalObject>>(denormalizedAncestorsService =>
            denormalizedAncestorsService.Initialize(cancellationToken));

    private Task<SyncResult<TestHierarchicalObject, TestHierarchicalObjectDirectAncestorLink>> GetSyncState(CancellationToken cancellationToken) =>
        this.Evaluate((IAncestorLinkExtractor<TestHierarchicalObject, TestHierarchicalObjectDirectAncestorLink> ancestorLinkExtractor) =>
            ancestorLinkExtractor.GetSyncAllResult(cancellationToken));

    private static IEnumerable<TestHierarchicalObject> CreateTree(TestHierarchicalObject current, int deepSize = 6, int branchCount = 3)
    {
        yield return current;

        if (deepSize != 0)
        {
            foreach (var branchIndex in Enumerable.Range(0, branchCount))
            {
                foreach (var next in CreateTree(new TestHierarchicalObject { Parent = current }, deepSize - 1, branchCount))
                {
                    yield return next;
                }
            }
        }
    }
}