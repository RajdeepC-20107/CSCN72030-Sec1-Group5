using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SCADAHMI_API.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StereoSpeakerController : ControllerBase
    {
        [HttpGet("maxVolume")]
        public ActionResult<int> GetMaxVolume()
        {
            int volumeStub = 100;
            return volumeStub;
        }

        [HttpPost("maxVolume/{volume}")]
        public IActionResult SetMaxVolume(
            short volume
        )
        {
            if (volume < 0 || volume > 100) //TODO: Make this use MaxVolume of the manager class instead
            {
                return BadRequest("Volume must be between 0 and 100.");
            }

            //TODO: Run command via manager class here

            return NoContent();
        }

        [HttpPost("balance")]
        public IActionResult BalanceSpeakers()
        {
            //TODO: Run command via manager class here

            return NoContent();
        }
    }
}
