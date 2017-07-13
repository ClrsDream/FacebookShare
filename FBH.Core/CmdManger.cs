using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace FBH.Core
{
    /// <summary>
    /// 命令管理
    /// </summary>
    public class CmdManger
    {
        static readonly Dictionary<string, Action<object>> Cmd = new Dictionary<string, Action<object>>(0);

        /// <summary>
        /// 注册命令
        /// </summary>
        /// <param name="cmdName">命令名称</param>
        /// <param name="action">动作</param>
        public void RegisterCmd(string cmdName, Action<object> action)
        {
            if (Cmd.ContainsKey(cmdName)) return;

            // var cmd = new RelayCommand<ExCommandParameter>(action);
            Cmd.Add(cmdName, action);
        }

        /// <summary>
        /// 注册命令
        /// </summary>
        /// <param name="cmdName">命令名称</param>
        /// <param name="action">动作</param>
        public void RegisterCmd(CommondTypes cmdName, Action<object> action)
        {
            RegisterCmd(cmdName.ToString(), action);
        }

        /// <summary>
        /// 确定是否有指定命令
        /// </summary>
        /// <param name="cmdName">命令名称</param>
        /// <returns></returns>
        public bool Contains(string cmdName)
        {
            return Cmd.ContainsKey(cmdName);
        }

        public bool Contains(CommondTypes cmdName)
        {
            return Contains(cmdName.ToString());
        }

        /// <summary>
        /// 移除命令
        /// </summary>
        /// <param name="cmdName">命令名称</param>
        public void RemoveCmd(string cmdName)
        {
            if (Cmd.ContainsKey(cmdName))
            {
                Cmd.Remove(cmdName);
            }
        }

        public void RemoveCmd(CommondTypes cmdName)
        {
            RemoveCmd(cmdName.ToString());
        }

        /// <summary>
        /// 移除命令
        /// </summary>
        /// <param name="cmdNames">命令名称</param>
        public void RemoveCmd(params string[] cmdNames)
        {
            cmdNames.ToList().ForEach(item =>
            {
                if (item != null)
                {
                    RemoveCmd(item);
                }
            });
        }

        public void RemoveCmd(params CommondTypes[] cmdNames)
        {
            cmdNames.ToList().ForEach(RemoveCmd);
        }

        public ICommand this[string cmdName]
        {
            get
            {
                return Cmd.ContainsKey(cmdName) ? new RelayCommand<object>(Cmd[cmdName]) : null;
            }
        }

        public ICommand this[CommondTypes cmdName]
        {
            get
            {
                return this[cmdName.ToString()];
            }
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="cmdName">命令名称</param>
        /// <param name="parameter">参数</param>
        public void Execute(string cmdName, object parameter = null)
        {
            if (!Cmd.ContainsKey(cmdName)) return;

            var cmd = new RelayCommand<object>(Cmd[cmdName]);

           // var paraObj = new ExCommandParameter { Parameter = parameter };

            cmd.Execute(parameter);
        }

        public void Execute(CommondTypes cmdName, object parameter = null)
        {
            Execute(cmdName.ToString(), parameter);
        }

        ///// <summary>
        ///// 执行并返回值
        ///// 需要注册时实现 CallBackHandler 参数
        ///// </summary>
        ///// <typeparam name="T">返回值类型</typeparam>
        ///// <param name="cmdName">命令</param>
        ///// <param name="parameter">执行命令参数</param>
        ///// <returns></returns>
        //public T ExecuteResult<T>(string cmdName, object parameter = null)
        //{
        //    if (!Cmd.ContainsKey(cmdName)) return default(T);

        //    var cmd = new RelayCommand<ExCommandParameter>(Cmd[cmdName]);

        //    var paraObj = new ExCommandParameter { Parameter = parameter };

        //    cmd.Execute(paraObj);

        //    if (paraObj.CallBackHandler == null) return default(T);

        //    return (T)paraObj.CallBackHandler();
        //}

        ///// <summary>
        ///// 执行并返回值
        ///// 需要注册时实现 CallBackHandler 参数
        ///// </summary>
        ///// <typeparam name="T">返回值类型</typeparam>
        ///// <param name="cmdName">命令</param>
        ///// <param name="parameter">执行命令参数</param>
        ///// <returns></returns>
        //public T ExecuteResult<T>(CommondTypes cmdName, object parameter = null)
        //{
        //    return ExecuteResult<T>(cmdName.ToString(), parameter);
        //}

        ///// <summary>
        ///// 执行并调用回调函数
        ///// 需要注册时实现 CallBackActionHandler 参数
        ///// </summary>
        ///// <param name="cmdName">命令</param>
        ///// <param name="parameter">执行命令参数</param>
        //public void ExecuteCallBack(CommondTypes cmdName, object parameter = null)
        //{
        //    var _cmdName = cmdName.ToString();
        //    if (!Cmd.ContainsKey(_cmdName)) return;

        //    var cmd = new RelayCommand<ExCommandParameter>(Cmd[_cmdName]);

        //    var paraObj = new ExCommandParameter { Parameter = parameter };

        //    cmd.Execute(paraObj);

        //    if (paraObj.CallBackHandler == null) return;

        //    paraObj.CallBackActionHandler();
        //}

        /// <summary>
        /// 移除所有命令
        /// </summary>
        public void Clear()
        {
            Cmd.Clear();
        }

    }
}
