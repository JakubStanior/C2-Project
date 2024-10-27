using C2.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace C2.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AgentsController : ControllerBase
    {
        private readonly IAgentService _agentService;

        public AgentsController(IAgentService agentService)
        {
            _agentService = agentService;
        }

        [HttpGet]
        public IActionResult GetAgents()
        {
            var agents = _agentService.GetAgents();

            return Ok(agents);
        }

        [HttpGet("{name}")]
        public IActionResult GetAgent(string name)
        {
            var agent = _agentService.GetAgent(name);

            if (agent is null)
            {
                return NotFound();
            }

            return Ok(agent);
        }
    }
}
