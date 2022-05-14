using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ExwhyzeeNotification.Data;

namespace ExwhyzeeNotification.Pages.MSG
{
    public class IndexModel : PageModel
    {
        private readonly ExwhyzeeNotification.Data.ApplicationDbContext _context;

        public IndexModel(ExwhyzeeNotification.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Message> Message { get;set; }

        public async Task OnGetAsync()
        {
            Message = await _context.Messages
                .Include(m => m.User).ToListAsync();
        }
    }
}
