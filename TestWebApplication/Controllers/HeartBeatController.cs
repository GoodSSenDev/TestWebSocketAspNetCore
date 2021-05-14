using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NetworkModule.Messages;

namespace TestWebApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HeartBeatController : Controller
    {

        [HttpPost("request")]
        public async Task<ActionResult<HeartBeatRequestMessage>> RequestAsync([FromBody] HeartBeatRequestMessage message)
        {
            Debug.WriteLine($"message recieved : {message.Id}");
            var response = new HeartBeatResponseMessage
            {
                Id = message.Id,
                POSData = message.POSData,
                Result = new Result { Status = Status.Success }
            };
            return Ok(response);
        }

    }
}
