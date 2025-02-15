using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoneyMind_BLL.DTOs;
using MoneyMind_BLL.DTOs.DataDefaults;
using MoneyMind_BLL.Services.Implementations;
using MoneyMind_BLL.Services.Interfaces;

namespace MoneyMind_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DefaultFileController : ControllerBase
    {

        private readonly IDefaultFileService defaultFileService;

        public DefaultFileController(IDefaultFileService defaultFileService)
        {
            this.defaultFileService = defaultFileService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDefaults()
        {
            try
            {
                var dataDefaults = await defaultFileService.GetDefaultDataAsync();
                var response = new ResponseObject
                {
                    Status = System.Net.HttpStatusCode.OK,
                    Message = "Get all data default successfully!",
                    Data = dataDefaults
                };
                return Ok(response);
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDefaults([FromBody] DataDefaults data)
        {
            try
            {
                await defaultFileService.WriteDefaultDataAsync(data);
                return Ok("Dữ liệu mặc định đã được cập nhật thành công.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
