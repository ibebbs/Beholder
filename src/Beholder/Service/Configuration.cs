using System.ComponentModel.DataAnnotations;

namespace Beholder.Service
{
    public class Configuration
    {
        [Required]
        public string Location { get; set; } = "Testing";

        [Required]
        public float RecognitionConfidence { get; set; } = 0.8f;
    }
}
