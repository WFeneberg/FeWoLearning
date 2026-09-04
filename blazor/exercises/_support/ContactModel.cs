using System.ComponentModel.DataAnnotations;

namespace FeWoLearning.Blazor.Support;

/// <summary>Test fixture model for the EditForm exercises. Not an exercise.</summary>
public sealed class ContactModel : IValidatableObject
{
    [Required(ErrorMessage = "Name is required")]
    public string? Name { get; set; }

    [Range(1, 120, ErrorMessage = "Age must be between 1 and 120")]
    public int Age { get; set; } = 1;

    public AddressModel Address { get; set; } = new();

    // Recurses into Address by hand, because the framework's DataAnnotationsValidator
    // does not validate nested complex-type properties on its own (there is no
    // supported [ValidateComplexType] in .NET 10's shared framework - the only
    // place that attribute ever shipped was an abandoned .NET Core 3.2-era
    // preview package, which this track deliberately does not depend on).
    //
    // Measured behaviour worth recording here rather than rediscovering later:
    // the nested ValidationResult below DOES reach a <ValidationSummary /> (its
    // ErrorMessage is rendered there), but it does NOT reach a field-scoped
    // <ValidationMessage For="() => Model.Address.City" />. That component
    // matches on the FieldIdentifier (Model.Address, "City"), while the member
    // name yielded here produces (Model, "Address.City") instead - the two
    // identifiers don't line up, so the field-level message never shows even
    // though the form correctly goes invalid and the summary is populated.
    // This mismatch is itself a lesson for a later exercise, not a bug to
    // work around here.
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(
            Address, new ValidationContext(Address), results, validateAllProperties: true);

        foreach (var result in results)
            yield return new ValidationResult(
                result.ErrorMessage,
                new[] { $"{nameof(Address)}.{result.MemberNames.FirstOrDefault() ?? string.Empty}" });
    }
}

/// <summary>Nested fixture model for Ex042. Not an exercise.</summary>
public sealed class AddressModel
{
    [Required(ErrorMessage = "City is required")]
    public string? City { get; set; }
}
