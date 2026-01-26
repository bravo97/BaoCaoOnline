using Application.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class FeedbackService
    {
        private readonly IFeedbackRepository _repository;

        public FeedbackService(IFeedbackRepository repository)
        {
            _repository = repository;
        }

        public async Task<Feedback> CreateAsync(Feedback feedback)
        {
            // basic business rules: sanitize, rate-limit placeholder
            feedback.Subject = feedback.Subject?.Trim();
            feedback.Message = feedback.Message?.Trim();
            var created = await _repository.AddAsync(feedback);
            return created;
        }

        public async Task<Feedback?> GetByIdAsync(string id) => await _repository.GetByIdAsync(id);

        public async Task<IEnumerable<Feedback>> QueryAsync(int page = 1, int pageSize = 20) => await _repository.QueryAsync(page, pageSize);

        public async Task<bool> RespondAsync(string id, string response)
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null) return false;
            item.Response = response;
            item.ResponseAt = DateTime.UtcNow;
            item.Status = FeedbackStatus.Closed;
            await _repository.UpdateAsync(item);
            return true;
        }

        public async Task<bool> UpdateStatusAsync(string id, FeedbackStatus status)
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null) return false;
            item.Status = status;
            await _repository.UpdateAsync(item);
            return true;
        }
    }
}
