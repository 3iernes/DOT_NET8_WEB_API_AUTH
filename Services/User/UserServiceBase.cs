using DOT_NET_WEB_API_AUTH.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using DOT_NET_WEB_API_AUTH.Data;
using DOT_NET_WEB_API_AUTH.Core.App;
using DOT_NET_WEB_API_AUTH.Services.Mail;

namespace DOT_NET_WEB_API_AUTH.Services.User
{
    public partial class UserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly AppSettings _appSettings;
        private readonly ApplicationDbContext _context;
        private readonly IEmailSenderService _emailService;
        private readonly ILogger<UserService> _logger;
        public UserService(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager,
            ApplicationDbContext applicationDbContext,
            AppSettings appSettings,
            IEmailSenderService emailSenderService,
            ILogger<UserService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appSettings = appSettings;
            _roleManager = roleManager;
            _context = applicationDbContext;
            _emailService = emailSenderService;
            _logger = logger;
        }
        private async Task<UserLoginResponce> GenerateUserToken(ApplicationUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var token = TokenUtil.GetToken(_appSettings, user, userRoles);
            await _userManager.RemoveAuthenticationTokenAsync(user,TokenOptions.DefaultProvider, "RefreshToken");
            var refreshToken = await _userManager.GenerateUserTokenAsync(user, TokenOptions.DefaultProvider, "RefreshToken");
            await _userManager.SetAuthenticationTokenAsync(user, TokenOptions.DefaultProvider,"RefreshToken", refreshToken);
            return new UserLoginResponce() { AccessToken = token, RefreshToken = refreshToken };
        }        
    }
}
