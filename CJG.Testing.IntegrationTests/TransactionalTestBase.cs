using System.Data.Entity;
using CJG.Infrastructure.EF;
using CJG.Infrastructure.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog;

namespace CJG.Testing.IntegrationTests
{
    /// <summary>
    /// <typeparamref name="TransactionalTestBase"/> abstract class, provides a way to create and destroy a local database for unit tests.
    /// </summary>
    [TestClass]
    public abstract class TransactionalTestBase
    {
        #region Variables
        private static readonly bool _deleteLocalDb = true;

        private static LocalDatabase _localDb;
        #endregion

        #region Properties
        /// <summary>
        /// get/set - The current DbContext being executed.
        /// </summary>
        protected IDataContext Context { get; set; }

        /// <summary>
        /// get/set - The test transaction wrapper, if PersistDataChanges = false.
        /// </summary>
        protected DbContextTransaction Transaction { get; set; }

        /// <summary>
        /// get/set - Whether the test will persist its db changes after it completes.
        /// </summary>
        protected bool PersistDataChanges { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="TransactionalTestBase"/> object.
        /// </summary>
        /// <param name="persistDataChanges"></param>
        protected TransactionalTestBase(bool persistDataChanges = true)
        {
            LogManager.Configuration = new NLog.Config.LoggingConfiguration();
            this.PersistDataChanges = persistDataChanges;
        }
        #endregion

        #region Methods

        [AssemblyInitialize]
        public static void InitializeDatabase(TestContext testContext)
        {
            ExtendedDbMigration.SeedTestData = false;
            ExtendedDbMigration.MigrationScriptsPath = @"..\..\..\CJG.Infrastructure.DB\Migrations\";

            _localDb = new LocalDatabase(LocalDatabase.CreateConnectionString());
            _localDb.Create();
        }

        [TestInitialize]
        public void InitializeContext()
        {
            Context = _localDb.CreateContext();

            if(!this.PersistDataChanges)
            {
                Transaction = Context.Database.BeginTransaction();
            }
        }

        [TestCleanup]
        public void Dispose()
        {
            if (!this.PersistDataChanges)
            {
                Transaction.Rollback();
                Transaction.Dispose();
            }
            
            Context.Dispose();
        }

        [AssemblyCleanup]
        public static void Cleanup()
        {
            if(_deleteLocalDb)
            {
                _localDb?.Delete();
            }
        }

        public static IDataContext CreateContext()
        {
            return _localDb.CreateContext();
        }

        #endregion
    }
}