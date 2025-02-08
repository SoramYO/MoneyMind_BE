using Libook_API.Configure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MoneyMind_BLL.DTOs;
using MoneyMind_BLL.DTOs.SubWalletTypes;
using MoneyMind_BLL.Services.Implementations;
using MoneyMind_BLL.Services.Interfaces;
using MoneyMind_DAL.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;

namespace MoneyMind_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubWalletTypeController : ControllerBase
    {
        private readonly ISubWalletTypeService subWalletTypeService;

        public SubWalletTypeController(ISubWalletTypeService subWalletTypeService)
        {
            this.subWalletTypeService = subWalletTypeService;
        }

        [HttpGet]
        [Route("{userId:Guid}")]
        [Authorize]
        public async Task<IActionResult> GetAsync(
            [FromRoute] Guid userId,
            [FromQuery] Guid? walletTypeId = null,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 12)
        {
            // Đảm bảo pageIndex và pageSize hợp lệ
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 12;

            // Start with a base filter
            Expression<Func<SubWalletType, bool>> filterExpression = s => s.IsActive;
            filterExpression = filterExpression.AndAlso(s => s.UserId == userId);

            //Combine filters using AndAlso method
            if (walletTypeId.HasValue)
            {
                filterExpression = filterExpression.AndAlso(s => s.WalletTypeId == walletTypeId);
            }

            // Map string orderBy to the appropriate property (use switch or reflection if necessary)
            Func<IQueryable<SubWalletType>, IOrderedQueryable<SubWalletType>> orderByFunc = q => q.OrderBy(s => s.CreateAt);

            var listDataResponse = await subWalletTypeService.GetSubWalletTypesAsync(
                filter: filterExpression,
                orderBy: orderByFunc,
                includeProperties: "WalletType",
                pageIndex: pageIndex,
                pageSize: pageSize
            );

            var response = new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Get sub wallet type of user successfully!",
                Data = listDataResponse
            };

            return Ok(response);
        }

        [HttpGet]
        [Route("detail/{subWalletTypeId:Guid}")]
        public async Task<IActionResult> GetMonthlyGoalByIdAsync([FromRoute] Guid subWalletTypeId)
        {
            var subWalletType = await subWalletTypeService.GetSubWalletTypeByIdAsync(subWalletTypeId);
            if (subWalletType == null)
            {
                return NotFound(new ResponseObject
                {
                    Status = System.Net.HttpStatusCode.NotFound,
                    Message = "Sub wallet type not found!",
                    Data = null
                });
            }

            return Ok(new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Get Sub wallet type successfully!",
                Data = subWalletType
            });
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CreateAsync(SubWalletTypeRequest subWalletTypeRequest)
        {
            // Sử dụng helper để lấy UserId từ token
            var userId = JwtHelper.GetUserIdFromToken(HttpContext.Request, out var errorMessage);

            if (userId == null)
            {
                // Trả về lỗi nếu token không hợp lệ
                return Unauthorized(errorMessage);
            }

            var subWalletTypeResponse = await subWalletTypeService.AddSubWalletTypeAsync(userId.Value, subWalletTypeRequest);

            var response = new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Create sub wallet type successfully !",
                Data = subWalletTypeResponse
            };
            return Ok(response);
        }

        [HttpPost]
        [Route("create-default")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CreateDefaultSubWalletTypesAsync()
        {

            // Sử dụng helper để lấy UserId từ token
            var userId = JwtHelper.GetUserIdFromToken(HttpContext.Request, out var errorMessage);

            if (userId == null)
            {
                // Trả về lỗi nếu token không hợp lệ
                return Unauthorized(errorMessage);
            }

            var necessitiesId = "b203ae2f-3023-41c1-a25a-2b2ec238321d";
            var financialFreedomId = "654a9673-4d23-44b1-9af8-a9562341a60e";
            var educationId = "19ea7e67-8095-4a13-bba4-bda0a4a47a38";
            var leisureId = "6193fcb1-c8c4-44e9-abde-78cdb4258c4e";
            var charityId = "b79d14db-7a81-4046-b66e-1acd761123bb";
            var savingsId = "ebebc667-520d-4eac-88ed-ef9eb8e26aab";

            // Gọi Service để tạo SubWalletTypes
            await subWalletTypeService.CreateDefaultSubWalletTypesAndActivitiesAsync(
                Guid.Parse(necessitiesId),
                Guid.Parse(financialFreedomId),
                Guid.Parse(educationId),
                Guid.Parse(leisureId),
                Guid.Parse(charityId),
                Guid.Parse(savingsId),
                userId.Value
            );
            return Ok("Default sub wallet types created successfully.");
        }

        [HttpPut]
        [Route("{id:Guid}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] SubWalletTypeRequest subWalletTypeRequest)
        {
            // Sử dụng helper để lấy UserId từ token
            var userId = JwtHelper.GetUserIdFromToken(HttpContext.Request, out var errorMessage);

            if (userId == null)
            {
                // Trả về lỗi nếu token không hợp lệ
                return Unauthorized(errorMessage);
            }

            var subWalletTypeResponse = await subWalletTypeService.UpdateSubWalletTypeAsync(id, userId.Value, subWalletTypeRequest);
            if (subWalletTypeResponse == null)
            {
                return NotFound();
            }
            var response = new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Update sub wallet types successfully !",
                Data = subWalletTypeResponse
            };
            return Ok(response);
        }

        [HttpPut]
        [Route("{id:Guid}/delete")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
        {
            // Sử dụng helper để lấy UserId từ token
            var userId = JwtHelper.GetUserIdFromToken(HttpContext.Request, out var errorMessage);

            if (userId == null)
            {
                // Trả về lỗi nếu token không hợp lệ
                return Unauthorized(errorMessage);
            }

            var subWalletTypeResponse = await subWalletTypeService.DeleteSubWalletTypeAsync(id, userId.Value);
            if (subWalletTypeResponse == null)
            {
                return NotFound();
            }
            var response = new ResponseObject
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = "Delete sub wallet types successfully !",
                Data = subWalletTypeResponse
            };
            return Ok(response);
        }
    }
}
