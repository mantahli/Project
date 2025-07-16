using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ResourceBookingSystem.Data;
using ResourceBookingSystem.Models;

namespace ResourceBookingSystem.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Resource)
                .ToListAsync();
            return View(bookings);
        }

        // GET: Bookings/Create
        public async Task<IActionResult> Create()
        {
            var resources = await _context.Resources
                .Where(r => r.IsAvailable)
                .ToListAsync();

            ViewData["ResourceId"] = new SelectList(resources, "Id", "Name");
            return View();
        }

        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ResourceId,StartTime,EndTime,BookedBy,Purpose")] Booking booking)
        {
            if (booking.EndTime <= booking.StartTime)
            {
                ModelState.AddModelError("EndTime", "End time must be after start time.");
            }

            var hasConflict = await _context.Bookings
                .AnyAsync(b => b.ResourceId == booking.ResourceId &&
                    ((booking.StartTime >= b.StartTime && booking.StartTime < b.EndTime) ||
                     (booking.EndTime > b.StartTime && booking.EndTime <= b.EndTime) ||
                     (booking.StartTime <= b.StartTime && booking.EndTime >= b.EndTime)));

            if (hasConflict)
            {
                ModelState.AddModelError(string.Empty, "This resource is already booked during the requested time. Please choose another slot or resource, or adjust your times.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var resources = await _context.Resources
                .Where(r => r.IsAvailable)
                .ToListAsync();

            ViewData["ResourceId"] = new SelectList(resources, "Id", "Name", booking.ResourceId);
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            ViewData["ResourceId"] = new SelectList(_context.Resources, "Id", "Name", booking.ResourceId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ResourceId,StartTime,EndTime,BookedBy,Purpose")] Booking booking)
        {
            if (id != booking.Id)
            {
                return NotFound();
            }

            if (booking.EndTime <= booking.StartTime)
            {
                ModelState.AddModelError("EndTime", "End time must be after start time.");
            }

            if (booking.StartTime < DateTime.Now)
            {
                ModelState.AddModelError("StartTime", "Cannot book in the past.");
            }

            var hasConflict = await _context.Bookings
                .AnyAsync(b => b.Id != booking.Id &&
                              b.ResourceId == booking.ResourceId &&
                              ((booking.StartTime >= b.StartTime && booking.StartTime < b.EndTime) ||
                               (booking.EndTime > b.StartTime && booking.EndTime <= b.EndTime) ||
                               (booking.StartTime <= b.StartTime && booking.EndTime >= b.EndTime)));

            if (hasConflict)
            {
                ModelState.AddModelError(string.Empty, "This resource is already booked during the requested time.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewData["ResourceId"] = new SelectList(_context.Resources, "Id", "Name", booking.ResourceId);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Resource)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.Id == id);
        }
    }
}
