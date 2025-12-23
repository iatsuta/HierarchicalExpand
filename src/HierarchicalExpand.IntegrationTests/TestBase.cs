using CommonFramework.DependencyInjection;
using CommonFramework.GenericRepository;
using GenericQueryable.EntityFramework;
using HierarchicalExpand.DependencyInjection;
using HierarchicalExpand.IntegrationTests.Domain;
using HierarchicalExpand.IntegrationTests.Services;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace HierarchicalExpand.IntegrationTests;

public abstract class TestBase : IAsyncLifetime
{
    protected readonly IServiceProvider RootServiceProvider;

    protected TestBase()
    {
        this.RootServiceProvider =
            new ServiceCollection()
                .AddDbContext<TestDbContext>(optionsBuilder => optionsBuilder
                    .UseSqlite("Data Source=test.db")
                    .UseGenericQueryable())

                .AddHierarchicalExpand(scb => scb
                    .AddHierarchicalInfo(
                        v => v.Parent,
                        new AncestorLinkInfo<BusinessUnit, BusinessUnitDirectAncestorLink>(link => link.Ancestor, link => link.Child),
                        new AncestorLinkInfo<BusinessUnit, BusinessUnitUndirectAncestorLink>(view => view.Source, view => view.Target)))

                .AddScoped<IQueryableSource, EfQueryableSource>()
                .AddScoped<IGenericRepository, EfGenericRepository>()

                .AddScoped<TestDataInitializer>()

                .AddValidator<DuplicateServiceUsageValidator>()
                .Validate()
                .BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
    }

    public async ValueTask InitializeAsync()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var scope = this.RootServiceProvider.CreateAsyncScope();

        await scope.ServiceProvider.GetRequiredService<TestDataInitializer>().Initialize(cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
    }
}