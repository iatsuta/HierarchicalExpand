using HierarchicalExpand.IntegrationTests.Environment;

using Microsoft.Extensions.DependencyInjection;

namespace HierarchicalExpand.IntegrationTests;

public abstract class TestBase(TestEnvironment testEnvironment) : IAsyncLifetime
{
    protected IServiceProvider RootServiceProvider => testEnvironment.RootServiceProvider;

    protected ScopeEvaluator ScopeEvaluator => this.RootServiceProvider.GetRequiredService<ScopeEvaluator>();

    public virtual async ValueTask InitializeAsync()
    {
        await testEnvironment.InitializeDatabase();

        var cancellationToken = TestContext.Current.CancellationToken;

        await this.RootServiceProvider.GetRequiredService<TestDataInitializer>().Initialize(cancellationToken);
    }

    public virtual ValueTask DisposeAsync() => ValueTask.CompletedTask;
}