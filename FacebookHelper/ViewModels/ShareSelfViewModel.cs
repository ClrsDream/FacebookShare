using FacebookHelper.Codes;
using FBH.Core;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System;
using Windows.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using System.Linq;

namespace FacebookHelper.ViesModels
{
    public  class ShareSelfViewModel : ViewModelBase
    {
        public ShareSelfViewModel()
        {
        }

        ObservableCollection<ProductItem> _productCollection;
        /// <summary>
        /// 品类集合
        /// </summary>
        public ObservableCollection<ProductItem> ProductEntities
        {
            get {
                if (_productCollection == null)
                {
                    _productCollection = new ObservableCollection<ProductItem>();
                }
                return _productCollection;
            }
            set
            {
                _productCollection = value;
                this.NotifyPropertyChanged(p => p.ProductEntities);
            }
        }

        private string _shopUrl;
        public string ShopUrl
        {
            get { return _shopUrl; }
            set
            {
                _shopUrl = string.Format("http://m.facebook.com/{0}/shop", value);
                this.NotifyPropertyChanged(p => p.ShopUrl);
            }
        }

        private string _loginWebSource;
        public string LoginWebSource
        {
            get { return _shopUrl; }
            set
            {
                _loginWebSource = value;
                this.NotifyPropertyChanged(p => p.LoginWebSource);
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

                PopShow();
            }
        }
        private bool _isSettingOpen;
        public bool IsSettingOpen
        {
            get { return _isSettingOpen; }
            set
            {
                _isSettingOpen = value;
                this.NotifyPropertyChanged(p => p.IsSettingOpen);
            }
        }

        private bool _isLoginOpen;
        public bool IsLoginOpen
        {
            get { return _isLoginOpen; }
            set
            {
                _isLoginOpen = value;
                this.NotifyPropertyChanged(p => p.IsLoginOpen);
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
        

        private bool isLoaded;

        private async void PopShow()
        {
            IsPopOpen = true;
            await Task.Delay(2000);

            IsPopOpen = false;
        }

        public ICommand LoadedCmd
        {
            get
            {
                return new RelayCommand<object>(async (p) =>
                {
                    Cmd[CommondTypes.ShowTipMsg].Execute("正在读取数据……");


                    var products = await ProductHelper.GetProducts();

                    if (products == null)
                    {
                        TipMsg = "请在设置界面填写店铺名称！";
                    }
                    else
                    {
                        products.ToList().ForEach(item =>
                        {
                            ProductEntities.Add(item);
                            _productList.Enqueue(item);
                        });
                    }

                    Cmd[CommondTypes.HideTipMsg].Execute(null);

                });
            }
        }

        //public ICommand Submit
        //{
        //    get
        //    {
        //        return new RelayCommand<object>(p=> {

        //            if (string.IsNullOrEmpty(ShopUrl))
        //            {
        //                TipMsg = "Shop Address is empty";
        //                return;
        //            }

        //            //获取商品信息
        //            IsSettingOpen = false;
        //            localSettings.Values["shopurl"] = ShopUrl;//在本地设置中添加一个设置项，类似字典赋值方式，theme是localSettings里面的key，而"Light"是值，可以设置的类型在上面已经列出
        //                                                      //localSettings.Values.Remove("theme");//删除设置项

        //            try
        //            {
        //                Task.Factory.StartNew(async () => {
        //                    await AppHelper.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
        //                    {
        //                        var web = WebEnginner.CreateInstance("getproduct");

        //                        web.LoadCompleted += (s, e) => isLoaded = true;
        //                        web.Navigate(new Uri(ShopUrl));
        //                    });
        //                });
                       
        //            }
        //            catch (Exception ex)
        //            {
                        
        //            }
        //        });
        //    }
        //}
        private readonly Queue<ProductItem> _productList = new Queue<ProductItem>(0);
        public ICommand PostShare
        {
            get
            {
                return new RelayCommand<object>( p =>
                {
                    

                    if (!AppHelper.LocalData.Values.ContainsKey("shopurl")|| ProductEntities.Count<=0)
                    {
                        //IsSettingOpen = true;
                        TipMsg="请先设置店铺名称";
                        return;
                    }
                    
                    currentPid = 0;
                    _productList.Clear();

                    ProductEntities.ToList().ForEach(item=> {
                        item.ProValue = 0;
                        _productList.Enqueue(item);

                        });

                    var itemd = _productList.Dequeue();
                    itemd.ProValue = 10;

                    var firstpro = itemd.ProductUrl;

                    var urls = firstpro.TrimEnd('/').Split('/');
                    var rid = urls[urls.Length - 1];
                    currentPid = itemd.Id;

                    var postView = WebEnginner.CreateInstance("postReq");

                    postView.NavigationStarting -= postView_OnNavigationStarting;
                    postView.LoadCompleted -= postView_OnLoadCompleted;

                    postView.NavigationStarting += postView_OnNavigationStarting;
                    postView.LoadCompleted += postView_OnLoadCompleted;
                    postView.Navigate(new Uri(string.Format("https://m.facebook.com/sharer.php?fs=0&sid={0}&pid={1}", rid, currentPid)));
                    //                    var sender = WebEnginner.GetWebView("getproduct");

                    //                    string script = @"var productdivs=document.getElementsByClassName('_mCommerceReview__approvedProduct _rpc');

                    //var urls='';
                    //for(var i=0;i<productdivs.length;i++)
                    //{
                    //var pdiv=productdivs[i]; var picdiv=pdiv.childNodes[0].childNodes[0].childNodes[0]; var imgurl=picdiv.getAttribute('src');
                    //var ahref=pdiv.childNodes[1].childNodes[0].getAttribute('href'); var name=pdiv.childNodes[1].childNodes[0].innerText;
                    //urls+=ahref+','+imgurl+','+name+';';
                    //}
                    //urls;";
                    //                    var result = await sender.InvokeScriptAsync("eval", new string[] { script });


                    //                    if (!string.IsNullOrEmpty(result))
                    //                    {
                    //                        var proInfos = result.TrimEnd(';').Split(';');

                    //                        for (int i = 0; i < proInfos.Length; i++)
                    //                        {
                    //                            var item = proInfos[i];
                    //                            var infos = item.Split(',');

                    //                            var pro = new ProductItem
                    //                            {
                    //                                Id = i,
                    //                                ProductImg = infos[1],
                    //                                ProductUrl = infos[0],
                    //                                ProductName = infos[2]
                    //                            };

                    //                            ProductEntities.Add(pro);
                    //                            _productList.Enqueue(pro);
                    //                        }

                    //                        var itemd = _productList.Dequeue();
                    //                        itemd.ProValue = 10;

                    //                        var firstpro = itemd.ProductUrl;

                    //                        var urls = firstpro.TrimEnd('/').Split('/');
                    //                        var rid = urls[urls.Length - 1];
                    //                        currentPid = itemd.Id;

                    //                        var postView = WebEnginner.CreateInstance("postReq");

                    //                        postView.NavigationStarting -= postView_OnNavigationStarting;
                    //                        postView.LoadCompleted -= postView_OnLoadCompleted;

                    //                        postView.NavigationStarting += postView_OnNavigationStarting;
                    //                        postView.LoadCompleted += postView_OnLoadCompleted;
                    //                        postView.Navigate(new Uri(string.Format("https://m.facebook.com/sharer.php?fs=0&sid={0}&pid={1}", rid, currentPid)));
                    //                    }
                    //                    else
                    //                    {
                    //                        MessageDialog dialog = new MessageDialog("获取链接异常");
                    //                        await dialog.ShowAsync();
                    //                    }
                });
            }
        }
        private int currentPid = 0;
        private async void postView_OnNavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            var url = args.Uri.ToString();
            if (url.Contains("story"))
            {
                ProductEntities.Where(c => c.Id == currentPid).First().ProValue = 100;

                if (_productList.Count > 0)
                {
                    args.Cancel = true;

                    var itemd = _productList.Dequeue();
                    var firstpro = itemd.ProductUrl;

                    var urls = firstpro.TrimEnd('/').Split('/');
                    var rid = urls[urls.Length - 1];
                    currentPid = itemd.Id;

                    await Task.Delay(1000);

                    var postView = WebEnginner.CreateInstance("postReq");

                    postView.NavigationStarting -= postView_OnNavigationStarting;
                    postView.LoadCompleted -= postView_OnLoadCompleted;

                    postView.NavigationStarting += postView_OnNavigationStarting;
                    postView.LoadCompleted += postView_OnLoadCompleted;
                    postView.Navigate(new Uri(string.Format("https://m.facebook.com/sharer.php?fs=0&sid={0}&pid={1}", rid, currentPid)));
                }
            }
        }

        private async void postView_OnLoadCompleted(object sender, NavigationEventArgs e)
        {
            if (e.Uri.ToString().Contains("sharer"))
            {
                string script = @"function run(){document.getElementById('share_submit').click(); setTimeout(run,500);} run();";
                await WebEnginner.GetWebView("postReq").InvokeScriptAsync("eval", new string[] { script });
                
                ProductEntities.Where(c => c.Id == currentPid).First().ProValue = 70;

            }
        }
        
        //public ICommand ShowSetting
        //{
        //    get
        //    {
        //        return new RelayCommand<object>(p => IsSettingOpen = true);
        //    }
        //}
    }
}
