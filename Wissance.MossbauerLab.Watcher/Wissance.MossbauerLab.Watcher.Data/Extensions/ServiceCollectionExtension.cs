using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Wissance.MossbauerLab.Watcher.Common.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection ConfigureSqliteDbContext<TContext>(this IServiceCollection serviceCollection,
            string connectionString)
            where TContext : DbContext
        {
            serviceCollection.AddDbContext<TContext>(options => options.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll)
                .UseSqlite(connectionString)
                .UseLazyLoadingProxies());
            return serviceCollection;
        }
    }
}
