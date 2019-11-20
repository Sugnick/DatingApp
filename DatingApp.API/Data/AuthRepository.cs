using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DatingApp.API.Model;

namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;

        public AuthRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<User> Login(string userName, string password)
        {
            //It does not throw exception but if it is FirstAsync it will throw exception
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == userName);

            if(user == null)
                return null;

            if(!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        /// <summary>
        /// To Register the User
        /// </summary>
        /// <param name="user">The User Type to save changes for password</param>
        /// <param name="password">The Password to be hashed and salted</param>
        /// <returns>The User</returns>
        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            //The Values to be Inserted to DB
            await _context.Users.AddAsync(user);

            //To Save the Values in DB ie. it will be inserted to DB
            await _context.SaveChangesAsync();

            //return User
            return user;
        }

        /// <summary>
        /// Hashing Password
        /// </summary>
        /// <param name="password">The Password provided by user.</param>
        /// <param name="passwordHash">Reference of Hashed Password</param>
        /// <param name="passwordSalt">Reference of Salt Password</param>
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> UserExists(string userName)
        {
            if(await _context.Users.AnyAsync(x => x.UserName == userName))
            {
                return true;
            }

            return false;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[]passwordSalt) 
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {                
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                
                for(int i=0; i<computedHash.Length; i++)
                {
                    if(computedHash[i] != passwordHash[i])
                        return false;
                }
            }

            return true;
        }                                                                                                     
    }
}