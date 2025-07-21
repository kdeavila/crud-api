using System.ComponentModel.DataAnnotations;

namespace crud_api.Common.ValidationAttributes;

public class IsValidEnumAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return ValidationResult.Success;
        }

        if (value.GetType().IsEnum)
        {
            if (Enum.IsDefined(value.GetType(), value))
            {
                return ValidationResult.Success;
            }
        }
        else if (value is int intValue)
        {
            var propertyType = validationContext.ObjectInstance.GetType()
                .GetProperty(validationContext.MemberName!)?
                .PropertyType;

            if (propertyType != null && propertyType.IsEnum)
            {
                if (Enum.IsDefined(propertyType, intValue))
                {
                    return ValidationResult.Success;
                }
            }
        }
        else if (value is string stringValue)
        {
            var propertyType = validationContext.ObjectInstance.GetType()
                .GetProperty(validationContext.MemberName!)?
                .PropertyType;

            if (propertyType != null && propertyType.IsEnum)
            {
                if (Enum.TryParse(propertyType, stringValue, true, out object? result) && result != null)
                {
                    if (Enum.IsDefined(propertyType, result))
                    {
                        return ValidationResult.Success;
                    }
                }
            }
        }
        
        return new ValidationResult(ErrorMessage ?? $"The field {validationContext.DisplayName} has an invalid value.");
    }
}