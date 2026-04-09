using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using AxisTrix.Accessor;

namespace AxisTrix.Storage.CloudflareR2.UnitTests.CloudflareR2;

public class CloudflareR2StorageAdapterTests
{
    private readonly Mock<IAmazonS3> _s3Mock;
    private readonly Mock<IAxisMediatorAccessor> _accessor;
    private readonly CloudflareR2StorageAdapter _adapter;
    private readonly IAxisMediator _defaultCancellationToken;
    private readonly IAxisMediator _canceledToken;

    public CloudflareR2StorageAdapterTests()
    {
        var defaultCancellationMock = new Mock<IAxisMediator>();
        defaultCancellationMock.SetupGet(x => x.CancellationToken).Returns(CancellationToken.None);
        _defaultCancellationToken = defaultCancellationMock.Object;

        var canceledMock = new Mock<IAxisMediator>();
        var cts = new CancellationTokenSource();
        cts.CancelAsync().Wait();
        canceledMock.SetupGet(x => x.CancellationToken).Returns(cts.Token);
        _canceledToken = canceledMock.Object;

        _s3Mock = new Mock<IAmazonS3>();
        _accessor = new Mock<IAxisMediatorAccessor>();
        var settings = new CloudflareR2Settings
        {
            AccountId = "test-account",
            AccessKey = "test-access-key",
            SecretKey = "test-secret-key",
            BucketName = "test-bucket"
        };
        _adapter = new CloudflareR2StorageAdapter(_accessor.Object, _s3Mock.Object, settings);
    }

    #region UploadAsync

    [Fact]
    public async Task UploadAsync_ShouldReturnSuccess_WhenUploadSucceeds()
    {
        // Arrange
        _s3Mock.Setup(s => s.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PutObjectResponse());
        _accessor.SetupGet(x => x.AxisMediator).Returns(_defaultCancellationToken);

        using var stream = new MemoryStream([1, 2, 3]);

        // Act
        var result = await _adapter.UploadAsync("files/test.txt", stream, "text/plain");

        // Assert
        Assert.True(result.IsSuccess);
        _s3Mock.Verify(s => s.PutObjectAsync(
            It.Is<PutObjectRequest>(r =>
                r.BucketName == "test-bucket" &&
                r.Key == "files/test.txt" &&
                r.ContentType == "text/plain"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UploadAsync_ShouldReturnFailure_WhenS3Throws()
    {
        // Arrange
        _s3Mock.Setup(s => s.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AmazonS3Exception("Upload failed"));
        _accessor.SetupGet(x => x.AxisMediator).Returns(_defaultCancellationToken);

        using var stream = new MemoryStream([1, 2, 3]);

        // Act
        var result = await _adapter.UploadAsync("files/test.txt", stream, "text/plain");

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task UploadAsync_ShouldThrow_WhenCancelled()
    {
        // Arrange
        _accessor.SetupGet(x => x.AxisMediator).Returns(_canceledToken);

        using var stream = new MemoryStream([1, 2, 3]);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(()
            => _adapter.UploadAsync("files/test.txt", stream, "text/plain"));
    }

    #endregion

    #region DownloadAsync

    [Fact]
    public async Task DownloadAsync_ShouldReturnStream_WhenKeyExists()
    {
        // Arrange
        var expectedStream = new MemoryStream([1, 2, 3]);
        var response = new GetObjectResponse { ResponseStream = expectedStream };

        _s3Mock.Setup(s => s.GetObjectAsync(It.IsAny<GetObjectRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);
        _accessor.SetupGet(x => x.AxisMediator).Returns(_defaultCancellationToken);

        // Act
        var result = await _adapter.DownloadAsync("files/test.txt");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Same(expectedStream, result.Value);
        _s3Mock.Verify(s => s.GetObjectAsync(
            It.Is<GetObjectRequest>(r =>
                r.BucketName == "test-bucket" &&
                r.Key == "files/test.txt"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DownloadAsync_ShouldReturnNotFound_WhenKeyDoesNotExist()
    {
        // Arrange
        _s3Mock.Setup(s => s.GetObjectAsync(It.IsAny<GetObjectRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AmazonS3Exception("Not Found") { StatusCode = HttpStatusCode.NotFound });
        _accessor.SetupGet(x => x.AxisMediator).Returns(_defaultCancellationToken);

        // Act
        var result = await _adapter.DownloadAsync("files/missing.txt");

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task DownloadAsync_ShouldReturnFailure_WhenS3Throws()
    {
        // Arrange
        _s3Mock.Setup(s => s.GetObjectAsync(It.IsAny<GetObjectRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AmazonS3Exception("Server error") { StatusCode = HttpStatusCode.InternalServerError });
        _accessor.SetupGet(x => x.AxisMediator).Returns(_defaultCancellationToken);

        // Act
        var result = await _adapter.DownloadAsync("files/test.txt");

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task DownloadAsync_ShouldThrow_WhenCancelled()
    {
        // Arrange
        _accessor.SetupGet(x => x.AxisMediator).Returns(_canceledToken);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(()
            => _adapter.DownloadAsync("files/test.txt"));
    }

    #endregion

    #region DeleteAsync

    [Fact]
    public async Task DeleteAsync_ShouldReturnSuccess_WhenDeleteSucceeds()
    {
        // Arrange
        _s3Mock.Setup(s => s.DeleteObjectAsync(It.IsAny<DeleteObjectRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DeleteObjectResponse());
        _accessor.SetupGet(x => x.AxisMediator).Returns(_defaultCancellationToken);

        // Act
        var result = await _adapter.DeleteAsync("files/test.txt");

        // Assert
        Assert.True(result.IsSuccess);
        _s3Mock.Verify(s => s.DeleteObjectAsync(
            It.Is<DeleteObjectRequest>(r =>
                r.BucketName == "test-bucket" &&
                r.Key == "files/test.txt"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFailure_WhenS3Throws()
    {
        // Arrange
        _s3Mock.Setup(s => s.DeleteObjectAsync(It.IsAny<DeleteObjectRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AmazonS3Exception("Delete failed"));
        _accessor.SetupGet(x => x.AxisMediator).Returns(_defaultCancellationToken);

        // Act
        var result = await _adapter.DeleteAsync("files/test.txt");

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrow_WhenCancelled()
    {
        // Arrange
        _accessor.SetupGet(x => x.AxisMediator).Returns(_canceledToken);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(()
            => _adapter.DeleteAsync("files/test.txt"));
    }

    #endregion

    #region ExistsAsync

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenKeyExists()
    {
        // Arrange
        _s3Mock.Setup(s => s.GetObjectMetadataAsync(It.IsAny<GetObjectMetadataRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetObjectMetadataResponse());
        _accessor.SetupGet(x => x.AxisMediator).Returns(_defaultCancellationToken);

        // Act
        var result = await _adapter.ExistsAsync("files/test.txt");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenKeyDoesNotExist()
    {
        // Arrange
        _s3Mock.Setup(s => s.GetObjectMetadataAsync(It.IsAny<GetObjectMetadataRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AmazonS3Exception("Not Found") { StatusCode = HttpStatusCode.NotFound });
        _accessor.SetupGet(x => x.AxisMediator).Returns(_defaultCancellationToken);

        // Act
        var result = await _adapter.ExistsAsync("files/missing.txt");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.Value);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFailure_WhenS3Throws()
    {
        // Arrange
        _s3Mock.Setup(s => s.GetObjectMetadataAsync(It.IsAny<GetObjectMetadataRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AmazonS3Exception("Server error") { StatusCode = HttpStatusCode.InternalServerError });
        _accessor.SetupGet(x => x.AxisMediator).Returns(_defaultCancellationToken);

        // Act
        var result = await _adapter.ExistsAsync("files/test.txt");

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task ExistsAsync_ShouldThrow_WhenCancelled()
    {
        // Arrange
        _accessor.SetupGet(x => x.AxisMediator).Returns(_canceledToken);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(()
            => _adapter.ExistsAsync("files/test.txt"));
    }

    [Fact]
    public async Task ExistsAsync_ShouldUseBucketNameFromSettings()
    {
        // Arrange
        _s3Mock.Setup(s => s.GetObjectMetadataAsync(It.IsAny<GetObjectMetadataRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetObjectMetadataResponse());
        _accessor.SetupGet(x => x.AxisMediator).Returns(_defaultCancellationToken);

        // Act
        await _adapter.ExistsAsync("files/test.txt");

        // Assert
        _s3Mock.Verify(s => s.GetObjectMetadataAsync(
            It.Is<GetObjectMetadataRequest>(r =>
                r.BucketName == "test-bucket" &&
                r.Key == "files/test.txt"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region GetPresignedUrlAsync

    [Fact]
    public async Task GetPresignedUrlAsync_ShouldReturnUrl_WhenSuccessful()
    {
        // Arrange
        const string expectedUrl = "https://test-bucket.r2.cloudflarestorage.com/files/test.txt?signature=abc";
        _s3Mock.Setup(s => s.GetPreSignedURL(It.IsAny<GetPreSignedUrlRequest>()))
            .Returns(expectedUrl);
        _accessor.SetupGet(x => x.AxisMediator).Returns(_defaultCancellationToken);

        // Act
        var result = await _adapter.GetPresignedUrlAsync("files/test.txt", TimeSpan.FromHours(1));

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedUrl, result.Value);
    }

    [Fact]
    public async Task GetPresignedUrlAsync_ShouldPassCorrectParameters()
    {
        // Arrange
        _s3Mock.Setup(s => s.GetPreSignedURL(It.IsAny<GetPreSignedUrlRequest>()))
            .Returns("https://url");
        _accessor.SetupGet(x => x.AxisMediator).Returns(_defaultCancellationToken);

        // Act
        await _adapter.GetPresignedUrlAsync("files/test.txt", TimeSpan.FromMinutes(30));

        // Assert
        _s3Mock.Verify(s => s.GetPreSignedURL(
            It.Is<GetPreSignedUrlRequest>(r =>
                r.BucketName == "test-bucket" &&
                r.Key == "files/test.txt" &&
                r.Verb == HttpVerb.GET)), Times.Once);
    }

    [Fact]
    public async Task GetPresignedUrlAsync_ShouldReturnFailure_WhenS3Throws()
    {
        // Arrange
        _s3Mock.Setup(s => s.GetPreSignedURL(It.IsAny<GetPreSignedUrlRequest>()))
            .Throws(new AmazonS3Exception("Presign failed"));
        _accessor.SetupGet(x => x.AxisMediator).Returns(_defaultCancellationToken);

        // Act
        var result = await _adapter.GetPresignedUrlAsync("files/test.txt", TimeSpan.FromHours(1));

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task GetPresignedUrlAsync_ShouldThrow_WhenCancelled()
    {
        // Arrange
        _accessor.SetupGet(x => x.AxisMediator).Returns(_canceledToken);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(()
            => _adapter.GetPresignedUrlAsync("files/test.txt", TimeSpan.FromHours(1)));
    }

    #endregion
}
