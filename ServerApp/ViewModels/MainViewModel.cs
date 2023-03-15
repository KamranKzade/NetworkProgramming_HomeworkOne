using System;
using System.Net;
using System.Text;
using System.Windows;
using Newtonsoft.Json;
using ServerApp.Models;
using System.Net.Sockets;
using System.Threading.Tasks;
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
            GalaryImages = new ObservableCollection<GalaryImage>(Repositories.FakeRepo.GetGalaryImages());

            var port = 27001;
            var ipAddress = IPAddress.Parse("192.168.1.16");

            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                var endPoint = new IPEndPoint(ipAddress, port);
                socket.Bind(endPoint);
                socket.Listen(5);


                int count = 0;
                while (count == 0)
                {
                    var client = socket.Accept();

                    //var task = new Task(() =>
                    //{

                    MessageBox.Show(($"{client.RemoteEndPoint} connected successfully"), "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    var length = 0;
                    var bytes = new byte[10000];

                    do
                    {
                        length = client.Receive(bytes);
                        var msj = Encoding.UTF8.GetString(bytes);
                        if (count > 0)
                        {
                            break;
                        }
                        var ClientGalaryImage = JsonConvert.DeserializeObject<GalaryImage>(msj);


                        GalaryImages.Add(ClientGalaryImage);

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
                            count++;
                        }

                        if (ClientGalaryImage.Name == "Exit")
                        {
                            client.Shutdown(SocketShutdown.Both);
                            client.Dispose();
                            break;
                        }
                    } while (true);
                    
                    //});
                    //task.Start();
                }
            }
        }
    }
}
