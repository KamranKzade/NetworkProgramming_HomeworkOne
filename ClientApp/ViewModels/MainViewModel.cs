using System;
using System.Net;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using Newtonsoft.Json;
using ClientApp.Models;
using ClientApp.Commands;
using System.Net.Sockets;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ClientApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public string FilePath { get; set; }
        public GalaryImage Image { get; set; }
        public ImageBrush Picture { get; set; }


        public RelayCommand AddImageCommand { get; set; }
        public RelayCommand CLoadModelFromDisk { get; set; }
        public RelayCommand AddImageButtonWithCommand { get; set; }



        private string imageNameTxt;
        public string ImageNameTxt
        {
            get { return imageNameTxt; }
            set { imageNameTxt = value; OnPropertyChanged(); }
        }


        private string creationDateTxt;
        public string CreationDateTxt
        {
            get { return creationDateTxt; }
            set { creationDateTxt = value; OnPropertyChanged(); }
        }

        private string authorNameTxt;
        public string AuthorNameTxt
        {
            get { return authorNameTxt; }
            set { authorNameTxt = value; OnPropertyChanged(); }
        }



        public MainViewModel()
        {
            Image = new GalaryImage();


            AddImageCommand = new RelayCommand((o) =>
            {
                var Picture = o as ImageBrush;
                OpenFileDialog Op = new OpenFileDialog();

                Op.Title = "Select a picture";
                Op.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";

                if (Op.ShowDialog() == true)
                {
                    FilePath = Op.FileName;

                    Picture.ImageSource = new BitmapImage(new Uri(Op.FileName));
                    Picture.Stretch = Stretch.Uniform;
                }
            });



            AddImageButtonWithCommand = new RelayCommand((o) =>
            {
                var imageName_name = ImageNameTxt;
                var author_name = AuthorNameTxt;
                var creation_time = CreationDateTxt;

                try
                {
                    Image.Name = imageName_name;
                    Image.Author = author_name;
                    Image.ImageUrl = FilePath;
                    Image.Time = DateTime.Parse(creation_time);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                var port = 27001;
                var ipAddress = IPAddress.Parse("192.168.1.16");
                var endPoint = new IPEndPoint(ipAddress, port);


                try
                {
                    socket.Connect(endPoint);

                    if (socket.Connected)
                    {
                        MessageBox.Show("Connected to the Server");
                        var sender = Task.Run(() =>
                        {
                            while (true)
                            {
                                string s = JsonConvert.SerializeObject(Image);
                                var bytes = Encoding.UTF8.GetBytes(s);
                                socket.Send(bytes);
                            }
                        });

                        var receiver = Task.Run(() =>
                        {
                            var length = 0;
                            var bytes = new byte[1024];
                            do
                            {
                                length = socket.Receive(bytes);
                                if (length > 0)
                                {
                                    var msg = Encoding.UTF8.GetString(bytes, 0, length);
                                    Console.WriteLine(msg);
                                }
                            }
                            while (true);
                        });

                        Task.WaitAll(receiver, sender);
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Can not connect to the server");
                }


            });

            CLoadModelFromDisk = new RelayCommand((o) =>
            {
                var ellipse = o as Ellipse;
                var picture = ellipse.Fill as ImageBrush;
                Picture = picture;

                ellipse.DragEnter += Ellipse_DragEnter;
            });
        }

        private void Ellipse_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filenames = e.Data.GetData(DataFormats.FileDrop, true) as string[];

                foreach (string fileName in filenames)
                {
                    Picture.ImageSource = new BitmapImage(new Uri(fileName));

                    FilePath = fileName;
                }
            }
        }
    }
}