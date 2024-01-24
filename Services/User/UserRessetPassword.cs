using DOT_NET_WEB_API_AUTH.Core.App;
using Org.BouncyCastle.Ocsp;
using System.ComponentModel.DataAnnotations;

namespace DOT_NET_WEB_API_AUTH.Services.User
{
    public class UserRessetPasswordRequest()
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
    public partial class UserService
    {
        public async Task<SimpleAppResponse<bool>>
            UserRessetPasswordFromEmailAsync(UserRessetPasswordRequest req)
        {
            try
            {
                var validationResult = ValidateUserRessetPasswordRequest(req);
                if (!string.IsNullOrEmpty(validationResult))
                    return new SimpleAppResponse<bool>().Error(validationResult);

                var user = await _userManager.FindByEmailAsync(req.Email);
                if (user is null)
                    return new SimpleAppResponse<bool>().Error("Usuario no encontrado."); 

                var resultChangePassword = await _userManager.ResetPasswordAsync(user, req.Token, req.Password);
                if (!resultChangePassword.Succeeded)
                {
                    foreach (var error in resultChangePassword.Errors)
                    {
                        _logger.LogInformation(error.Code, error.Description);
                    }
                    return new SimpleAppResponse<bool>().Error("Error actualizando contraseña.");
                }

                return new SimpleAppResponse<bool>().Success(default,"Contraseña actualizada con éxito.");
            }
            catch (Exception)
            {
                throw;
            }
        }
        public string ValidateUserRessetPasswordRequest(UserRessetPasswordRequest req)
        {
            var result = string.Empty;
            if (string.IsNullOrEmpty(req.Email) || !IsValidEmail(req.Email))
            {
                result = "No se ingresó mail o el mismo no cumle con el formato" +
                    "de una dirección de email.";
                return result;
            }

            if (string.IsNullOrEmpty(req.Password))
            {
                result = "Debe ingresar una contraseña.";
                return result;
            }                

            if (string.IsNullOrEmpty(req.ConfirmPassword))
            {
                result = "Debe confirmar la nueva contraseña.";
                return result;
            }
                
            if (req.Password != req.ConfirmPassword)
            {
                result = "La contraseña y su confirmación no son iguales.";
                return result;
            }

            if (string.IsNullOrEmpty(req.Token))
            {
                result = "Faltan campos en la consulta.";
                return result;
            }
            return result;
        }
    }
}
