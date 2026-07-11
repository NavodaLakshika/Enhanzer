using Enhanzer.Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Enhanzer.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PurchaseBillController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PurchaseBillController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/purchasebill
        [HttpPost]
        public async Task<ActionResult<PurchaseBill>> CreateBill(PurchaseBill bill)
        {
            if (bill.Items == null || !bill.Items.Any())
            {
                return BadRequest("Bill must contain at least one item.");
            }

            bill.CreatedAt = DateTime.UtcNow;

            _context.PurchaseBills.Add(bill);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBill), new { id = bill.Id }, bill);
        }

        // GET: api/purchasebill/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PurchaseBill>> GetBill(int id)
        {
            var bill = await _context.PurchaseBills
                .Include(b => b.Items)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bill == null)
            {
                return NotFound();
            }

            return bill;
        }
    }
}
