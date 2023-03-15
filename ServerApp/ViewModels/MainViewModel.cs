using System;
using ServerApp.Models;
using ServerApp.Views.UserControls;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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

            var ipAddress = IPAddress.Parse("192.168.1.16");
            var port = 27001;

            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                var endPoint = new IPEndPoint(ipAddress, port);
                socket.Bind(endPoint);

                socket.Listen(5);

                MessageBox.Show($"Listen over {socket.LocalEndPoint}", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                while (true)
                {
                    var client = socket.Accept();
                    Task.Run(() =>
                    {
                        MessageBox.Show(($"{client.RemoteEndPoint} connected successfully"), "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                        var length = 0;
                        var bytes = new byte[10000];


                        do
                        {
                            length = client.Receive(bytes);
                            var msj = FromByteArray<GalaryImage>(bytes);

                            GalaryImages.Add(msj);
                           
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

                            if (msj.Name == "Exit")
                            {
                                client.Shutdown(SocketShutdown.Both);
                                client.Dispose();
                                break;
                            }
                        } while (true);
                    });

                }


            }

            GalaryImages = new ObservableCollection<GalaryImage>(Repositories.FakeRepo.GetGalaryImages());






         //   //////////////////////////////////////////////////////////////////////////////////////////////////////////////
         //
         //   foreach (var image in GalaryImages)
         //   {
         //       BitmapImage picture = new BitmapImage(new Uri(image.ImageUrl, UriKind.Relative));
         //       CurrentPicture = picture;
         //
         //       var vm = new UC_ViewModel();
         //       vm.CurrentImageSource = picture;
         //       vm.Photo = image;
         //
         //       var uc = new Picture_UserControl();
         //       uc.DataContext = vm;
         //
         //       uniform.Children.Add(uc);
         //   }
         //

        }
        public T FromByteArray<T>(byte[] data)
        {
            if (data == null)
                return default(T);
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(data))
            {
                object obj = bf.Deserialize(ms);
                return (T)obj;
            }
        }
    }
}
