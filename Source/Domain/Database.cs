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
			InitialiseTables ();
		}

		private Database(string databasePath) : base(databasePath)
		{
			CreateTablesAsync<TagEntity,ImageEntity> ()
				.Wait();
		}

		private static void InitialiseTables ()
		{
			var count = _database.Table<TagEntity> ().CountAsync ().Result;
			if (count > 0)
			{
				return;
			}
			Add (TagEntity.All)
				.ContinueWith (x => Add (TagEntity.Untagged))
				.ContinueWith(x => Add(new TagEntity { Name = "MyTag"}));
		}

		public static List<T> GetAll<T>()
			where T: new()
		{
			return _database.Table<T> ().ToListAsync ().Result;
		}

		public static Task<int> Add(object value)
		{
			return _database.InsertAsync (value);
		}

		public static Task<List<T>> GetAllAsync<T>()
			where T: new()
		{
			return _database.Table<T> ().ToListAsync ();
		}

		private static string GetDatabasePath()
		{
			var documentPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			return Path.Combine (documentPath, "NeliburPocket.db");
		}
	}
}