using Microsoft.EntityFrameworkCore;

namespace Corona_Statistics.DataAccess
{
    public class CoronaStatisticsContext: DbContext
    {
        public CoronaStatisticsContext(DbContextOptions<CoronaStatisticsContext> options)
            : base(options)
        { }

        public DbSet<FederalState> FederalStates { get; set; }
        
        public DbSet<District> Districts { get; set; }
        
        public DbSet<Covid19Case> Covid19Cases { get; set; }
        
        public DbSet<TotalCase> TotalCases { get; set; }
    }
}