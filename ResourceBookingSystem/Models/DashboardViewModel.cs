using ResourceBookingSystem.Models;
using System.Collections.Generic;

namespace ResourceBookingSystem.ViewModels
{
    public class DashboardViewModel
    {
        public List<Booking> TodaysBookings { get; set; }
        public List<Booking> UpcomingBookings { get; set; }
    }
}
