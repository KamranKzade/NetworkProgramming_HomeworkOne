using System;
using ServerApp.Models;
using System.Collections.ObjectModel;



namespace ServerApp.Repositories
{
    public class FakeRepo
    {
        public static ObservableCollection<MyImage> GetGalaryImages()
        {
            return new ObservableCollection<MyImage>()
            {
                 new MyImage()
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
