using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Lensman.Shared
{
    public class FaceProvider
    {
        private const string ConnectionString = @"AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://192.168.1.24:10000/devstoreaccount1;QueueEndpoint=http://192.168.1.24:10001/devstoreaccount1;TableEndpoint=http://192.168.1.24:10002/devstoreaccount1;";
        private const string UnknownContainerName = "unknown";
        private const string IdentifiedContainerName = "known";

        public FaceProvider()
        {
        }

        public async IAsyncEnumerable<Face> Faces()
        {
            var containerClient = new BlobContainerClient(ConnectionString, UnknownContainerName);

            await foreach (var item in containerClient.GetBlobsAsync(Azure.Storage.Blobs.Models.BlobTraits.None))
            {
                var blobClient = containerClient.GetBlobClient(item.Name);
                var downloaded = await blobClient.DownloadAsync();

                using (var stream = downloaded.Value.Content)
                {
                    using (var memory = new MemoryStream((int)downloaded.Value.ContentLength))
                    {
                        await stream.CopyToAsync(memory);

                        memory.Seek(0, SeekOrigin.Begin);

                        var bitmap = new BitmapImage();

                        var ras = memory.AsRandomAccessStream();

                        bitmap.SetSource(ras);

                        yield return new Face(item.Name, bitmap);
                    }
                }
            }
        }

        public async Task<Uri> MoveItemToPerson(Identification identification)
        {
            var containerClient = new BlobContainerClient(ConnectionString, UnknownContainerName);
            var sourceBlob = containerClient.GetBlobClient(identification.BlobName);

            var targetBlob = new BlobClient(ConnectionString, IdentifiedContainerName, $"{identification.Person}/{identification.BlobName}");
            var operation = await targetBlob.StartCopyFromUriAsync(sourceBlob.Uri);

            await operation.WaitForCompletionAsync();

            await sourceBlob.DeleteIfExistsAsync(Azure.Storage.Blobs.Models.DeleteSnapshotsOption.IncludeSnapshots);

            return targetBlob.Uri;
        }
    }
}
