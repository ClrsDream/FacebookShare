using FBH.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace FacebookHelper.Codes
{
   public class ProductHelper
    {
        public static async Task<IList<ProductItem>> GetProducts()
        {
            try
            {
                if (AppHelper.TempData.ContainsKey("products"))
                {
                    return AppHelper.TempData["products"] as List<ProductItem>;
                }

                StorageFolder folder = ApplicationData.Current.LocalFolder;//获得本地文件夹

                StorageFile fileOpen = await folder.GetFileAsync("shopproducts.fbh");
                string content = await FileIO.ReadTextAsync(fileOpen);//读取文本  

                if (!string.IsNullOrEmpty(content))
                {
                    IList<ProductItem> _list = new List<ProductItem>();

                    var proInfos = content.TrimEnd(';').Split(';');

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

                    return _list;
                }
            }
            catch (Exception ex)
            {

            }

            return null;
        }
    }
}
