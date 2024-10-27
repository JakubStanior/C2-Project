using C2.ApiModels.Requests;
using C2.Server.Models.Listeners;
using C2.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace C2.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ListenersController : ControllerBase
    {
        private readonly IListenerService _listeners;
        private readonly IAgentService _agentService;

        public ListenersController(IListenerService listeners, IAgentService agentService)
        {
            _listeners = listeners;
            _agentService = agentService;
        }

        [HttpGet]
        [Route("/Listeners")]
        public IActionResult GetListeners()
        {
            var listeners = _listeners.GetListeners();

            return Ok(listeners);
        }

        [HttpGet("{name}")]
        public IActionResult GetListener(string name)
        {
            var listener = _listeners.GetListener(name);
            
            if (listener is null)
            {
                return NotFound();
            }

            return Ok(listener);
        }

        [HttpPost]
        public IActionResult StartListener([FromBody] StartHttpListenerRequest request)
        {
            var listener = new HttpListener(request.Name, request.BindPort);
            var allListeners = _listeners.GetListeners();

            if (allListeners.Any(l => l.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase)))
            {
                return BadRequest("Listener with this name already exists");
            }

            if (allListeners.Any(l => l.BindPort.Equals(request.BindPort)))
            {
                return BadRequest("Listener with this port already exists");
            }

            if (request.BindPort < 1 || request.BindPort > 65535)
            {
                return BadRequest("You have to specify correct port (1-65535)");
            }

            listener.Start();
            
            _listeners.AddListener(listener);

            var root = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}";
            var path = $"{root}/{listener.Name}";

            return Created(path, listener);
        }

        [HttpDelete("{name}")]
        public IActionResult RemoveListener(string name)
        {
            var listener = _listeners.GetListener(name);

            if (listener is null)
            {
                return NotFound(name);
            }

            listener.Stop();
            _listeners.RemoveListener(listener);

            return Ok();
        }
    }
}
