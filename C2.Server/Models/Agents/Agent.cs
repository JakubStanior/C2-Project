using System.Collections.Concurrent;

namespace C2.Server.Models.Agents
{
    public class Agent
    {
        public AgentMetadata Metadata { get; }
        public DateTime LastSeen { get; private set; }
        private readonly ConcurrentQueue<AgentTask> _pendingTasks = new ConcurrentQueue<AgentTask>();
        private readonly List<AgentTaskResult> _taskResults = new List<AgentTaskResult>();

        public Agent(AgentMetadata metadata)
        {
            Metadata = metadata;
        }

        public void CheckIn()
        {
            LastSeen = DateTime.UtcNow;
        }

        public void QueueTask(AgentTask task)
        {
            _pendingTasks.Enqueue(task);
        }

        public IEnumerable<AgentTask> GetPendingTasks()
        {
            List<AgentTask> tasks = new List<AgentTask>();

            while(_pendingTasks.TryDequeue(out AgentTask task))
            {
                tasks.Add(task);
            }

            return tasks;
        }

        public IEnumerable<AgentTaskResult> GetTasksResults()
        {
            return _taskResults;
        }

        public AgentTaskResult GetTaskResult(string taskId)
        {
            return GetTasksResults().FirstOrDefault(r => r.Id.Equals(taskId));
        }
    }
}
