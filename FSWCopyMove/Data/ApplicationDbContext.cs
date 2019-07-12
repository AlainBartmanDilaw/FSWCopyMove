using Microsoft.EntityFrameworkCore;
//using System.Data.Entity;
//using System.Data.SQLite;

namespace FSWCopyMove.Data
{
    public class ApplicationDbContext : DbContext
    {
        //public ApplicationDbContext() : base("name=SearchDuplicates")
        public ApplicationDbContext() : base()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=d:\Profiles\aleglise\Documents\Repository\FSWCopyMove\_Databases\SearchDuplicates.db");
            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<Prm> Prm { get; set; }
        public DbSet<PrmVal> PrmVal { get; set; }

        public static void Initialize(ApplicationDbContext context)
        {
            using (var dbContextTransaction = context.Database.BeginTransaction())
            {

                dbContextTransaction.Commit();
            }
        }
    }
}
