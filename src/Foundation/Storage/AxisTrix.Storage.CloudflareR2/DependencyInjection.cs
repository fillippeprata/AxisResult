using Amazon;
using Amazon.S3;
using AxisTrix.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace AxisTrix.Storage.CloudflareR2;

public static class DependencyInjection
{
    public static ServiceCollectionBuilder AddCloudflareR2StorageTrix(
        this ServiceCollectionBuilder builder,
        CloudflareR2Settings settings)
    {
        var s3Client = new AmazonS3Client(
            settings.AccessKey,
            settings.SecretKey,
            new AmazonS3Config
            {
                ServiceURL = settings.ServiceUrl,
                AuthenticationRegion = RegionEndpoint.USEast1.SystemName,
                ForcePathStyle = true
            });

        builder.Services.AddSingleton(settings);
        builder.Services.AddSingleton<IAmazonS3>(s3Client);
        builder.Services.AddSingleton<IAxisStorage, CloudflareR2StorageAdapter>();
        return builder;
    }
}
