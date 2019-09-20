using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;

namespace BambooRest.DataType
{
    public class Rank
    {
        [BsonId]
        public string Id { get; set; }

        public string Name { get; set; }

        public int Score { get; set; }
    }
}

