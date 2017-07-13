using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Popups;

namespace FBH.Core
{
   public class AppHelper
    {
       public static CoreDispatcher Dispatcher { get; set; }

       public static ApplicationDataContainer LocalData { get { return ApplicationData.Current.LocalSettings; } } //获取本地设置，你也可以获取漫游设置和临时设置，后面的操作都一样

        /// <summary>
        /// 店铺名称设置是否成功
        /// </summary>
        public static bool IsShopNameLoaded { get; set; }

        public static async void MessageShow(string msg)
        {
            MessageDialog dialog = new MessageDialog(msg);
            await dialog.ShowAsync();
        }

        private static Dictionary<string, object> _tempData = new Dictionary<string, object>(0);
        public static Dictionary<string, object> TempData { get { return _tempData; } }
    }

    
}
