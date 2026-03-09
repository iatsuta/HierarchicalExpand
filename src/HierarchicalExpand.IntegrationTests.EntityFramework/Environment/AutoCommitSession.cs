using Microsoft.EntityFrameworkCore.Storage;

using System.Data;

using Microsoft.EntityFrameworkCore;

namespace HierarchicalExpand.IntegrationTests.Environment;

public sealed class AutoCommitSession : IAsyncDisposable, IDisposable
{
    private readonly RelationalTransaction efTransaction;

    private bool closed;

    public AutoCommitSession(TestDbContext nativeSession)
    {
        this.NativeSession = nativeSession;
        this.efTransaction = (RelationalTransaction)this.NativeSession.Database.BeginTransaction();
        this.Transaction = this.efTransaction.GetDbTransaction();
    }

    public DbContext NativeSession { get; }

    public IDbTransaction Transaction { get; }

    public void Dispose() => this.DisposeAsync().GetAwaiter().GetResult();

    public ValueTask DisposeAsync() => this.CloseAsync();

    public async ValueTask CloseAsync(CancellationToken cancellationToken = default)
    {
        if (this.closed)
        {
            return;
        }

        this.closed = true;

        await using (this.NativeSession)
        {
            await using (this.efTransaction)
            {
                await this.NativeSession.SaveChangesAsync(cancellationToken);

                await this.efTransaction.CommitAsync(cancellationToken);
            }
        }
    }
}