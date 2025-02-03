using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoneyMind_BLL.DTOs.Transactions;
using MoneyMind_BLL.Services.Interfaces;

namespace MoneyMind_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MLController : ControllerBase
    {
        private readonly IMLService mlService;

        public MLController(IMLService mlService)
        {
            this.mlService = mlService;
        }

        // POST: api/ml/classify
        [HttpPost("classify")]
        public async Task<IActionResult> ClassifyTag([FromBody] TransactionToClassificationRequest transactionToClassification)
        {
            if (string.IsNullOrWhiteSpace(transactionToClassification.Description))
                return BadRequest("Description cannot be null or empty.");

            try
            {
                var category = await mlService.ClassificationTag(transactionToClassification.Description);

                if (category == null)
                    return NotFound("No matching tag found.");

                return Ok(new
                {
                    CategoryId = category.Id,
                    CategoryName = category.Name
                });
                return null;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

    }
}
