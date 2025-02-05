using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoneyMind_BLL.DTOs.MonthlyGoals;
using MoneyMind_BLL.DTOs;
using MoneyMind_BLL.Services.Implementations;
using MoneyMind_BLL.Services.Interfaces;

namespace MoneyMind_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IconController : ControllerBase
    {
        private readonly IIConService iconService;

        public IconController(IIConService iconService)
        {
            this.iconService = iconService;
        }

        [HttpGet("all-icons")]
        public IActionResult GetAllIcons()
        {
            var icons = iconService.GetAllIconUrls();
            var response = new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Get all url icon successfully !",
                Data = icons
            };
            return Ok(response);
        }
    }
}
