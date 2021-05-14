using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NetworkModule;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace TestWebApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TriggerController : Controller
    {
        readonly WebSocketChannelManager _channelManager;

        public TriggerController(WebSocketChannelManager channelManager)
        {
            _channelManager = channelManager;
        }

        [HttpPost("startRemoteTransaction")]
        public async Task<ActionResult<string>> StartRemoteTransaction([FromBody] string uri)
        {
            await _channelManager.RequestConnectionAsync(uri);

            return Ok();
        }
    }
}
