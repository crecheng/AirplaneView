using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace AirplaneView
{
    public static class MyTool
    {
        public static Color AirportColor;
        public static Color AirplaneColor;
        public static Color AirlineColor;
        public const int AirportInt=1;
        public const int AirplaneInt=2;
        public const int AirlineInt=3;
        public static List<T> CsvParse<T>(string[] data) where T:new ()
        {
            if (data == null || data.Length <= 1)
                return null;
            List<T> list = new List<T>();
            var names = data[0].Replace("\r","").Split(',');
            var type = typeof(T);
            Dictionary<string, FieldInfo> fieldInfos = new Dictionary<string, FieldInfo>();
            try
            {
                foreach (var name in names)
                {
                    var f = type.GetField(name);
                    if (f != null)
                    {
                        fieldInfos.Add(name,f);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            try
            {
                for (int i = 1; i < data.Length; i++)
                {
                    if(data[i].Length<=0)
                        continue;
                    T o = new T();
                    var d = Split(data[i]);
                    for (int j = 0; j < d.Length; j++)
                    {
                        if (d[j].Length > 0)
                        {
                            object value = null;
                            if(d.Length>names.Length)
                                Debug.Log(data[i]);
                            var f=fieldInfos[names[j]];
                            switch (f.FieldType.FullName)
                            {
                                case "System.Int32":
                                    int.TryParse(d[j], out int num);
                                    value = num;
                                    break;
                                case "System.String":
                                    value = d[j];
                                    break;
                                case "System.Double":
                                    double.TryParse(d[j], out double num1);
                                    value = num1;
                                    break;
                                case "System.Float":
                                    float.TryParse(d[j], out float num2);
                                    value = num2;
                                    break;
                                case "System.Boolean":
                                    if (!bool.TryParse(d[j], out bool num3))
                                    {
                                        num3 = d[j] == "1";
                                    }
                                    value = num3;
                                    break;
                                case "System.DateTime":
                                    if (DateTime.TryParse(d[j], out DateTime num4))
                                    {
                                        value = num4;
                                    }
                                    break;
                                default:
                                {
                                    if (f.FieldType.BaseType == (typeof(Enum)))
                                    {
                                        value = Enum.Parse(f.FieldType, d[j]);
                                    }
                                    break;
                                }

                            }
                            f.SetValue(o,value);
                        }
                    }
                    list.Add(o);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            return list;
        }

        public static string[] Split(string s)
        {
            var ss = s.Split(',');
            int max = ss.Length;
            for (int i = 0; i < ss.Length; i++)
            {
                if (ss[i].StartsWith("\"") && !ss[i].EndsWith("\""))
                {
                    if (i + 1 < ss.Length)
                    {
                        ss[i] +=","+ss[i + 1];
                        max--;
                        for (int j = i+1; j < ss.Length-1; j++)
                        {
                            ss[j] = ss[j + 1];
                        }
                        
                        i--;
                    }
                }
            }

            string[] result = new string[max];
            Array.Copy(ss,result,max);
            return result;
        }

        public static DateTime Timestamp(long timestamp)
        {
            DateTime startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);//当地时区
            return startTime.AddSeconds(timestamp);
        }

        public static long Timestamp(DateTime time)
        {
            DateTime startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);//当地时区
            TimeSpan ts = time - startTime; 
            return Convert.ToInt64(ts.TotalSeconds);
        }

        public static void SetSelect(Transform transform, int type, bool select)
        {
            switch (type)
            {
                case AirportInt:
                {
                    var render = transform.GetComponent<SpriteRenderer>();
                    if (render != null)
                    {
                        if (select)
                        {
                            if (AirportColor == default)
                            {
                                AirportColor = render.color;
                            }

                            render.color = Color.red;
                        }
                        else
                            render.color = AirportColor;
                        
                    }
                } break;
                case AirlineInt:
                {
                    var render = transform.GetComponent<LineRenderer>();
                    if (render != null)
                    {
                        if (select)
                        {
                            if (AirlineColor == default)
                            {
                                AirlineColor = render.endColor;
                            }

                            render.startColor=Color.red;
                            render.endColor = Color.red;
                        }
                        else
                        {
                            render.startColor=AirlineColor;
                            render.endColor = AirlineColor;
                        }
                    } break;
                }
                case AirplaneInt:
                {
                    var render = transform.GetChild(0).GetComponent<SpriteRenderer>();
                    if (render != null)
                    {
                        if (select)
                        {
                            if (AirplaneColor == default)
                            {
                                AirplaneColor = render.color;
                            }

                            render.color = Color.red;
                        }
                        else
                            render.color = AirplaneColor;
                        
                    }
                }break;
            }
        } 
    }
}