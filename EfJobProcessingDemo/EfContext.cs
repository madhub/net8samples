using Microsoft.EntityFrameworkCore;

namespace EfJobProcessingDemo;

public class EfContext :DbContext
{
    public EfContext(DbContextOptions<EfContext> options)
        :base(options)
    {
        
    }
    public DbSet<ImportJob> Jobs { get; set; }
}
