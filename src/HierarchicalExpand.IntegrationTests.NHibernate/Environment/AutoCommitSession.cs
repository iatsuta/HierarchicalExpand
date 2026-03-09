using System.Data;

using NHibernate;

namespace HierarchicalExpand.IntegrationTests.Environment;

public sealed class AutoCommitSession : IAsyncDisposable, IDisposable
{
    private readonly ITransaction nhibTransaction;

    private bool closed;

    public AutoCommitSession(ISessionFactory sessionFactory)
    {
        this.NativeSession = sessionFactory.OpenSession();
        this.NativeSession.FlushMode = FlushMode.Manual;

        this.nhibTransaction = this.NativeSession.BeginTransaction();

        this.Transaction = GetDbTransaction(this.nhibTransaction, this.NativeSession);
    }

    public ISession NativeSession { get; }

    public IDbTransaction Transaction { get; }

    private static IDbTransaction GetDbTransaction(ITransaction transaction, ISession session)
    {
        using var dbCommand = session.Connection.CreateCommand();
        dbCommand.Cancel();
        transaction.Enlist(dbCommand);
        return dbCommand.Transaction!;
    }

    public void Dispose() => this.DisposeAsync().GetAwaiter().GetResult();

    public ValueTask DisposeAsync() => this.CloseAsync();

    public async ValueTask CloseAsync(CancellationToken cancellationToken = default)
    {
        if (this.closed)
        {
            return;
        }

        this.closed = true;

        using (this.NativeSession)
        {
            using (this.nhibTransaction)
            {
                await this.NativeSession.FlushAsync(cancellationToken);

                await this.nhibTransaction.CommitAsync(cancellationToken);
            }
        }
    }
}