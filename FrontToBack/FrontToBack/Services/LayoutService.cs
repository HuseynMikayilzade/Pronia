using FrontToBack.DAL;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace FrontToBack.Services
{
    public class LayoutService
    {
        private readonly AppDbContext _context;

        public LayoutService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Dictionary<string, string>> GetSettingsAsync()
        {
            Dictionary<string, string> setting = await _context.Settings.ToDictionaryAsync(s => s.Key, s => s.Value);
            return setting;
        }
    }
}
