using Domain.Entities;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IDatabaseConnectionFactory
    {
        string CreateConnectionString(Customer customer);
    }
}
