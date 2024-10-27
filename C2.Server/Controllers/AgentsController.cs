using C2.ApiModels.Requests;
using C2.Server.Models.Agents;
using C2.Server.Models.Listeners;
using C2.Server.Services;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

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

        [HttpGet("{agentId}")]
        public IActionResult GetAgent(string agentId)
        {
            var agent = _agentService.GetAgent(agentId);

            if (agent is null)
            {
                return NotFound();
            }

            return Ok(agent);
        }

        [HttpGet("{agentId}/tasks")]
        public IActionResult GetTasksResults(string agentId, string taskId)
        {
            var agent = _agentService.GetAgent(agentId);

            if (agent is null)
            {
                return NotFound("Agent not found");
            }

            var tasks = agent.GetTasksResults();

            return Ok(tasks);
        }

        [HttpGet("{agentId}/tasks/{taskId}")]
        public IActionResult GetTaskResult(string agentId, string taskId)
        {
            var agent = _agentService.GetAgent(agentId);

            if (agent is null)
            {
                return NotFound("Agent not found");
            }

            var task = agent.GetTaskResult(taskId);
        
            if (task is null)
            {
                return NotFound("Task not found");
            }

            return Ok(task);
        }

        [HttpPost("{agentId}")]
        public IActionResult TaskAgent(string agentId, [FromBody] TaskAgentRequest request)
        {
            var agent = _agentService.GetAgent(agentId);

            if (agent is null)
            {
                return NotFound();
            }

            AgentTask task = new AgentTask()
            {
                Id = Guid.NewGuid().ToString(),
                Command = request.Command,
                Arguments = request.Arguments,
                File = request.File
            };

            agent.QueueTask(task);

            var root = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}";
            var path = $"{root}/tasks/{task.Id}";

            return Created(path, task);
        }
    }
}
