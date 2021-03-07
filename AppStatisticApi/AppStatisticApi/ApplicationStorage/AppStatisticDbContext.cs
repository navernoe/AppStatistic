using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

using AppStatisticApi.Storage.Entities;

namespace AppStatisticApi.Storage
{
    public class AppStatisticDbContext: DbContext
    {

        public AppStatisticDbContext(
            DbContextOptions<AppStatisticDbContext> options
        ): base(options) { }

        public static AppStatisticDbContext instance;
        public DbSet<AppEntity> Applications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppEntity>()
                .Property(app => app.id)
                    .HasColumnType("bigserial")
                    .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            modelBuilder.Entity<AppEntity>()
                .HasIndex(app => app.url).IsUnique();
        }

    }
}
