using UnityEngine;

namespace AirplaneView
{
    public static class Expand
    {
        public static float Loop(this float value, float min, float max)
        {
            if (value > max)
            {
                return value - (max - min);
            }

            if (value < min)
            {
                return value + (max - min);
            }

            return value;
        }

        public static void SetZoom(this GameObject gameObject,float MaxScale)
        {
            if(gameObject==null)
                return;
            var zoom = Main.Instance.zoom;
            if (zoom < 0.3f)
                zoom = 0.3f;
            zoom = MaxScale * (zoom / 100f);
            gameObject.transform.localScale = new Vector3(zoom, zoom, zoom);
        }
        
        public static void SetZoom(this Transform transform,float MaxScale)
        {
            SetZoom(transform.gameObject,MaxScale);
        }
        
        public static void ChildrenShowOnly(this Transform transform,int index=-1)
        {
            if(transform==null)
                return;
            if(index<-1|| transform.childCount<=index)
                return;
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(i==index);
            }
        }
    }
}