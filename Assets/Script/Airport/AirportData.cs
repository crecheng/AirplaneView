using System;
using UnityEngine;

namespace AirplaneView
{
    public class AirportData
    {
        public int Id;
        public string Ident;

        public string TranslateName;
        
        /// 类型
        public AirportType Type;

        public string Name;

        /// 纬度
        public double Latitude_deg;

        /// 经度
        public double Longitude_deg;

        /// 海拔
        public int Elevation_ft;

        /// 大陆
        public string Continent;

        /// 国家
        public string Country_name;

        public string Iso_country;

        /// 地区名
        public string Region_name;

        public string Iso_region;
        public string Local_region;

        /// 直辖市
        public string Municipality;

        public bool Scheduled_service;
        public string Gps_code;
        public string Iata_code;
        public string Local_code;
        public string Home_link;
        public string Wikipedia_link;
        public string Keywords;
        public string Score;
        public DateTime Last_updated;
        public Transform View;

        public AirportData()
        {
            
        }

        public static string TypeName(AirportType type)
        {
            switch (type)
            {
                case AirportType.large_airport:
                    return "大型";
                case AirportType.medium_airport:
                    return "中型";
                case AirportType.small_airport:
                    return "小型";
                case AirportType.seaplane_base:
                    return "海上";
                case AirportType.balloonport:
                    return "热气球";
                case AirportType.closed:
                    return "关闭";
                case AirportType.heliport:
                    return "直升机";
            }

            return "";
        }

        public override string ToString()
        {
            return $"ID: {Id}\n" +
                   $"Ident: {Ident}\n" +
                   $"类型: {TypeName(Type)}\n" +
                   $"名字: {Name}\n\t{TranslateName}\n" +
                   $"三字节码: {Iata_code}\n" +
                   $"维度: {Latitude_deg}\n" +
                   $"经度: {Longitude_deg}\n" +
                   $"海拔: {Elevation_ft}\n" +
                   $"大陆: {Continent}\n" +
                   $"国家: {Country_name}\n" +
                   $"{nameof(Iso_country)}: {Iso_country}\n" +
                   $"{nameof(Region_name)}: {Region_name}\n" +
                   $"{nameof(Iso_region)}: {Iso_region}\n" +
                   $"{nameof(Local_region)}: {Local_region}\n" +
                   $"{nameof(Municipality)}: {Municipality}\n" +
                   $"{nameof(Scheduled_service)}: {Scheduled_service}\n" +
                   $"{nameof(Gps_code)}: {Gps_code}\n" +
                   $"{nameof(Local_code)}: {Local_code}\n" +
                   $"{nameof(Home_link)}: {Home_link}\n" +
                   $"{nameof(Wikipedia_link)}: {Wikipedia_link}\n" +
                   $"{nameof(Keywords)}: {Keywords}\n" +
                   $"{nameof(Score)}: {Score}\n" +
                   $"{nameof(Last_updated)}: {Last_updated}";
        }

        public MyResult.Item GetItem()
        {
            return new MyResult.Item()
            {
                Data = this,
                Name = GetButtonString(),
                Action = (d) =>
                {
                    var o = d as AirportData;
                    if(o==null)
                        return;
                    var pos = MapManage.GetPos(o.Latitude_deg, o.Longitude_deg);
                    var c = GameObject.Find("Main Camera").transform;
                    c.position = new Vector3(pos.x, pos.y, c.position.z);
                    Main.Instance.SelectObject(View, MyTool.AirportInt, true);
                    Main.Instance.ShowData(o);
                }
            };
        }

        public string GetButtonString()
        {
            return $"{Id}-{Iata_code}-{Name}";
        }
    }
}