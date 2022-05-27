using System.ComponentModel;

namespace Server.Models
{
    public class CardModel : INotifyPropertyChanged
    {
        private string _name;

        public int Id { get; set; }

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value)
                    return;
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        private string _photo;

        public string Photo
        {
            get { return _photo; }
            set
            {
                if (_photo == value)
                    return;
                _photo = value;
                OnPropertyChanged("Photo");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
