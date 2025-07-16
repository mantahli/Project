using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResourceBookingSystem.Data;
using ResourceBookingSystem.Models;
using ResourceBookingSystem.ViewModels; // <- Make sure your ViewModel is in a ViewModels folder/namespace

namespace ResourceBookingSystem.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var todayStart = DateTime.Today;
            var todayEnd = DateTime.Today.AddDays(1).AddTicks(-1);

            var model = new DashboardViewModel
            {
                TodaysBookings = await _context.Bookings
                    .Include(b => b.Resource)
                    .Where(b => b.StartTime >= todayStart && b.StartTime <= todayEnd)
                    .OrderBy(b => b.StartTime)
                    .ToListAsync(),

                UpcomingBookings = await _context.Bookings
                    .Include(b => b.Resource)
                    .Where(b => b.StartTime > todayEnd)
                    .OrderBy(b => b.StartTime)
                    .Take(5)
                    .ToListAsync()
            };

            return View(model);
        }
    }
}
