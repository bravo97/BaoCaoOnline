using Application.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class NotificationService
    {
        private readonly INotificationReponsitory _notificationRepository;

        public NotificationService(INotificationReponsitory notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<IEnumerable<Notification>> GetAllAsync()
        {
            return await _notificationRepository.GetAllAsync();
        }

        public async Task<Notification?> GetByIdAsync(string id)
        {
            return await _notificationRepository.GetByIdAsync(id);
        }

        public async Task<Notification> AddNotificationAsync(Notification notification)
        {
            var all = await _notificationRepository.GetAllAsync();

            var result = await _notificationRepository.AddAsync(notification);
            return result;
        }

        public async Task<bool> UpdateNotificationAsync(Notification notification)
        {
            var existing = await _notificationRepository.GetByIdAsync(notification.Id);
            if (existing == null) return false;

            await _notificationRepository.UpdateAsync(notification);
            return true;
        }

        public async Task<IEnumerable<Notification>> GetByUserIdAsync(string userId, int page = 1, int pageSize = 20)
        {
            var all = await _notificationRepository.GetAllAsync();
            return all.Where(n => n.UserId == userId || string.IsNullOrEmpty(n.UserId)) // Include broadcast
                      .OrderByDescending(n => n.DateCreate)
                      .Skip((page - 1) * pageSize)
                      .Take(pageSize);
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            var all = await _notificationRepository.GetAllAsync();
            return all.Count(n => (n.UserId == userId || string.IsNullOrEmpty(n.UserId)) && !n.IsRead);
        }

        public async Task<bool> MarkAsReadAsync(string id)
        {
            var item = await _notificationRepository.GetByIdAsync(id);
            if (item == null) return false;
            
            item.IsRead = true;
            await _notificationRepository.UpdateAsync(item);
            return true;
        }

        public async Task<bool> DeleteNotificationAsync(string id)
        {
            var existing = await _notificationRepository.GetByIdAsync(id);
            if (existing == null) return false;

            await _notificationRepository.DeleteAsync(id);
            return true;
        }
    }
}
