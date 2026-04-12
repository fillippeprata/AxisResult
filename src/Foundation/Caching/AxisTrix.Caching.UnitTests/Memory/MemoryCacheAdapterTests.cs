using AxisTrix.Accessor;
using AxisTrix.Caching.Memory;
using AxisTrix.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace AxisTrix.Caching.UnitTests.Memory;

public class MemoryCacheAdapterTests : IDisposable
{
    private readonly IMemoryCache _memoryCache;

    public MemoryCacheAdapterTests()
    {
        // Arrange
        var builder = new ServiceCollectionBuilder(new ServiceCollection());

        // Act
        builder.AddAxisMemoryCache();
        builder.Services.AddSingleton<IAxisMediatorAccessor>(_ =>
        {
            var mediator = new Mock<IAxisMediator>();
            mediator.SetupGet(x => x.CancellationToken).Returns(CancellationToken.None);
            return new AxisMediatorAccessor
            {
                AxisMediator = mediator.Object
            };
        });
        var serviceProvider = builder.Services.BuildServiceProvider();
        _memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
    }

    private MemoryCacheAdapter MemoryAdapter()
    {
        var mediator = new Mock<IAxisMediator>();
        mediator.SetupGet(x => x.CancellationToken).Returns(CancellationToken.None);
        var initAccessor = new AxisMediatorAccessor
        {
            AxisMediator = mediator.Object
        };
        return new MemoryCacheAdapter(_memoryCache, initAccessor);
    }


    private async Task<IAxisCache> CanceledTokenAsync()
    {
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        var mediator = new Mock<IAxisMediator>();
        mediator.SetupGet(x => x.CancellationToken).Returns(cts.Token);
        var initAccessor = new AxisMediatorAccessor
        {
            AxisMediator = mediator.Object
        };
        return new MemoryCacheAdapter(_memoryCache, initAccessor);
    }

    public void Dispose()
    {
        _memoryCache.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnCachedValue_WhenKeyExists()
    {
        // Arrange
        const string key = "testKeyAsync";
        const string value = "testValueAsync";
        _memoryCache.Set(key, value);

        // Act
        var result = await MemoryAdapter().GetAsync<string>(key);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(value, result.Value);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnNull_WhenKeyDoesNotExist()
    {
        // Act
        var result = await MemoryAdapter().GetAsync<string>("nonExistentKeyAsync");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task SetAsync_ShouldStoreValue()
    {
        // Arrange
        var key = "testKeySetAsync";
        var value = "testValueSetAsync";

        // Act
        var result = await MemoryAdapter().SetAsync(key, value);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(value, _memoryCache.Get<string>(key));
    }

    [Fact]
    public async Task SetAsync_ShouldStoreValue_WithExpiration()
    {
        // Arrange
        var key = "testKeySetAsyncExp";
        var value = "testValueSetAsyncExp";
        var expiration = TimeSpan.FromHours(1);

        // Act
        var result = await MemoryAdapter().SetAsync(key, value, expiration);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(value, _memoryCache.Get<string>(key));
    }

    [Fact]
    public async Task SetAsync_ShouldOverwriteExistingValue()
    {
        // Arrange
        var key = "testKeyOverwriteAsync";
        _memoryCache.Set(key, "oldValue");
        var newValue = "newValue";

        // Act
        var result = await MemoryAdapter().SetAsync(key, newValue);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newValue, _memoryCache.Get<string>(key));
    }

    [Fact]
    public async Task RemoveAsync_ShouldDeleteKey()
    {
        // Arrange
        var key = "testKeyRemoveAsync";
        var value = "testValueRemoveAsync";
        _memoryCache.Set(key, value);

        // Act
        var result = await MemoryAdapter().RemoveAsync(key);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(_memoryCache.TryGetValue(key, out _));
    }

    [Fact]
    public async Task RemoveAsync_ShouldDoNothing_WhenKeyDoesNotExist()
    {
        // Act
        var result = await MemoryAdapter().RemoveAsync("nonExistentKeyRemoveAsync");

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenKeyExists()
    {
        // Arrange
        var key = "testKeyExistsAsync";
        _memoryCache.Set(key, "value");

        // Act
        var result = await MemoryAdapter().ExistsAsync(key);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenKeyDoesNotExist()
    {
        // Act
        var result = await MemoryAdapter().ExistsAsync("nonExistentKeyExistsAsync");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.Value);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnFailure_WhenCancelled()
    {
        // Arrange
        var memoryAdapter = await CanceledTokenAsync();

        // Act
        await Assert.ThrowsAsync<OperationCanceledException>(() => memoryAdapter.GetAsync<string>("key"));
    }

    [Fact]
    public async Task SetAsync_ShouldReturnFailure_WhenCancelled()
    {
        // Arrange
        var memoryAdapter = await CanceledTokenAsync();

        // Act
        await Assert.ThrowsAsync<OperationCanceledException>(() => memoryAdapter.SetAsync("key", "value"));
    }

    [Fact]
    public async Task RemoveAsync_ShouldReturnFailure_WhenCancelled()
    {
        // Arrange
        var memoryAdapter = await CanceledTokenAsync();

        // Act
        await Assert.ThrowsAsync<OperationCanceledException>(() => memoryAdapter.RemoveAsync("key"));
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFailure_WhenCancelled()
    {
        // Arrange
        var memoryAdapter = await CanceledTokenAsync();

        // Act
        await Assert.ThrowsAsync<OperationCanceledException>(() => memoryAdapter.ExistsAsync("key"));
    }

    [Fact]
    public async Task GetOrCreateAsync_ShouldReturnValueFromCache_WhenKeyExists()
    {
        // Arrange
        var key = "existing-key-async";
        var value = "cached-value";
        _memoryCache.Set(key, value);
        var factoryCalled = false;

        // Act
        var result = await MemoryAdapter().GetOrCreateAsync(key, () =>
        {
            factoryCalled = true;
            return Task.FromResult(AxisResult.AxisResult.Ok("new-value"));
        });

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(value, result.Value);
        Assert.False(factoryCalled);
    }

    [Fact]
    public async Task GetOrCreateAsync_ShouldCallFactoryAndStoreValue_WhenKeyDoesNotExist()
    {
        // Arrange
        var key = "new-key-async";
        var value = "new-value";
        var factoryCalled = false;

        // Act
        var result = await MemoryAdapter().GetOrCreateAsync(key, () =>
        {
            factoryCalled = true;
            return Task.FromResult(AxisResult.AxisResult.Ok(value));
        });

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(value, result.Value);
        Assert.True(factoryCalled);
        Assert.Equal(value, _memoryCache.Get<string>(key));
    }

    [Fact]
    public async Task GetOrCreateAsync_ShouldStoreValueWithExpiration()
    {
        // Arrange
        var key = "expiring-key-async";
        var value = "value";
        var expiration = TimeSpan.FromMinutes(10);

        // Act
        var result = await MemoryAdapter().GetOrCreateAsync(key, () => Task.FromResult(AxisResult.AxisResult.Ok(value)), expiration);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(value, _memoryCache.Get<string>(key));
    }

    [Fact]
    public async Task GetOrCreateAsync_ShouldReturnFailure_WhenCancelled()
    {
        // Arrange
        var memoryAdapter = await CanceledTokenAsync();

        // Act
        var result = await memoryAdapter.GetOrCreateAsync("any", () => Task.FromResult(AxisResult.AxisResult.Ok("val")));

        // Assert
        Assert.True(result.IsFailure);
    }
}
