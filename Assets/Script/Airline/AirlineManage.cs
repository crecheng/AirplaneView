using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace AirplaneView
{
    public class AirlineManage
    {
        private static Dictionary<long, Airline> _data;

        public const string StartString = "airline";

        public static bool IsRun = false;

        public static AirlineManage Instance { get; } = new AirlineManage();

        public static float UseTime = 0f;
        private AirlineManage()
        {
            _data = new Dictionary<long, Airline>();
        }
        
        public static void Init(GameObject pre, Transform parent)
        {
            AirlinePool.Prefab = pre;
            AirlinePool.Parent = parent;
        }

        public static void SetZoom(float zoom)
        {
            if (zoom<0.5F)
            {
                return;
            }
            foreach (var airline in _data)
            {
                airline.Value.SetZoom(zoom);
            }
        }

        public void Update(DateTime time, bool flag=true)
        {
            if(!flag)
                return;
            List<long> hide = new List<long>();
            foreach (var airline in _data)
            {
                if(airline.Value.ArriveTime < time)
                    hide.Add(airline.Key);
            }
            hide.ForEach(Unload);
        }

        public static IEnumerator Show(string keyword,int page)
        {
            IsRun = true;
            List<AirlineData> list = null;
            Task.Run( async () =>
            {
                list = await AirlineDataManage.GetByDeparture(keyword, page);
            });
            while (list==null)
            {
                yield return null;
            }
            foreach (var data in list)
            {
                if (_data.ContainsKey(data.Lid))
                    continue;
                var l = new Airline(data, AirlinePool.Get().GetComponent<LineRenderer>());
                _data.Add(data.Lid, l);
                l.SetShow(true);
                l.LineRenderer.gameObject.name = StartString + ":" + data.Lid;
            }
            IsRun = false;
        }

        public static IEnumerator Show(DateTime time, int page, bool showAirplane = false)
        {
            IsRun = true;
            long beginTime = DateTime.Now.Ticks;
            List<AirlineData> list = null;
            Task.Run(async () =>
            {
                list = await AirlineDataManage.GetByTime(time, page);
            });
            while (list == null)
            {
                yield return null;
            }

            int i = 0;
            foreach (var data in list)
            {
                if (_data.ContainsKey(data.Lid))
                {
                    if (showAirplane)
                    {
                        AirplaneManage.Instance.ShowAirPlane(_data[data.Lid], time);
                    }
                    continue;
                }
                var l = new Airline(data, AirlinePool.Get().GetComponent<LineRenderer>());
                _data.Add(data.Lid, l);
                l.SetShow(true);
                l.LineRenderer.gameObject.name = StartString + ":" + data.Lid;
                i++;
                if (i >= 200)
                {
                    i = 0;
                    yield return null;
                }
                if (showAirplane)
                {
                    AirplaneManage.Instance.ShowAirPlane(l,time);
                }
            }
            
            var EndTime=DateTime.Now.Ticks;
            var useTime = EndTime - beginTime;
            UseTime = useTime / 100000f;
            AirplaneManage.Instance.SetZoom(Main.Instance.zoom);
            Main.Refresh();
            IsRun = false;
        }

        public static void Unload(long id)
        {
            if (_data.TryGetValue(id, out Airline data))
            {
                AirlinePool.Back(data.LineRenderer.gameObject);
                AirplaneManage.Instance.Unload(data.Data.Aid);
                _data.Remove(id);
            }
        }
    }
}