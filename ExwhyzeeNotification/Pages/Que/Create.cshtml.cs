using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ExwhyzeeNotification.Data;

namespace ExwhyzeeNotification.Pages.Que
{
    public class CreateModel : PageModel
    {
        private readonly ExwhyzeeNotification.Data.ApplicationDbContext _context;

        public CreateModel(ExwhyzeeNotification.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["MessageId"] = new SelectList(_context.Messages, "Id", "Id");
            return Page();
        }

        [BindProperty]
        public Queue Queue { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Queues.Add(Queue);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
