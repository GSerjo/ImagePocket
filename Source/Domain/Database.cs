using System;
using SQLite;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain
{
	public sealed class Database : SQLiteAsyncConnection
	{
		private static readonly Database _database;

		static Database()
		{
			_database = new Database(GetDatabasePath());
		}

		public static List<T> GetAll<T>()
			where T: new()
		{
			return _database.Table<T> ().ToListAsync ().Result;
		}

		public static Task<List<T>> GetAllAsync<T>()
			where T: new()
		{
			return _database.Table<T> ().ToListAsync ();
		}

		private Database(string databasePath) : base(databasePath)
		{
			CreateTablesAsync<TagEntity,ImageEntity>().Wait();
		}

		private static string GetDatabasePath()
		{
			var documentPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			var libraryPath = Path.Combine (documentPath, "../Library/");
			return Path.Combine (libraryPath, "Nelibur.db");
		}
	}
}

