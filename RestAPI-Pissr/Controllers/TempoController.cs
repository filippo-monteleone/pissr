using Microsoft.AspNetCore.Mvc;

namespace RestAPI_Pissr.Controllers
{
    [Route("api/[controller]")]
    public class TempoController : Controller
    {
        private readonly BackgroundMqttClient _client;

        public TempoController(BackgroundMqttClient client)
        {
            _client = client;
        }

        [HttpGet]
        public ActionResult GetTempo()
        {
            return Ok(_client.Date);
        }

        [HttpPut]
        public ActionResult UpdateVelocita(int test)
        {
            _client.Speed = test;
            return Ok();
        }
    }
}
