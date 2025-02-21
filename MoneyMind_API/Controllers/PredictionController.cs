//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using MoneyMind_API.Client;

//namespace MoneyMind_API.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class PredictionController : ControllerBase
//    {
//        private readonly NamedPipeClient _pipeClient;

//        public PredictionController()
//        {
//            // Có thể đăng ký NamedPipeClient thông qua DI nếu muốn, ở đây khởi tạo trực tiếp
//            _pipeClient = new NamedPipeClient();
//        }

//        [HttpGet("intent")]
//        public async Task<IActionResult> GetIntent([FromQuery] string message)
//        {
//            if (string.IsNullOrEmpty(message))
//                return BadRequest("Message không được để trống.");

//            // Gửi message qua Named Pipe và nhận kết quả dự đoán intent
//            var response = await _pipeClient.SendMessageAsync(message);
//            return Ok(new { Message = message, PredictedIntent = response });
//        }
//    }
//}
