using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadTrip.Infrastructure
{
    public interface IApplicationDbContext
    {
        // DbSet<Something> Something { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
