using System;
using ServerApp.Models;
using ServerApp.Views.UserControls;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;



namespace ServerApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private ObservableCollection<GalaryImage> galaryImages;
        public ObservableCollection<GalaryImage> GalaryImages
        {
            get { return galaryImages; }
            set
            {
                galaryImages = value;
                OnPropertyChanged();
            }
        }
        public BitmapImage CurrentPicture { get; set; }




        public MainViewModel(UniformGrid uniform)
        {
            GalaryImages = new ObservableCollection<GalaryImage>();
           
            
            // Bu hissede olaraq 34 un yerine 

            GalaryImages = new ObservableCollection<GalaryImage>(Repositories.FakeRepo.GetGalaryImages());


            foreach (var image in GalaryImages)
            {
                BitmapImage picture = new BitmapImage(new Uri(image.ImageUrl, UriKind.Relative));
                CurrentPicture = picture;

                var vm = new UC_ViewModel();
                vm.CurrentImageSource = picture;
                vm.Photo = image;

                var uc = new Picture_UserControl();
                uc.DataContext = vm;

                uniform.Children.Add(uc);
            }

        }

    }
}
