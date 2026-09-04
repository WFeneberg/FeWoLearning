using System.ComponentModel.DataAnnotations;

namespace FeWoLearning.Blazor.Support;

/// <summary>Test fixture model for the EditForm exercises. Not an exercise.</summary>
public sealed class ContactModel
{
    [Required(ErrorMessage = "Name is required")]
    public string? Name { get; set; }

    [Range(1, 120, ErrorMessage = "Age must be between 1 and 120")]
    public int Age { get; set; } = 1;

    [ValidateComplexType]
    public AddressModel Address { get; set; } = new();
}

/// <summary>Nested fixture model for Ex042. Not an exercise.</summary>
public sealed class AddressModel
{
    [Required(ErrorMessage = "City is required")]
    public string? City { get; set; }
}
