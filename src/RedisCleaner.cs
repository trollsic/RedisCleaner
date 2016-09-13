using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace RedisCleaner
{
    public class RedisKeysCleaner
    {

        static string ipPattern = @"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b";
        private static Regex IpAddressRegex = new Regex(ipPattern, RegexOptions.Compiled);

        private int itemsRemoved = 0;
        private int itemsReceived = 0;
        private int pageSize;

        public void CleanKeys(string connectionString, string pattern = "*", int bulkSize = 100)
        {
            this.pageSize = bulkSize;

            ConfigurationOptions config = ConfigurationOptions.Parse(connectionString);

            DnsEndPoint addressEndpoint = config.EndPoints.First() as DnsEndPoint;
            int port = addressEndpoint.Port;

            bool isIp = IsIpAddress(addressEndpoint.Host);
            if (!isIp)
            {
                IPHostEntry ip = Dns.GetHostEntryAsync(addressEndpoint.Host).Result;
                config.EndPoints.Remove(addressEndpoint);
                config.EndPoints.Add(ip.AddressList.First(), port);
            }

            var redis = ConnectionMultiplexer.Connect(config);

            var db = redis.GetDatabase();

            EndPoint endpoint = db.Multiplexer.GetEndPoints().FirstOrDefault();

            IServer server = db.Multiplexer.GetServer(endpoint);

            IEnumerable<RedisKey> seq = server.Keys(0, pattern, bulkSize, CommandFlags.PreferSlave | CommandFlags.HighPriority);

            Parallel.ForEach(seq, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }, key => HandleKeyTTLCheck(key, db));

            Console.WriteLine("Finished. Scanned: {0}", itemsReceived);
        }

        private void HandleKeyTTLCheck(RedisKey key, IDatabase db)
        {
            try
            {
                Interlocked.Increment(ref itemsReceived);

                TimeSpan? keyTimeToLive = db.KeyTimeToLive(key);
                if (keyTimeToLive.HasValue)
                {
                    Interlocked.Increment(ref itemsRemoved);

                    if (itemsRemoved % (this.pageSize * 100) == 0)
                    {
                        Console.WriteLine("Keys - Checked: {0}, Received: {1}", itemsRemoved, itemsReceived);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private bool IsIpAddress(string host)
        {
            return IpAddressRegex.IsMatch(host);
        }
    }
}