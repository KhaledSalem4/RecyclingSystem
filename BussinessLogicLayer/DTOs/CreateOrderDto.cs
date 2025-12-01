using System.ComponentModel.DataAnnotations;

namespace BusinessLogicLayer.DTOs
{
    public class CreateOrderDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Type of material is required")]
        public string TypeOfMaterial { get; set; } = string.Empty;

        [Required(ErrorMessage = "Quantity is required")]
        [Range(2, int.MaxValue, ErrorMessage = "Quantity must be at least 2 kg")]
        public double Quantity { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Street is required")]
        public string Street { get; set; } = string.Empty;

        [Required(ErrorMessage = "Building number is required")]
        public string BuildingNo { get; set; } = string.Empty;

        public string? Apartment { get; set; }
    }
}
