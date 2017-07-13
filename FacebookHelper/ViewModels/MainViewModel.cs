using FacebookHelper.Codes;
using FBH.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FacebookHelper.ViewModels
{
   public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            Cmd.RegisterCmd(CommondTypes.ShowTipMsg, (p)=> { TipMsg = p.ToString(); });
            Cmd.RegisterCmd(CommondTypes.HideTipMsg,(p)=> { IsPopOpen = false; });
        }

        private string _tipMsg;
        public string TipMsg
        {
            get { return _tipMsg; }
            set
            {
                _tipMsg = value;
                this.NotifyPropertyChanged(p => p.TipMsg);
                IsPopOpen = true;
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
        
    }
}
