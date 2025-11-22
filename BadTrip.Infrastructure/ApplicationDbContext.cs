using Microsoft.EntityFrameworkCore;

namespace BadTrip.Infrastructure
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Fluent-configs?
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // change tracker etc.
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
