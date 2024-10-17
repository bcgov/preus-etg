using CJG.Infrastructure.EF.Helpers;
using System;
using System.ComponentModel;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace CJG.Infrastructure.EF
{
	/// <summary>
	/// <typeparamref name="ExtendedDbMigration"/> abstract class, provides additional behaviour for the <typeparamref name="DbMigration"/> objects.
	/// </summary>
	public abstract class ExtendedDbMigration : DbMigration
	{
		#region Variables
		internal static string MigrationScriptsPath;
		internal static bool SeedTestData = true;
		#endregion

		#region Constructors
		static ExtendedDbMigration()
		{
			// MigrationScriptsPath = $"{AppDomain.CurrentDomain.BaseDirectory}..\\..\\..\\CJG.Infrastructure.DB\\Migrations\\";
			MigrationScriptsPath = $"{AppDomain.CurrentDomain.BaseDirectory}\\Migrations\\";
		}
		#endregion

		#region Methods
		/// <summary>
		/// Drop the constraint from the datasource for the specified column name on the specified table.
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="columnName"></param>
		protected void DropConstraint(string tableName, string columnName)
		{
			if (String.IsNullOrWhiteSpace(tableName))
				throw new ArgumentException($"Argument {nameof(tableName)} cannot be null, empty or whitespace.", nameof(tableName));

			if (String.IsNullOrWhiteSpace(columnName))
				throw new ArgumentException($"Argument {nameof(columnName)} cannot be null, empty or whitespace.", nameof(columnName));

			Sql($@"DECLARE @var0 nvarchar(128)
              SELECT @var0 = name
              FROM sys.default_constraints
              WHERE parent_object_id = object_id(N'{tableName}')
              AND col_name(parent_object_id, parent_column_id) = '{columnName}';
              IF @var0 IS NOT NULL
              EXECUTE('ALTER TABLE [{tableName}] DROP CONSTRAINT [' + @var0 + ']')");
		}

		/// <summary>
		/// Drop the constraint from the datasource with the specified name on the specified table.
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="constraintName"></param>
		protected void DropConstraintByName(string tableName, string constraintName)
		{
			if (String.IsNullOrWhiteSpace(tableName))
				throw new ArgumentException($"Argument {nameof(tableName)} cannot be null, empty or whitespace.", nameof(tableName));

			if (String.IsNullOrWhiteSpace(constraintName))
				throw new ArgumentException($"Argument {nameof(constraintName)} cannot be null, empty or whitespace.", nameof(constraintName));

			Sql($@"ALTER TABLE {tableName} DROP CONSTRAINT {constraintName}");
		}

		/// <summary>
		/// Get the migration name, which is either the class name or the <typeparamref name="DescriptionAttribute"/> value.
		/// </summary>
		/// <returns></returns>
		private string GetMigrationName()
		{
			var description = this.GetType().GetCustomAttribute<DescriptionAttribute>();
			return description == null ? this.GetType().Name : description.Description;
		}

		/// <summary>
		/// Run the pre-deployment scripts.
		/// </summary>
		protected void PreDeployment()
		{
			DeployScripts("PreDeploy", true, $"{MigrationScriptsPath}\\", $"{MigrationScriptsPath}\\_PreDeploy\\");
		}

		/// <summary>
		/// Run the pre-post-deployment scripts.
		/// These are scripts that must be run after certain database changes are made, but before all of them are made.
		/// </summary>
		/// <example>
		///     If you delete a primary key and create a new one it requires the primary key to be delete a new column added and then a script before the new primary key is created.
		/// </example>
		protected void PrePostDeployment()
		{
			DeployScripts("PrePostDeploy", false, $"{MigrationScriptsPath}\\", $"{MigrationScriptsPath}\\_PostDeploy\\");
		}

		/// <summary>
		/// Run the post-deployment scripts.
		/// </summary>
		protected void PostDeployment()
		{
			DeployScripts("PostDeploy", true, $"{MigrationScriptsPath}\\", $"{MigrationScriptsPath}\\_PostDeploy\\");
		}

		/// <summary>
		/// Run the pre-Undeployment scripts.
		/// These are scripts that must be run before making the database changes for the DOWN method.
		/// </summary>
		/// <example>
		///     If you have added data which cannot be supported by the older database format, it must be deleted or altered here.
		/// </example>
		protected void PreUndeployment()
		{
			DeployScripts("PreUndeploy", false, $"{MigrationScriptsPath}\\", $"{MigrationScriptsPath}\\_PreUndeploy\\");
		}


		/// <summary>
		/// Run the deployment scripts in the specified folder.
		/// </summary>
		/// <param name="folderName"></param>
		/// <param name="debug"></param>
		/// <param name="initScriptsPath"></param>
		protected void DeployScripts(string folderName, bool debug = false, params string[] initScriptsPath)
		{
			var scriptsPath = $"{MigrationScriptsPath}\\{GetMigrationName()}\\{folderName}\\Seed";

			ExecuteScripts(scriptsPath, initScriptsPath);

#if (DEBUG || QA || TEST)
			if (SeedTestData && debug) ExecuteScripts(Regex.Replace(scriptsPath, @"\bSeed\b$", "TestSeed"), initScriptsPath);
#endif
		}

        /// <summary>
        /// Creates a nonclustered index with an include.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="columns"></param>
        /// <param name="includes"></param>
        /// <param name="name"></param>
        protected void CreateIndexWithInclude(string table, string[] columns, string[] includes, string name)
        {
            Sql($"CREATE NONCLUSTERED INDEX [{name}] ON {table} ([{String.Join("],[", columns)}]) INCLUDE ([{String.Join("],[", includes)}])");
        }

        /// <summary>
        /// Seed the database with scripts at the specified path.
        /// </summary>
        /// <param name="scriptsPath">The folder to the SQL scripts to execute.</param>
        /// <param name="initScriptsPath">The folder to the SQL init scripts to call before executing all other scripts.</param>
        private void ExecuteScripts(string scriptsPath, params string[] initScriptsPath)
		{
			// Initialize application data.
			Sql($"PRINT '---------------> Starting {scriptsPath}'");

			var setup = new StringBuilder();
			if (initScriptsPath != null && initScriptsPath.Length > 0)
			{
				foreach (var path in initScriptsPath.Where(p => !String.IsNullOrEmpty(p)))
				{
					// Get setup scripts.
					foreach (var sql in this.GetSeedScripts(path))
					{
						setup.AppendLine(sql.Value);
					}
				}
			}

			foreach (var sql in this.GetSeedScripts(scriptsPath))
			{
				Sql($"{setup}{Environment.NewLine}{sql.Value}");
			}
		}
		#endregion
	}
}
