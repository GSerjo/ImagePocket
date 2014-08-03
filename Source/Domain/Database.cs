using System;
using SQLite;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core;
using System.Collections;
using System.Linq;

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
			AddOrUpdate (TagEntity.All)
				.ContinueWith (x => AddOrUpdate (TagEntity.Untagged))
				.ContinueWith(x => 5.Times(y => AddOrUpdate(new TagEntity { Name = "MyTag" + y})));
		}

		public static List<T> GetAll<T>()
			where T: new()
		{
			return _database.Table<T> ().ToListAsync ().Result;
		}

		public static Task<int> AddOrUpdate<T>(T value)
			where T: Entity
		{
			if (value.New)
			{
				return _database.InsertAsync (value);
			}
			return _database.UpdateAsync (value);
		}

		public static Task AddOrUpdateAll<T>(IList<T> values)
			where T : Entity
		{
			IEnumerable<IGrouping<bool, T>> groups = values.GroupBy (x => x.New);
			foreach (IGrouping<bool, T> group in groups)
			{
				if (group.Key)
				{
					_database.InsertAllAsync (values.ToList ()).Wait();
				}
				else
				{
					_database.UpdateAllAsync (values.ToList ()).Wait();
				}
			}
			return Task.FromResult (true);
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