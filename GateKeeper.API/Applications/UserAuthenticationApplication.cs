using GateKeeper.API.Models;
using GateKeeper.Core.Context;
using GateKeeper.Core.Models.ApiModels;
using GateKeeper.Core.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;

namespace GateKeeper.Core.Application
{
    public class UserAuthenticationApplication
    {
        private AddressesDbContext dbContext;
        private IOptions<ApiSettings> settings;
        public UserAuthenticationApplication(IOptions<ApiSettings> settings, AddressesDbContext dbContext) 
        { 
            this.dbContext = dbContext;
            this.settings = settings;
        }
        public async Task<string?> Authenticate(LoginRequest request)
        {
            User? user = await dbContext.Set<User>().FirstOrDefaultAsync(user=>user.UserName == request.Login);
            if (user == null || !user.Enabled)
            {
                return null;
            }
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Hash))
            {
                return null;
            }
            var claims = new List<Claim> ();
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.GivenName, user.FullName));
            var jwt = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Value.JwtSecretCode)), SecurityAlgorithms.HmacSha256)
                );
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
