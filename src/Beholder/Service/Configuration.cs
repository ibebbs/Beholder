using System.ComponentModel.DataAnnotations;

namespace Beholder.Service
{
    public class Configuration
    {
        [Required]
        public string Location { get; set; } = "Testing";
    }
}
