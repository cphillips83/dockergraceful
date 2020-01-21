//https://github.com/marcel-dempers/docker-development-youtube-series
//https://lostechies.com/patricklioi/2016/01/27/powerful-integration-testing/
//https://docs.microsoft.com/en-us/virtualization/windowscontainers/kubernetes/kube-windows-services?tabs=ManagementIP
//https://github.com/microsoft/vscode-docker/wiki/Choosing-Development-Environment
//https://hub.docker.com/_/microsoft-dotnet-core-runtime/
//https://garfbradaz.github.io/blog/2018/12/13/debug-dotnet-core-in-docker.html
//https://martinfowler.com/articles/microservice-testing/#definition
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace dockergraveful
{
    class Program
    {
        private static readonly CancellationTokenSource _stoppingCts =
                                               new CancellationTokenSource();


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

        static async Task Main(string[] args)
        {
            //var shutdown = new ManualResetEvent(false);
            var complete = new ManualResetEventSlim();
            try
            {
                var hr = new HandlerRoutine(type =>
                {
                    Console.WriteLine($"ConsoleCtrlHandler got signal: {type}");

                    _stoppingCts.Cancel();
                    //shutdown.Set();
                    complete.Wait();

                    return false;
                });
                SetConsoleCtrlHandler(hr, true);

                System.Console.CancelKeyPress += (e, s) => { };

                await new HostBuilder()
                    .ConfigureLogging(logging =>
                    {
                        logging.AddDebug();
                        logging.AddConsole();
                    })
                    .ConfigureServices((hostContext, services) =>
                    {
                        services.AddHostedService<GracePeriodManagerService>();
                    })
                    .RunConsoleAsync(_stoppingCts.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Fucks");
            }
            finally
            {

                Console.WriteLine("Wtf");
                for (var i = 0; i < 10; i++)
                {
                    Console.WriteLine("pong");
                    await Task.Delay(1000);
                }
                complete.Set();
            }
        }
    }
}