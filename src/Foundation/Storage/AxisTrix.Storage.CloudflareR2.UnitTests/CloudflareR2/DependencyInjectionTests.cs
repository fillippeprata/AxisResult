using Amazon.S3;
using AxisTrix.Accessor;
using AxisTrix.DependencyInjection;
using AxisTrix.Storage.CloudflareR2;
using Microsoft.Extensions.DependencyInjection;

namespace AxisTrix.Storage.CloudflareR2.UnitTests.CloudflareR2;

public class DependencyInjectionTests
{
    private static CloudflareR2Settings CreateSettings() => new()
    {
        AccountId = "test-account",
        AccessKey = "test-access-key",
        SecretKey = "test-secret-key",
        BucketName = "test-bucket"
    };

    private static ServiceCollectionBuilder CreateBuilderWithAccessor()
    {
        var services = new ServiceCollection();
        services.AddSingleton(new Mock<IAxisMediatorAccessor>().Object);
        return new ServiceCollectionBuilder(services);
    }

    [Fact]
    public void AddCloudflareR2StorageTrix_ShouldRegisterIStorageTrixAsCloudflareR2StorageAdapter()
    {
        // Arrange
        var builder = CreateBuilderWithAccessor();

        // Act
        builder.AddCloudflareR2StorageTrix(CreateSettings());
        var serviceProvider = builder.Services.BuildServiceProvider();

        // Assert
        var storage = serviceProvider.GetService<IAxisStorage>();
        Assert.NotNull(storage);
        Assert.IsType<CloudflareR2StorageAdapter>(storage);
    }

    [Fact]
    public void AddCloudflareR2StorageTrix_ShouldRegisterIAmazonS3AsSingleton()
    {
        // Arrange
        var builder = new ServiceCollectionBuilder(new ServiceCollection());

        // Act
        builder.AddCloudflareR2StorageTrix(CreateSettings());
        var serviceProvider = builder.Services.BuildServiceProvider();

        // Assert
        var s3Client1 = serviceProvider.GetService<IAmazonS3>();
        var s3Client2 = serviceProvider.GetService<IAmazonS3>();
        Assert.NotNull(s3Client1);
        Assert.Same(s3Client1, s3Client2);
    }

    [Fact]
    public void AddCloudflareR2StorageTrix_ShouldRegisterSettingsAsSingleton()
    {
        // Arrange
        var builder = new ServiceCollectionBuilder(new ServiceCollection());
        var settings = CreateSettings();

        // Act
        builder.AddCloudflareR2StorageTrix(settings);
        var serviceProvider = builder.Services.BuildServiceProvider();

        // Assert
        var resolved = serviceProvider.GetService<CloudflareR2Settings>();
        Assert.NotNull(resolved);
        Assert.Same(settings, resolved);
    }

    [Fact]
    public void AddCloudflareR2StorageTrix_ShouldRegisterStorageAsSingleton()
    {
        // Arrange
        var builder = CreateBuilderWithAccessor();

        // Act
        builder.AddCloudflareR2StorageTrix(CreateSettings());
        var serviceProvider = builder.Services.BuildServiceProvider();

        // Assert
        var instance1 = serviceProvider.GetService<IAxisStorage>();
        var instance2 = serviceProvider.GetService<IAxisStorage>();
        Assert.Same(instance1, instance2);
    }

    [Fact]
    public void AddCloudflareR2StorageTrix_ShouldReturnBuilderForChaining()
    {
        // Arrange
        var builder = new ServiceCollectionBuilder(new ServiceCollection());

        // Act
        var result = builder.AddCloudflareR2StorageTrix(CreateSettings());

        // Assert
        Assert.Same(builder, result);
    }
}
