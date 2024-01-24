using DOT_NET_WEB_API_AUTH.Core.App;
using DOT_NET_WEB_API_AUTH.Entities;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DOT_NET_WEB_API_AUTH.Services.User
{
    public static class TokenUtil
    {
        public static string GetToken(AppSettings appSettings, ApplicationUser user, IList<string> userRoles)
        {
            try
            {
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.SecretKey));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                var userClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                    new Claim(ClaimTypes.Name,user.UserName??""),
                    new Claim(ClaimTypes.Email,user.Email??"")
                };

                var userRolesClaims = new List<Claim>();

                foreach (var userRol in userRoles)
                {
                    userRolesClaims.Add(new Claim(ClaimTypes.Role,userRol));
                }

                userClaims.AddRange(userRolesClaims);

                var tokeOptions = new JwtSecurityToken(
                    issuer: appSettings.Issuer,
                    audience: appSettings.Audience,
                    claims: userClaims,
                    expires: DateTime.UtcNow.AddSeconds(appSettings.TokenExpireSeconds),
                    signingCredentials: signinCredentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                return tokenString;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static ClaimsPrincipal GetPrincipalFromExpiredToken(AppSettings appSettings, string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = appSettings.Audience,
                ValidIssuer = appSettings.Issuer,
                ValidateLifetime = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.SecretKey))
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("GetPrincipalFromExpiredToken Token is not valiated");

            return principal;
        }
    }
}
