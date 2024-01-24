using DOT_NET_WEB_API_AUTH.Core.App;
using Microsoft.AspNetCore.Identity;
using Org.BouncyCastle.Ocsp;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace DOT_NET_WEB_API_AUTH.Services.User
{
    public class UserChangePasswordRequest()
    {
        public string CurrentPassword { get; set; } = string.Empty;

        public string NewPassword { get; set; } = string.Empty;

        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
    public partial class UserService
    {
        public async Task<SimpleAppResponse<bool>> UserChangePasswordAsync(UserChangePasswordRequest req, string userId)
        {
            try
            {
                if(string.IsNullOrEmpty(userId)) return new SimpleAppResponse<bool>().Error("Usuario no encontrado.");

                var validationResult = ValidateUserChangePasswordRequest(req);
                if (!string.IsNullOrEmpty(validationResult))
                    return new SimpleAppResponse<bool>().Error(validationResult);

                var dbUser = await _userManager.FindByIdAsync(userId);
                if (dbUser == null) return new SimpleAppResponse<bool>().Error("Usuario no encontrado.");

                IdentityResult result = await _userManager.ChangePasswordAsync(
                dbUser,
                    req.CurrentPassword,
                    req.NewPassword);

                if (!result.Succeeded)
                    return new SimpleAppResponse<bool>()
                        .Error("Error actualizando contraseña, revise los campos.");

                return  new SimpleAppResponse<bool>().Success(default,"Contraseña actualizada con éxito.");
            }
            catch (Exception)
            {
                throw;
            }
        }
        public string ValidateUserChangePasswordRequest(UserChangePasswordRequest req)
        {
            var result = string.Empty;

            if (string.IsNullOrEmpty(req.CurrentPassword))
            {
                result = "Debe ingresar su contraseña actual.";
                return result;
            }

            if (string.IsNullOrEmpty(req.NewPassword))
            {
                result = "Debe ingresar su nueva contraseña.";
                return result;
            }

            if (string.IsNullOrEmpty(req.ConfirmNewPassword))
            {
                result = "Debe ingresar la confirmación de su nueva contraseña.";
                return result;
            }

            if (req.NewPassword != req.ConfirmNewPassword)
            {
                result = "La nueva contraseña y su confirmación no son iguales.";
                return result;
            }

            return result;
        }
    }
}
