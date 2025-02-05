using Microsoft.AspNetCore.Http;
using MoneyMind_BLL.Services.Interfaces;
using System;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MoneyMind_BLL.Services.Implementations
{
    public class IconService : IIConService
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public IconService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public List<string> GetAllIconUrls()
        {
            // Lấy Request hiện tại, phòng trường hợp null nếu code được gọi ngoài bối cảnh HTTP
            var request = httpContextAccessor.HttpContext?.Request;
            if (request == null)
            {
                return new List<string>();
            }

            // Tạo baseUrl = https://domain:port
            var baseUrl = $"{request.Scheme}://{request.Host.Value}";

            // Đường dẫn vật lý đến folder "wwwroot/Assets/Icons" (hoặc bạn tự điều chỉnh nếu Assets ở nơi khác)
            var iconDirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Icons");

            // Định dạng ảnh muốn lấy
            var supportedExtensions = new[] { ".png", ".jpg", ".jpeg", ".gif", ".svg" };

            // Lấy tất cả các file (có extension hợp lệ) trong folder
            var filePaths = Directory
                .EnumerateFiles(iconDirPath)
                .Where(file => supportedExtensions.Contains(Path.GetExtension(file).ToLower()))
                .ToList();

            // Chuyển mỗi file thành URL tĩnh
            var iconUrls = filePaths.Select(file =>
            {
                var fileName = Path.GetFileName(file);
                return $"{baseUrl}/Assets/Icons/{fileName}";
            }).ToList();

            return iconUrls;
        }
    }
}
