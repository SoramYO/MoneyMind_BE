using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoneyMind_BLL.DTOs;
using MoneyMind_BLL.Services.Implementations;
using MoneyMind_BLL.Services.Interfaces;

namespace MoneyMind_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly ITagService tagService;

        public TagController(ITagService tagService)
        {
            this.tagService = tagService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 50)
        {
            // Đảm bảo pageIndex và pageSize hợp lệ
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 50;

            var listDataResponse = await tagService.GetTagsAsync(
                filter: null,
                orderBy: null,
                includeProperties: "",
                pageIndex: pageIndex,
                pageSize: pageSize
            );

            var response = new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Get all tag successfully!",
                Data = listDataResponse
            };

            return Ok(response);
        }
    }
}
