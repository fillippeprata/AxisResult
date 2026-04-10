using System.Linq.Expressions;
using AxisTrix.Types;
using AxisTrix.Validation.Localization;
using AxisTrix.Validation.Localization.Brazil;
using FluentValidation;

namespace AxisTrix.Validation;

public class AxisValidatorBase<T> : AbstractValidator<T>
{
    public const int DefaultMaxLength = 255;

    protected void NotNullOrEmpty<TProperty>(Expression<Func<T, TProperty?>> expression, string errorCode)
    {
        RuleFor(expression)
            .Must(x => x != null && !Equals(x, default(TProperty)) && (x is not string str || !string.IsNullOrWhiteSpace(str)))
            .WithErrorCode(errorCode);
    }

    protected void NotNullOrEmpty<TProperty>(Expression<Func<T, TProperty?>> expression, string errorCode, Action dependentRules)
    {
        RuleFor(expression)
            .NotNull()
            .WithErrorCode(errorCode);

        var compiled = expression.Compile();
        When(x => compiled(x) != null, dependentRules);
    }

    protected void NotNullOrEmpty<TProperty>(Expression<Func<T, TProperty?>> expression, string errorCode, Action<TProperty> dependentRules)
    {
        RuleFor(expression)
            .NotNull().WithErrorCode(errorCode)
            .DependentRules(() =>
                RuleFor(expression).Custom((_, context) =>
                {
                    var value = expression.Compile()(context.InstanceToValidate);
                    if (value is not null)
                        dependentRules(value);
                }));
    }

    public void DocumentId(Expression<Func<T, string?>> expression, CountryId countryId, string errorCode)
    {
        PrivateNotNullOrEmpty(expression, errorCode)
            .DependentRules(() =>
                RuleFor(expression).Must(x =>
                {
                    if (countryId == CountryIds.Br) return CpfValidator.Validate(x);

                    return false;
                }).WithErrorCode("INVALID_DOCUMENT_ID"));
    }

    public void DocumentId(Expression<Func<T, string?>> expression, Func<T, CountryId?> countrySelector, string errorCode)
    {
        PrivateNotNullOrEmpty(expression, errorCode)
            .DependentRules(() =>
                RuleFor(expression).Must((instance, x) =>
                {
                    var countryId = countrySelector(instance);
                    if (countryId == CountryIds.Br) return CpfValidator.Validate(x);

                    return false;
                }).WithErrorCode("INVALID_DOCUMENT_ID"));
    }

    protected void RequiredGuid7(Expression<Func<T, string?>> expression, string errorCode)
    {
        var compiled = expression.Compile();
        When(x => compiled(x) == null, () => PrivateNotNullOrEmpty(expression, errorCode));
        When(x => compiled(x) != null, () => PrivateNotNullOrEmpty(expression, errorCode)
            .Must(x =>
            {
                if (!Guid.TryParse(x, out var guid))
                    return false;
                return guid.Version == 7;
            }).WithErrorCode(errorCode));
    }

    protected void RequiredWithMaxLength(Expression<Func<T, string?>> expression, string errorCode, int? length = DefaultMaxLength)
    {
        PrivateNotNullOrEmpty(expression, errorCode)
            .Must((_, propertyValue) => propertyValue != null && propertyValue.ToString().Length <= length).WithErrorCode(errorCode);
    }

    protected void RequiredEmail(Expression<Func<T, string?>> expression, string errorCode)
    {
        PrivateNotNullOrEmpty(expression, errorCode)
            .EmailAddress().WithErrorCode(errorCode);
    }

    protected void RequiredCellPhone(Expression<Func<T, string?>> expression, CountryId countryId, string errorCode)
    {
        PrivateNotNullOrEmpty(expression, errorCode)
            .DependentRules(() =>
                RuleFor(expression).Must(phone =>
                        !string.IsNullOrWhiteSpace(countryId.GetFormattedPhone(phone)))
                    .WithErrorCode(errorCode));
    }

    public void RequiredTryParse(Expression<Func<T, string?>> expression, string errorCode, Func<object?, bool> parse)
    {
        var compiled = expression.Compile();
        When(x => compiled(x) == null, () => PrivateNotNullOrEmpty(expression, errorCode));
        When(x => compiled(x) != null, () => PrivateNotNullOrEmpty(expression, errorCode).Must(x => parse(x)).WithErrorCode(errorCode));
    }

    private IRuleBuilderOptions<T, TProperty> PrivateNotNullOrEmpty<TProperty>(Expression<Func<T, TProperty?>> expression, string errorCode)
    {
        return RuleFor(expression)
            .NotNull()
            .DependentRules(() =>
                RuleFor(expression).Must(x => x != null
                                              && !Equals(x, default(TProperty))
                                              && (x is not string str || !string.IsNullOrWhiteSpace(str))))
            .WithErrorCode(errorCode)!;
    }
}
