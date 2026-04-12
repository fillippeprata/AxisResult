using System.Linq.Expressions;
using AxisTrix.Results;
using AxisTrix.Types.Localization;
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

    protected void DependentRules<TProperty1, TProperty2>(
        Expression<Func<T, TProperty1?>> expression1, string errorCode1,
        Expression<Func<T, TProperty2?>> expression2, string errorCode2,
        Func<TProperty1, TProperty2, AxisResult> dependentRules)
    {
        RuleFor(expression1)
            .NotNull().WithErrorCode(errorCode1)
            .DependentRules(() =>
                RuleFor(expression2)
                    .NotNull().WithErrorCode(errorCode2)
                    .DependentRules(() =>
                        RuleFor(expression1).Custom((_, context) =>
                        {
                            var value1 = expression1.Compile()(context.InstanceToValidate);
                            var value2 = expression2.Compile()(context.InstanceToValidate);
                            if (value1 is not null && value2 is not null)
                                RuleFor(expression2).Must(_ => dependentRules(value1, value2).IsSuccess).WithErrorCode(errorCode2);
                        })));
    }

    public void DocumentId(Expression<Func<T, string?>> expression, Func<T, string?> countrySelector, string errorCode)
    {
        PrivateNotNullOrEmpty(expression, errorCode)
            .DependentRules(() =>
                RuleFor(expression).Must((instance, x) =>
                {
                    var countryId = countrySelector(instance);
                    if (string.IsNullOrEmpty(countryId))
                        return false;

                    if ((CountryId)countryId == CountryIds.Br) return CpfValidator.Validate(x);

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
