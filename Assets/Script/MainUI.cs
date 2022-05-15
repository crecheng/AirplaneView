using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using AirplaneView;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    
    public Transform MainPanel;
    public static MainUI Instance;
    public static Main MainInstance;

    #region 主菜单
    
    public Transform FunTransform;
    public Button AirportBtn;
    public Button AirlineBtn;
    public Button AirplaneBtn;
    public Button MapBtn;
    public Button MainBtn;

    #endregion
    
    #region 地图

    public Text MapTip;
    
    public InputField MapTimeInput;
    public Text MapSpeed;
    public Button SpeedUp;
    public Button SpeedUp1;
    public Button SpeedDown;
    public Button SpeedDown1;
    public InputField JinInput;
    public InputField WeiInput;
    public Button MapToBtn;

    #endregion

    #region 机场

    public MyResult AirportResult;
    public Transform AirportResultTransform;
    public Button AirportFind;
    public InputField AirportId;
    public InputField AirportCode;
    public InputField AirportName;


    #endregion

    #region 航线

    public MyResult AirlineResult;
    public Transform AirlineResultTransform;
    

    #endregion
    
    #region 飞机

    public MyResult AirplaneResult;
    public Transform AirplaneResultTransform;
    public Button AirplaneFind;
    public InputField AirplaneId;
    public InputField AirplaneLid;
    public InputField AirplaneArrive;
    public InputField AirplaneDeparture;
    
    #endregion


    
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        RegisterMain();
        RegisterMapPanel();
        RegisterAirport();
        RegisterAirline();
        RegisterAirplane();
    }

    private void RegisterMain()
    {
        MainBtn.onClick.AddListener(()=>MainPanel.gameObject.SetActive(!MainPanel.gameObject.activeSelf));
        AirportBtn.onClick.AddListener(()=>FunTransform.ChildrenShowOnly(0));
        AirlineBtn.onClick.AddListener(()=>FunTransform.ChildrenShowOnly(1));
        AirplaneBtn.onClick.AddListener(()=>FunTransform.ChildrenShowOnly(2));
        MapBtn.onClick.AddListener(()=>FunTransform.ChildrenShowOnly(3));
        FunTransform.ChildrenShowOnly();
        MainPanel.gameObject.SetActive(false);
    }
    private void RegisterMapPanel()
    {
        MapTimeInput.onValueChanged.AddListener((text) =>
        {
            try
            {
                DateTime time=DateTime.ParseExact(text, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                MapTip.text = "设置时间为：" + time.ToString("yyyy-MM-dd HH:mm:ss");
                MainInstance.SetTime(time); 
            }
            catch (Exception e)
            {
                MapTip.text = "时间格式不对";
            }
            
        });
        
        SpeedUp.onClick.AddListener(() =>
        {
            int s = (int)(MainInstance.TimeSpeed * 0.1f);
            if (s < 1)
                s = 1;
            int speed = MainInstance.TimeSpeed - s;
            SetMapSpeed(speed);
        });
        SpeedDown.onClick.AddListener(() =>
        {
            int s = (int)(MainInstance.TimeSpeed * 0.1f);
            if (s < 1)
                s = 1;
            s += MainInstance.TimeSpeed;
            SetMapSpeed(s);
        });
        SpeedUp1.onClick.AddListener(() => SetMapSpeed(MainInstance.TimeSpeed >> 1));
        SpeedDown1.onClick.AddListener(() => SetMapSpeed(MainInstance.TimeSpeed << 1));
        MapToBtn.onClick.AddListener(() =>
        {
            float j = 0;
            float w = 0;
            if (float.TryParse(JinInput.text, out j) && float.TryParse(WeiInput.text, out w))
            {
                if (Mathf.Abs(w) > 85 || Mathf.Abs(j) > 180)
                {
                    MapTip.text = "数据超出范围！";
                }

                var pos= MapManage.GetPos(w, j);
                var c = GameObject.Find("Main Camera").transform;
                c.position = new Vector3(pos.x, pos.y, c.position.z);
            }
        });
    }

    private void RegisterAirport()
    {
        AirportResult = new MyResult(AirportResultTransform, 7);
        AirportFind.onClick.AddListener(() =>
        {
            string id= AirportId.text.Trim();
            string code= AirportCode.text.Trim();
            string airName= AirportName.text.Trim();
            List<MyResult.Item> list = new List<MyResult.Item>();
            if (!string.IsNullOrEmpty(id))
            {
                if (int.TryParse(id, out int i))
                {
                    var r= AirportManage.Get(i);
                    if (r != null)
                    {
                        list.Add(r.GetItem());
                    }
                }
            }

            if (!string.IsNullOrEmpty(code))
            {
                var r=AirportManage.Get(code);
                if (r != null)
                {
                    list.Add(r.GetItem());
                }
            }
            
            if (!string.IsNullOrEmpty(airName))
            {
                var r=AirportManage.GetIndexOfName(airName);
                r.ForEach(i => list.Add(i.GetItem()));
            }
            AirportResult.Set(list);
        });
    }

    private void RegisterAirline()
    {
        AirlineResult = new MyResult(AirlineResultTransform, 6);
        
    }

    private void RegisterAirplane()
    {
        AirplaneResult = new MyResult(AirplaneResultTransform, 6);
        AirplaneFind.onClick.AddListener(() =>
        {
            string id=AirplaneId.text.Trim();
            string lid=AirplaneLid.text.Trim();
            string arrive=AirplaneArrive.text.Trim();
            string departure=AirplaneDeparture.text.Trim();
            List<MyResult.Item> list = new List<MyResult.Item>();
            if (!string.IsNullOrEmpty(id))
            {
                if (int.TryParse(id, out int i))
                {
                    var r= AirplaneManage.Instance.Get(i);
                    if(r!=null)
                        list.Add(r.GetItem());
                }
            }

            if (!string.IsNullOrEmpty(lid))
            {
                var r = AirplaneManage.Instance.Get(lid);
                if (r != null)
                    list.Add(r.GetItem());
            }
            if (!string.IsNullOrEmpty(arrive) || !string.IsNullOrEmpty(arrive))
            {
                int a;
                int d;
                if (string.IsNullOrEmpty(arrive))
                    a = -1;
                if (string.IsNullOrEmpty(arrive))
                    d = -1;
                if (!int.TryParse(arrive, out a))
                    a = -1;
                if (!int.TryParse(departure, out d))
                    d = -1;
                var r = AirplaneManage.Instance.Get(a,d);
                r.ForEach(i => list.Add(i.GetItem()));
            }
            
            AirplaneResult.Set(list);
        });
    }

    public void SetMapSpeed( int speed)
    {
        if (speed < 1)
            speed = 1;
        MainInstance.TimeSpeed = speed;
        MapSpeed.text = speed.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (MainInstance == null)
        {
            MainInstance = transform.GetComponent<Main>();
        }
        
    }
    
    
}
