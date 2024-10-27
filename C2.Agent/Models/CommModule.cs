using System.Collections.Concurrent;

namespace C2.Agent.Models
{
    public abstract class CommModule
    {
        public abstract Task Start();
        public abstract void Stop();
        protected ConcurrentQueue<AgentTask> Inbound = new ConcurrentQueue<AgentTask>();
        protected ConcurrentQueue<AgentTaskResult> Outbound = new ConcurrentQueue<AgentTaskResult>();
        protected AgentMetadata AgentMetadata;

        public virtual void Init(AgentMetadata agentMetadata)
        {
            AgentMetadata = agentMetadata;
        }

        public bool RecvData(out IEnumerable<AgentTask>? tasks)
        {
            if (Inbound.IsEmpty)
            {
                tasks = null;
                return false;
            }
            
            List<AgentTask> agentTasks = new List<AgentTask>();
            
            while (Inbound.TryDequeue(out var task))
            {
                agentTasks.Add(task);
            }

            tasks = agentTasks;
            return true;
        }

        public void SendData(AgentTaskResult result)
        {
            Outbound.Enqueue(result);
        }

        protected IEnumerable<AgentTaskResult> GetOutbound()
        {
            var outbound = new List<AgentTaskResult>();
            while (Outbound.TryDequeue(out var task))
            {
                outbound.Add(task);
            }

            return outbound;
        }
    }
}
