
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MoneyMind_BLL.DTOs;
using MoneyMind_BLL.DTOs.Accounts;
using MoneyMind_BLL.DTOs.AccountTokens;
using MoneyMind_BLL.DTOs.Emails;
using MoneyMind_BLL.Services.Implementations;
using MoneyMind_BLL.Services.Interfaces;
using MoneyMind_DAL.Entities;
namespace MoneyMind_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountBankController : ControllerBase
    {
        private readonly IAccountBankService _accountBankService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailService _emailService;
        public AccountBankController(
            IAccountBankService accountBankService,
            UserManager<IdentityUser> userManager,
            IEmailService emailService)
        {
            _accountBankService = accountBankService;
            _userManager = userManager;
            _emailService = emailService;
        }
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> AddAccountBank([FromBody] AccountBankDTO accountBankDTO)
        {
            var user = await _userManager.FindByIdAsync(accountBankDTO.UserId);
            if (user == null)
            {
                return BadRequest(new { message = "User not found" });
            }
            var accountBank = new AccountBank
            {
                UserId = Guid.Parse(accountBankDTO.UserId),
                BankName = accountBankDTO.BankName,
                AccountNumber = accountBankDTO.AccountNumber,
                Username = accountBankDTO.UserName,
                Password = accountBankDTO.Password
            };
            await _accountBankService.AddAccountBankAsync(accountBank);
            return Ok(new { message = "Account bank added successfully" });
        }
        [HttpGet]
        [Route("get")]
        public async Task<IActionResult> GetAccountBankByUserId([FromQuery] string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest(new { message = "User not found" });
            }
            var accountBanks = await _accountBankService.GetAccoutBankByUserIdAsync(Guid.Parse(userId));
            return Ok(accountBanks);
        }
    }
}
