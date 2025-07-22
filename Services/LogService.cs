using Microsoft.EntityFrameworkCore;
using Library_Final.Data;
using Library_Final.Models;

namespace Library_Final.Services
{
    public class LogService
    {
        private readonly LibraryDbContext _context;

        public LogService(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task LogActionAsync(string action, string entityType, int? entityId = null, string? description = null)
        {
            try
            {
                var logEntry = new LogEntry
                {
                    UserId = Utilities.SessionManager.Instance.CurrentUser?.UserId,
                    Action = action,
                    EntityType = entityType,
                    EntityId = entityId,
                    Description = description,
                    Timestamp = DateTime.Now
                };

                _context.LogEntries.Add(logEntry);
                await _context.SaveChangesAsync();
            }
            catch
            {
                // Log errors silently to avoid breaking the main functionality
            }
        }

        public async Task<IEnumerable<LogEntry>> GetRecentLogsAsync(int count = 100)
        {
            return await _context.LogEntries
                .Include(l => l.User)
                .OrderByDescending(l => l.Timestamp)
                .Take(count)
                .ToListAsync();
        }
    }
}
