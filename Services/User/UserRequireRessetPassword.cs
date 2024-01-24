using DOT_NET_WEB_API_AUTH.Core.App;
using DOT_NET_WEB_API_AUTH.Core.Mailing;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace DOT_NET_WEB_API_AUTH.Services.User
{
    public class UserForgotPasswordRequest()
    {
        public string Email { get; set; } = string.Empty;
    }
    public partial class UserService
    {
        public async Task<SimpleAppResponse<bool>> UserRequireRessetPasswordAsync(UserForgotPasswordRequest req)
        {
            try
			{
                if (string.IsNullOrEmpty(req.Email) || !IsValidEmail(req.Email))
                {
                    return new SimpleAppResponse<bool>().Error("El mail no cumple con el formato esperado.");
                }
                var user = await _userManager
                    .FindByEmailAsync(req.Email);

                if (user is null)
                    return new SimpleAppResponse<bool>().Error("Algo salió mal, revise los datos del formulario.");

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var message = new EmailMessage(
                    new string[] { user.Email! },
                    "Cambio de contraseña.",
                    "Para cambiar tu contraseña haga click en el siguiente link " +
                    $"{_appSettings.ClientURL}/recover/password?changePasswordVerificationToken={token}");


                await _emailService.SendEmailAsync(message);

                return new SimpleAppResponse<bool>().Success(default,"Enviamos un mail a su correo con las instrucciones para " +
                    "que pueda cambiar su contraseña.");
            }
			catch (Exception)
			{
				throw;
			}
        }
    }
}
