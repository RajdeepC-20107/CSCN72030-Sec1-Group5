using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SCADAHMI_API.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LightsController : ControllerBase
    {
        [HttpGet("state")]
        public ActionResult<bool> GetState()
        {
            bool stateStub = true;
            return stateStub;
        }

        [HttpPost("state/{isOn:bool}")]
        public IActionResult ChangeState(
            bool isOn
        )
        {
            //TODO: Perform command via manager class here

            return NoContent();
        }

        [HttpPost("brightness/{isBright:bool}")]
        public IActionResult ChangeBrightness(
            bool isBright
        )
        {
            //TODO: Perform command via manager class here

            return NoContent();
        }

        [HttpGet("minBrightness")]
        public ActionResult<int> GetMinimumBrightness()
        {
            int brightnessStub = 100;
            return brightnessStub;
        }

        [HttpPost("minBrightness/{value:int}")]
        public IActionResult SetMinimumBrightness(
            int value
        )
        {
            if (true) //TODO: Add condition for if min brightness over max
            {
                
            }

            return NoContent();
        }

        [HttpGet("maxBrightness")]
        public ActionResult<int> GetMaximumBrightness()
        {
            int brightnessStub = 100;
            return brightnessStub;
        }

        [HttpPost("maxBrightness/{value:int}")]
        public IActionResult SetMaximumBrightness(
            int value
        )
        {
            if (true) //TODO: Add condition for if max brightness under min
            {
                
            }

            return NoContent();
        }
    }
}
