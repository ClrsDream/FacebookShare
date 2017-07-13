using System;
using System.ComponentModel;
using System.Linq.Expressions;
using Windows.UI.Xaml;

namespace FBH.Core
{
    /// <summary>
    /// ViewModel基类
    /// </summary>
    public class ViewModelBase : DependencyObject, INotifyPropertyChanged
    {

        /// <summary>
        /// 属性改变事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged == null) return;

            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

        }

        static ViewModelBase()
        {
            Cmd = new CmdManger();
        }

        public static CmdManger Cmd { get; private set; }
    }

    /// <summary>
    /// 扩展类
    /// </summary>
    public static class ViewModelBaseEx
    {
        public static void NotifyPropertyChanged<T, TProperty>(this T propertyChangedBase, Expression<Func<T, TProperty>> expression) where T : ViewModelBase
        {
            var memberExpression = expression.Body as MemberExpression;

            if (memberExpression == null)
                throw new NotImplementedException();

            var propertyName = memberExpression.Member.Name;
            propertyChangedBase.NotifyPropertyChanged(propertyName);

        }
    }
}
