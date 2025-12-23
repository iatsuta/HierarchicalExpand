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

        var testRootBu = new BusinessUnit { Name = "TestRootBu" };
        dbContext.Add(testRootBu);

        foreach (var index in Enumerable.Range(1, 2))
        {
            var testBu = new BusinessUnit { Name = $"Test{nameof(BusinessUnit)}{index}", Parent = testRootBu };
            dbContext.Add(testBu);

            var testChildBu = new BusinessUnit { Name = $"Test{nameof(BusinessUnit)}{index}-Child", Parent = testBu };
            dbContext.Add(testChildBu);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        await denormalizedAncestorsService.SyncAllAsync(cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}