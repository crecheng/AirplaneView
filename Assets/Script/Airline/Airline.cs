using System;
using UnityEngine;

namespace AirplaneView
{
    public class Airline
    {
        public LineRenderer LineRenderer;
        public AirportData Departure;
        public AirportData Arrive;
        public AirlineData Data;
        public DateTime DepartureTime { get; }
        public DateTime ArriveTime { get; }
        public DateTime PlanDepartureTime { get; }
        public DateTime PlanArriveTime { get; }


        public Airline()
        {
            
        }

        public Airline(AirlineData data)
        {
            Data = data;
            DepartureTime = MyTool.Timestamp(data.DepartureTime);
            ArriveTime = MyTool.Timestamp(data.ArriveTime);
            PlanDepartureTime = MyTool.Timestamp(data.PlanDepartureTime);
            PlanArriveTime = MyTool.Timestamp(data.PlanArriveTime);
            Departure = AirportManage.GetAndShow(data.Departure);
            Arrive = AirportManage.GetAndShow(data.Arrive);
        }

        public Airline(AirlineData data, LineRenderer lineRenderer) : this(data)
        {
            LineRenderer = lineRenderer;
            LineRenderer.SetPosition(0,MapManage.GetPos(Departure));
            LineRenderer.SetPosition(1,MapManage.GetPos(Arrive));
        }

        public void SetShow(bool active)
        {
            if(LineRenderer==null)
                return;
            LineRenderer.gameObject.SetActive(active);
        }

        public void SetZoom(float zoom)
        {
            if(LineRenderer==null)
                return;
            LineRenderer.endWidth =  zoom/100;
            LineRenderer.startWidth =  zoom/100;
        }

        public override string ToString()
        {
            return $"航班id：{Data.Lid}\n" +
                   $"航班号：{AirlineData.GetId(Data.Id)}";
        }
    }
}