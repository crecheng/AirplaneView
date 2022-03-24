using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update

    private const float imgLen = 256.0f;
    void Start()
    {
        var str= Resources.Load<TextAsset>("MapData/4").text.Split('\n');
        data = new Dictionary<int, Dictionary<int, bool>>();
        int j = 0;
        int t = 0;
        for (int i = 0; i < str.Length; i++)
        {
            if (j == 0)
            {
                if(int.TryParse(str[i], out t))
                    data.Add(t,new Dictionary<int, bool>());
                
            }else if (j == -1)
            {
                int.TryParse(str[i], out j);
                j++;
            }
            else
            {
                var s = str[i].Split('-');
                int.TryParse(s[0], out int num2);
                bool.TryParse(s[1], out bool num3);
                data[t].Add(num2,num3);
            }

            j--;
        }

        int index = 6;
        
        var b=Resources.Load<Sprite>("MapData/"+index);

        var o0= GameObject.Find("0");
        var p= GameObject.Find("/0/0-0");
        
        int w = (int)Math.Pow(2, index);
        float zoom = 160.0f/w;
        for (int i = 0; i < w; i++)
        {
            for (int k = 0; k < w; k++)
            {
                var tmp = Resources.Load<Sprite>($"MapData/{index}/{i}-{k}");
                if(tmp==null)
                    continue;
                var go = GameObject.Instantiate(p, o0.transform);
                go.GetComponent<SpriteRenderer>().sprite = tmp;
                go.transform.position = new Vector3(i * zoom * imgLen / 100f, -k * zoom * imgLen / 100f);
                go.transform.localScale = new Vector3(zoom, zoom);
            }
        }

    }

    public Dictionary<int, Dictionary<int, bool>> data;

    // Update is called once per frame
    void Update()
    {
        
    }
}
