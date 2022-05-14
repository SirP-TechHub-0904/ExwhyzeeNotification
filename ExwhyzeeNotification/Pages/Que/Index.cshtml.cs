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
    public class IndexModel : PageModel
    {
        private readonly ExwhyzeeNotification.Data.ApplicationDbContext _context;

        public IndexModel(ExwhyzeeNotification.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Queue> Queue { get;set; }

        public async Task OnGetAsync()
        {
            Queue = await _context.Queues
                .Include(q => q.Message).ToListAsync();
        }
    }
}
