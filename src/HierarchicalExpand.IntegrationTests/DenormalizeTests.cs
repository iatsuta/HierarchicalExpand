using HierarchicalExpand.AncestorDenormalization;
using HierarchicalExpand.IntegrationTests.Domain;

namespace HierarchicalExpand.IntegrationTests;

public class DenormalizeTests : TestBase
{
    [Fact]
    public async Task Initialize_Should_DenormalizeAncestors_ForLargeTree()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;

        await this.Evaluate<TestDbContext>(async dbContext =>
        {
            var tree = CreateTree(new TestHierarchicalObject()).ToList();

            foreach (var node in tree)
            {
                await dbContext.AddAsync(node, cancellationToken);
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        });

        var before = await this.GetSyncState(cancellationToken);

        // Act
        await this.Evaluate<IDenormalizedAncestorsService<TestHierarchicalObject>>(denormalizedAncestorsService =>
            denormalizedAncestorsService.Initialize(cancellationToken));

        // Assert
        var after = await this.GetSyncState(cancellationToken);

        before.Adding.Count.Should().Be(7108);
        after.Should().Be(SyncResult<TestHierarchicalObject, TestHierarchicalObjectDirectAncestorLink>.Empty);
    }

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