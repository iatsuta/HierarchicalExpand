using CommonFramework.GenericRepository;

using GenericQueryable.EntityFramework;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HierarchicalExpand.IntegrationTests.Environment;

public class TestEnvironmentImpl : TestEnvironment
{
    protected override IServiceCollection InitializeServices(IServiceCollection services)
    {
        return services
            .AddDbContext<TestDbContext>(optionsBuilder => optionsBuilder
                .UseSqlite("Data Source=test.db")
                .UseGenericQueryable())
            .AddScoped<AutoCommitSession>()
            .AddScoped<IGenericRepository, EfGenericRepository>()
            .AddScoped<IQueryableSource, EfQueryableSource>();
    }

    public override async Task InitializeDatabase()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var scope = this.RootServiceProvider.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();

        await dbContext.Database.EnsureDeletedAsync(cancellationToken);
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        foreach (var createViewCode in base.GetViews(null))
        {
            await dbContext.Database.ExecuteSqlRawAsync(createViewCode, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public static TestEnvironmentImpl Instance { get; } = new();
}