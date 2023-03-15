using System;
using ServerApp.ViewModels;



namespace ServerApp.Models
{
    public class GalaryImage:BaseViewModel
    {
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }


        private string author;
        public string Author
        {
            get { return author; }
            set
            {
                author = value;
                OnPropertyChanged();
            }
        }


        private string imageUrl;
        public string ImageUrl
        {
            get { return imageUrl; }
            set
            {
                imageUrl = value;
                OnPropertyChanged();
            }
        }


        private DateTime time;

        public DateTime Time
        {
            get { return time; }
            set
            {
                time = value;
                OnPropertyChanged();
            }
        }


        public override string ToString()
        {
            return base.ToString();
        }
    }
}
