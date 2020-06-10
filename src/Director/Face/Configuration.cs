namespace Director.Face
{
    public class Configuration
    {
        public bool MapBlobsToRequestHost { get; set; } = true;

        public string MapBlobsToSpecificHost { get; set; } = string.Empty;

        public int MapBlobsToPost { get; set; } = 10000;
    }
}
