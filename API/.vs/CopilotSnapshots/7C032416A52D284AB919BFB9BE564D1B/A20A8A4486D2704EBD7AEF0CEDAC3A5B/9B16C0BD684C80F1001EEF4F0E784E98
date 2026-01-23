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

        public async Task<Notification> AddCustomerAsync(Notification notification)
        {
            var all = await _notificationRepository.GetAllAsync();

            var result = await _notificationRepository.AddAsync(notification);
            return result;
        }

        public async Task<bool> UpdateCustomerAsync(Notification notification)
        {
            var existing = await _notificationRepository.GetByIdAsync(notification.Id);
            if (existing == null) return false;

            await _notificationRepository.UpdateAsync(notification);
            return true;
        }

        public async Task<bool> DeleteCustomerAsync(string id)
        {
            var existing = await _notificationRepository.GetByIdAsync(id);
            if (existing == null) return false;

            await _notificationRepository.DeleteAsync(id);
            return true;
        }
    }
}
