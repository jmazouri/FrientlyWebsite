using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Dapper;
using FrientlyWebsite.Database.Migrations;
using FrientlyWebsite.Models;
using Microsoft.Extensions.Logging;
using Npgsql;
using Microsoft.Extensions.Configuration;

namespace FrientlyWebsite.Database
{
    public class DatabaseContainer : IDisposable
    {
        private DbConnection _dbConnection;
        private ILogger<DatabaseContainer> _logger;
        private List<IMigration> Migrations = new List<IMigration> {new BlogAddGameTag(), new AddEventsTables()};

        public DatabaseContainer(ILogger<DatabaseContainer> logger, IConfiguration configuration)
        {
            try
            {
                _logger = logger;

                _dbConnection =
                    new NpgsqlConnection(configuration.Get<string>("ConnectionString"));

                try
                {
                    _dbConnection.Open();
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode != SocketError.Success)
                    {
                        throw;
                    }
                }

                if (!_dbConnection.ExecuteScalar<bool>(@"
                SELECT EXISTS (
                   SELECT 1
                   FROM   information_schema.tables 
                   WHERE  table_schema = 'public'
                   AND    table_name = 'users'
                );"))
                {
                    _logger.LogCritical("DATABASE HAS NO TABLES! Initializing...");
                    InitializeDatabase();
                }
                else
                {
                    _logger.LogInformation("Database has some tables.");
                    _logger.LogInformation("Executing potential migrations...");

                    foreach (IMigration migration in Migrations)
                    {
                        _logger.LogInformation("Executing migration \"{migration.GetType().Name}\"");
                        var result = migration.ApplyMigration(_dbConnection);

                        if (result == true)
                        {
                            _logger.LogInformation("Migration completed.");
                        }
                        else
                        {
                            _logger.LogInformation("Migration was not executed - probably unnecessary.");
                        }
                    }
                }
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode != SocketError.Success)
                {
                    throw;
                }
            }
        }

        public void InitializeDatabase()
        {
            _logger.LogInformation("Initializing Database");
            _dbConnection.Execute(@"
                CREATE TABLE users
                (
                    steamid    VARCHAR PRIMARY KEY,
                    lastlogin  timestamp without time zone,
                    admin      BOOLEAN
                )");
            _dbConnection.Execute(@"
                CREATE TABLE posts
                (
                    postid      SERIAL PRIMARY KEY,
                    title       TEXT,
                    game        VARCHAR,
                    content     TEXT,
                    posted      timestamp without time zone,
                    userid      VARCHAR REFERENCES users (steamid)
                )");
        }

        public async Task<List<string>> GetUsers()
        {
            var ret = await _dbConnection.QueryAsync<string>("SELECT steamid FROM users");
            return ret.ToList();
        }

        public async Task<bool> IsAdmin(string id)
        {
            //Jmazouri
            if (id == "76561197974339263")
            {
                return true;
            }

            var ret = await _dbConnection.ExecuteScalarAsync<bool>("SELECT admin FROM users WHERE steamid = @SteamId", new { SteamId = id });
            return ret;
        }

        public async Task<List<BlogPost>> GetPosts()
        {
            var ret = await _dbConnection.QueryAsync<BlogPost>("SELECT * FROM posts");
            return ret.ToList();
        }

        public async Task<List<Event>> GetEvents()
        {
            var ret = await _dbConnection.QueryAsync<Event>("SELECT * FROM events");
            return ret.ToList();
        }

        public async Task AddEvent(Event newEvent)
        {
            await _dbConnection.ExecuteAsync("INSERT INTO events (name, datestart, dateend, creatorid) VALUES (@Name, @DateStart, @DateEnd, @CreatorId)",
                                              new { newEvent.Name, newEvent.DateStart, newEvent.DateEnd, newEvent.CreatorId});
        }

        public async Task AddPost(BlogPost post, string id)
        {
            await _dbConnection.ExecuteAsync("INSERT INTO posts (title, content, posted, game, userid) VALUES (@Title, @Content, @PostDate, @Game, @UserId)",
                                              new { post.Title, post.Content, PostDate = DateTime.UtcNow, UserId = id, post.Game });
        }

        public async Task DeletePost(int id)
        {
            await _dbConnection.ExecuteAsync("DELETE FROM posts WHERE postid = @PostId", new { PostId = id });
        }

        public async Task AddOrUpdateUser(string id)
        {
            await _dbConnection.ExecuteAsync("INSERT INTO users (steamid, lastlogin) VALUES (@SteamId, @LastLogin) " +
                                             "ON CONFLICT ON CONSTRAINT users_pkey DO UPDATE SET lastlogin=excluded.lastlogin;", new {SteamId = id, LastLogin = DateTime.UtcNow});
        }

        public void Dispose()
        {
            _dbConnection.Close();
            _dbConnection.Dispose();
        }
    }
}
