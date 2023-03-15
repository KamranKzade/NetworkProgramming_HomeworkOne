using System;
using ServerApp.Models;
using System.Collections.ObjectModel;



namespace ServerApp.Repositories
{
    public class FakeRepo
    {
        public static ObservableCollection<GalaryImage> GetGalaryImages()
        {
            return new ObservableCollection<GalaryImage>()
            {
            new GalaryImage()
            {
                 Author ="Kamran",
                 Name = "Server",
                 Time = new DateTime(1503,1, 1),
                 ImageUrl = "C:\\Users\\Kamran\\source\\repos\\NetworkProgramming_HomeworkOne\\ServerApp\\Images\\server.png"
            }
        };

        }
    }
}
