using System;
using UnityEngine;

namespace AirplaneView
{
    public class Airplane 
    {
        public double X;
        public double Y;
        public string Type;
        public float RealSpeed;
        public float ExpectedSpeed;
        public Airline Airline;
        public GameObject GameObject;
        public Vector3 Pos;
        public Vector3 Direct;
        public Vector2 Local;
        
        public int Aid;

        public Airplane()
        {
            
        }

        public Airplane(Airline airline)
        {
            Airline = airline;
            Aid = airline.Data.Aid;
            AirplaneManage.Instance.Add(this);
        }
        
        public Airplane(Airline airline,DateTime dateTime):this(airline)
        {
            long time = airline.Data.ArriveTime - airline.Data.DepartureTime;
            long now = MyTool.Timestamp(dateTime)-airline.Data.DepartureTime;
            float p = 1f * now / time;
            Vector3 begin = MapManage.GetPos(airline.Departure);
            Vector3 end = MapManage.GetPos(airline.Arrive);
            Vector3 len = end - begin;
            Vector3 nowLne = len * p;
            Pos = begin + nowLne;
            Direct = len.normalized;
            Local = MapManage.GetPos(Pos);
        }

        public Airplane(Airline airline, DateTime dateTime, GameObject gameObject) : this(airline, dateTime)
        {
            GameObject = gameObject;
            gameObject.name = AirplaneManage.StartString + ":" + Aid;
            gameObject.transform.position = Pos;
            gameObject.transform.LookAt(Pos+Direct);
        }

        public MyResult.Item GetItem()
        {
            return new MyResult.Item()
            {
                Data = this,
                Name = this.GetButtonString(),
                Action = (d) =>
                {
                    var o = d as Airplane;
                    if(o==null)
                        return;
                    var c = GameObject.Find("Main Camera").transform;
                    c.position = new Vector3(o.GameObject.transform.position.x,o.GameObject.transform.position.y,c.position.z);
                    Main.Instance.SelectObject(GameObject.transform, MyTool.AirplaneInt, true);
                    Main.Instance.ShowData(o);
                }
            };
        }

        public string GetButtonString()
        {
            return $"{Aid}-{AirlineData.GetId(Airline.Data.Id)}";
        }

        public void SetPos(DateTime dateTime)
        {
            long time = Airline.Data.ArriveTime - Airline.Data.DepartureTime;
            long now = MyTool.Timestamp(dateTime) - Airline.Data.DepartureTime;
            float p = 1f * now / time;
            Vector3 begin = MapManage.GetPos(Airline.Departure);
            Vector3 end = MapManage.GetPos(Airline.Arrive);
            Vector3 len = end - begin;
            Vector3 nowLne = len * p;
            Pos = begin + nowLne;
            Direct = len.normalized;
            Local = MapManage.GetPos(Pos);
            GameObject.transform.position = Pos;
            GameObject.transform.LookAt(Pos + Direct);
        }

        public override string ToString()
        {
            return $"{Airline}\nid：{Aid}\n出发时间：{Airline.DepartureTime}\n出发地点：{Airline.Departure.Name}\n" +
                   $"到达时间：{Airline.ArriveTime}\n到达地点：{Airline.Arrive.Name}" +
                   $"\n预计剩余时间：{(Airline.ArriveTime-Main.Instance.MapTime):g}\n" +
                   $"当前位置\n纬度：{Local.x}\n经度：{Local.y}";
        }
    }
}