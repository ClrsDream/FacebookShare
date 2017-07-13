using System;
using System.Windows.Input;
using Windows.UI.Xaml;
using Microsoft.Xaml.Interactivity;
namespace FBH.Core
{
    /// <summary>
    /// 命令 支持泛型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RelayCommand<T> : ICommand where T : class
    {
        private readonly Func<bool> _canExecute;
        private readonly Action<T> _execute;

        public RelayCommand(Action<T> execute) : this(execute, null)
        {
        }

        public RelayCommand(Action<T> execute, Func<bool> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        public void Execute(object parameter)
        {
            _execute(parameter as T);
        }

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }

    /// <summary>
    /// 扩展CommandParameter，使CommandParamerer可以带事件参数
    /// </summary>
    public class ExCommandParameter
    {
        /// <summary>
        /// 事件触发源
        /// </summary>
        public DependencyObject Sender { get; set; }

        /// <summary>
        /// 事件参数
        /// </summary>
        public EventArgs EventArgs { get; set; }

        /// <summary>
        /// 额外参数
        /// </summary>
        public object Parameter { get; set; }
        /// <summary>
        /// 回调委托
        /// </summary>
        public Func<object> CallBackHandler { get; set; }

        /// <summary>
        /// 回调委托
        /// </summary>
        public Action CallBackActionHandler { get; set; }
    }


    ///// <summary>
    ///// 扩展InvokeCommandAction
    ///// </summary>
    //public class ExInvokeCommandAction : Windows.UI.Xaml.TriggerAction
    //{
    //    public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command",
    //        typeof(ICommand), typeof(ExInvokeCommandAction), null);

    //    public static readonly DependencyProperty CommandParameterProperty =
    //        DependencyProperty.Register("CommandParameter", typeof(object), typeof(ExInvokeCommandAction), null);

    //    /// <summary>
    //    /// 获取或设置此操作应调用的命令，这是依赖属性。
    //    /// </summary>
    //    /// <value>要执行的命令</value>
    //    /// <remarks>如果设置了此属性和CommandName属性，则此属性将优先于后者</remarks>
    //    public ICommand Command
    //    {
    //        get { return (ICommand)GetValue(CommandProperty); }

    //        set { SetValue(CommandProperty, value); }
    //    }

    //    /// <summary>
    //    /// 获取或设置命令参数，这是依赖属性
    //    /// </summary>
    //    /// <value>命令参数</value>
    //    /// <remarks>这是传递给ICommand.CanExecute 和 ICommand.Execute 的值</remarks>
    //    public object CommandParameter
    //    {
    //        get { return GetValue(CommandParameterProperty); }

    //        set { SetValue(CommandParameterProperty, value); }
    //    }

    //    /// <summary>
    //    /// 调用操作
    //    /// </summary>
    //    /// <param name="parameter">操作的参数，如果不需要参数，则可以将参数设置为空引用</param>
    //    protected override void Invoke(object parameter)
    //    {
    //        if (AssociatedObject == null) return;

    //        var command = ResolveCommand();

    //        var exParameter = new ExCommandParameter
    //        {
    //            Sender = AssociatedObject,
    //            Parameter = CommandParameter,
    //            EventArgs = parameter as EventArgs
    //        };

    //        if (command != null && command.CanExecute(exParameter))
    //        {
    //            command.Execute(exParameter);
    //        }

    //    }


    //    private ICommand ResolveCommand()
    //    {
    //        ICommand result = null;

    //        if (Command != null)
    //        {
    //            result = Command;
    //        }
    //        else if (AssociatedObject != null)
    //        {
    //            var assType = AssociatedObject.GetType();

    //            var propertyes = assType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

    //            var array =
    //                propertyes.Where(
    //                    propertyInfo =>
    //                        typeof(ICommand).IsAssignableFrom(propertyInfo.PropertyType) &&
    //                        string.Equals(propertyInfo.Name, "", StringComparison.Ordinal)); //CommandName 替换为""

    //            foreach (var item in array)
    //            {
    //                result = (ICommand)item.GetValue(base.AssociatedObject, null);
    //            }
    //        }

    //        return result;
    //    }
    //}
}
