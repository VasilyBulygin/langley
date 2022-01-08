using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace langley.core
{
    public class Server : IDisposable
    {
        private const uint BufferSize = 1024;

        private readonly ConnectionSettings _connectionSettings;
        private Socket _socket;

        public Server(ConnectionSettings connectionSettings)
        {
            _connectionSettings = connectionSettings;
        }

        public async Task StartAsync()
        {
            if (_socket is {Connected: true})
                return;
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            await _socket.ConnectAsync(_connectionSettings.Address, _connectionSettings.Port);
        }

        public async Task<bool> TrySendAsync(string filePath, IProgress<int> progress)
        {
            if (_socket is null || !_socket.Connected)
                return false;
            await using var stream = new FileStream(filePath, FileMode.Open);
            using var reader = new BinaryReader(stream);

            //Sending length of the file in a little-endian format
            await _socket.SendAsync(BitConverter.GetBytes(stream.Length).Reverse().ToArray(), SocketFlags.DontRoute);

            var buffer = new byte[BufferSize];
            int read;
            while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
            {
                try
                {
                    await _socket.SendAsync(new ArraySegment<byte>(buffer, 0, read), SocketFlags.DontRoute);
                }
                catch (Exception)
                {
                    return false;
                }
                progress.Report(read);
            }

            return true;
        }

        public void Dispose()
        {
            if (_socket is null || !_socket.Connected)
                return;
            _socket.Dispose();
            _socket = null;
        }
    }
}