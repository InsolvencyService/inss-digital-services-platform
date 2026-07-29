using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Inss.GovUk.Forms.IPUpload.Application.Clients;

namespace Inss.GovUk.Forms.IPUpload.Infrastructure.Clients;

public sealed class UploadContentBlobClient : IUploadContentBlobClient
{
    private readonly BlobServiceClient _client;
    private const string ContainerName = "ipus";

    public UploadContentBlobClient(BlobServiceClient client)
    {
        _client = client;
    }

    public async Task<string> GetAsync(string sessionId)
    {
        BlobContainerClient containerClient = _client.GetBlobContainerClient(ContainerName);
        BlobClient blobClient = containerClient.GetBlobClient($"{sessionId}.xml");
        Response<BlobDownloadResult> response = await blobClient.DownloadContentAsync();
        BinaryData data = response.Value.Content;
        return data.ToString();
    }
    
    public async Task SaveAsync(string xml, string sessionId)
    {
        BlobContainerClient containerClient = _client.GetBlobContainerClient(ContainerName);
        BlobClient blobClient = containerClient.GetBlobClient($"{sessionId}.xml");
        BinaryData data = BinaryData.FromString(xml);
        await blobClient.UploadAsync(data, overwrite: true);
    }
}