using Amazon.S3;
using AxisMediator.Contracts;
using Moq;

namespace AxisStorage.CloudflareR2.IntegrationTests;

[Collection("MinioCollection")]
public class CloudflareR2StorageAdapterIntegrationTests(MinioFixture fixture) : IDisposable
{
    private readonly IAmazonS3 _s3Client = fixture.CreateS3Client();
    private readonly CloudflareR2StorageAdapter _adapter = CreateAdapter(fixture);

    private static CloudflareR2StorageAdapter CreateAdapter(MinioFixture fixture)
    {
        var s3Client = fixture.CreateS3Client();
        var accessorMock = new Mock<IAxisMediatorAccessor>();
        var mediatorMock = new Mock<IAxisMediator>();
        mediatorMock.SetupGet(x => x.CancellationToken).Returns(CancellationToken.None);
        accessorMock.SetupGet(x => x.AxisMediator).Returns(mediatorMock.Object);
        return new CloudflareR2StorageAdapter(accessorMock.Object, s3Client, fixture.Settings);
    }

    public void Dispose() => _s3Client.Dispose();

    private static string UniqueKey(string prefix = "test") => $"{prefix}/{Guid.NewGuid():N}.txt";

    #region UploadAsync

    [Fact]
    public async Task UploadAsync_ShouldUploadContent_AndBeRetrievable()
    {
        // Arrange
        var key = UniqueKey();
        var content = "Hello, R2!"u8.ToArray();
        using var stream = new MemoryStream(content);

        // Act
        var result = await _adapter.UploadAsync(key, stream, "text/plain");

        // Assert
        Assert.True(result.IsSuccess, $"Failed: {result.JoinErrorCodes()}");

        var response = await _s3Client.GetObjectAsync(fixture.Settings.BucketName, key);
        using var reader = new StreamReader(response.ResponseStream);
        var retrieved = await reader.ReadToEndAsync();
        Assert.Equal("Hello, R2!", retrieved);
    }

    [Fact]
    public async Task UploadAsync_ShouldPreserveContentType()
    {
        // Arrange
        var key = UniqueKey();
        using var stream = new MemoryStream([1, 2, 3]);

        // Act
        var result = await _adapter.UploadAsync(key, stream, "application/octet-stream");

        // Assert
        Assert.True(result.IsSuccess, $"Failed: {result.JoinErrorCodes()}");

        var metadata = await _s3Client.GetObjectMetadataAsync(fixture.Settings.BucketName, key);
        Assert.Equal("application/octet-stream", metadata.Headers.ContentType);
    }

    [Fact]
    public async Task UploadAsync_ShouldOverwriteExistingKey()
    {
        // Arrange
        var key = UniqueKey();
        using var first = new MemoryStream("first"u8.ToArray());
        using var second = new MemoryStream("second"u8.ToArray());

        await _adapter.UploadAsync(key, first, "text/plain");

        // Act
        var result = await _adapter.UploadAsync(key, second, "text/plain");

        // Assert
        Assert.True(result.IsSuccess, $"Failed: {result.JoinErrorCodes()}");

        var response = await _s3Client.GetObjectAsync(fixture.Settings.BucketName, key);
        using var reader = new StreamReader(response.ResponseStream);
        Assert.Equal("second", await reader.ReadToEndAsync());
    }

    #endregion

    #region DownloadAsync

    [Fact]
    public async Task DownloadAsync_ShouldReturnStream_WhenKeyExists()
    {
        // Arrange
        var key = UniqueKey();
        var content = "download me"u8.ToArray();
        using var uploadStream = new MemoryStream(content);
        await _adapter.UploadAsync(key, uploadStream, "text/plain");

        // Act
        var result = await _adapter.DownloadAsync(key);

        // Assert
        Assert.True(result.IsSuccess, $"Failed: {result.JoinErrorCodes()}");

        using var reader = new StreamReader(result.Value);
        Assert.Equal("download me", await reader.ReadToEndAsync());
    }

    [Fact]
    public async Task DownloadAsync_ShouldReturnFailure_WhenKeyDoesNotExist()
    {
        // Act
        var result = await _adapter.DownloadAsync(UniqueKey("missing"));

        // Assert
        Assert.True(result.IsFailure);
    }

    #endregion

    #region DeleteAsync

    [Fact]
    public async Task DeleteAsync_ShouldRemoveObject_WhenKeyExists()
    {
        // Arrange
        var key = UniqueKey();
        using var stream = new MemoryStream([1, 2, 3]);
        await _adapter.UploadAsync(key, stream, "text/plain");

        // Act
        var result = await _adapter.DeleteAsync(key);

        // Assert
        Assert.True(result.IsSuccess, $"Failed: {result.JoinErrorCodes()}");

        var exists = await _adapter.ExistsAsync(key);
        Assert.False(exists.Value);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnSuccess_WhenKeyDoesNotExist()
    {
        // S3 DeleteObject is idempotent
        var result = await _adapter.DeleteAsync(UniqueKey("nonexistent"));

        Assert.True(result.IsSuccess, $"Failed: {result.JoinErrorCodes()}");
    }

    #endregion

    #region ExistsAsync

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenKeyExists()
    {
        // Arrange
        var key = UniqueKey();
        using var stream = new MemoryStream([1, 2, 3]);
        await _adapter.UploadAsync(key, stream, "text/plain");

        // Act
        var result = await _adapter.ExistsAsync(key);

        // Assert
        Assert.True(result.IsSuccess, $"Failed: {result.JoinErrorCodes()}");
        Assert.True(result.Value);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenKeyDoesNotExist()
    {
        // Act
        var result = await _adapter.ExistsAsync(UniqueKey("nonexistent"));

        // Assert
        Assert.True(result.IsSuccess, $"Failed: {result.JoinErrorCodes()}");
        Assert.False(result.Value);
    }

    #endregion

    #region GetPresignedUrlAsync

    [Fact]
    public async Task GetPresignedUrlAsync_ShouldReturnUrl_WhenKeyExists()
    {
        // Arrange
        var key = UniqueKey();
        using var stream = new MemoryStream([1, 2, 3]);
        await _adapter.UploadAsync(key, stream, "text/plain");

        // Act
        var result = await _adapter.GetPresignedUrlAsync(key, TimeSpan.FromMinutes(5));

        // Assert
        Assert.True(result.IsSuccess, $"Failed: {result.JoinErrorCodes()}");
        Assert.Contains(key, result.Value);
    }

    [Fact]
    public async Task GetPresignedUrlAsync_ShouldReturnUrl_EvenWhenKeyDoesNotExist()
    {
        // S3 presigned URLs are generated client-side, no server check
        var result = await _adapter.GetPresignedUrlAsync(UniqueKey("future"), TimeSpan.FromMinutes(5));

        Assert.True(result.IsSuccess, $"Failed: {result.JoinErrorCodes()}");
        Assert.False(string.IsNullOrWhiteSpace(result.Value));
    }

    #endregion

    #region Full Lifecycle

    [Fact]
    public async Task FullLifecycle_UploadExistsDownloadDeleteExists()
    {
        // Arrange
        var key = UniqueKey("lifecycle");
        var content = "lifecycle test"u8.ToArray();

        // Upload
        using var uploadStream = new MemoryStream(content);
        var uploadResult = await _adapter.UploadAsync(key, uploadStream, "text/plain");
        Assert.True(uploadResult.IsSuccess, $"Upload failed: {uploadResult.JoinErrorCodes()}");

        // Exists (true)
        var existsResult = await _adapter.ExistsAsync(key);
        Assert.True(existsResult.IsSuccess && existsResult.Value);

        // Download
        var downloadResult = await _adapter.DownloadAsync(key);
        Assert.True(downloadResult.IsSuccess, $"Download failed: {downloadResult.JoinErrorCodes()}");
        using var reader = new StreamReader(downloadResult.Value);
        Assert.Equal("lifecycle test", await reader.ReadToEndAsync());

        // Presigned URL
        var urlResult = await _adapter.GetPresignedUrlAsync(key, TimeSpan.FromMinutes(1));
        Assert.True(urlResult.IsSuccess, $"Presigned URL failed: {urlResult.JoinErrorCodes()}");

        // Delete
        var deleteResult = await _adapter.DeleteAsync(key);
        Assert.True(deleteResult.IsSuccess, $"Delete failed: {deleteResult.JoinErrorCodes()}");

        // Exists (false)
        var existsAfterDelete = await _adapter.ExistsAsync(key);
        Assert.True(existsAfterDelete.IsSuccess && !existsAfterDelete.Value);
    }

    #endregion
}
