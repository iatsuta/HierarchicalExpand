using CommonFramework.DependencyInjection;
using CommonFramework.GenericRepository;
using CommonFramework.IdentitySource.DependencyInjection;

using GenericQueryable.NHibernate;

using Microsoft.Extensions.DependencyInjection;
using NHibernate.Tool.hbm2ddl;

namespace HierarchicalExpand.IntegrationTests.Environment;

public class TestEnvironmentImpl : TestEnvironment
{
    private readonly string dbName = "test.db";

    protected override IServiceCollection InitializeServices(IServiceCollection services)
    {
        return services

            .AddIdentitySource()
            .AddSingleton(BuildConfigurationHelper.BuildConfiguration($"Data Source={this.dbName}"))
            .AddSingletonFrom((global::NHibernate.Cfg.Configuration cfg) => cfg.BuildSessionFactory())

            .AddScoped<AutoCommitSession>()

            .AddSingleton(typeof(IDomainObjectSaveStrategy<>), typeof(DomainObjectSaveStrategy<>))
            .BindServiceProxy(typeof(IDomainObjectSaveStrategy<>), typeof(DomainObjectSaveStrategyServiceProxyBinder<>))

            .AddScoped<IGenericRepository, NHibGenericRepository>()
            .AddScoped<IQueryableSource, NHibQueryableSource>()

            .AddNHibernateGenericQueryable();
    }

    public override async Task InitializeDatabase()
    {
        if (File.Exists(this.dbName))
        {
            File.Delete(this.dbName);
        }

        var cancellationToken = TestContext.Current.CancellationToken;

        var configuration = this.RootServiceProvider.GetRequiredService<NHibernate.Cfg.Configuration>();

        var schemaExport = new SchemaExport(configuration);

        //await schemaExport.DropAsync(false, true, cancellationToken);
        await schemaExport.CreateAsync(false, true, cancellationToken);

        await using var scope = this.RootServiceProvider.CreateAsyncScope();
        await using var session = scope.ServiceProvider.GetRequiredService<AutoCommitSession>();

        foreach (var createViewCode in GetViews())
        {
            await session.NativeSession.CreateSQLQuery(createViewCode).ExecuteUpdateAsync(cancellationToken);
        }
    }

    public static TestEnvironmentImpl Instance { get; } = new ();
}