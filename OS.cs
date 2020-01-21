using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

public static class Win32
{
    [DllImport("Kernel32")]
    internal static extern bool SetConsoleCtrlHandler(HandlerRoutine handler, bool Add);

    internal delegate bool HandlerRoutine(CtrlTypes ctrlType);

    internal enum CtrlTypes
    {
        CTRL_C_EVENT = 0,
        CTRL_BREAK_EVENT,
        CTRL_CLOSE_EVENT,
        CTRL_LOGOFF_EVENT = 5,
        CTRL_SHUTDOWN_EVENT
    }

    public static async Task RunConsoleAsyncWithGracefulShutdown(this IHostBuilder host, CancellationTokenSource _stoppingCts)
    {
        var complete = new ManualResetEventSlim();
        var hr = new HandlerRoutine(type =>
        {
            Console.WriteLine("Shutdown request");
            _stoppingCts.Cancel();
            complete.Wait();
            Console.WriteLine("Shutdown completed");

            return false;
        });

        SetConsoleCtrlHandler(hr, true);

        try
        {
            await host.RunConsoleAsync(_stoppingCts.Token);
            Console.WriteLine("Exiting");
        }
        finally
        {
            complete.Set();
            Console.WriteLine("returning");
        }
    }
}