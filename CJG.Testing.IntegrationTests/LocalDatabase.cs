using System.Data.Entity;
using CJG.Infrastructure.EF;
using CJG.Infrastructure.EF.Migrations;
using System.IO;
using System;
using CJG.Testing.IntegrationTests.Properties;
using CJG.Infrastructure.Entities;

namespace CJG.Testing.IntegrationTests
{
    /// <summary>
    /// <typeparamref name="LocalDatabase"/> internal class, provides a way to create a local database for testing.
    /// </summary>
    internal class LocalDatabase
    {
        #region Variables
        private readonly string _connectionString;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="LocalDatabase"/> object.
        /// </summary>
        /// <param name="connectionString"></param>
        public LocalDatabase(string connectionString)
        {
            _connectionString = connectionString;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Creates a <typeparamref name="CJGContext"/> object for the local database.
        /// </summary>
        /// <returns></returns>
        public IDataContext CreateContext()
        {
            return new DataContext(_connectionString);
        }

        /// <summary>
        /// Creats a new instance of a local database if it doesn't already exist.
        /// </summary>
        public void Create()
        {
            var databaseInitializer = new MigrateDatabaseToLatestVersion<CJGContext, Configuration>(true, new Configuration());

            Database.SetInitializer(databaseInitializer);

            using (var ctx = CreateContext())
            {
                ctx.Database.Initialize(true);
            }
        }

        /// <summary>
        /// Deletes the local database if it exists.
        /// </summary>
        public void Delete()
        {
            if (!Database.Exists(_connectionString)) return;
            using (var ctx = CreateContext())
            {
                ctx.Database.Delete();
            }
        }

        /// <summary>
        /// Creates a connection string for the local database.
        /// </summary>
        /// <returns></returns>
        public static string CreateConnectionString()
        {
            return CreateConnectionString($"{Guid.NewGuid():N}");
        }

        /// <summary>
        /// Creates a connection string for the local database.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string CreateConnectionString(string name)
        {
            var dataDirectoryPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Settings.Default.DataDirectory));

            return string.Format(Settings.Default.ConnectionStringTemplate, dataDirectoryPath, $"CJGTest_{name}");
        }
        #endregion
    }
}
