using DOT_NET_WEB_API_AUTH.Core.App;
namespace DOT_NET_WEB_API_AUTH.Services.User
{
    public class UserLoginRequest
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }
    public class UserLoginResponce
    {
        public string AccessToken { get; set; } = "";
        public string RefreshToken { get; set; } = "";
    }
    public partial class UserService
    {
        public async Task<SimpleAppResponse<UserLoginResponce>> UserLoginAsync(UserLoginRequest request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    return new SimpleAppResponse<UserLoginResponce>().Error("Mail no encontrado.");
                }
                else
                {
                    var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, true);
                    if (result.Succeeded)
                    {
                        var token = await GenerateUserToken(user);
                        return new SimpleAppResponse<UserLoginResponce>().Success(token,"Login exitoso.");
                    }
                    else
                    {
                        return new SimpleAppResponse<UserLoginResponce>().Error(result.ToString());
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
