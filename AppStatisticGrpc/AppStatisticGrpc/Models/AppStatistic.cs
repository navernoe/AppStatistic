using System;
using AppStatisticGrpc.Interface;

namespace AppStatisticGrpc.Models
{
    [Serializable]
    public class AppStatistic: IAppStatistic
    {
        public int id { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public string downloads { get; set; }
    }

}
