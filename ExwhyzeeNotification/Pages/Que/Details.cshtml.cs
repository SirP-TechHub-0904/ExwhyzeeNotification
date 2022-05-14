using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ExwhyzeeNotification.Data;

namespace ExwhyzeeNotification.Pages.Que
{
    public class DetailsModel : PageModel
    {
        private readonly ExwhyzeeNotification.Data.ApplicationDbContext _context;

        public DetailsModel(ExwhyzeeNotification.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Queue Queue { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Queue = await _context.Queues
                .Include(q => q.Message).FirstOrDefaultAsync(m => m.Id == id);

            if (Queue == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
