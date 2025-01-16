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
        private readonly IMLService _mlService;

        public MLController(IMLService mlService)
        {
            _mlService = mlService;
        }

        // POST: api/ml/classify
        [HttpPost("classify")]
        public async Task<IActionResult> ClassifyTransaction([FromBody] TransactionToClassificationRequest transactionToClassification)
        {
            if (string.IsNullOrWhiteSpace(transactionToClassification.Description))
                return BadRequest("Description cannot be null or empty.");

            if (transactionToClassification.Amount <= 0)
                return BadRequest("Amount must be greater than 0.");

            try
            {
                var category = await _mlService.ClassificationCategory(transactionToClassification.Description, transactionToClassification.Amount);

                if (category == null)
                    return NotFound("No matching category found.");

                return Ok(new
                {
                    CategoryId = category.Id,
                    CategoryName = category.Name
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

    }
}
