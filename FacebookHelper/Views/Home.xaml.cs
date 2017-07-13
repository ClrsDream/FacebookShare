using FacebookHelper.Codes;
using FBH.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace FacebookHelper.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Home : Page
    {
        public Home()
        {
            this.InitializeComponent();

            ViewModelBase.Cmd[CommondTypes.ShowTipMsg].Execute("Waiting……");
        }

        private void LoginWv_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            if (!args.Uri.ToString().Contains("login"))
            {
                //WebEnginner.Add("login", loginwv);
                sender.Visibility = Visibility.Collapsed;
            }
        }

        private async void loginwv_LoadCompleted(object sender, NavigationEventArgs e)
        {
            if (e.Uri.ToString().Contains("login"))
            {
                loginwv.Visibility = Visibility.Visible;
            }
            else
            {
                await Task.Delay(500);
                var result = await loginwv.InvokeScriptAsync("eval", new string[] { @" var scrpts=document.getElementsByTagName('script');

var txt='';
                for (var i = 0; i < scrpts.length; i++)
                {

                    var scrpt = scrpts[i];
                    var scrtxt = scrpt.innerText;
                    if (scrtxt != '' && scrtxt.indexOf('logout') >= 0)
                    {
                        txt += scrpt.innerText;
                    }

                }
                txt; " });

                if (!string.IsNullOrEmpty(result))
                {
                    var scriptxts = result.Split(new string[] { "logoutURL\":\"\\", "\",\"", "push_phase" },StringSplitOptions.RemoveEmptyEntries);
                    
                    var url = scriptxts.Where(c=>c.Contains("logout.php")).FirstOrDefault();

                    AppHelper.TempData.Add("logouturl", url);
                }
            }

            ViewModelBase.Cmd[CommondTypes.HideTipMsg].Execute(null);
        }
    }
}
