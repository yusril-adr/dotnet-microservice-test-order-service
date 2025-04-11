using DotNetService.Infrastructure.Databases;
using DotNetService.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace DotNetService.Domain.Notification.Repositories
{
    public class NotificationStoreRepository(
        DotnetServiceDBContext context
        )
    {
        private readonly DotnetServiceDBContext _context = context;

        public async Task ReadNotificationById(Guid id, Guid userId)
        {
            try
            {
                await _context.Notifications.Where(n => n.Id == id && n.UserId == userId).ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new UnprocessableEntityException("No notification was read.");
            }
        }

        public async Task ReadAllNotificationByUserId(Guid userId)
        {
            try
            {
                await _context.Notifications.Where(n => n.UserId == userId && !n.IsRead).ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new UnprocessableEntityException("No notification was read.");
            }
        }
    }
}