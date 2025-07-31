using _01.Contracts.Models;
using Microsoft.AspNetCore.Mvc;

namespace _06.NotificationService.Controllers
{
    [ApiController]
    [Route("api/notify")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationSender _sender;

        public NotificationController(INotificationSender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<IActionResult> Notify([FromBody] NotificationRequestDto dto)
        {
            if (dto == null) return BadRequest("Payload required.");
            await _sender.SendAsync(dto);
            return Ok(new { status = "notification_sent" });
        }
    }
}
