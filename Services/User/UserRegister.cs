using DOT_NET_WEB_API_AUTH.Core.App;
using DOT_NET_WEB_API_AUTH.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace DOT_NET_WEB_API_AUTH.Services.User
{
    public class UserRegisterRequest
    {
        public string Email { get; set; } = "";
        public string UserName { get; set; } = "";
        public string Password { get; set; } = "";
    }
    public partial class UserService
    {

        public async Task<SimpleAppResponse<bool>> UserRegisterAsync(UserRegisterRequest request)
        {
            try
            {
                var user = new ApplicationUser()
                {
                    UserName = request.UserName,
                    Email = request.Email,
                };
                var result = await _userManager.CreateAsync(user, request.Password);
                if (result.Succeeded)
                {
                    return new SimpleAppResponse<bool>().Success(true);
                }
                else
                {
                    var errors = GetRegisterErrors(result);
                    return new SimpleAppResponse<bool>().Error("Error creando cuenta", errors);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private Dictionary<string, string[]> GetRegisterErrors(IdentityResult result)
        {
            var errorDictionary = new Dictionary<string, string[]>(1);

            foreach (var error in result.Errors)
            {
                string[] newDescriptions;

                if (errorDictionary.TryGetValue(error.Code, out var descriptions))
                {
                    newDescriptions = new string[descriptions.Length + 1];
                    Array.Copy(descriptions, newDescriptions, descriptions.Length);
                    newDescriptions[descriptions.Length] = error.Description;
                }
                else
                {
                    newDescriptions = [error.Description];
                }

                errorDictionary[error.Code] = newDescriptions;
            }

            return errorDictionary;
        }
    }
}
