using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IFeedbackRepository
    {
        Task<Feedback> AddAsync(Feedback feedback);
        Task<Feedback?> GetByIdAsync(string id);
        Task<IEnumerable<Feedback>> QueryAsync(int page, int pageSize);
        Task UpdateAsync(Feedback feedback);
        Task DeleteAsync(string id);
    }
}
