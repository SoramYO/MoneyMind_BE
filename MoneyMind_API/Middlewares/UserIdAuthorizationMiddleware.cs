using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace MoneyMind_API.Middlewares
{
    public class UserIdAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public UserIdAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Kiểm tra route có chứa userId hay không
            var userIdFromRoute = context.Request.RouteValues["userId"]?.ToString();
            if (string.IsNullOrEmpty(userIdFromRoute))
            {
                // Nếu không có userId trong route, bỏ qua middleware
                await _next(context);
                return;
            }

            // Lấy token từ header Authorization
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var tokenHandler = new JwtSecurityTokenHandler();

                    // Giải mã và kiểm tra token
                    var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
                    if (jwtToken == null)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Unauthorized: Invalid token.");
                        return;
                    }

                    // Lấy UserId và Role từ token
                    var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                    var roleClaims = jwtToken.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);

                    if (!string.IsNullOrEmpty(userIdClaim))
                    {
                        // Kiểm tra nếu userId không khớp và không phải Admin
                        if (userIdClaim != userIdFromRoute && !roleClaims.Contains("Admin"))
                        {
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            await context.Response.WriteAsync("Access denied: User ID mismatch or insufficient permissions.");
                            return;
                        }
                    }
                    else
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Unauthorized: Missing UserId in token.");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync($"Unauthorized: Invalid token. Error: {ex.Message}");
                    return;
                }
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized: Missing or invalid Authorization header.");
                return;
            }

            // Nếu mọi thứ hợp lệ, chuyển tiếp request
            await _next(context);
        }


    }
}
