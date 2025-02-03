using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoneyMind_BLL.DTOs;
using MoneyMind_BLL.Services.Interfaces;
using MoneyMind_DAL.Entities;
using System.Linq.Expressions;

namespace MoneyMind_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletTypeController : ControllerBase
    {
        private readonly IWalletTypeService walletTypeService;

        public WalletTypeController(IWalletTypeService walletTypeService)
        {
            this.walletTypeService = walletTypeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 12)
        {
            // Đảm bảo pageIndex và pageSize hợp lệ
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 12;

            var listDataResponse = await walletTypeService.GetWalletTypesAsync(
                filter: null,
                orderBy: null,
                includeProperties: "",
                pageIndex: pageIndex,
                pageSize: pageSize
            );

            var response = new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Get all wallet type successfully!",
                Data = listDataResponse
            };

            return Ok(response);
        }
    }
}
