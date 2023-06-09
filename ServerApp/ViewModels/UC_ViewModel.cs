﻿using ServerApp.Models;
using System.Windows.Media;


namespace ServerApp.ViewModels
{
    public class UC_ViewModel : BaseViewModel
    {
        private MyImage photo;
        public MyImage Photo
        {
            get { return photo; }
            set
            {
                photo = value;
                OnPropertyChanged();
            }
        }


        private ImageSource _currentImageSource;
        public ImageSource CurrentImageSource
        {
            get { return _currentImageSource; }
            set
            {
                _currentImageSource = value;
                OnPropertyChanged();
            }
        }
    }
}
