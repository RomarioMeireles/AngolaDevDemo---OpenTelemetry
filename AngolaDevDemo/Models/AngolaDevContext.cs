using Microsoft.EntityFrameworkCore;

namespace AngolaDevDemo.Models
{
    public class AngolaDevContext:DbContext
    {
        public AngolaDevContext(DbContextOptions<AngolaDevContext> options):base(options)
        {

        }
        public DbSet<Palestrante> Palestrante { get; set; }
    }
}
