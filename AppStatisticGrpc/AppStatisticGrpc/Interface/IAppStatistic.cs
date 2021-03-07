namespace AppStatisticGrpc.Interface
{
    public interface IAppStatistic
    {
        public int id { get; }
        public string url { get; }
        public string name { get; set; }
        public string downloads { get; set; }
    }
}
