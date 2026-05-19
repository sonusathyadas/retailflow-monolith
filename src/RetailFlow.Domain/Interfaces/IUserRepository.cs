using RetailFlow.Domain.Entities;

namespace RetailFlow.Domain.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        User GetByEmail(string email);
        User GetByRefreshToken(string refreshToken);
    }
}
