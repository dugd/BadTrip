using FluentValidation;
using FluentValidation.Validators;
using System.ComponentModel.DataAnnotations;

namespace BadTrip.Application.Common.Validation
{
    public static class RuleBuilderExtensions
    {
        public static IRuleBuilderOptions<T, string> Phone<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new PhoneValidator<T>());
        }
    }

    public class PhoneValidator<T> : PropertyValidator<T, string>
    {
        private readonly PhoneAttribute _phoneAttribute = new PhoneAttribute();

        public override bool IsValid(ValidationContext<T> context, string value)
        {
            if (value is null)
            {
                return true;
            }

            return _phoneAttribute.IsValid(value);
        }

        public override string Name => "PhoneValidator";

        protected override string GetDefaultMessageTemplate(string errorCode)
            => "'{PropertyName}' is not a valid phone number.";
    }
}
