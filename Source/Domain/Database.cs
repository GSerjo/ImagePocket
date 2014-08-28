using System;
using SQLite;
using System.IO;
using System.Collections.Generic;
using Core;
using System.Collections;
using System.Linq;

namespace Domain
{
	public sealed class Database : SQLiteConnection
	{
		private static readonly Database _database;
		private static readonly object _locker = new object();

		static Database()
		{
			_database = new Database(GetDatabasePath());
		}

		private Database(string databasePath) : base(databasePath)
		{
			CreateTable<TagEntity> ();
			CreateTable<ImageEntity> ();
		}

		public static List<T> GetAll<T>()
			where T: new()
		{
			lock (_locker)
			{
				return _database.Table<T> ().ToList ();
			}
		}

		public static int AddOrUpdate<T>(T value)
			where T: Entity
		{
			lock (_locker)
			{
				if (value.New) {
					return _database.Insert (value);
				}
				return _database.Update (value);
			}
		}

		public static void AddOrUpdateAll<T>(IList<T> values)
			where T : Entity
		{
			lock (_locker)
			{
				List<IGrouping<bool, T>> groups = values.GroupBy (x => x.New).ToList ();
				foreach (IGrouping<bool, T> group in groups)
				{
					if (group.Key)
					{
						_database.InsertAll (group.ToList ());
					}
					else
					{
						_database.UpdateAll (group.ToList ());
					}
				}
			}
		}

		public static List<T> GetAllAsync<T>()
			where T: new()
		{
			lock (_locker)
			{
				return _database.Table<T> ().ToList ();
			}
		}

		private static string GetDatabasePath()
		{
			var documentPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			return Path.Combine (documentPath, "NeliburPocket.db");
		}
	}
}