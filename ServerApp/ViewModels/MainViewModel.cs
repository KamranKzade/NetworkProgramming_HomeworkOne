﻿using System;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using ServerApp.Models;
using System.Threading;
using System.Net.Sockets;
using ServerApp.Commands;
using System.Threading.Tasks;
using ServerApp.Views.UserControls;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;



namespace ServerApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private ObservableCollection<MyImage> galaryImages;
        public ObservableCollection<MyImage> GalaryImages
        {
            get { return galaryImages; }
            set
            {
                galaryImages = value;
                OnPropertyChanged();
            }
        }


        public Socket MySocket { get; set; }
        public UniformGrid MyUniformGrid { get; set; }
        public RelayCommand WindowLoaded { get; set; }
        public BitmapImage CurrentPicture { get; set; }



        public MainViewModel(UniformGrid uniform)
        {
            MyUniformGrid = uniform;
            GalaryImages = new ObservableCollection<MyImage>();


            GalaryImages.CollectionChanged += GalaryImages_CollectionChanged;

            var port = 45678;
            var ipAddress = IPAddress.Parse("192.168.1.16");

            var endPoint = new IPEndPoint(ipAddress, port);
            
            MySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            MySocket.Bind(endPoint);
            MySocket.Listen(5);


            WindowLoaded = new RelayCommand((o) =>
            {
                Thread thread = new Thread(() =>
                {
                    GalaryImages.Clear();

                    while (true)
                    {
                        var client = MySocket.Accept();

                        var task = new Task(() =>
                        {
                            // MessageBox.Show(($"{client.RemoteEndPoint} connected successfully"), "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                            var length = 0;
                            var bytes = new byte[100000000];

                            do
                            {
                                length = client.Receive(bytes);
                                var msj = Encoding.UTF8.GetString(bytes);
                                var ClientGalaryImage = JsonConvert.DeserializeObject<MyImage>(msj);


                                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                                {
                                    GalaryImages.Add(ClientGalaryImage);
                                });

                                if (ClientGalaryImage.Name == "Exit")
                                {
                                    client.Shutdown(SocketShutdown.Both);
                                    client.Dispose();
                                    break;
                                }
                            } while (true);
                        });
                        task.Start();
                    }
                });
                thread.Start();
            });
        }




        private void GalaryImages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                MyUniformGrid.Children.Clear();
            });


            foreach (var item in GalaryImages)
            {
                BitmapImage picture = new BitmapImage(new Uri(item.ImageUrl, UriKind.Relative));
                CurrentPicture = picture;

                var vm = new UC_ViewModel();
                vm.CurrentImageSource = picture;
                vm.Photo = item;

                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    var uc = new Picture_UserControl();
                    uc.DataContext = vm;
                    MyUniformGrid.Children.Add(uc);
                });
            }
        }
    }
}
