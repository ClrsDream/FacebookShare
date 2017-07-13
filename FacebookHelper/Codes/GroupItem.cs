using FBH.Core;
using System.Collections.ObjectModel;

namespace FacebookHelper.Codes
{
    public class GroupItem : ViewModelBase
    {
        public string ImgUrl { get { return "https://static.xx.fbcdn.net/rsrc.php/v3/yE/r/1rlhwgjUxPz.png"; } }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        private bool _isChecked;
        public bool IsCheck {
            get { 

                return _isChecked;
            }
            set
            {
                _isChecked = value;
                this.NotifyPropertyChanged(p=>p.IsCheck);
            }
        }

        public string GroupId
        {
            get
            {
                if (Address!="")
                {
                    var urlP = Address.Split('?');
                    var urlPid = urlP[0].Split('/');
                    var gid = urlPid[urlPid.Length-1];
                    return gid;
                }

                return "535659029961696";
            }
        }

        public string MerCount { get; set; }

    }

    public class GroupProductItem : ViewModelBase
    {
        public string ProductImg { get; set; }

        public string ProductName { get; set; }

        public string ProductUrl { get; set; }

        public int Id { get; set; }

        private int _value;
        public int ProValue
        {
            get { return _value; }
            set
            {
                _value = value;
                this.NotifyPropertyChanged(p => p.ProValue);

            }
        }

        ObservableCollection<GroupItem> _postedGroups;

        public ObservableCollection<GroupItem>  PostedGroups {
            get
            {
                if (_postedGroups == null)
                {
                    _postedGroups = new ObservableCollection<GroupItem>();
                }
                return _postedGroups;
            }
            set
            {
                _postedGroups = value;
                this.NotifyPropertyChanged(p => p.PostedGroups);
            }
        }

    }
}
