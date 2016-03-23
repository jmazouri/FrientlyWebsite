using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace FrientlyWebsite.Database.Migrations
{
    public interface IMigration
    {
        bool ApplyMigration(DbConnection connection);
    }
}
