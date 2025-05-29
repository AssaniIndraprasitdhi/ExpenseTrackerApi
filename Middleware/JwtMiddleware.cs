using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ExpenseTrackerApi.Data;
using ExpenseTrackerApi.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ExpenseTrackerApi.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _secret;

        public JwtMiddleware(RequestDelegate next, IOptions<JwtSettings> jwtOptions)
        {
            _next = next;
            _secret = jwtOptions.Value.Secret;
        }

        public async Task Invoke(HttpContext context, AppDbContext db)
        {
            var token = context
                .Request.Headers["Authorization"]
                .FirstOrDefault()
                ?.Split(" ")
                .Last();

            if (token != null)
                AttachUserToContext(context, db, token);

            await _next(context);
        }

        private void AttachUserToContext(HttpContext context, AppDbContext db, string token)
        {
            try
            {
                var key = Encoding.ASCII.GetBytes(_secret);
                var tokenHandler = new JwtSecurityTokenHandler();
                tokenHandler.ValidateToken(
                    token,
                    new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero,
                    },
                    out SecurityToken validatedToken
                );

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(
                    jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value
                );

                context.Items["User"] = db.Users.Find(userId);
            }
            catch { }
        }
    }
}
