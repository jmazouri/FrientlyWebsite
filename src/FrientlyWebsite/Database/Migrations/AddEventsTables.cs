using Dapper;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace FrientlyWebsite.Database.Migrations
{
    public class AddEventsTables : IMigration
    {
        public bool ApplyMigration(DbConnection connection)
        {
            bool didAny = false;

            if (!connection.ExecuteScalar<bool>(@"
                SELECT EXISTS (
                   SELECT 1
                   FROM   information_schema.tables 
                   WHERE  table_schema = 'public'
                   AND    table_name = 'events'
                );"))
            {
                connection.Execute(@"
                CREATE TABLE events
                (
                    eventid      SERIAL PRIMARY KEY,
                    name         TEXT,
                    datestart    timestamp without time zone,
                    dateend      timestamp without time zone,
                    creatorid    VARCHAR REFERENCES users (steamid)
                )");

                didAny = true;
            }

            if (!connection.ExecuteScalar<bool>(@"
                SELECT EXISTS (
                   SELECT 1
                   FROM   information_schema.tables 
                   WHERE  table_schema = 'public'
                   AND    table_name = 'eventcommitments'
                );"))
            {
                connection.Execute(@"
                CREATE TABLE eventcommitments
                (
                    userid           VARCHAR REFERENCES users (steamid),
                    commitmentstate  TEXT,
                    comment          TEXT,
                    sourceevent      INTEGER REFERENCES events (eventid),
                    primary key      (userid, sourceevent)
                )");

                didAny = true;
            }

            return didAny;
        }
    }
}
