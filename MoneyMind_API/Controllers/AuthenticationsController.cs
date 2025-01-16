using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MoneyMind_BLL.DTOs;
using MoneyMind_BLL.DTOs.Accounts;
using MoneyMind_BLL.DTOs.AccountTokens;
using MoneyMind_BLL.DTOs.Emails;
using MoneyMind_BLL.Services.Implementations;
using MoneyMind_BLL.Services.Interfaces;

namespace MoneyMind_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationsController : ControllerBase
    {
        private readonly ITokenService tokenService;
        private readonly IEmailService emailService;
        private readonly UserManager<IdentityUser> userManager;

        public AuthenticationsController(ITokenService tokenService, IEmailService emailService, UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
            this.tokenService = tokenService;
            this.emailService = emailService;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] AccountRegisterRequest accountRegisterRequest)
        {
            var existingUser = await userManager.FindByEmailAsync(accountRegisterRequest.Email);
            if (existingUser != null)
            {
                return BadRequest("Email already exists.");
            }

            var identityUser = new IdentityUser
            {
                UserName = accountRegisterRequest.Username,
                Email = accountRegisterRequest.Email
            };

            var identityResult = await userManager.CreateAsync(identityUser, accountRegisterRequest.Password);

            if (identityResult.Succeeded)
            {
                // Add roles to this User
                if (accountRegisterRequest.Roles != null && accountRegisterRequest.Roles.Any())
                {
                    identityResult = await userManager.AddToRolesAsync(identityUser, accountRegisterRequest.Roles);

                    if (identityResult.Succeeded)
                    {
                        // Tạo token xác nhận email
                        var token = await userManager.GenerateEmailConfirmationTokenAsync(identityUser);

                        // Encode token
                        var encodedToken = Uri.EscapeDataString(token);

                        // Tạo URL xác nhận email
                        var confirmationLink = $"{Request.Scheme}://{Request.Host}/api/Authentications/ConfirmEmail?userId={identityUser.Id}&token={encodedToken}";

                        var emailConfirmRequest = new EmailConfirmRequest
                        {
                            Username = identityUser.UserName,
                            ConfirmationLink = confirmationLink
                        };

                        await emailService.SendConfirmEmailAsync(identityUser.Email, "Confirm your email", "Templates/EmailTemplate/EmailConfirmTemplate.html", emailConfirmRequest);

                        return Ok("User was registered! Please login.");
                    }
                }
            }

            return BadRequest(identityResult.Errors);
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] AccountLoginRequest accountLoginRequest)
        {
            var user = await userManager.FindByEmailAsync(accountLoginRequest.Email);
            if (user == null || !await userManager.CheckPasswordAsync(user, accountLoginRequest.Password))
            {
                return BadRequest("Username or password incorrect");
            }

            if (!await userManager.IsEmailConfirmedAsync(user))
            {
                return BadRequest("Email is not confirmed. Please confirm your email first.");
            }

            var roles = await userManager.GetRolesAsync(user) ?? new List<string>();
            var accessToken = tokenService.CreateJWTToken(user, roles.ToList());
            var refreshToken = tokenService.GenerateRefreshToken();

            var expiryDate = DateTime.UtcNow.AddDays(7); // Ví dụ: Refresh token có hạn 7 ngày
            await userManager.SetAuthenticationTokenAsync(user, "MoneyMind", "RefreshToken", refreshToken);
            await userManager.SetAuthenticationTokenAsync(user, "MoneyMind", "RefreshTokenExpiry", expiryDate.ToString("o"));

            var response = new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Login successfully!",
                Data = new AccountResponse
                {
                    UserId = Guid.Parse(user.Id),
                    Username = user.UserName,
                    Email = user.Email,
                    Roles = roles.ToArray(),
                    Tokens = new AccountTokenResponse
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken
                    }
                }
            };

            return Ok(response);
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] AccountTokenRequest accountTokenRequest)
        {
            // Kiểm tra tính hợp lệ của refresh token
            var user = await userManager.FindByIdAsync(accountTokenRequest.UserId.ToString());
            if (user == null)
            {
                return BadRequest("Invalid user.");
            }

            var savedRefreshToken = await userManager.GetAuthenticationTokenAsync(user, "MoneyMind", "RefreshToken");
            if (savedRefreshToken != accountTokenRequest.RefreshToken)
            {
                return BadRequest("Invalid refresh token.");
            }

            // Lấy thời gian hết hạn của refresh token
            var expiryToken = await userManager.GetAuthenticationTokenAsync(user, "MoneyMind", "RefreshTokenExpiry");
            if (DateTime.TryParse(expiryToken, out var expiryDate) && expiryDate < DateTime.UtcNow)
            {
                return BadRequest("Refresh token has expired.");
            }

            // Tạo access token mới
            var roles = await userManager.GetRolesAsync(user);
            var newAccessToken = tokenService.CreateJWTToken(user, roles.ToList());

            // Tùy chọn: Tạo refresh token mới và lưu lại
            var newRefreshToken = tokenService.GenerateRefreshToken();
            var newExpiryDate = DateTime.UtcNow.AddDays(7); // Ví dụ: Refresh token có hạn 7 ngày
            await userManager.SetAuthenticationTokenAsync(user, "MoneyMind", "RefreshToken", newRefreshToken);
            await userManager.SetAuthenticationTokenAsync(user, "MoneyMind", "RefreshTokenExpiry", newExpiryDate.ToString("o"));

            // Trả về token mới cho người dùng
            return Ok(new AccountTokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }

        [HttpGet]
        [Route("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            // Đọc file template từ thư mục hiện tại

            string failedTemplatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "EmailTemplate", "EmailConfirmFailureTemplate.html");
            string successTemplatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "EmailTemplate", "EmailConfirmSuccessTemplate.html");

            // Read the correct template based on the outcome
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
            {
                return Content(await System.IO.File.ReadAllTextAsync(failedTemplatePath), "text/html");
            }

            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return Content(await System.IO.File.ReadAllTextAsync(failedTemplatePath), "text/html");
            }

            var result = await userManager.ConfirmEmailAsync(user, token);

            string templatePath = result.Succeeded ? successTemplatePath : failedTemplatePath;
            return Content(await System.IO.File.ReadAllTextAsync(templatePath), "text/html");
        }

        [HttpPost("GoogleLogin")]
        public async Task<IActionResult> GoogleLogin([FromBody] AccountLoginGoogleRequest accountLoginGoogleRequest)
        {
            // Xác minh token JWT nhận được từ phía client
            var payload = await GoogleJsonWebSignature.ValidateAsync(accountLoginGoogleRequest.Token);

            // Nếu token hợp lệ, bạn có thể truy cập các thông tin từ payload
            // Ví dụ: email, name, và các claims khác
            var email = payload.Email;

            // Xử lý đăng nhập tại đây, ví dụ: kiểm tra người dùng trong cơ sở dữ liệu, tạo session, v.v.

            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Nếu người dùng chưa tồn tại, tạo mới
                var newUser = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };
                var createResult = await userManager.CreateAsync(newUser);
                if (!createResult.Succeeded)
                {
                    return BadRequest("Failed to create a new user from Google account.");
                }
                await userManager.AddToRoleAsync(newUser, "User");
                user = newUser;
            }
            else
            {
                if (user.PasswordHash != null)
                {
                    return BadRequest("Invalid token");
                }
            }

            var roles = await userManager.GetRolesAsync(user) ?? new List<string>();
            var accessToken = tokenService.CreateJWTToken(user, roles.ToList());
            var refreshToken = tokenService.GenerateRefreshToken();

            var expiryDate = DateTime.UtcNow.AddDays(7); // Ví dụ: Refresh token có hạn 7 ngày
            await userManager.SetAuthenticationTokenAsync(user, "MoneyMind", "RefreshToken", refreshToken);
            await userManager.SetAuthenticationTokenAsync(user, "MoneyMind", "RefreshTokenExpiry", expiryDate.ToString("o"));

            var response = new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Login successfully!",
                Data = new AccountResponse
                {
                    UserId = Guid.Parse(user.Id),
                    Username = user.UserName,
                    Email = user.Email,
                    Roles = roles.ToArray(),
                    Tokens = new AccountTokenResponse
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken
                    }
                }
            };

            return Ok(response);
        }
    }
}