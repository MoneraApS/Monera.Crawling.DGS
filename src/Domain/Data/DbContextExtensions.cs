using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monera.Crawling.DGS.Domain.Data
{
    public static class DbContextExtensions
    {
        /// <summary>
        /// Use it, when you need to use paging with your select query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public static IQueryable<T> ToPagedQuery<T>(this IQueryable<T> query, int pageSize, int pageNumber)
        {
            if (pageNumber < 1)
                throw new System.ArgumentOutOfRangeException("pageNumber", pageNumber, "PageNumber can't be less than 1");

            if (pageSize < 1)
                throw new System.ArgumentOutOfRangeException("pageSize", pageSize, "PageSize can't be less than 1");

            return query.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
        }

        /// <summary>
        /// Suspend Azure retry strategy and run query in transaction scope with READUNCOMMITED level
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db">DbContext</param>
        /// <param name="func">func with query to execute</param>
        /// <returns></returns>
        public static T WithNOLOCK<T>(this DgsContext db, Func<DbContextTransaction, T> func)
        {
            return new SqlAzureExecutionStrategy().Execute<T>(() =>
            {
                // So we suspend strategy for this transaction
                DbContextConfiguration.SuspendExecutionStrategy = true;

                try
                {
                    // Make ReadUncommited to speedup
                    using (var dbContextTransaction = db.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
                    {
                        return func(dbContextTransaction);
                    }
                }
                catch
                {
                    DbContextConfiguration.SuspendExecutionStrategy = false;
                    throw;
                }
                finally
                {
                    DbContextConfiguration.SuspendExecutionStrategy = false;
                }
            });
        }
    }
}
