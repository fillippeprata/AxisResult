using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using Axis;
using AxisMediator.Contracts;

namespace AxisStorage.CloudflareR2;

public class CloudflareR2StorageAdapter(IAxisMediatorAccessor accessor, IAmazonS3 s3Client, CloudflareR2Settings settings) : IAxisStorage
{
    public Task<AxisResult> UploadAsync(string key, Stream content, string contentType)
        => AxisResult.TryAsync(async () =>
        {
            var ct = accessor.AxisMediator!.CancellationToken;
            ct.ThrowIfCancellationRequested();
            await s3Client.PutObjectAsync(new PutObjectRequest
            {
                BucketName = settings.BucketName,
                Key = key,
                InputStream = content,
                ContentType = contentType
            }, ct);
        });

    public Task<AxisResult<Stream>> DownloadAsync(string key)
        => AxisResult.TryAsync(async () =>
        {
            var ct = accessor.AxisMediator!.CancellationToken;
            ct.ThrowIfCancellationRequested();
            var response = await s3Client.GetObjectAsync(new GetObjectRequest
            {
                BucketName = settings.BucketName,
                Key = key
            }, ct);
            return response.ResponseStream;
        });

    public Task<AxisResult> DeleteAsync(string key)
        => AxisResult.TryAsync(async () =>
        {
            var ct = accessor.AxisMediator!.CancellationToken;
            ct.ThrowIfCancellationRequested();
            await s3Client.DeleteObjectAsync(new DeleteObjectRequest
            {
                BucketName = settings.BucketName,
                Key = key
            }, ct);
        });

    public Task<AxisResult<bool>> ExistsAsync(string key)
    {
        var ct = accessor.AxisMediator!.CancellationToken;
        ct.ThrowIfCancellationRequested();
        return AxisResult.TryAsync(async () =>
        {
            try
            {
                await s3Client.GetObjectMetadataAsync(new GetObjectMetadataRequest
                {
                    BucketName = settings.BucketName,
                    Key = key
                }, ct);
                return true;
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }
        });
    }

    public Task<AxisResult<string>> GetPresignedUrlAsync(string key, TimeSpan expiration)
        => AxisResult.TryAsync(() =>
        {
            var ct = accessor.AxisMediator!.CancellationToken;
            ct.ThrowIfCancellationRequested();
            var url = s3Client.GetPreSignedURL(new GetPreSignedUrlRequest
            {
                BucketName = settings.BucketName,
                Key = key,
                Verb = HttpVerb.GET,
                Expires = DateTime.UtcNow.Add(expiration)
            });
            return Task.FromResult(url);
        });
}
