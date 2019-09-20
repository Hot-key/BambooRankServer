using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BambooRest.DataType;
using LiteDB;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BambooRest
{
    public static class DataBase
    {
        public static void InitDataBase()
        {
            _database = new LiteDatabase(@"RankData.db");
            _rankCollection = _database.GetCollection<Rank>("ranks");
        }

        public static void Insert(Rank data)
        {
            lock (_databaseLock)
            {
                _rankCollection.Insert(data);
            }
        }

        public static void Update(Rank data)
        {
            lock (_databaseLock)
            {
                _rankCollection.Update(data);
            }
        }

        public static IEnumerable<Rank> Select(Expression<Func<Rank,bool>> selectAction)
        {
            lock (_databaseLock)
            {
                return _rankCollection.Find(selectAction);
            }
        }

        public static IEnumerable<Rank> Find(Query selectAction)
        {
            lock (_databaseLock)
            {
                return _rankCollection.Find(selectAction);
            }
        }

        public static object LockDataBase()
        {
            return _databaseLock;
        }

        public static LiteCollection<Rank> Query()
        {
            return _rankCollection;
        }

        private static LiteDatabase _database;
        private static object _databaseLock = new object();
        private static LiteCollection<Rank> _rankCollection;
    }
}
