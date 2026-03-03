using CommonFramework.DependencyInjection;
using CommonFramework.GenericRepository;

using GenericQueryable.EntityFramework;

using HierarchicalExpand.DependencyInjection;
using HierarchicalExpand.IntegrationTests.Domain;
using HierarchicalExpand.IntegrationTests.Services;

using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.DependencyInjection;

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
                        new AncestorLinkInfo<BusinessUnit, BusinessUnitUndirectAncestorLink>(view => view.Source, view => view.Target))
                    .AddHierarchicalInfo(
                        v => v.Parent,
                        new AncestorLinkInfo<TestHierarchicalObject, TestHierarchicalObjectDirectAncestorLink>(link => link.Ancestor, link => link.Child),
                        new AncestorLinkInfo<TestHierarchicalObject, TestHierarchicalObjectUndirectAncestorLink>(view => view.Source, view => view.Target)))

                .AddScoped<IQueryableSource, EfQueryableSource>()
                .AddScoped<IGenericRepository, EfGenericRepository>()

                .AddScoped<TestDataInitializer>()

                .AddValidator<DuplicateServiceUsageValidator>()
                .Validate()
                .BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
    }

    protected Task Evaluate(Func<IServiceProvider, Task> action) => this.Evaluate<IServiceProvider>(action);

    protected Task Evaluate<TService>(Func<TService, Task> action)
        where TService : notnull => this.Evaluate<TService, object?>(async service =>
    {
        await action(service);

        return null;
    });

    protected Task<TResult> Evaluate<TResult>(Func<IServiceProvider, Task<TResult>> func) => this.Evaluate<IServiceProvider, TResult>(func);

    protected async Task<TResult> Evaluate<TService, TResult>(Func<TService, Task<TResult>> func)
        where TService : notnull
    {
        await using var scope = this.RootServiceProvider.CreateAsyncScope();

        return await func(scope.ServiceProvider.GetRequiredService<TService>());
    }

    public virtual async ValueTask InitializeAsync()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await this.Evaluate<TestDataInitializer>(initializer => initializer.Initialize(cancellationToken));
    }

    public virtual ValueTask DisposeAsync() => ValueTask.CompletedTask;
}