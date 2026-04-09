using System.Reflection;
using AxisTrix.DependencyInjection;
using AxisTrix.Validation.FluentValidator;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AxisTrix.Validation;

internal static class DependencyInjection
{
    extension(ServiceCollectionBuilder builder)
    {
        public ServiceCollectionBuilder AddValidationAssemblies(params Assembly[] assemblies)
        {
            builder.Services.AddValidatorsFromAssemblies(assemblies.AsEnumerable(), includeInternalTypes: true);
            builder.Services.AddScoped(typeof(IAxisValidator<>), typeof(FluentValidatorAdapter<>));
            return builder;
        }
    }
}
