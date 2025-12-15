using System.ComponentModel.DataAnnotations;

namespace BussinessLogicLayer.DTOs.AppUser
{
    public class HireCollectorDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }
    }

    public class CollectorDto
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public int AssignedOrdersCount { get; set; }
        public DateTime HiredDate { get; set; }
    }
}