using System.Data.Entity;

namespace DietaParserTest.Model
{
    class DietDbContext : DbContext
    {
        public DietDbContext() : base("DietDbContext")
        {

        }

        public DbSet<Meal> Meals { get; set; }

        public DbSet<Ingridient> Ingridients { get; set; }
    }
}
