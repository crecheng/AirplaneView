using System;
using System.Collections;
using System.Collections.Generic;
using AirplaneView;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update

    
    private Camera _camera;
    public Transform Map;
    public Transform Airport;
    public MapManage MapManage;
    public AirportManage AirportManage;
    public AirlineManage AirlineManage;
    public AirplaneManage AirplaneManage;
    public Vector2 Min;
    public Vector2 Max;
    public bool DebugShowMapLevel;
    public bool IsClickUI;
    public Text AirportInfoText;
    public Text MapTimeText;

    public Transform Select;
    public int SelectType;
   

    public LineRenderer line;

    public int click = 0;

    public static Main Instance;
    public Scrollbar MapZoom;
    public float zoom=100f;

    private Action _onShowObject;
    public DateTime MapTime;
    private float MapShowTime = 0f;
    public bool IsNowTime = true;
    public int TimeSpeed = 1;

    public Transform AirportSelectPanel;

    void Start()
    {
        zoom = 100f;
        Instance = this;
        Input.multiTouchEnabled = true;
        _camera = Camera.main;
        RegisterButtonAction();
        var p = GameObject.Find("/pre/0-0");
        MapZoom=GameObject.Find("/Canvas/MapZoom").GetComponent<Scrollbar>();
        MapZoom.onValueChanged.AddListener((value) =>
        {
            float x = value * 10 + 1;
            float z = ((11f / x) * 10f - 10f);
            zoom = z;
            SetMapZoom(z);
            AirportManage.SetZoom(z);
            AirlineManage.SetZoom(z);
            AirplaneManage.SetZoom(z);
        });
        MapManage=MapManage.Instance;
        MapManage.Init(p, Map);
        
        MapManage.LoadMaxMap();
        
        AirportManage=AirportManage.Instance;
        AirportManage.Init(GameObject.Find("/pre/airport"),Airport);
        AirportManage.InitView(AirportSelectPanel);
        AirportManage.Load(AirportType.large_airport);

        line = Object.Instantiate(GameObject.Find("/pre/Line")).GetComponent<LineRenderer>();
        
        AirlineManage=AirlineManage.Instance;
        AirlineManage.Init(line.gameObject,new GameObject("airline").transform);

        AirplaneManage = AirplaneManage.Instance;
        AirplaneManage.Init(GameObject.Find("/pre/airplane"),new GameObject("airplane").transform);

        Debug.Log(AirlineDataManage._data.Count);
        MapTime = new DateTime(2016, 1, 1);
        //AirlineManage.Show(MapTime, 0, true);
        StartCoroutine(AirlineManage.Show(MapTime,0,true));


        //Debug.Log(MySqlHelper.InsertData(list)); 
    }

    public void SetTime(DateTime time)
    {
        AirportInfoText.transform.parent.gameObject.SetActive(false);
        _onShowObject = null;
        MapTime = time;
        StartCoroutine(AirlineManage.Show(MapTime,0,true));
    }
    
    
    public Dictionary<int, Dictionary<int, bool>> data;

    // Update is called once per frame
    void Update()
    {
        AirportManage.Update();
        AirlineManage.Update(MapTime,IsNowTime);
        UpdateFirst();
        UpdateMap();
        OnClickShowInfo();
        UpdateShowAirplane();
        //AirportManage.Instance.TestUpdate();
    }

    private void UpdateFirst()
    {
        IsClickUI = (Input.touchCount>0&&EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))||EventSystem.current.IsPointerOverGameObject();
    }

    private float _load = -60f;

    private void UpdateMap()
    {
        MapTimeText.text = MapTime.ToString("yyyy-M-d HH:mm:ss");
        float mouseScrollWheel = Input.GetAxis("Mouse ScrollWheel");
        if (mouseScrollWheel != 0)
        {
            UpdateMapZoom(mouseScrollWheel > 0);
        }
        
        var movePos = UpdateMouseMove();
        if (movePos != Vector3.zero)
        {
            var pos = _camera.transform.position - movePos;
            pos.x = pos.x.Loop(Min.x, Max.x);
            pos.y = pos.y.Loop(Min.y, Max.y);
            _camera.transform.position = pos;
        }


        float load = _load;
        int index = 2;
        while (_camera.transform.position.z > load && index < 10)
        {
            load /= 2f;
            index++;
        }

        MapManage.LoadMap(index, _camera.transform.position);
        MapManage.Unload(index + 2);


    }

    public void UpdateShowAirplane()
    {
        if (IsNowTime && !AirlineManage.IsRun && Time.time - MapShowTime > AirlineManage.UseTime/100f * 2)
        {
            if(Time.time - MapShowTime<3)
                return;
            TimeSpan span = new TimeSpan(0, 0, 0, 0, (int) ((Time.time - MapShowTime) * 1000 * TimeSpeed));
            MapTime += span;
            MapShowTime = Time.time;
            StartCoroutine(AirlineManage.Show(MapTime,0,true));
        }
    }



    private void OnClickShowInfo()
    {
        //AirportManage.TestUpdate();
        if (!IsClickUI)
        {
            Vector3 pos=Vector3.zero;
            bool onclick = false;
            if (Input.touchSupported)
            {
                if (Input.touchCount == 1)
                {
                    if (Input.touches[0].phase == TouchPhase.Began)
                    {
                        onclick = true;
                        pos = Input.touches[0].position;
                    }
                }
                
            }
            else if(Input.GetMouseButtonDown(0))
            {
                onclick = true;
                pos = Input.mousePosition;
            }

            
            if (onclick)
            {
                Ray ray = Camera.main.ScreenPointToRay(pos);
                if (Physics.Raycast(ray, out var hitInfo, 9999f,1<<9,QueryTriggerInteraction.UseGlobal))
                {
                    if (hitInfo.transform.name.StartsWith(AirportManage.StartString))
                    {
                        int.TryParse(hitInfo.transform.name.Replace(AirportManage.StartString,"").Replace(":",""), out int id);
                        if (id!=0)
                        {
                            _onShowObject = () => ShowData(AirportManage.Get(id));
                            _onShowObject.Invoke();
                            SelectObject(hitInfo.transform,MyTool.AirportInt,true);
                        }

                    }else if (hitInfo.transform.name.StartsWith(AirplaneManage.StartString))
                    {
                        int.TryParse(hitInfo.transform.parent.name.Replace(AirplaneManage.StartString,"").Replace(":",""), out int id);
                        if (id!=0)
                        {
                            _onShowObject = () => ShowData(AirplaneManage.Get(id));
                            _onShowObject.Invoke();
                            SelectObject(hitInfo.transform.parent, MyTool.AirplaneInt, true);
                        }
                    }
                }
            }
        }
        line.endWidth =  zoom/100;
        line.startWidth =  zoom/100;
    }


    private void ShowData(string text)
    {
        if(AirportInfoText==null)
            return;
        AirportInfoText.transform.parent.gameObject.SetActive(true);
        AirportInfoText.text = text;
    }

    public void SelectObject(Transform transform, int type, bool active)
    {
        
        if (Select!=null)
        {
            MyTool.SetSelect(Select,SelectType,false);
            Select = null;
        }
        if(transform==null)
            return;

        if (active)
        {
            Select = transform;
            SelectType = type;
            MyTool.SetSelect(transform,type,true);
        }
    }
    
    public void ShowData(object text)
    {
        ShowData(text.ToString());
    }
    
    private void UpdateMapZoom(bool isEnlarge)
    {
        MapZoom.value = Mathf.Clamp(MapZoom.value + (isEnlarge ? 0.01f : -0.01f), 0f, 1f);
    }

    public void SetMapZoom(float zoom)
    {
        var transform1 = _camera.transform;
        var pos = transform1.position;
        pos.z =-3.7f*zoom;
        pos.z = Mathf.Clamp(pos.z, -371f, -1f);
        transform1.position = pos;
    }

    private bool _beginMove;
    private long _isMove;
    private Vector3 _lastMousePos;

    public Vector3 UpdateMouseMove()
    {
        if (Input.touchSupported)
        {
            if (Input.touchCount == 1)
            {
                if (!IsClickUI)
                {
                    if (Input.touches[0].phase == TouchPhase.Began)
                    {
                        _beginMove = true;
                    }
                }

                if (Input.touches[0].phase == TouchPhase.Moved)
                {
                    _isMove++;
                    if (_beginMove && _isMove > 10)
                    {
                        var offset = new Vector3(Input.touches[0].deltaPosition.x,Input.touches[0].deltaPosition.y,0);
                        return offset * (Mathf.Abs(_camera.transform.position.z) / 1000f);
                    }
                }
                
            }
            else
            {
                _isMove = 0;
                _beginMove = false;
                _lastMousePos = Vector3.zero;
            }
        }
        else
        {

            if (!IsClickUI)
            {
                if (Input.GetMouseButtonDown(0))
                    _beginMove = true;
            }

            if (Input.GetMouseButton(0))
            {
                _isMove++;
            }

            if (Input.GetMouseButtonUp(0))
            {
                _isMove = 0;
                _beginMove = false;
                _lastMousePos = Vector3.zero;
            }

            if (_beginMove && _isMove > 10)
            {
                //Debug.Log($"{_lastMousePos}-{_camera.ScreenToWorldPoint(Input.mousePosition)}-{Input.mousePosition}");
                if (_lastMousePos != Vector3.zero)
                {
                    var offset = Input.mousePosition - _lastMousePos;
                    _lastMousePos = Input.mousePosition;
                    return offset * (Mathf.Abs(_camera.transform.position.z) / 1000f);
                }

                _lastMousePos = Input.mousePosition;
            }
        }
        return Vector3.zero;
    }

    private void RegisterButtonAction()
    {
        AirportInfoText.transform.parent.GetChild(1).GetComponent<Button>().onClick
            .AddListener(() =>
            {
                AirportInfoText.transform.parent.gameObject.SetActive(false);
                _onShowObject = null;
            });
    }

    public static void Refresh()
    {
        Instance._onShowObject?.Invoke();
    }
    
    
}
