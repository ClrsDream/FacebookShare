using FBH.Core;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using System;
namespace FacebookHelper.Codes
{
    public class WebEnginner
    {
        static IDictionary<string, WebView> webList = new Dictionary<string, WebView>();

        public static WebView CreateInstance(string name)
        {
            if (webList.ContainsKey(name)) return webList[name];

            WebView _instance = new WebView();
            webList.Add(name, _instance);

            return _instance;
        }

        public static void Add(string name,WebView wv)
        {
            if (webList.ContainsKey(name)) return ;
            
            webList.Add(name, wv);
        }

        public static WebView GetWebView(string name)
        {
            return webList[name];
        }

        public WebView this[string name]
        {
            get
            {
                return webList[name];
            }
        }
    }
}
