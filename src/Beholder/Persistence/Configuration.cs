using System.ComponentModel.DataAnnotations;
using System.Dynamic;

namespace Beholder.Persistence
{
    public class Configuration
    {
        [Required]
        public string BlobConnectionString { get; set; } = @"AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";

        [Required]
        public string BlobContainer { get; set; } = "faces";

        [Required]
        public string DirectorEndpoint { get; set; } = "http://localhost:5000";
    }
}
