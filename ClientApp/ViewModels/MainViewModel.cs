using System;
using System.IO;
using System.Net;
using System.Windows;
using Microsoft.Win32;
using ClientApp.Models;
using System.Net.Sockets;
using ClientApp.Commands;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Runtime.Serialization.Formatters.Binary;


namespace ClientApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public GalaryImage Image { get; set; }
        public string FilePath { get; set; }
        public ImageBrush Picture { get; set; }


        public RelayCommand AddImageCommand { get; set; }
        public RelayCommand AddImageButtonWithCommand { get; set; }
        public RelayCommand CLoadModelFromDisk { get; set; }



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

                Image.Name = imageName_name;
                Image.Author = author_name;
                Image.ImageUrl = FilePath;




                try
                {
                    Image.Time = DateTime.Parse(creation_time);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                // MessageBox.Show("Successfully");

                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                var ipAddress = IPAddress.Parse("192.168.1.16");
                var port = 27001;

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
                                var bytes = ToByteArray(Image);
                                //var bytes = Encoding.UTF8.GetBytes(Image);
                                socket.Send(bytes);
                            }
                        });
                        Task.WaitAll(sender);
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


        private byte[] ToByteArray(object source)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, source);
                return stream.ToArray();
            }
        }
    }

}
