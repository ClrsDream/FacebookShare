using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookHelper.Codes
{
    /// <summary>
    /// 命令操作接口
    /// </summary>
    public interface ICmdOption
    {
        /// <summary>
        /// 初始化命令
        /// </summary>
        void InitCmd();

        /// <summary>
        /// 清理命令
        /// </summary>
        void ClearCmd();
    }
}
