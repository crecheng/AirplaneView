using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AirplaneView
{
    public class MapManage
    {
        public static float Zoom;
        private const float imgLen = 256.0f;
        private GameObject[] parent;
        private Sprite _black;
        private Sprite _white;
        public static Vector2 LeftTop = new Vector2(-51.2f, 51.2f);
        public static Vector2 RightDown = new Vector2(358.4f, -358.4f);
        public static Vector2 CenterPos;
        
        private MapManage()
        {
            
        }

        private Dictionary<int, Dictionary<int, Dictionary<int, bool>>> _dic;
        public Dictionary<int, Dictionary<int, GameObject>> _go;

        public static MapManage Instance { get; } = new MapManage();

        public void Init(GameObject prefab, Transform mapParent)
        {
            MapPool.Prefab = prefab;
            MapPool.Map = mapParent;
            InitPos();
            _dic = new Dictionary<int, Dictionary<int, Dictionary<int, bool>>>();
            _go = new Dictionary<int, Dictionary<int, GameObject>>();
            for (int i = 2; i < 10; i++)
            {
                _dic.Add(i,null);
                _go.Add(i,new Dictionary<int, GameObject>());
            }

            parent = new GameObject[8];
            for (int i = 0; i < parent.Length; i++)
            {
                parent[i] = new GameObject($"{i + 2}-Map");
                parent[i].transform.SetParent(mapParent);
            }
            
            _black=Resources.Load<Sprite>("MapData/black");
            _white=Resources.Load<Sprite>("MapData/white");
        }
        

        public void InitPos()
        {
            var center=  GameObject.Find("/pos/center").transform;
            var right = GameObject.Find("/pos/right").transform;
            var top = GameObject.Find("/pos/top").transform;
           
            var position = center.position;
            CenterPos = new Vector2(position.x, position.y);
            float w = (right.position.x - position.x) * 18f;
            LeftTop.x = position.x - w;
            RightDown.x = position.x + w;
            float h = (position.y-top.position.y) * 18f;
            LeftTop.y = position.y - h;
            RightDown.y = position.y + h;
        }

        public void LoadMaxMap()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    LoadMap(2,i,j);
                }
            }
        }

        public void LoadMap(int index, Vector3 pos)
        {
            int count = (int)Mathf.Pow(2, index);
            int x = (int)((pos.x + 51f) / 409f * count);
            int y = (int)((-pos.y + 51f) / 409f * count);
            for (int i = x-1; i < x+2; i++)
            {
                for (int j = y-1; j < y+2; j++)
                {
                    LoadMap(index,i,j);
                }
            }
            CheckUnload(index,x,y);
        }

        public void DebugUpdate()
        {
            InitPos();
        }

        public static Vector3 GetPos(double latitudeDeg, double longitudeDeg)
        {
            longitudeDeg += 180.0;
            float mapHeight = RightDown.y - LeftTop.y;
            float mapWidth = RightDown.x - LeftTop.x;
            
            double t =  Math.Log(Math.Tan((90+latitudeDeg)*Math.PI/360.0))/(Math.PI/180.0);
            double h =   t * mapHeight / 100f;
            double w = longitudeDeg / 360.0 * mapWidth;
            Vector3 pos = Vector3.zero;
            pos.y = CenterPos.y - (float)h;
            pos.x = LeftTop.x + (float)w;
            return pos;
        }
        public static Vector2 GetPos(Vector3 position)
        {
            Vector2 pos=Vector2.zero;
            float mapHeight = RightDown.y - LeftTop.y;
            float mapWidth = RightDown.x - LeftTop.x;
            
            double h= CenterPos.y - position.y;
            double w=position.x- LeftTop.x;

            pos.y = (float)(w / mapWidth * 360.0f);
            double t = h * 100f / mapHeight;

            pos.x = (float) ( Math.Atan(Math.Exp(t * Math.PI / 180.0))*360f/Math.PI-90);
            pos.y -= 180.0f;
            
            return pos;
        }
        
        public static Vector3 GetPos(AirportData data)
        {

            if (data == null)
            {
                return Vector3.zero;
            }
            return GetPos(data.Latitude_deg,data.Longitude_deg);
        }

        private void CheckUnload(int index,int x,int y)
        {
            if(index<=3)
                return;
            int unloadDistance = 10;
            List<int> needUnload = new List<int>();
            foreach (var pair in _go[index])
            {
                int i = pair.Key >> 16;
                int j = pair.Key & 0xffff;
                int dx = Mathf.Abs(x - i);
                int dy = Mathf.Abs(y - j);
                if (dx > unloadDistance || dy > unloadDistance)
                {
                    needUnload.Add(pair.Key);
                    MapPool.Back(pair.Value);
                }
            }
            foreach (var i in needUnload)
            {
                _go[index].Remove(i);
            }
        }

        public void LoadMap(int index, int x, int y)
        {
            index = Mathf.Clamp(index, 1, 9);
            if (_dic[index] == null)
            {
                LoadJson(index);
            }

            int max = (int) Mathf.Pow(2, index);
            if (x < 0 || y < 0 || x >= max || y >= max)
                return;
            int w = (int) Mathf.Pow(2, index);
            Zoom = 160.0f / w;

            int num = (x << 16) + y;
            if (_go[index].ContainsKey(num))
            {
                return;
            }
            string fileName = index >= 9 ? $"MapData/{index}/{x}/{x}-{y}" : $"MapData/{index}/{x}-{y}";
            var png = Resources.Load<Sprite>(fileName);
            if (png == null)
            {
                png = _dic[index][x][y] ? _black : _white;
            }

            var go = MapPool.Get();
            go.transform.SetParent(parent[index - 2].transform);
            var sr = go.GetComponent<SpriteRenderer>();
            if (Main.Instance.DebugShowMapLevel)
            {
                switch (index)
                {
                    case 3:
                        sr.color = Color.cyan;
                        break;
                    case 4:
                        sr.color = Color.yellow;
                        break;
                    case 5:
                        sr.color = Color.green;
                        break;
                    case 6:
                        sr.color = Color.red;
                        break;
                    case 7:
                        sr.color = Color.cyan;
                        break;
                    case 8:
                        sr.color = Color.yellow;
                        break;
                    case 9:
                        sr.color = Color.green;
                        break;

                }
            }

            sr.sprite = png;
            sr.sortingOrder = index;
            float offset = 0;
            if (index > 2)
            {
                offset = imgLen / 10f;
                float o = 1f;
                float all = 0;
                int count = index - 3;
                while (count > 0)
                {
                    o /= 2f;
                    all += o;
                    count--;
                }

                offset *= 1 + all;

            }

            go.transform.position = new Vector3(x * Zoom * imgLen / 100f - offset, -y * Zoom * imgLen / 100f + offset);
            go.transform.localScale = new Vector3(Zoom, Zoom);
            go.SetActive(true);
            _go[index].Add(num, go);
        }

        public void Unload(int index)
        {
            for (int i = index; i <= 9; i++)
            {
                foreach (var gameObject in _go[i])
                {
                    MapPool.Back(gameObject.Value);
                }
                _go[i].Clear();
            }
        }

        private void LoadJson(int index)
        {
            var data = new Dictionary<int, Dictionary<int, bool>>();
            var str = Resources.Load<TextAsset>($"MapData/{index}").text.Split('\n');
            int j = 0;
            int t = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (j == 0)
                {
                    if (int.TryParse(str[i], out t))
                        data.Add(t, new Dictionary<int, bool>());

                }
                else if (j == -1)
                {
                    int.TryParse(str[i], out j);
                    j++;
                }
                else
                {
                    var s = str[i].Split('-');
                    int.TryParse(s[0], out int num2);
                    bool.TryParse(s[1], out bool num3);
                    data[t].Add(num2, num3);
                }
                j--;
            }

            _dic[index] = data;
        }
    }
}