using Microsoft.EntityFrameworkCore;

namespace gridLevel2LL.Data
{
    public class GridDbContext : DbContext
    {
        public DbSet<Entities.GridEntity> Grids { get; set; }
        public DbSet<Entities.RowEntity> Rows { get; set; }
        public DbSet<Entities.CellEntity> Cells { get; set; }

        public GridDbContext(DbContextOptions<GridDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Entities.GridEntity>(entity =>
            {
                entity.HasKey(e => e.GridId);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.DateCreated).IsRequired();
                entity.Property(e => e.DateUpdated).IsRequired();

                entity.HasMany(e => e.Rows)
                    .WithOne(e => e.Grid)
                    .HasForeignKey(e => e.GridId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Entities.RowEntity>(entity =>
            {
                entity.HasKey(e => e.RowId);
                entity.Property(e => e.RowIndex).IsRequired();
                entity.Property(e => e.GridId).IsRequired();

                entity.HasMany(e => e.Cells)
                    .WithOne(e => e.Row)
                    .HasForeignKey(e => e.RowId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Entities.CellEntity>(entity =>
            {
                entity.HasKey(e => e.CellId);
                entity.Property(e => e.ColumnIndex).IsRequired();
                entity.Property(e => e.RowId).IsRequired();
            });
        }
    }
}