using System.Text;

namespace AirplaneView
{
    public class AirlineData : IMysqlData
    {

        public long Id;
        public string Departure;
        public string Arrive;
        public long PlanDepartureTime;
        public long PlanArriveTime;
        public long DepartureTime;
        public long ArriveTime;
        public int Aid;
        public bool Plan;
        public long Lid;

        public AirlineData()
        {
            
        }
        /*
         * 出发机场,到达机场,航班编号,计划起飞时间,计划到达时间,实际起飞时间,实际到达时间,飞机编号,航班是否取消
HGH,DLC,CZ6328,1453809600,1453817100,1453813080,1453819380,1,正常
SHA,XMN,FM9261,1452760800,1452767100,1452762600,1452767940,2,正常
         */
        public AirlineData(string data)
        {
            var s = data.Split(',');
            if (s.Length>=9)
            {
                int i = 0;
                long.TryParse(s[i++], out Lid);
                Departure = s[i++];
                Arrive = s[i++];
                Id = GetId(s[i++]);
                long.TryParse(s[i++], out PlanDepartureTime);
                long.TryParse(s[i++], out PlanArriveTime);
                long.TryParse(s[i++], out DepartureTime);
                long.TryParse(s[i++], out ArriveTime);
                int.TryParse(s[i++], out Aid);
                Plan = s[i] == "正常\r"||s[i] == "正常";

                if (Plan && DepartureTime==0)
                {
                    DepartureTime = PlanDepartureTime;
                }
            }
        }
        
        public string GetInsertString(bool haveAutoInc=false)
        {
            if(haveAutoInc)
                return $"{Lid},{Id},'{Departure}','{Arrive}',{PlanDepartureTime},{PlanArriveTime},{DepartureTime},{ArriveTime},{Aid},{(Plan? 1 :0)}";
            return $"{Id},'{Departure}','{Arrive}',{PlanDepartureTime},{PlanArriveTime},{DepartureTime},{ArriveTime},{Aid},{(Plan? 1 :0)}";
        }

        public string GetTableHead(bool haveAutoInc=false)
        {
            if(haveAutoInc)
                return "lid,id,departure,arrive,plandeparturetime,planarrivetime,departuretime,arrivetime,aid,plan";
            return "id,departure,arrive,plandeparturetime,planarrivetime,departuretime,arrivetime,aid,plan";
        }

        public static long GetId(string s)
        {
            long i = 0;
            int index = 0;
            while (index<s.Length)
            {
                i *= 36;
                int num = 0;
                var c = s[index];
                if (c >= '0' && c <= '9')
                {
                    num = c - '0';
                }else if (c >= 'A' && c <= 'Z')
                {
                    num = c - 'A' + 10;
                }
                i += num;
                index++;
            }

            return i;
        }

        public static string GetId(long i)
        {
            StringBuilder sb = new StringBuilder();
            while (i>0)
            {
                long num = i % 36;
                if (num < 10)
                {
                    sb.Insert(0, (char) ('0' + num));
                }
                else
                {
                    num -= 10;
                    sb.Insert(0, (char) ('A' + num));
                }

                i /= 36;
            }

            return sb.ToString();
        }
    }
}