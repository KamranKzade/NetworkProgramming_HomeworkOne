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
                      Author ="Server",
                      Name = "Kamran",
                      Time = new DateTime(2019,1, 1),
                      ImageUrl = "C:\\Users\\Kamran\\source\\repos\\NetworkProgramming_HomeworkOne\\ServerApp\\Images\\server.png"
                 }
            };

        }
    }
}
