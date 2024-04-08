using Castle.DynamicProxy;
using Core.Exceptions;
using Core.Utilities.Interceptors;
using FluentValidation;

namespace Business.Aspects
{
    public class Validator : MethodInterception
    {
        private readonly Type _validatorType;
        public Validator(Type validatorType)
        {
            if (!typeof(IValidator).IsAssignableFrom(validatorType)) throw new InvalidCastException("Invalid validator type.");

            _validatorType = validatorType;
        }

        protected override void OnBefore(IInvocation invocation)
        {
            var validator = (IValidator)Activator.CreateInstance(_validatorType)!;
            var entityType = _validatorType.BaseType?.GetGenericArguments().FirstOrDefault();
            var entity_to_validate = invocation.Arguments.FirstOrDefault(x => x.GetType() == entityType);
            if (entity_to_validate is null) throw new InvalidDataException("No entity to verify found.");

            var context = new ValidationContext<object>(entity_to_validate);
            var result = validator.Validate(context);
            if (!result.IsValid)
                throw new InvalidValidationException();
        }
    }
}