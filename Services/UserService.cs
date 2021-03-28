using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.Cookies;
using BC = BCrypt.Net.BCrypt;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using doggo.Models;
using doggo.Helpers;
using doggo.Data;

namespace doggo.Services
{
    public interface IUserService
    {
        Backpass Authenticate(LoginView credential);
        Backpass CookieAuthenticate(LoginView credential);
        Task<Backpass> SignUpAndAuthenticate(RegisterView credential);
        Task<Backpass> ChangePassword(int Id, ChangePasswordView credential);
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

        public Backpass Authenticate(LoginView credential)
        {
            var res = (from u in db.User
                       where u.Email == credential.Email
                       select u);

            if (res.Any())
            {
                UserDTO user = res.FirstOrDefault();
                if(user.Status == "Deleted"){
                    return new Backpass
                    {
                        Error = true,
                        Data = "User Deleted"
                    };
                } else if(user.Status == "Locked") {
                    return new Backpass
                    {
                        Error = true,
                        Data = "User Locked"
                    };
                }
                if (BC.Verify(credential.Password, user.Password))
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

        public Backpass CookieAuthenticate(LoginView credential)
        {
            var res = (from u in db.User
                       where u.Email == credential.Email
                       select u);

            if (res.Any())
            {
                UserDTO user = res.FirstOrDefault();
                if(user.Status == "Deleted"){
                    return new Backpass
                    {
                        Error = true,
                        Data = "User Deleted"
                    };
                } else if(user.Status == "Locked") {
                    return new Backpass
                    {
                        Error = true,
                        Data = "User Locked"
                    };
                }
                if (BC.Verify(credential.Password, user.Password))
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

        public async Task<Backpass> SignUpAndAuthenticate(RegisterView credential)
        {
            UserDTO user = new UserDTO();
            user.Name = credential.Name;
            user.Email = credential.Email;
            user.Password = BC.HashPassword(credential.Password);
            user.UserRole = "User";
            user.Status = "Unlocked";
            user.CreatedDate = DateTime.Now;
            user.UpdatedDate = DateTime.Now;

            db.Add(user);
            await db.SaveChangesAsync();

            return CookieAuthenticate(new LoginView
            {
                Email = credential.Email,
                Password = credential.Password
            });
        }

        public async Task<Backpass> ChangePassword(int Id, ChangePasswordView credential)
        {
            var user = GetById(Id);
            if(user != null){
                if (BC.Verify(credential.CurrentPassword, user.Password))
                {
                    user.UpdatedDate = DateTime.Now;
                    user.Password = BC.HashPassword(credential.NewPassword);
                    try{
                        db.Update(user);
                        await db.SaveChangesAsync();
                    } catch (DbUpdateConcurrencyException){
                        if(!db.User.Any(e=> e.Id == Id)){
                            return new Backpass
                            {
                                Error = true,
                                Data = "User Not Found"
                            };
                        } else {
                            throw;
                        }
                    }
                    return new Backpass
                    {
                        Error = false,
                        Data = "Password Updated"
                    };
                }
                return new Backpass
                {
                    Error = true,
                    Data = "Wrong Password"
                };
            }
            return new Backpass
            {
                Error = true,
                Data = "User Not Found"
            };
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