using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.Shared
{
    public static class AuthenticationConfig
    {
        //Generate JWS Token
        public static string GenerateJSONWebToken()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("21C544391C4F90E66420D57E0EED8631D064AB414097FFC84A35D9C885EEED35"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("UserName","Admin"),
                new Claim("Role","1"),
            };

            var token = new JwtSecurityToken("xyz",
                "xyz",
                claims,
                DateTime.UtcNow,
                expires: DateTime.Now.AddHours(7),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        //Congure Jwt Authentication
        internal static TokenValidationParameters tokenValidationParams;
        public static void CongureJwtAuthentication(this IServiceCollection services)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("21C544391C4F90E66420D57E0EED8631D064AB414097FFC84A35D9C885EEED35"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            tokenValidationParams = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                ValidIssuer = "xyz",
                ValidateLifetime = true,
                ValidAudience = "xyz",
                RequireSignedTokens = true,
                IssuerSigningKey = credentials.Key,
                ClockSkew = TimeSpan.FromHours(7)
            };

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })

            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = tokenValidationParams;
#if PROD || UAT
                options.IncludeErrorDefaults = false;
#elif DEBUG
                options.RequireHttpsMetadata = false;
#endif
            });
        }
    }
}
