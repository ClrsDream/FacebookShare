using FacebookHelper.Codes;
using FBH.Core;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace FacebookHelper
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            AppHelper.Dispatcher = Dispatcher;

            itemDic.Add("Home", HamburgerMenu.Content);

            HamburgerMenu.ItemsSource = new List<BnttonDataItem> {
                new BnttonDataItem { Title="Home", Category= "Home", Thumbnail= "Home" },
                new BnttonDataItem { Title="分享到自己时间线", Category= "FacebookHelper.Views.ShareSelf", Thumbnail= "Clock" },
                 new BnttonDataItem { Title="分享到小组", Category= "FacebookHelper.Views.ShareGroup", Thumbnail= "People" },
                
            };

            HamburgerMenu.OptionsItemsSource = new List<BnttonDataItem>
            {
                 new BnttonDataItem { Title="设置", Category= "FacebookHelper.Views.Setting", Thumbnail= "Setting" },
                 new BnttonDataItem { Title="关于", Category= "FacebookHelper.Views.About", Thumbnail= "Comment" }
            };

            }

        Dictionary<string, object> itemDic = new Dictionary<string, object>();
        private void HamburgerMenu_ItemClick(object sender, ItemClickEventArgs e)
        {
            var category = (e.ClickedItem as BnttonDataItem).Category;

            object obj = null;

            if (itemDic.ContainsKey(category))
            {
                obj = itemDic[category];
            }
            else
            {
                var ty = Type.GetType(category);
                obj = Activator.CreateInstance(ty);
            }

            HamburgerMenu.Content = obj; 
        }

        private void HamburgerMenu_OptionsItemClick(object sender, ItemClickEventArgs e)
        {
            var category = (e.ClickedItem as BnttonDataItem).Category;

            object obj = null;

            if (itemDic.ContainsKey(category))
            {
                obj = itemDic[category];
            }
            else
            {
                var ty = Type.GetType(category);
                obj = Activator.CreateInstance(ty);
            }

            HamburgerMenu.Content = obj;
        }
    }
}
