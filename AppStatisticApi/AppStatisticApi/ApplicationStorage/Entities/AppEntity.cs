using System.ComponentModel.DataAnnotations;

namespace AppStatisticApi.Storage.Entities
{
    public class AppEntity
    {
        public int id { get; set; }
        [Required]
        public string url { get; set; }
        public string name { get; set; }
        public long? downloads { get; set; }
    }
}
