using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace QuizApplication.Services
{
public class JwtService
    {
        private readonly string _expDate;
        private readonly string _issuer;
        private readonly string _secret;

        public JwtService(IConfiguration config)
        {
            var JwtConfig = config.GetSection("JwtConfig");
            _secret = JwtConfig.GetSection("secret").Value;
            _expDate = JwtConfig.GetSection("expirationInMinutes").Value;
            _issuer = JwtConfig.GetSection("issuer").Value;
        }

        public string GenerateSecurityToken(string userName)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, userName)
                }),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_expDate)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
                Issuer = _issuer
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static string GetAuthToken(IConfiguration _config, HttpContext httpContext)
        {
            var userName = httpContext.User.Identity.Name;
            var jwt = new JwtService(_config);
            var authToken = jwt.GenerateSecurityToken(userName);
            return authToken;
        }

        public static JwtSecurityToken ReadToken(string accessToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(accessToken);
            return token;
        }

        public static JwtSecurityToken GetTokenFromHeader(HttpContext context)
        {
            string authHeaderValue = context.Request.Headers["Authorization"];
            var authToken = authHeaderValue.Split().GetValue(1).ToString();
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(authToken);
            return token;
        }
    }
}