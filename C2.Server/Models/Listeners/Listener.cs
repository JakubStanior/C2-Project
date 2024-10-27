using C2.Server.Services;

namespace C2.Server.Models.Listeners
{
    public abstract class Listener
    {
        public abstract string Name { get; }
        public abstract int BindPort { get; }
        public abstract Task Start();
        public abstract void Stop();
    }
}
