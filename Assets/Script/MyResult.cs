using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AirplaneView
{
    public class MyResult
    {
        private int _pageCount;
        private List<Item> _list;
        private List<Button> _buttons;
        private Text _text;
        private int _page;
        private int _pageAll;
        private Transform _root;
        public MyResult(Transform root, int count)
        {
            if(root==null)
                return;
            _root = root;
            _list = new List<Item>();
            _pageCount = count;
            _buttons = new List<Button>();
            root.Find("Up").GetComponent<Button>().onClick.AddListener(() =>
            {
                ShowPage(_page-1);
            });
            root.Find("Down").GetComponent<Button>().onClick.AddListener(() =>
            {
                ShowPage(_page+1);
            });
            _text = root.Find("RText").GetComponent<Text>();
            for (int i = 0; i < count; i++)
            {
                int index = i;
                var button = root.Find("R"+(i+1)).GetComponent<Button>();
                _buttons.Add(button);
                button.onClick.AddListener(() =>
                {
                    int c = (_page-1) * _pageCount + index;
                    if(c>_list.Count)
                        return;
                    _list[c].Invoke();
                });
            }
            _page = 1;
            _pageAll = 1;
            ShowPage(1);
        }

        public void Clear()
        {
            
        }

        public void ShowPage(int page)
        {
            if(page<1|| page>_pageAll)
                return;
            _page = page;
            _text.text = $"{_page}/{_pageAll}";
            int start = (_page - 1) * _pageCount;
            for (int i = 0; i < _pageCount; i++)
            {
                if(start+i>=_list.Count)
                    _buttons[i].gameObject.SetActive(false);
                else
                {
                    _buttons[i].gameObject.SetActive(true);
                    _buttons[i].GetComponentInChildren<Text>().text = _list[start + i].Name;
                }
            }

        }

        public void Set(List<Item> list)
        {
            _list = list;
            _pageAll = _list.Count / _pageCount + 1;
            _page = 1;
            ShowPage(1);
        }
        public class Item
        {
            public string Name;
            public Action<object> Action;
            public object Data;

            public void Invoke()
            {
                Action?.Invoke(Data);
            }
        }
    }
}