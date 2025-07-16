using System.ComponentModel.DataAnnotations;

namespace ResourceBookingSystem.Models
{
    public class Resource
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public string Location { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be a positive number")]
        public int Capacity { get; set; }

        //public bool IsAvailable { get; set; } = true;
        public bool IsAvailable { get; set; }


        public ICollection<Booking> Bookings { get; set; }
    }
}