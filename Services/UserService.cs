using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.Tasks;
using doggo.Models;
using doggo.Helpers;
using doggo.Data;

namespace doggo.Services
{
    public interface IUserService
    {
        Backpass Authenticate(LoginDTO credential);
        Backpass CookieAuthenticate(LoginDTO credential);
        Backpass SignUpAndAuthenticate(RegisterView credential);
        IEnumerable<UserDTO> GetAll();
        UserDTO GetById(int id);
    }

    public class UserService : IUserService
    {
        private readonly AppSettings _appSettings;
        private readonly DBContext db;

        public UserService(IOptions<AppSettings> appSettings, DBContext context)
        {
            _appSettings = appSettings.Value;
            db = context;
        }

        public Backpass Authenticate(LoginDTO credential)
        {
            var res = (from u in db.User
                       where u.Email == credential.Email
                       select u);

            if (res.Any())
            {
                UserDTO user = res.FirstOrDefault();
                if (user.Password == credential.Password)
                {
                    user.Password = null;
                    // authentication successful so generate jwt token
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.Name, user.Id.ToString()),
                            new Claim(ClaimTypes.Role, user.UserRole)
                        }),
                        Expires = DateTime.UtcNow.AddHours(4),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    AuthenticationDTO authen = new AuthenticationDTO
                    {
                        UserInfo = user,
                        Token = tokenHandler.WriteToken(token)
                    };

                    return new Backpass
                    {
                        Error = false,
                        Data = authen
                    };
                }
                return new Backpass
                {
                    Error = true,
                    Data = "Invalid Password"
                };
            }
            return new Backpass
            {
                Error = true,
                Data = "User Not Found"
            };
        }

        public Backpass CookieAuthenticate(LoginDTO credential)
        {
            var res = (from u in db.User
                       where u.Email == credential.Email
                       select u);

            if (res.Any())
            {
                UserDTO user = res.FirstOrDefault();
                if (user.Password == credential.Password)
                {
                    user.Password = null;
                    
                    var claims = new List<Claim>
                    {
                        new Claim("Id", user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.Name),
                        new Claim("Email", user.Email),
                        new Claim(ClaimTypes.Role, user.UserRole)
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    return new Backpass
                    {
                        Error = false,
                        Data = new ClaimsPrincipal(claimsIdentity)
                    };
                }
                return new Backpass
                {
                    Error = true,
                    Data = "Invalid Password"
                };
            }
            return new Backpass
            {
                Error = true,
                Data = "User Not Found"
            };
        }

        public Backpass SignUpAndAuthenticate(RegisterView credential)
        {
            UserDTO user = new UserDTO();
            user.Id = credential.Id;
            user.Name = credential.Name;
            user.Email = credential.Email;
            user.Password = credential.Password;
            user.UserRole = "User";
            user.CreatedDate = DateTime.Now;
            user.UpdatedDate = DateTime.Now;

            db.Add(user);
            db.SaveChanges();

            return CookieAuthenticate(new LoginDTO
            {
                Email = credential.Email,
                Password = credential.Password
            });
        }

        public IEnumerable<UserDTO> GetAll()
        {
            return db.User.ToList();
        }

        public UserDTO GetById(int id)
        {
            var user = db.User.FirstOrDefault(x => x.Id == id);
            return user;
        }
    }
}