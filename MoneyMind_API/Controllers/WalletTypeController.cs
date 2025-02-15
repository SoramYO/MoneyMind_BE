using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoneyMind_BLL.DTOs;
using MoneyMind_BLL.DTOs.Wallets;
using MoneyMind_BLL.DTOs.WalletTypes;
using MoneyMind_BLL.Services.Implementations;
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

            Expression<Func<WalletType, bool>> filterExpression = w => w.IsDisabled == false ;

            var listDataResponse = await walletTypeService.GetWalletTypesAsync(
                filter: filterExpression,
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

        [HttpGet]
        [Route("detail/{walletTypeId:Guid}")]
        public async Task<IActionResult> GetMonthlyGoalByIdAsync([FromRoute] Guid walletTypeId)
        {
            var walletType = await walletTypeService.GetWalletTypeByIdAsync(walletTypeId);
            if (walletType == null)
            {
                return NotFound(new ResponseObject
                {
                    Status = System.Net.HttpStatusCode.NotFound,
                    Message = "Wallet type not found!",
                    Data = null
                });
            }

            return Ok(new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Get wallet type successfully!",
                Data = walletType
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAsync(WalletTypeRequest walletTypeRequest)
        {

            var walletTypeResponse = await walletTypeService.AddWalletTypeAsync(walletTypeRequest);

            var response = new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Create wallet types successfully!",
                Data = walletTypeResponse
            };
            return Ok(response);
        }

        [HttpPut]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] WalletTypeRequest walletTypeRequest)
        {
            var walletTypeResponse = await walletTypeService.UpdateWalletTypeAsync(id, walletTypeRequest);
            if (walletTypeResponse == null)
            {
                return NotFound();
            }
            var response = new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Update wallet type successfully!",
                Data = walletTypeResponse
            };
            return Ok(response);
        }

        [HttpPut]
        [Route("{id:Guid}/delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
        {
            var walletTypeResponse = await walletTypeService.DeleteWalletTypeAsync(id);
            if (walletTypeResponse == null)
            {
                return NotFound();
            }
            var response = new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Delete wallet type successfully!",
                Data = walletTypeResponse
            };
            return Ok(response);
        }
    }
}
