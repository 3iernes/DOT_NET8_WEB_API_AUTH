using DOT_NET_WEB_API_AUTH.Core.App;
using System.Security.Claims;

namespace DOT_NET_WEB_API_AUTH.Services.User
{
    public partial class UserService
    {
        public async Task<SimpleAppResponse<bool>> UserLogoutAsync(ClaimsPrincipal user)
        {
			try
			{
                if (user.Identity?.IsAuthenticated ?? false)
                {
                    var username = user.Claims.First(x => x.Type == "UserName").Value;
                    var appuser = _context.Users.First(x => x.UserName == username);
                    if (appuser != null) { await _userManager.UpdateSecurityStampAsync(appuser); }
                    return new SimpleAppResponse<bool>().Success(true);
                }
                return new SimpleAppResponse<bool>().Success(true);
            }
			catch (Exception)
			{
				throw;
			}
        }
    }
}
