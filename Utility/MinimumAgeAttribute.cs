using System;
using System.ComponentModel.DataAnnotations;

public class MinimumAgeAttribute : ValidationAttribute
{
    private readonly int _minimumAge;

    public MinimumAgeAttribute(int minimumAge)
    {
        _minimumAge = minimumAge;
        this.ErrorMessage = $"You must be at least {_minimumAge} years old.";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is DateOnly dateOfBirth)
        {
            var age = DateTime.UtcNow.Year - dateOfBirth.Year;

            if (DateTime.Today.DayOfYear < new DateTime(DateTime.Today.Year, dateOfBirth.Month, dateOfBirth.Day).DayOfYear)
            {
                age--;
            }

            if (age < _minimumAge)
            {
                return new ValidationResult(ErrorMessage);
            }
        }

        return ValidationResult.Success;
    }
}
