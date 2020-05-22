using System.ComponentModel.DataAnnotations;

namespace Beholder.Face.Recognizer
{
    public class Configuration
    {
        [Required]
        public string ModelUri { get; set; }
    }
}
