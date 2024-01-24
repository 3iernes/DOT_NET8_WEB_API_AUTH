using DOT_NET_WEB_API_AUTH.Core.App;
using DOT_NET_WEB_API_AUTH.Core.Mailing;
using DOT_NET_WEB_API_AUTH.Entities;
using DOT_NET_WEB_API_AUTH.Services.Mail;
using DOT_NET_WEB_API_AUTH.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Ocsp;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace DOT_NET_WEB_API_AUTH.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AuthController> _logger;
        private readonly AppSettings _appSettings;
        private readonly UserService _userService;
        private readonly IEmailSenderService _emailService;
        public AuthController(UserService userService, 
            ILogger<AuthController> logger, 
            UserManager<ApplicationUser> userManager,
            AppSettings appSettings,
            IEmailSenderService emailService)
        {
            _userService = userService;
            _logger = logger;
            _userManager = userManager;
            _appSettings = appSettings;
            _emailService = emailService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(SimpleAppResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(SimpleAppResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(SimpleAppResponse<bool>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SimpleAppResponse<bool>>> 
            Register(UserRegisterRequest req)
        {
            try
            {
                var result = await _userService.UserRegisterAsync(req);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(Register));
                return StatusCode(500, 
                        new SimpleAppResponse<bool>().Error("Server error."));
            }

        }

        [HttpPost]
        [ProducesResponseType(typeof(SimpleAppResponse<UserLoginResponce>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(SimpleAppResponse<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(SimpleAppResponse<>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SimpleAppResponse<UserLoginResponce>>> 
            Login(UserLoginRequest req)
        {
            try
            {
                var result = await _userService.UserLoginAsync(req);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(Login));
                return StatusCode(500, 
                    new SimpleAppResponse<bool>().Error("Server error."));
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(SimpleAppResponse<UserRefreshTokenResponce>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(SimpleAppResponse<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(SimpleAppResponse<>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SimpleAppResponse<UserRefreshTokenResponce>>> 
            RefreshToken(UserRefreshTokenRequest req)
        {
            try
            {
                var result = await _userService.UserRefreshTokenAsync(req);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(RefreshToken));
                return StatusCode(500, 
                    new SimpleAppResponse<bool>().Error("Server error."));
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(SimpleAppResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(SimpleAppResponse<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(SimpleAppResponse<>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SimpleAppResponse<bool>>> Logout()
        {
            try
            {
                var result = await _userService.UserLogoutAsync(User);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(Logout));
                return StatusCode(500,
                    new SimpleAppResponse<bool>().Error("Server error."));
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(SimpleAppResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(SimpleAppResponse<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(SimpleAppResponse<>), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = $"Enviar mail recuperar contraseña.")]
        public async Task<ActionResult<SimpleAppResponse<bool>>> 
            ForgotPassWord(UserForgotPasswordRequest req)
        {
            try
            {
                var result = await _userService.UserRequireRessetPasswordAsync(req);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(ForgotPassWord));
                return StatusCode(500,
                    new SimpleAppResponse<bool>().Error("Server error."));
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(SimpleAppResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(SimpleAppResponse<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(SimpleAppResponse<>), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = $"Recuperar contraseña después de recibir mail con el token")]
        public async Task<ActionResult> ResetPassword(UserRessetPasswordRequest req)
        {
            try
            {
                var result = await _userService.UserRessetPasswordFromEmailAsync(req);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(ResetPassword));
                return StatusCode(500,
                    new SimpleAppResponse<bool>().Error("Server error."));
            }
        }

        [HttpPost, Authorize]
        [ProducesResponseType(typeof(SimpleAppResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(SimpleAppResponse<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(SimpleAppResponse<>), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = $"Cambiar contraseña")]
        public async Task<ActionResult<SimpleAppResponse<bool>>> 
            ChangePassword(UserChangePasswordRequest req)
        {
            try
            {
                string userId = User.Claims
                    .FirstOrDefault(c => c.Type is ClaimTypes.NameIdentifier)!.Value;

                var result = await _userService.UserChangePasswordAsync(req, userId);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(ChangePassword));
                return StatusCode(500,
                    new SimpleAppResponse<bool>().Error("Server error."));
            }
        }

        [HttpPost]
        [Authorize]
        [SwaggerOperation(Summary = $"Rol: Con token válido ya ingresa <> Retorna el nombre del usuario que consulta el endpoint.")]
        public string Profile()
        {
            return User.FindFirst(ClaimTypes.Name)?.Value ?? "";
        }

        [HttpGet]
        [Authorize(Roles =$"{UserRoles.Admin}")]
        [SwaggerOperation(Summary = $"Rol: {UserRoles.Admin} <> Retorna el nombre del usuario que consulta el endpoint.")]
        public string Admin()
        {
            return User.FindFirst(ClaimTypes.Name)?.Value ?? "";
        }

        [HttpGet]
        [Authorize(Roles = $"{UserRoles.User}")]
        [SwaggerOperation(Summary = $"Rol: {UserRoles.Admin}, {UserRoles.User} <> Retorna el nombre del usuario que consulta el endpoint.")]
        public string Users()
        {
            return User.FindFirst(ClaimTypes.Name)?.Value ?? "";
        }
    }
}
