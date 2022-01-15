using langley;
using System;
using System.Threading.Tasks;

namespace langley.cli
{
    internal class Program
    {
        private class ConsoleProgressObserver : IProgress<int>
        {
            public void Report(int value)
            {
                Console.WriteLine($"Progress : {value}");
            }
        }

        static async Task Main(string[] args)
        {
            if(args.Length < 2)
                return;
            var split = args[0].Split(":");
            if(split.Length != 2)
                return;
            var address = split[0];
            if(!int.TryParse(split[1], out var port))
                return;

            var settings = new ConnectionSettings
            {
                Address = address,
                Port = port
            };

            var file = args[1];

            using var server = new Server(settings);
            await server.StartAsync();
            if (!await server.TrySendAsync(file, new ConsoleProgressObserver()))
            {
                Console.WriteLine("Cannot send file to console");
            }
        }
    }
}
