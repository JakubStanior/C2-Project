using C2.Server.Models.Listeners;

namespace C2.Server.Services
{
    public interface IListenerService
    {
        void AddListener(Listener listener);
        IEnumerable<Listener> GetListeners();
        Listener? GetListener(string name);
        void RemoveListener(Listener listener);
    }

    public class ListenerService : IListenerService
    {
        private readonly List<Listener> _listeners = new();

        public void AddListener(Listener listener)
        {
            _listeners.Add(listener);
        }
        public IEnumerable<Listener> GetListeners()
        {
            return _listeners;
        }

        public Listener? GetListener(string name)
        {
            return GetListeners().FirstOrDefault(listener => listener.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public void RemoveListener(Listener listener)
        {
            _listeners.Remove(listener);
        }
    }
}
