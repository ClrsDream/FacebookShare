using FacebookHelper.Codes;
using FBH.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

namespace FacebookHelper.ViewModels
{
    public class ShareGroupViewModel : ViewModelBase
    {
        public ShareGroupViewModel()
        {
           

        }
        
        private bool isLoaded;
        private bool isGroupLoaded;
        
        ObservableCollection<GroupProductItem> _productCollection;
        public ObservableCollection<GroupProductItem> ProductEntities
        {
            get
            {
                if (_productCollection == null)
                {
                    _productCollection = new ObservableCollection<GroupProductItem>();
                }
                return _productCollection;
            }
            set
            {
                _productCollection = value;
                this.NotifyPropertyChanged(p => p.ProductEntities);
            }
        }

        ObservableCollection<Codes.GroupItem> _groupCollection;
        public ObservableCollection<Codes.GroupItem> GroupEntities
        {
            get
            {
                if (_groupCollection == null)
                {
                    _groupCollection = new ObservableCollection<Codes.GroupItem>();
                }
                return _groupCollection;
            }
            set
            {
                _groupCollection = value;
                this.NotifyPropertyChanged(p => p.GroupEntities);
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

        private string _groupProductInfo;

        public string GroupProductInfo
        {
            get { return _groupProductInfo; }
            set
            {
                _groupProductInfo =  value;
                this.NotifyPropertyChanged(p => p.GroupProductInfo);
            }
        }

        private string _currentPostInfo;

        public string CurrentPostInfo
        {
            get { return _currentPostInfo; }
            set
            {
                _currentPostInfo = value;
                this.NotifyPropertyChanged(p => p.CurrentPostInfo);
            }
        }

        private int _PostProValue;

        public int PostProValue
        {
            get
            {
                return _PostProValue;
            }
            set
            {
                _PostProValue = value;
                this.NotifyPropertyChanged(p=>p.PostProValue);
            }
        }

        private int _PostMaximun=100;

        public int PostMaximun
        {
            get
            {
                return _PostMaximun;
            }
            set
            {
                _PostMaximun = value;
                this.NotifyPropertyChanged(p => p.PostMaximun);
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

        private string charset_test;
        private string sessionId;
        private string fb_dtsg;

        private async void PopShow()
        {
            IsPopOpen = true;
            await Task.Delay(2000);

            IsPopOpen = false;
        }

        public ICommand Loaded
        {
            get
            {
                return new RelayCommand<object>(async p =>
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
                            var pitem = new GroupProductItem();
                            pitem.Id = item.Id;
                            pitem.ProductImg = item.ProductImg;
                            pitem.ProductName = item.ProductName;
                            pitem.ProductUrl = item.ProductUrl;
                           
                            ProductEntities.Add(pitem);
                        });
                    }
                    
                    var web = WebEnginner.CreateInstance("getgroup");

                    web.LoadCompleted += async (s, e) =>
                    {
                        var nextCount = 0;
                        var oldCount = -1;

                        while (nextCount != oldCount)
                        {
                            oldCount = nextCount;
                            string scriptp = @"var lef=document.getElementsByClassName('_4g33 _5fiy').length;lef.toString();";
                            var result = await web.InvokeScriptAsync("eval", new string[] { scriptp });
                            nextCount = int.Parse(result == "" ? "0" : result);

                            scriptp = @"window.scrollTo(0,document.body.scrollHeight);";

                            await web.InvokeScriptAsync("eval", new string[] { scriptp });

                            await Task.Delay(1000);

                            await web.InvokeScriptAsync("eval", new string[] { scriptp });
                        }

                        string script = @"
                                var urls='';
try{
                                var btns=document.getElementsByClassName('_4g33 _5fiy');
                                for(var i=0;i<btns.length;i++)
	                                {
		                                var pdiv=btns[i];
		                                var infoDiv=pdiv.childNodes[1];
		                                var nameDiv=infoDiv.childNodes[0];
		                                var namea=nameDiv.childNodes[0];
		                                var ahref=namea.getAttribute('href'); 
		                                var name=namea.innerText;
		
		                                var memSpn=infoDiv.childNodes[1];
		                                var memCount=memSpn.childNodes[2].textContent;
		
		                                urls+=name+'>'+ahref+'>'+memCount+';';
	                                } 
}
catch(err){}
                                urls;";

                        var resultGroup = await web.InvokeScriptAsync("eval", new string[] { script });
                        if (!string.IsNullOrEmpty(resultGroup))
                        {
                            GroupEntities.Clear();
                            _groupList.Clear();
                            currentGroupItem = null;
                            var groInfos = resultGroup.TrimEnd(';').Split(';');

                            for (int i = 0; i < groInfos.Length; i++)
                            {
                                var item = groInfos[i];
                                var infos = item.Split('>');

                                var pro = new Codes.GroupItem
                                {
                                    Id = i,
                                    Name = infos[0],
                                    Address = infos[1],
                                    MerCount = infos[2]
                                };

                                GroupEntities.Add(pro);
                            }

                            isGroupLoaded = true;
                        }
                        else
                        {
                            TipMsg = "获取小组信息失败";
                        }

                        GroupProductInfo = string.Format("小组：{0}个，商品：{1}个",GroupEntities.Count,ProductEntities.Count);
                        
                    };
                    web.Navigate(new Uri("https://m.facebook.com/groups/?category=membership"));

                    var formInfoWv = WebEnginner.CreateInstance("form");

                    formInfoWv.DOMContentLoaded += async (s, e) =>
                    {

                        string script = "document.getElementsByName('session_id')[0].value";

                        sessionId = await formInfoWv.InvokeScriptAsync("eval", new string[] { script });

                        script = "document.getElementsByName('fb_dtsg')[0].value";

                        fb_dtsg = await formInfoWv.InvokeScriptAsync("eval", new string[] { script });

                        script = "document.getElementsByName('charset_test')[0].value";

                        charset_test = await formInfoWv.InvokeScriptAsync("eval", new string[] { script });
                        Cmd[CommondTypes.HideTipMsg].Execute(null);
                    };

                    formInfoWv.Navigate(new Uri("https://m.facebook.com/sharer.php?m=group&sid=0&usedialogwithselector=1"));

                });
            }
        }
        //public ICommand Submit
        //{
        //    get
        //    {
        //        return new RelayCommand<object>(p => {

        //            if (string.IsNullOrEmpty(ShopUrl))
        //            {
        //                TipMsg = "Shop Name is empty";
        //                return;
        //            }

        //            //获取商品信息
        //            IsSettingOpen = false;
        //            AppHelper.LocalData.Values["shopurl"] = ShopUrl;//在本地设置中添加一个设置项，类似字典赋值方式，theme是localSettings里面的key，而"Light"是值，可以设置的类型在上面已经列出
        //                                                      //localSettings.Values.Remove("theme");//删除设置项

        //            try
        //            {
        //                var web = WebEnginner.CreateInstance("getproductg");

        //                web.LoadCompleted += (s, e) => isLoaded = true;
        //                web.Navigate(new Uri(ShopUrl));
        //            }
        //            catch (Exception ex)
        //            {

        //            }
        //        });
        //    }
        //}

        private readonly Queue<GroupProductItem> _productList = new Queue<GroupProductItem>(0);
        private readonly Queue<Codes.GroupItem> _groupList = new Queue<Codes.GroupItem>(0);
        private Codes.GroupItem currentGroupItem;
        public ICommand PostShare
        {
            get
            {
                return new RelayCommand<object>(async p =>
               {
                   if (!AppHelper.LocalData.Values.ContainsKey("shopurl"))
                   {
                       IsSettingOpen = true;
                       TipMsg = "请先设置店铺地址";
                       return;
                   }

                   if (ProductEntities.Count <= 0 || !isGroupLoaded)
                   {
                       TipMsg = "系统正在更新设置，请稍后再试";
                       return;
                   }

                   var list = GroupEntities.Where(c => c.IsCheck).ToList();

                   if (list.Count <= 0)
                   {
                       TipMsg = "请选择要推送的小组";
                       return;
                   }

                   _productList.Clear();
                   _groupList.Clear();
                   currentPid = 0;

                   ProductEntities.ToList().ForEach(item =>
                   {
                       item.ProValue = 0;
                       item.PostedGroups.Clear();
                       _productList.Enqueue(item);
                   });

                   list.ForEach(item => _groupList.Enqueue(item));

                   PostMaximun = _groupList.Count * _productList.Count;

                  

                   var postView = WebEnginner.GetWebView("form");
                   postView.NavigationStarting += postView_OnNavigationStarting;

                   while (_groupList.Count > 0)
                   {
                       if (_productList.Count<=0)
                       {
                           ProductEntities.ToList().ForEach(item => _productList.Enqueue(item));
                       }
                       currentGroupItem = _groupList.Dequeue();

                       while (_productList.Count>0)
                       {
                           PostProValue++;

                           var itemd = _productList.Dequeue();
                           itemd.ProValue = 10;

                           var firstpro = itemd.ProductUrl;
                           var uri = new Uri("https://m.facebook.com/"+firstpro.TrimEnd('/'));
                           //var urls = firstpro.TrimEnd('/').Split('/');
                           var rid = uri.Segments[uri.Segments.Length - 1];
                           currentPid = itemd.Id;
                           
                           CurrentPostInfo = string.Format("当前推送小组为：{0},商品为：{1}", currentGroupItem.Name, itemd.ProductName);

                           Windows.Web.Http.HttpRequestMessage request = new Windows.Web.Http.HttpRequestMessage(
                        Windows.Web.Http.HttpMethod.Post,
                        new Uri("https://m.facebook.com/a/sharer.php?shouldRedirectToPermalink=1&usedialogwithselector=1&isthrowbackpost"));
                           request.Headers.TryAppendWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
                           request.Content = new HttpStringContent("app_id=&charset_test=" + charset_test + "&comment=&fb_dtsg=" + fb_dtsg + "&fr=&friend_target=&fs=&group_target=" + currentGroupItem.GroupId + "&m=group&privacyx=300645083384735&referrer=&session_id=" + sessionId + "&sid=" + rid, UnicodeEncoding.Utf8);

                           itemd.ProValue = 70;

                           postView.NavigateWithHttpRequestMessage(request);

                           itemd.ProValue = 100;
                           itemd.PostedGroups.Add(currentGroupItem);

                           await Task.Delay(5000);
                       }
                   }
                    //var postView = WebEnginner.CreateInstance("postReqg");

                    //postView.NavigationStarting -= postView_OnNavigationStarting;
                    //postView.LoadCompleted -= postView_OnLoadCompleted;

                    //postView.NavigationStarting += postView_OnNavigationStarting;
                    //postView.LoadCompleted += postView_OnLoadCompleted;
                    //postView.Navigate(new Uri(string.Format("https://m.facebook.com/sharer.php?m=group&sid={0}&usedialogwithselector=1", rid)));

                    //var sender = WebEnginner.GetWebView("getproductg");

                    //string script = @"var productdivs=document.getElementsByClassName('_mCommerceReview__approvedProduct _rpc');
                    //                var urls='';
                    //                for(var i=0;i<productdivs.length;i++)
                    //                {
                    //                var pdiv=productdivs[i]; var picdiv=pdiv.childNodes[0].childNodes[0].childNodes[0]; 
                    //                var imgurl=picdiv.getAttribute('src');
                    //                var ahref=pdiv.childNodes[1].childNodes[0].getAttribute('href'); 
                    //                var name=pdiv.childNodes[1].childNodes[0].innerText;
                    //                urls+=ahref+','+imgurl+','+name+';';
                    //                }
                    //                urls;";
                    //var result = await sender.InvokeScriptAsync("eval", new string[] { script });

                    //script = @"var btns=document.getElementsByClassName('_4g33 _5fiy');
                    //            var urls='';
                    //            for(var i=0;i<btns.length;i++)
                    //             {
                    //              var pdiv=btns[i];
                    //              var infoDiv=pdiv.childNodes[1];
                    //              var nameDiv=infoDiv.childNodes[0];
                    //              var namea=nameDiv.childNodes[0];
                    //              var ahref=namea.getAttribute('href'); 
                    //              var name=namea.innerText;

                    //              var memSpn=infoDiv.childNodes[1];
                    //              var memCount=memSpn.childNodes[2].textContent;

                    //              urls+=name+'>'+ahref+'>'+memCount+';';
                    //             } 
                    //            urls;";
                    //var groupWV = WebEnginner.GetWebView("getgroup");
                    //var resultGroup = await groupWV.InvokeScriptAsync("eval", new string[] { script });

                    //if (!string.IsNullOrEmpty(result))
                    //{
                    //    var proInfos = result.TrimEnd(';').Split(';');

                    //    for (int i = 0; i < proInfos.Length; i++)
                    //    {
                    //        var item = proInfos[i];
                    //        var infos = item.Split(',');

                    //        var pro = new GroupProductItem
                    //        {
                    //            Id = i,
                    //            ProductImg = infos[1],
                    //            ProductUrl = infos[0],
                    //            ProductName = infos[2]
                    //        };

                    //        ProductEntities.Add(pro);
                    //        _productList.Enqueue(pro);
                    //    }

                    //    var groInfos = resultGroup.TrimEnd(';').Split(';');

                    //    for (int i = 0; i < groInfos.Length; i++)
                    //    {
                    //        var item = groInfos[i];
                    //        var infos = item.Split('>');

                    //        var pro = new Codes.GroupItem
                    //        {
                    //            Id = i,
                    //            Name = infos[0],
                    //            Address = infos[1],
                    //            MerCount = infos[2]
                    //        };

                    //        GroupEntities.Add(pro);
                    //        _groupList.Enqueue(pro);
                    //    }

                    //    var itemd = _productList.Dequeue();
                    //    itemd.ProValue = 10;

                    //    var firstpro = itemd.ProductUrl;

                    //    var urls = firstpro.TrimEnd('/').Split('/');
                    //    var rid = urls[urls.Length - 1];
                    //    currentPid = itemd.Id;

                    //    currentGroupItem = _groupList.Dequeue();

                    //    var postView = WebEnginner.CreateInstance("postReqg");

                    //    postView.NavigationStarting -= postView_OnNavigationStarting;
                    //    postView.LoadCompleted -= postView_OnLoadCompleted;

                    //    postView.NavigationStarting += postView_OnNavigationStarting;
                    //    postView.LoadCompleted += postView_OnLoadCompleted;
                    //    postView.Navigate(new Uri(string.Format("https://m.facebook.com/sharer.php?m=group&sid={0}&usedialogwithselector=1", rid)));
                    //}
                    //else
                    //{
                    //    MessageDialog dialog = new MessageDialog("获取链接异常");
                    //    await dialog.ShowAsync();
                    //}
                });
            }
        }
        private int currentPid = 0;
        private void postView_OnNavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            var url = args.Uri.ToString();
            if (url.Contains("error"))
            {
                TipMsg = "请求异常";
                args.Cancel = true;

                //next();

                return;
            }

            if (url.Contains("groups"))
            {
               //var pro= ProductEntities.Where(c => c.Id == currentPid).First();
               // pro.ProValue = 100;
               // pro.PostedGroups.Add(currentGroupItem);

                args.Cancel = true;

                //next();
            }
        }


        private async void next()
        {
            if (_groupList.Count <= 0 && _productList.Count <= 0)
            {
                return;
            }

            await Task.Delay(5000);

            PostProValue++;

            if (_productList.Count > 0)
            {
                var itemd = _productList.Dequeue();
                var firstpro = itemd.ProductUrl;

                var urls = firstpro.TrimEnd('/').Split('/');
                var rid = urls[urls.Length - 1];
                currentPid = itemd.Id;
                itemd.ProValue = 10;
                CurrentPostInfo = string.Format("当前推送小组为：{0},商品为：{1}", currentGroupItem.Name, itemd.ProductName);
                var postView = WebEnginner.CreateInstance("postReqg");

                postView.NavigationStarting -= postView_OnNavigationStarting;
                postView.LoadCompleted -= postView_OnLoadCompleted;

                postView.NavigationStarting += postView_OnNavigationStarting;
                postView.LoadCompleted += postView_OnLoadCompleted;
                postView.Navigate(new Uri(string.Format("https://m.facebook.com/sharer.php?m=group&group_target=0&u&id&ids&sid={0}&usedialogwithselector=1&_rdr", rid)));
            }
            else
            {
                currentGroupItem = _groupList.Dequeue();

                ProductEntities.ToList().ForEach(item => _productList.Enqueue(item));

                var itemd = _productList.Dequeue();
                var firstpro = itemd.ProductUrl;

                var urls = firstpro.TrimEnd('/').Split('/');
                var rid = urls[urls.Length - 1];
                currentPid = itemd.Id;
                itemd.ProValue = 10;
                CurrentPostInfo = string.Format("当前推送小组为：{0},商品为：{1}", currentGroupItem.Name, itemd.ProductName);
                var postView = WebEnginner.CreateInstance("postReqg");

                postView.NavigationStarting -= postView_OnNavigationStarting;
                postView.LoadCompleted -= postView_OnLoadCompleted;

                postView.NavigationStarting += postView_OnNavigationStarting;
                postView.LoadCompleted += postView_OnLoadCompleted;
                postView.Navigate(new Uri(string.Format("https://m.facebook.com/sharer.php?m=group&group_target=0&u&id&ids&sid={0}&usedialogwithselector=1&_rdr", rid)));
            }
        }

        private async void postView_OnLoadCompleted(object sender, NavigationEventArgs e)
        {
            if (e.Uri.ToString().Contains("sharer"))
            {
                var wv = sender as WebView;
                var id = currentGroupItem.GroupId;
                string script = "try{ document.getElementsByName('group_target')[0].setAttribute('value','" + id + "');}catch(err){}";
                await wv.InvokeScriptAsync("eval", new string[] { script });

                await Task.Delay(200);
                script = "try{ document.getElementById('u_0_6').setAttribute('value','C'); }catch(err){}";
                await wv.InvokeScriptAsync("eval", new string[] { script });

                await Task.Delay(200);
                script = "try{ document.getElementsByName('privacyx')[0].setAttribute('value','291667064279714'); }catch(err){}";
                await wv.InvokeScriptAsync("eval", new string[] { script });

                await Task.Delay(500);

                script = @"function run(){ try{ document.getElementById('share_submit').click(); }catch(err){}  setTimeout(run,500);} run();";
                await wv.InvokeScriptAsync("eval", new string[] { script });

                ProductEntities.Where(c => c.Id == currentPid).First().ProValue = 70;

            }
        }

        public ICommand ShowSetting
        {
            get
            {
                return new RelayCommand<object>(p => IsSettingOpen = true);
            }
        }
    }
}
