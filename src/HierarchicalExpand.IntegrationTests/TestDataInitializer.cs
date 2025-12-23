using HierarchicalExpand.AncestorDenormalization;
using HierarchicalExpand.IntegrationTests.Domain;

namespace HierarchicalExpand.IntegrationTests;

public class TestDataInitializer(TestDbContext dbContext, IDenormalizedAncestorsService<BusinessUnit> denormalizedAncestorsService)
{
    public async Task Initialize(CancellationToken cancellationToken)
    {
        await dbContext.Database.EnsureDeletedAsync(cancellationToken);
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        await dbContext.EnsureViewsCreatedAsync(cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        foreach (var bu in GetTestBusinessUnits())
        {
            dbContext.Add(bu);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        await denormalizedAncestorsService.SyncAllAsync(cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static IEnumerable<BusinessUnit> GetTestBusinessUnits()
    {
        var rootBu = new BusinessUnit { Name = "TestRootBu" };
        yield return rootBu;

        foreach (var index in Enumerable.Range(1, 2))
        {
            var middleBu = new BusinessUnit { Name = $"Test{nameof(BusinessUnit)}{index}", Parent = rootBu };
            yield return middleBu;

            var leafBu = new BusinessUnit { Name = $"Test{nameof(BusinessUnit)}{index}-Child", Parent = middleBu };
            yield return leafBu;
        }
    }
}