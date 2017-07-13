namespace FBH.Core.Models
{
    public class ProductItem : ViewModelBase
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

    }
}
