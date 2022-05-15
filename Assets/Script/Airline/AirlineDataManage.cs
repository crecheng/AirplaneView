using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace AirplaneView
{
    public static class AirlineDataManage
    {

        /*
        #region SQL

        public static int LimitCount = 500;

        public static async Task<bool> Insert(AirlineData data)
        {
            return await MySqlHelper.Insert(data);
        }

        public static async Task<int> Insert(List<AirlineData> list)
        {
            return await MySqlHelper.Insert(list);
        }

        public static async Task<AirlineData> GetByLid(long lid)
        {
            var l = await MySqlHelper.Select<AirlineData>("lid=" + lid);
            return l?.Count > 0 ? l[0] : null;
        }

        public static async Task<List<AirlineData>> GetByAid(int aid, int page)
        {
            return await MySqlHelper.Select<AirlineData>("aid=" + aid, LimitCount, page * LimitCount);
        }

        public static async Task<List<AirlineData>> GetByArrive(string code, int page)
        {
            return await MySqlHelper.Select<AirlineData>("arrive like " + code, LimitCount, page * LimitCount);
        }

        public static async Task<List<AirlineData>> GetByDeparture(string code, int page)
        {
            return await MySqlHelper.Select<AirlineData>($"departure like '{code}'", LimitCount, page * LimitCount);
        }

        public static async Task<List<AirlineData>> GetByTime(DateTime time, int page)
        {
            long timestamp = Tool.Timestamp(time);
            return await MySqlHelper.Select<AirlineData>($"departuretime<={timestamp} and arrivetime>={timestamp}",
                LimitCount, page * LimitCount);
        }

        public static async Task<List<AirlineData>> GetByTimeAll(DateTime time)
        {

            long timestamp = Tool.Timestamp(time);
            List<AirlineData> list = new List<AirlineData>();
            for (int i = 0; i < 100000; i++)
            {
                var l = await MySqlHelper.Select<AirlineData>($"departuretime<={timestamp} and arrivetime>={timestamp}",
                    LimitCount, i * LimitCount);
                if (l == null)
                    break;

                list.AddRange(l);
                if (l.Count < LimitCount)
                    break;
            }

            return list;
        }

        #endregion
        */


        #region Test

        private static Dictionary<long, AirlineData> __data;

        public static Dictionary<long, AirlineData> _data
        {
            get
            {
                if (__data == null)
                {
                    __data = new Dictionary<long, AirlineData>();
                    var datas = Resources.Load<TextAsset>("test").text.Split('\n');
                    foreach (var data in datas)
                    {
                        var line = new AirlineData(data);
                        __data.Add(line.Lid,line);
                    }
                }
                return __data;
                
                
            }
            
    }
        
        public static int LimitCount = 500;

        public static  async Task<AirlineData> GetByLid(long lid)
        {
            AirlineData result = null;
            await Task.Run(() =>
            {
                result = _data.ContainsKey(lid) ? _data[lid] : null;

            });
            
            return result;
        }

        public static async Task<List<AirlineData>>  GetByAid(int aid,int page)
        {
            List<AirlineData> result = new List<AirlineData>();
            await Task.Run(() =>
            {
                foreach (var data in _data)
                {
                    if (data.Value.Aid == aid)
                        result.Add(data.Value);
                }
            });
            return result;
        }

        public static async Task<List<AirlineData>> GetByArrive(string code, int page)
        {
            List<AirlineData> result = new List<AirlineData>();
            await Task.Run(() =>
            {
                foreach (var data in _data)
                {
                    if (data.Value.Arrive == code)
                        result.Add(data.Value);
                }
            });
            return result;
        }
        
        public static async Task<List<AirlineData>> GetByDeparture(string code, int page)
        {
            List<AirlineData> result = new List<AirlineData>();
            await Task.Run(() =>
            {
                foreach (var data in _data)
                {
                    if (data.Value.Departure == code)
                        result.Add(data.Value);
                }
            });
            return result;
        }
        
        public static async Task<List<AirlineData>> GetByTime(DateTime time, int page)
        {
            long timestamp = MyTool.Timestamp(time);
            List<AirlineData> result = new List<AirlineData>();
            await Task.Run(() =>
            {
                foreach (var data in _data)
                {
                    if (data.Value.DepartureTime <= timestamp && data.Value.ArriveTime>=timestamp)
                        result.Add(data.Value);
                }
            });
            return result;
        }
        
        public static async Task<List<AirlineData>> GetByTimeAll(DateTime time)
        {
            
            long timestamp = MyTool.Timestamp(time);
            List<AirlineData> result = new List<AirlineData>();
            await Task.Run(() =>
            {
                foreach (var data in _data)
                {
                    if (data.Value.DepartureTime <= timestamp && data.Value.ArriveTime>=timestamp)
                        result.Add(data.Value);
                }
            });
            return result;
        }

        #endregion




    }


}
