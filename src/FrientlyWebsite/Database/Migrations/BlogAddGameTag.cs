using Dapper;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace FrientlyWebsite.Database.Migrations
{
    public class BlogAddGameTag : IMigration
    {
        public bool ApplyMigration(DbConnection connection)
        {
            if (connection.ExecuteScalar<int>(@"
                SELECT COUNT(column_name)
                FROM information_schema.columns 
                WHERE table_name='posts' and column_name='game';") != 0) return false;

            connection.Execute("ALTER TABLE posts ADD COLUMN game VARCHAR;");

            return true;
        }
    }
}
