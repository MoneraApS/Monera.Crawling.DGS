using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Monera.Crawling.DGS.Domain.Data
{
    public class DbContextConfiguration : DbConfiguration
    {
        private const int csDelaySec = 10;
        private const int csMaxRetries = 3;


        public DbContextConfiguration()
        {
            SetExecutionStrategy(
                "System.Data.SqlClient",
                () => SuspendExecutionStrategy
                    ? (IDbExecutionStrategy)new DefaultExecutionStrategy()
                    : new SqlAzureExecutionStrategy(csMaxRetries, TimeSpan.FromSeconds(csDelaySec)));
        }


        private static object suspendedCounterLocker = 0;
        private static int suspendedCounter = 0;

        public static bool SuspendExecutionStrategy
        {
            get
            {
                return (bool?)CallContext.LogicalGetData("SuspendExecutionStrategy") ?? false;
            }
            set
            {
                lock (suspendedCounterLocker)
                {
                    if (value)
                        suspendedCounter++;
                    else
                        suspendedCounter = Math.Max(0, suspendedCounter - 1);

                    CallContext.LogicalSetData("SuspendExecutionStrategy", suspendedCounter > 0);
                }
            }
        }
    }
}
