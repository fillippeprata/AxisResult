using System.Reflection;
using AxisTrix.CQRS;
using AxisTrix.DependencyInjection;
using DataPrivacyTrix.Application.Cellphones;

namespace DataPrivacyTrix.Application;

internal static class DependencyInjection
{
    public static ServiceCollectionBuilder AddIdentityTrixApplication(this ServiceCollectionBuilder builder)
    {
        return builder
            .AddCellphonesModule()
            .AddCqrsMediator(Assembly.GetExecutingAssembly());
    }
}
