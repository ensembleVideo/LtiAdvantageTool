﻿using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AdvantageTool.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdvantageTool.Pages.Platforms
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public CreateModel(ApplicationDbContext context, 
            IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public Client Client { get; set; }

        [BindProperty]
        public PlatformModel Platform { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var user = await _context.GetUserAsync(User);
            Client = user.Client;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _context.GetUserAsync(User);
            if (user.Platforms.Any(p => p.Issuer == Platform.Issuer))
            {
                ModelState.AddModelError($"{nameof(Platform)}.{nameof(Platform.Issuer)}",
                    "This Issuer is already registered.");
                return Page();
            }

            await Platform.DiscoverEndpoints(_httpClientFactory);

            var platform = new Platform { User = user };
            Platform.UpdateEntity(platform);

            _context.Platforms.Add(platform);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}