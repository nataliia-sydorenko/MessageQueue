using MessageQueue.Models;
using Microsoft.AspNetCore.Mvc;

namespace MessageQueue.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileController : ControllerBase
    {
        private readonly Producer _producer;
        private readonly ILogger<FileController> _logger;

        public FileController(Producer producer, ILogger<FileController> logger)
        {
            _producer = producer;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult CopyFile(string from, string to)
        {
            try
            {
                _producer.SendMessage(new FileMessage
                {
                    OriginLocation = from,
                    NewLocation = to
                });
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during copy file");
                throw;
            }
        }
    }
}