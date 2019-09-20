using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BambooRest.DataType;
using LiteDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json.Linq;

namespace BambooRest.Controllers
{
    [ApiController]
    public class RankController : ControllerBase
    {
        // GET api/values
        [Route("set/rank")]
        [HttpPost]
        public ActionResult<string> SetRank(RankModel data)
        {
            bool isUpdate = false;

            lock (DataBase.LockDataBase())
            {
                isUpdate = DataBase.Query().Exists(s => s.Id == data.Id);
            }

            if (isUpdate)
            {
                Rank myData = DataBase.Select(s => s.Id == data.Id).First();
                if (myData.Score < data.Score)
                {
                    DataBase.Update(new Rank()
                    {
                        Id = data.Id,
                        Name = data.Name,
                        Score = data.Score
                    });
                }
            }
            else
            {
                DataBase.Insert(new Rank()
                {
                    Id = data.Id,
                    Name = data.Name,
                    Score = data.Score
                });
            }

            return Ok();
        }

        // GET api/values/5
        [Route("get/rank")]
        [HttpPost]
        public ActionResult<string> GetRank(int id)
        {
            JObject rankJson = new JObject();
            lock(DataBase.LockDataBase())
            {
                var queryData = DataBase.Query().Find(Query.All("score", 1)).Take(10).ToList();
                int index = 0;

                rankJson.Add($"count", queryData.Count);

                foreach (var rank in queryData)
                {
                    rankJson.Add($"name{index}", rank.Name);
                    rankJson.Add($"score{index}", rank.Score);
                    rankJson.Add($"id{index}", rank.Id);
                    index++;
                }
            }
            return rankJson.ToString();
        }

        // POST api/values
        [Route("get/myrank")]
        [HttpPost]
        public ActionResult<string> GetMyRank(MyRankModel data)
        {
            JObject rankJson = new JObject();
            lock (DataBase.LockDataBase())
            {
                var queryData = DataBase.Query().Find(Query.All("score", 1)).OrderByDescending(s=>s.Score).ToList();

                int myRank = queryData.FindIndex(0, s => s.Id == data.Id);

                int index = 0;

                if (myRank == -1)
                {
                    return Ok();
                }

                if (myRank == 0) // 1등
                {
                }
                else if(myRank == 1) // 2등
                {
                    rankJson.Add($"name{index}", queryData[myRank - 1].Name);
                    rankJson.Add($"score{index}", queryData[myRank - 1].Score);
                    rankJson.Add($"id{index}", queryData[myRank - 1].Id);
                    index++;
                }
                else // 킹반인
                {

                    rankJson.Add($"name{index}", queryData[myRank - 2].Name);
                    rankJson.Add($"score{index}", queryData[myRank - 2].Score);
                    rankJson.Add($"id{index}", queryData[myRank - 2].Id);
                    index++;

                    rankJson.Add($"name{index}", queryData[myRank - 1].Name);
                    rankJson.Add($"score{index}", queryData[myRank - 1].Score);
                    rankJson.Add($"id{index}", queryData[myRank - 1].Id);
                    index++;
                }

                rankJson.Add($"name{index}", queryData[myRank].Name); // 본인 점수
                rankJson.Add($"score{index}", queryData[myRank].Score);
                rankJson.Add($"id{index}", queryData[myRank].Id);
                index++;

                if (myRank == queryData.Count - 1) // 뒤에서 1등
                {
                }
                else if (myRank == queryData.Count - 2) // 뒤에서 2등
                {
                    rankJson.Add($"name{index}", queryData[myRank + 1].Name);
                    rankJson.Add($"score{index}", queryData[myRank + 1].Score);
                    rankJson.Add($"id{index}", queryData[myRank + 1].Id);
                    index++;
                }
                else // 킹반인
                {

                    rankJson.Add($"name{index}", queryData[myRank + 1].Name);
                    rankJson.Add($"score{index}", queryData[myRank + 1].Score);
                    rankJson.Add($"id{index}", queryData[myRank + 1].Id);
                    index++;

                    rankJson.Add($"name{index}", queryData[myRank + 2].Name);
                    rankJson.Add($"score{index}", queryData[myRank + 2].Score);
                    rankJson.Add($"id{index}", queryData[myRank + 2].Id);
                    index++;
                }
                rankJson.Add("count", index);
            }
            return rankJson.ToString();
        }

        [Route("get/myrank")]
        [HttpGet]
        public ActionResult<string> Clear()
        {
            lock (DataBase.LockDataBase())
            {
                DataBase.Query().Delete(s => true);
            }

            return Ok();
        }


        public  class RankModel
        {
            public string Id { get; set; }

            public string Name { get; set; }

            public int Score { get; set; }
        }

        public class MyRankModel
        {
            public string Id { get; set; }
        }
    }
}
