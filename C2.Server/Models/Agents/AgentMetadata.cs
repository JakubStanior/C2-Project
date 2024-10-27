namespace C2.Server.Models.Agents
{
    public abstract class AgentMetadata
    {
        public string Id { get; set; }
        public string Hostname { get; set; }
        public string Username { get; set; }
        public string ProcessName { get; set; }
        public string ProcessId { get; set; }
        public string Integrity { get; set; }
        public string Architecture { get; set; }
    }
}
