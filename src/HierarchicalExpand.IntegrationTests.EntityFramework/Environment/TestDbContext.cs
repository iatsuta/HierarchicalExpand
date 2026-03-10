using HierarchicalExpand.IntegrationTests.Domain;

using Microsoft.EntityFrameworkCore;

namespace HierarchicalExpand.IntegrationTests.Environment;

public class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options)
{
    private const string DefaultIdPostfix = "Id";

    private const string DefaultSchema = "app";

    private const int DefaultMaxLength = 255;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        this.InitApp(modelBuilder);
        this.InitAncestors(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    private void InitApp(ModelBuilder modelBuilder)
    {
        {
            var entity = modelBuilder.Entity<BusinessUnit>().ToTable(nameof(BusinessUnit), DefaultSchema);
            entity.HasKey(v => v.Id);

            entity.Property(e => e.Name).IsRequired().HasMaxLength(DefaultMaxLength);
            entity.HasOne(e => e.Parent).WithMany().HasForeignKey($"{nameof(BusinessUnit.Parent)}{DefaultIdPostfix}").IsRequired(false);
        }

        {
            var entity = modelBuilder.Entity<TestHierarchicalObject>().ToTable(nameof(TestHierarchicalObject), DefaultSchema);
            entity.HasKey(v => v.Id);

            entity.HasOne(e => e.Parent).WithMany().HasForeignKey($"{nameof(TestHierarchicalObject.Parent)}{DefaultIdPostfix}").IsRequired(false);
        }
    }

    private void InitAncestors(ModelBuilder modelBuilder)
    {
        {
            var entity = modelBuilder.Entity<BusinessUnitDirectAncestorLink>().ToTable(nameof(BusinessUnitDirectAncestorLink), DefaultSchema);
            entity.HasKey(v => v.Id);

            var ancestorKey = $"{nameof(BusinessUnitDirectAncestorLink.Ancestor)}{DefaultIdPostfix}";
            var childKey = $"{nameof(BusinessUnitDirectAncestorLink.Child)}{DefaultIdPostfix}";

            entity.HasOne(e => e.Ancestor).WithMany().HasForeignKey(ancestorKey).IsRequired();
            entity.HasOne(e => e.Child).WithMany().HasForeignKey(childKey).IsRequired();

            entity.HasIndex(ancestorKey, childKey).IsUnique();
        }

        {
            var entity = modelBuilder.Entity<BusinessUnitUndirectAncestorLink>().ToView(nameof(BusinessUnitUndirectAncestorLink), DefaultSchema);
            entity.HasNoKey();

            entity.HasOne(e => e.Source).WithMany().HasForeignKey($"{nameof(BusinessUnitUndirectAncestorLink.Source)}{DefaultIdPostfix}").IsRequired();
            entity.HasOne(e => e.Target).WithMany().HasForeignKey($"{nameof(BusinessUnitUndirectAncestorLink.Target)}{DefaultIdPostfix}").IsRequired();
        }

        {
            var entity = modelBuilder.Entity<TestHierarchicalObjectDirectAncestorLink>().ToTable(nameof(TestHierarchicalObjectDirectAncestorLink), DefaultSchema);
            entity.HasKey(v => v.Id);

            var ancestorKey = $"{nameof(TestHierarchicalObjectDirectAncestorLink.Ancestor)}{DefaultIdPostfix}";
            var childKey = $"{nameof(TestHierarchicalObjectDirectAncestorLink.Child)}{DefaultIdPostfix}";

            entity.HasOne(e => e.Ancestor).WithMany().HasForeignKey(ancestorKey).IsRequired();
            entity.HasOne(e => e.Child).WithMany().HasForeignKey(childKey).IsRequired();

            entity.HasIndex(ancestorKey, childKey).IsUnique();
        }

        {
            var entity = modelBuilder.Entity<TestHierarchicalObjectUndirectAncestorLink>().ToView(nameof(TestHierarchicalObjectUndirectAncestorLink), DefaultSchema);
            entity.HasNoKey();

            entity.HasOne(e => e.Source).WithMany().HasForeignKey($"{nameof(TestHierarchicalObjectUndirectAncestorLink.Source)}{DefaultIdPostfix}").IsRequired();
            entity.HasOne(e => e.Target).WithMany().HasForeignKey($"{nameof(TestHierarchicalObjectUndirectAncestorLink.Target)}{DefaultIdPostfix}").IsRequired();
        }
    }
}