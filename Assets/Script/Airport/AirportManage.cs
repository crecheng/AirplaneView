using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AirplaneView
{
    public class AirportManage
    {
        private static Dictionary<AirportType, Dictionary<int,AirportData>> _data;

        private static Dictionary<int,GameObject> _go;

        private static Dictionary<string, AirportData> _keyword;

        private static Dictionary<AirportType, SourceData<List<AirportData>>> _preLoad;

        public const string StartString = "airport";

        public bool[] ShowAirport;
        public const float MaxScale = 30f;

        private AirportManage()
        {
            ShowAirport = new bool[10];
            _data = new Dictionary<AirportType, Dictionary<int,AirportData>>();
            _go = new Dictionary<int, GameObject>();
            _keyword = new Dictionary<string, AirportData>();
            _preLoad = new Dictionary<AirportType, SourceData<List<AirportData>>>();
            InitKeyword();
        }

        public AirportData this[string key] => _keyword.ContainsKey(key) ? _keyword[key] : null;

        public static AirportManage Instance { get; } = new AirportManage();

        public static void Load(AirportType type)
        {
            if(_data.ContainsKey(type))
                return;
            if (!LoadData(type))
            {
                Debug.Log($"{type} 机场数据加载错误!");
                return;
            }

            foreach (var data in _data[type])
            {
                GameObject gameObject = AirportPool.Get();
                gameObject.transform.position = MapManage.GetPos(data.Value.Latitude_deg, data.Value.Longitude_deg);
                gameObject.SetActive(true);
                data.Value.View=gameObject.transform;
                gameObject.name = StartString+":"+data.Key;
                if(!_go.ContainsKey(data.Value.Id))
                    _go.Add(data.Value.Id, gameObject);
                else
                 Debug.Log(data.Value.Id);
            }
            SetZoom(Main.Instance.zoom);
        }
        

        public static void SetZoom(float zoom)
        {
            if (zoom < 0.3f)
                zoom = 0.3f;
            zoom = MaxScale * (zoom / 100f);
            foreach (var gameObject in _go)
            {
                gameObject.Value.transform.localScale = new Vector3(zoom, zoom, zoom);
            }
        }
        
        public void TestUpdate()
        {
            MapManage.Instance.InitPos();
            foreach (var gameObject in _go)
            {
                int.TryParse(gameObject.Value.name.Replace(AirportManage.StartString,"").Replace(":",""), out int num);
                var data = Get(num);
                gameObject.Value.transform.position = MapManage.GetPos(data.Latitude_deg, data.Longitude_deg);;
            }
        }

        public void Update()
        {
            foreach (var d in _preLoad)
            {
                if (!d.Value.CheckGC(Time.time)) continue;
                d.Value.Data = null;
                GC.Collect();
            }
        }

        public static void Init(GameObject pre, Transform parent)
        {
            AirportPool.Prefab = pre;
            AirportPool.Parent = parent;
        }

        public static bool LoadData(AirportType type)
        {
            var list = GetResources(type);
            if (list != null)
            {
                if (list.Count > 0)
                {
                    Dictionary<int, AirportData> dic = null;
                    if (_data.ContainsKey(type))
                        dic = _data[type];
                    else
                    {
                        dic = new Dictionary<int, AirportData>();
                        _data.Add(type, dic);
                    }

                    foreach (var airportData in list)
                    {
                        if (!dic.ContainsKey(airportData.Id))
                            dic.Add(airportData.Id, airportData);
                    }

                    return true;
                }
            }
            
            return false;
        }

        public static bool Have(AirportType type)
        {
            return _data.ContainsKey(type);
        }

        public void InitView(Transform transform)
        {
            var airportPanel = transform;
            
            transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() =>
            {
                for (int i = 0; i < (int) AirportType.Max; i++)
                {
                    var go= airportPanel.transform.GetChild(i + 1);
                    go.GetComponent<Image>().color = Color.white;
                }
                AllNoShow();
            });
            for (int i = 0; i < (int) AirportType.Max; i++)
            {
                AirportType type = (AirportType) i;
                var go= transform.GetChild(i + 1);
                go.GetComponent<Button>().onClick.AddListener(() =>
                {
                    var t = type;
                    if (Have(t))
                    {
                        Unload(t);
                        go.GetComponent<Image>().color = Color.white;
                    }
                    else
                    {
                        Load(t);
                        go.GetComponent<Image>().color = Color.cyan;
                    }
                });
                go.GetComponentInChildren<Text>().text = AirportData.TypeName(type);
                if (type == AirportType.large_airport)
                {
                    go.GetComponent<Image>().color = Color.cyan;
                }
                go.gameObject.SetActive(true);
            }

        }
        
        
        
        public void AllNoShow()
        {
            for (int i = 0; i < (int) AirportType.Max; i++)
            {
                AirportType type = (AirportType) i;
                Unload(type);
            }
        }
        
        public static AirportData Get(int id)
        {
            foreach (var d in _data)     
            {
                if (d.Value.ContainsKey(id))
                {
                    return d.Value[id];
                }
            }
            return null;
        }
        public static AirportData Get(string code)
        {
            foreach (var d in _data)     
            {
                foreach (var i in d.Value)
                {
                    if (i.Value.Iata_code==code)
                    {
                        return i.Value;
                    }
                }
            }
            return null;
        }
        public static List<AirportData> GetIndexOfName(string name)
        {
            List<AirportData> list = new List<AirportData>();
            foreach (var d in _data)     
            {
                foreach (var i in d.Value)
                {
                    if (i.Value.Name.IndexOf(name, StringComparison.Ordinal)!=-1)
                    {
                        list.Add(i.Value);
                    }
                    else if (i.Value.TranslateName.IndexOf(name, StringComparison.Ordinal) != -1)
                    {
                        list.Add(i.Value);
                    }
                }
            }
            return list;
        }

        public static AirportData GetAndShow(string key)
        {
            var tmp = Instance[key];
            if (tmp == null)
                return tmp;
            LoadOne(tmp);
            return tmp;
        }

        public static void InitKeyword()
        {
            var data = Resources.Load<TextAsset>("AirportsData/keyword");
            if (data != null)
            {
                var list = MyTool.CsvParse<AirportData>(data.text.Split('\n'));
                if (list.Count > 0)
                {
                    foreach (var airportData in list)
                    {
                        if (_keyword.ContainsKey(airportData.Iata_code))
                        {
                            Debug.Log(airportData.Iata_code);
                            continue;
                        }
                        _keyword.Add(airportData.Iata_code, airportData);
                    }
                }
            }
        }

        public static void LoadOne(AirportData airport)
        {
            if(_data.ContainsKey(airport.Type) && _data[airport.Type].ContainsKey(airport.Id))
                return;
            var list = GetResources(airport.Type);
            if (list != null)
            {
                var type = airport.Type;
                if (list.Count > 0)
                {
                    Dictionary<int, AirportData> dic = null;
                    if (_data.ContainsKey(type))
                        dic = _data[type];
                    else
                    {
                        dic = new Dictionary<int, AirportData>();
                        _data.Add(type,dic);
                    }
                    foreach (var airportData in list)
                    {
                        if (airportData.Id == airport.Id && !dic.ContainsKey(airportData.Id))
                        {
                            dic.Add(airportData.Id, airportData);
                            GameObject gameObject = AirportPool.Get();
                            gameObject.transform.position = MapManage.GetPos(airport.Latitude_deg, airport.Longitude_deg);
                            gameObject.SetActive(true);
                            gameObject.name = StartString+":"+airport.Id;
                            gameObject.SetZoom(MaxScale);
                            airportData.View = gameObject.transform;
                            if(!_go.ContainsKey(airport.Id))
                                _go.Add(airport.Id, gameObject);
                            else
                                Debug.Log(airport.Id);
                            break;
                        }
                    }
                }
            }
        }

        public static void Unload(AirportType type)
        {
            if (!_data.ContainsKey(type))
            {
                return;
            }

            foreach (var airportData in _data[type])
            {
                int id = airportData.Value.Id;
                if (_go.ContainsKey(id))
                {
                    AirportPool.Back(_go[id]);
                    _go.Remove(id);
                }
            }

            _data.Remove(type);

        }

        public static void UnLoadOne(AirportData airport)
        {
            if (_data.ContainsKey(airport.Type))
            {
                if (_data[airport.Type].ContainsKey(airport.Id))
                {
                    _data[airport.Type].Remove(airport.Id);
                    if (_go.ContainsKey(airport.Id))
                    {
                        AirportPool.Back(_go[airport.Id]);
                        _go.Remove(airport.Id);
                    }
                }
            }
        }

        public static List<AirportData> GetResources(AirportType type)
        {
            if (_preLoad.ContainsKey(type) &&_preLoad[type].Data!=null)
                return _preLoad[type].Get();
            var data = Resources.Load<TextAsset>("AirportsData/" + type);
            if (data != null)
            {
                var list = MyTool.CsvParse<AirportData>(data.text.Split('\n'));

                if (_preLoad.ContainsKey(type))
                {
                    _preLoad[type].Data = list;
                    _preLoad[type].STime = Time.time;
                }
                else
                    _preLoad.Add(type,new SourceData<List<AirportData>>(list));
                return list;
            }
            else
            {
                return null;
            }
        }
        
        private class SourceData<T>
        {
            public T Data;
            public float STime;
            public const float GCTime =3f;

            public SourceData(T data)
            {
                Data = data;
                STime=Time.time;
            }

            public T Get()
            {
                STime=Time.time;
                return Data;
            }

            public bool CheckGC(float time)
            {
                return Data!=null && time - STime > GCTime;
            }
        }

    }
}