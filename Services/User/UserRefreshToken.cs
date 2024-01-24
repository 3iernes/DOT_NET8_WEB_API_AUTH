using DOT_NET_WEB_API_AUTH.Core.App;
using Microsoft.AspNetCore.Identity;

namespace DOT_NET_WEB_API_AUTH.Services.User
{
    public class UserRefreshTokenRequest
    {
        public string AccessToken { get; set; } = "";
        public string RefreshToken { get; set; } = "";
    }
    public class UserRefreshTokenResponce
    {
        public string AccessToken { get; set; } = "";
        public string RefreshToken { get; set; } = "";
    }
    public partial class UserService
    {
        public async Task<SimpleAppResponse<UserRefreshTokenResponce>> UserRefreshTokenAsync(UserRefreshTokenRequest request)
        {
            try
            {
                var principal = TokenUtil.GetPrincipalFromExpiredToken(_appSettings, request.AccessToken);
                if (principal == null || principal.FindFirst("UserName")?.Value == null)
                {
                    return new SimpleAppResponse<UserRefreshTokenResponce>().Error("Usuario no encontrado.");
                }
                else
                {
                    var user = await _userManager.FindByNameAsync(principal.FindFirst("UserName")?.Value ?? "");
                    if (user == null)
                    {
                        return new SimpleAppResponse<UserRefreshTokenResponce>().Error("Usuario no encontrado.");
                    }
                    else
                    {
                        if (!await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, "RefreshToken", request.RefreshToken))
                        {
                            return new SimpleAppResponse<UserRefreshTokenResponce>().Error("Refresh token expired");
                        }
                        var token = await GenerateUserToken(user);
                        return new SimpleAppResponse<UserRefreshTokenResponce>().Success(
                            new UserRefreshTokenResponce() 
                            { 
                                AccessToken = token.AccessToken, 
                                RefreshToken = token.RefreshToken 
                            });
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
