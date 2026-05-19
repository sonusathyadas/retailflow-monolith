using System.Linq;
using RetailFlow.Domain.Entities;
using RetailFlow.Domain.Interfaces;
using RetailFlow.Infrastructure.Data;

namespace RetailFlow.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(RetailFlowDbContext context) : base(context) { }

        public User GetByEmail(string email)
        {
            return _dbSet.FirstOrDefault(u => u.Email == email);
        }

        public User GetByRefreshToken(string refreshToken)
        {
            return _dbSet.FirstOrDefault(u => u.RefreshToken == refreshToken);
        }
    }
}
