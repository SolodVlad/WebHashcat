using Renci.SshNet;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;

namespace WebHashcat.Areas.Cabinet.Services
{
    public class ShellStreamService
    {
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, ShellStream>> _shellStreamsOfAllUsers;

        public ShellStreamService() => _shellStreamsOfAllUsers = new ConcurrentDictionary<string, ConcurrentDictionary<string, ShellStream>>();

        public void AddToCurrentUser(string username, string key, ShellStream shellStream)
        {
            var currentUserShellStreams = _shellStreamsOfAllUsers.GetOrAdd(username, new ConcurrentDictionary<string, ShellStream>());

            currentUserShellStreams.TryAdd(key, shellStream);
        }

        public ShellStream? GetShellStreamFromCurrentUser(string username, string key)
        {
            if (_shellStreamsOfAllUsers.TryGetValue(username, out var shellStreamsOfCurrentUser))
                if (shellStreamsOfCurrentUser.TryGetValue(key, out var value))
                    return value;
            return null;
        }

        public void RemoveFromUserSessionAsync(string username, string key)
        {
            if (_shellStreamsOfAllUsers.TryGetValue(username, out var shellStreamsOfCurrentUser)) 
                shellStreamsOfCurrentUser.TryRemove(key, out _);
        }

        private static async Task<string> ComputeMD5Async(byte[] data)
        {
            using var stream = new MemoryStream(data);
            var hashBytes = await MD5.Create().ComputeHashAsync(stream);
            return BitConverter.ToString(hashBytes).Replace("-", "");
        }
    }
}