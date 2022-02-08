using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class Holding
    {
        [Key]
        public string TickerExchange { get; set; }
        public string Name { get; set; }
        public double Weight { get; set; }
    }
}
