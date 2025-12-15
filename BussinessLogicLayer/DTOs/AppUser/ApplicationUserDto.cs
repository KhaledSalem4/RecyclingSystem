namespace BussinessLogicLayer.DTOs.AppUser
{
    public class ApplicationUserDto
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public int Points { get; set; }
    }
}
