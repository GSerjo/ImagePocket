using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core;
using SQLite;

namespace Domain
{
    /// Users/Serjo/Library/Developer/CoreSimulator/Devices/DDEF529C-DC65-4447-8317-FE1F42CE3EEE/data/Containers/Data/Application/2AD180F3-A827-49A5-BA04-0C8289E6F9FC/Documents/NeliburPocket.db
    public sealed class Database : SQLiteConnection
    {
        private static readonly Database _database;
        private static readonly object _locker = new object();

        static Database()
        {
            string databasePath = GetDatabasePath();
            _database = new Database(databasePath);
        }

        private Database(string databasePath) : base(databasePath)
        {
            CreateTable<TagEntity>();
            CreateTable<ImageEntity>();
        }

        private static string DocumentPath
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); }
        }

        private static string LibraryPath
        {
            get { return Path.Combine(RootPath, "Library"); }
        }

        private static string RootPath
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), ".."); }
        }

        private static string TempPath
        {
            get { return Path.Combine(RootPath, "tmp"); }
        }

        //        public static int AddOrUpdate<T>(T value)
        //            where T : Entity
        //        {
        //            lock (_locker)
        //            {
        //                if (value.New)
        //                {
        //                    return _database.Insert(value);
        //                }
        //                return _database.Update(value);
        //            }
        //        }

        public static void AddOrUpdate<T>(IList<T> values)
            where T : Entity
        {
            lock (_locker)
            {
                List<IGrouping<bool, T>> groups = values.GroupBy(x => x.New).ToList();
                foreach (IGrouping<bool, T> group in groups)
                {
                    if (group.Key)
                    {
                        _database.InsertAll(group.ToList());
                    }
                    else
                    {
                        _database.UpdateAll(group.ToList());
                    }
                }
            }
        }

        public static List<T> GetAll<T>()
            where T : new()
        {
            lock (_locker)
            {
                return _database.Table<T>().ToList();
            }
        }

        public static List<T> GetAllAsync<T>()
            where T : new()
        {
            lock (_locker)
            {
                return _database.Table<T>().ToList();
            }
        }

        public static void Remove<T>(IList<T> values)
        {
            lock (_locker)
            {
                values.Iter(x => _database.Delete(x));
            }
        }

        private static string GetDatabasePath()
        {
            return Path.Combine(DocumentPath, "ImagePocket_B326EC74.db");
        }
    }
}
