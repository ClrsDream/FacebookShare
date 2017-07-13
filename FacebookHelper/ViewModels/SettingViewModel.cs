using FacebookHelper.Codes;
using FBH.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace FacebookHelper.ViewModels
{
   public class SettingViewModel : ViewModelBase
    {
        public SettingViewModel()
        {
            //if (AppHelper.LocalData.Values.ContainsKey("shopname"))
            //{
            //    _shopName = AppHelper.LocalData.Values["shopname"].ToString();
            //}
        }

        private string _shopUrl;
        private string _shopName;
        public string ShopUrl
        {
            get { return _shopName; }
            set
            {
                _shopName = value;
                _shopUrl = string.Format("http://m.facebook.com/{0}/shop", value);
                this.NotifyPropertyChanged(p => p.ShopUrl);
            }
        }

        private string _tipMsg;
        public string TipMsg
        {
            get { return _tipMsg; }
            set
            {
                _tipMsg = value;
                this.NotifyPropertyChanged(p => p.TipMsg);

               // PopShow();
            }
        }
        private bool _isPopOpen;
        public bool IsPopOpen
        {
            get { return _isPopOpen; }
            set
            {
                _isPopOpen = value;
                this.NotifyPropertyChanged(p => p.IsPopOpen);
            }
        }
        private async void PopShow()
        {
            IsPopOpen = true;
            await Task.Delay(2000);

            IsPopOpen = false;
        }

        public ICommand Submit
        {
            get
            {
                return new RelayCommand<object>(p => {

                    if (string.IsNullOrEmpty(ShopUrl))
                    {
                        TipMsg = "Shop Name is empty";
                        return;
                    }
                    
                    try
                    {
                        Cmd[CommondTypes.ShowTipMsg].Execute("正在设置，请稍等……");
                        var web = WebEnginner.CreateInstance("product");
                        
                        web.LoadCompleted += async (s, e) =>
                        {
                            if (!e.Uri.ToString().Contains(_shopName))
                            {
                                TipMsg = "设置异常！";
                                Cmd[CommondTypes.HideTipMsg].Execute(null);
                                return;
                            }

                            await Task.Delay(1000);
                            string script = @"var productdivs=document.getElementsByClassName('_mCommerceReview__approvedProduct _rpc');
                                                var urls='';
                                                for(var i=0;i<productdivs.length;i++)
                                                {
                                                var pdiv=productdivs[i]; var picdiv=pdiv.childNodes[0].childNodes[0].childNodes[0]; var imgurl=picdiv.getAttribute('src');
                                                var ahref=pdiv.childNodes[1].childNodes[0].getAttribute('href'); var name=pdiv.childNodes[1].childNodes[0].innerText;
                                                urls+=ahref+','+imgurl+','+name+';';
                                                }
                                                urls;";

                            var result = await web.InvokeScriptAsync("eval", new string[] { script });

                            if (!string.IsNullOrEmpty(result))
                            {
                                IList<ProductItem> _list = new List<ProductItem>();

                                var proInfos = result.TrimEnd(';').Split(';');

                                for (int i = 0; i < proInfos.Length; i++)
                                {
                                    var item = proInfos[i];
                                    var infos = item.Split(',');

                                    var pro = new ProductItem
                                    {
                                        Id = i,
                                        ProductImg = infos[1],
                                        ProductUrl = infos[0],
                                        ProductName = infos[2]
                                    };

                                    _list.Add(pro);
                                }

                                AppHelper.TempData.Add("products",_list);

                                StorageFolder folder = ApplicationData.Current.LocalFolder;//获得本地文件夹
                                StorageFile file = await folder.CreateFileAsync("shopproducts.fbh", CreationCollisionOption.OpenIfExists);//创建文件
                                await FileIO.WriteTextAsync(file, result);
                              
                                //StorageFile fileOpen = folder.GetFileAsync("first.txt");
                                //string content = await FileIO.ReadTextAsync(fileOpen);//读取文本  
                                //AppHelper.LocalData.Values["products"] = result;
                                AppHelper.LocalData.Values["shopproducts"] = "shopproducts.fbh";
                                AppHelper.LocalData.Values["shopname"] = _shopName;
                                AppHelper.LocalData.Values["shopurl"] = _shopUrl;

                                TipMsg = "设置完成！";
                                
                            }
                            else
                            {
                                TipMsg = "设置异常！";
                            }
                            
                            Cmd[CommondTypes.HideTipMsg].Execute(null);
                        };

                        web.Navigate(new Uri(_shopUrl));
                    }
                    catch (Exception ex)
                    {
                        TipMsg = ex.Message;
                    }
                });
            }
        }

        public ICommand LogoutFB
        {
            get
            {
                return new RelayCommand<object>(p => {
                    var web = p as WebView;
                    try
                    {
                        TipMsg = "正在设置，请稍等……";
                        

                        web.NavigationStarting += (s,e)=>{

                            if (e.Uri.ToString().Contains("stype=lo&")|| e.Uri.ToString().Contains("login"))
                            {
                                TipMsg = "当前账号退出成功！";
                                web.Visibility = Windows.UI.Xaml.Visibility.Visible;
                            }
                        };

                        web.LoadCompleted += async (s, e) =>
                        {
                            if (e.Uri.ToString().Contains("home"))
                            {
                                var result = await web.InvokeScriptAsync("eval", new string[] { @"  var scrpts=document.getElementsByTagName('script');

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
                                    var scriptxts = result.Split(new string[] { "logoutURL\":\"\\", "\",\"", "push_phase" }, StringSplitOptions.RemoveEmptyEntries);

                                    var url = scriptxts.Where(c => c.Contains("logout.php")).FirstOrDefault();

                                    if (AppHelper.TempData.ContainsKey("logouturl"))
                                    {
                                        AppHelper.TempData["logouturl"]= url;
                                    }
                                    else
                                    {
                                        AppHelper.TempData.Add("logouturl", url);
                                    }
                                    
                                }

                            }
                        };

                        web.Navigate(new Uri("https://m.facebook.com" + AppHelper.TempData["logouturl"].ToString()));
                    }
                    catch (Exception ex)
                    {
                        TipMsg = ex.Message+",请手动执行切换！";

                        web.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    }
                });
            }
        }
    }
}
