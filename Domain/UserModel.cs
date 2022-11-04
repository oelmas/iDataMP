using DataAccess;
using Common.Cache;
namespace Domain
{
    public class UserModel
    {
        UserDao userDao = new UserDao();
        
        public bool LoginUser(string username, string password)
        {
            return userDao.Login(username, password);
        }
        public bool EditPassword(int user, string password)
        {
            if (user == UserLoginCache.UserId)
            {
                
                
            }

            return true;
        }

        
    }
}