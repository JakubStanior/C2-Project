using C2.Server.Services;

namespace C2.Server.Models.Listeners
{
    public class HttpListener : Listener
    {
        public override string Name { get; }

        public override int BindPort { get; }

        private CancellationTokenSource _tokenSource;

        public HttpListener(string name, int bindPort)
        {
            Name = name;
            BindPort = bindPort;
            _tokenSource = new CancellationTokenSource();
        }

        public override async Task Start()
        {
            var hostBuilder = new HostBuilder()
                .ConfigureWebHostDefaults(host =>
                {
                    host.UseUrls($"http://0.0.0.0:{BindPort}");
                    host.Configure(ConfigureListenerApp);
                    host.ConfigureServices(ConfigureListenerServices);
                });

            var host = hostBuilder.Build();

            host.RunAsync(_tokenSource.Token);
        }

        private void ConfigureListenerApp(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "handle_implant",
                    pattern: "/",
                    defaults: new { controller = "HttpListener", action = "HandleImplant" }
                );
            });
        }

        private void ConfigureListenerServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSingleton<IAgentService, AgentService>();
        }

        public override void Stop()
        {
            _tokenSource.Cancel();
        }
    }
}
