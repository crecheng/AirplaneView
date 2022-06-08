using System;
using System.Collections.Generic;
using UnityEngine;

namespace AirplaneView
{
    public class AirplaneManage
    {
        private Dictionary<int, Airplane> _data;

        public const string StartString = "airplane";
        
        public const float MaxScale = 60f;
        
        public static AirplaneManage Instance { get; }=new AirplaneManage();

        private AirplaneManage()
        {
            _data = new Dictionary<int, Airplane>();
        }
        
        public static void Init(GameObject pre, Transform parent)
        {
            AirplanePool.Prefab = pre;
            AirplanePool.Parent = parent;
        }

        public bool Add(Airplane airplane)
        {
            if (_data.ContainsKey(airplane.Aid))
            {
                return false;
            }
            _data.Add(airplane.Aid,airplane);
            return true;
        }

        
        public void SetZoom(float zoom)
        {
            if (zoom < 0.3f)
                zoom = 0.3f;
            zoom = MaxScale * (zoom / 100f);
            foreach (var gameObject in _data)
            {
                gameObject.Value.GameObject.transform.localScale = new Vector3(zoom, zoom, zoom);
            }
        }
        
        public void ShowAirPlane(Airline airline, DateTime time)
        {
            if (airline == null|| !airline.Data.Plan)
                return;
            var airplane = Instance.Get(airline);
            if (airplane.GameObject == null)
            {
                airplane.GameObject = AirplanePool.Get();
                airplane.GameObject.name = AirplaneManage.StartString + ":" + airplane.Aid;
            }

            airplane.SetPos(time);
            if(!_data.ContainsKey(airplane.Aid))
                _data.Add(airplane.Aid,airplane);
            airplane.GameObject.SetActive(true);

        }

        public void Unload(int id)
        {
            var d = Get(id);
            if(d==null)
                return;
            AirplanePool.Back(d.GameObject);
            _data.Remove(id);
        }

        public Airplane Get(int id)
        {
            return _data.ContainsKey(id) ? _data[id] : null;
        }

        public Airplane Get(string id)
        {
            long i = AirlineData.GetId(id);
            foreach (var airplane in _data)
            {
                if (airplane.Value.Airline.Data.Id == i)
                    return airplane.Value;
            }

            return null;
        }

        public List<Airplane> Get(int arriveId, int departureId)
        {
            List<Airplane> list = new List<Airplane>();
            foreach (var airplane in _data)
            {
                try
                {
                    int a = airplane.Value.Airline.Arrive.Id;
                    int d = airplane.Value.Airline.Departure.Id;
                    if (arriveId == -1 && departureId == d)
                        list.Add(airplane.Value);
                    else if (departureId == -1 && arriveId == a)
                        list.Add(airplane.Value);
                    else if(a==arriveId && d==departureId)
                        list.Add(airplane.Value);
                }
                catch (Exception e)
                {
                    Debug.Log("Data not find!");
                }

            }
            return list;
        }
        
        public Airplane Get(Airline line)
        {
            var airplane = Get(line.Data.Aid) ?? new Airplane(line);
            return airplane;
        }

    }
}