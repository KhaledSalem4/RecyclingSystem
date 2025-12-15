using System.ComponentModel.DataAnnotations;

namespace BussinessLogicLayer.DTOs.AppUser
{
    public class RegisterUserDto
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
        
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }
        
        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        
        // Address Information (Optional for registration)
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? BuildingNo { get; set; }
        public string? Apartment { get; set; }
    }
}
