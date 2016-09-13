using System;
using System.Collections.Generic;
using System.Text;

namespace RedisCleaner
{
    public class Program
    {
        public static void Main(string[] args)
        {            
            var connectionString = GetArgsValue("-c", args);
            var pattern = GetArgsValue("-p", args);
            var bulkSize = GetArgsValue("-b", args);

            if (string.IsNullOrWhiteSpace(connectionString.Value) ||
            string.IsNullOrWhiteSpace(pattern.Value) ||
            string.IsNullOrWhiteSpace(bulkSize.Value))
            {
                PrintUsageHelp();
            }
            else
            {
                Console.WriteLine(connectionString.Value);
                Console.WriteLine(pattern.Value);
                Console.WriteLine(bulkSize.Value);

                var redisCleaner = new RedisKeysCleaner();
                redisCleaner.CleanKeys(connectionString.Value, pattern.Value, Convert.ToInt32(bulkSize.Value));
            }
        }

        static void PrintUsageHelp()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Parameters:");
            sb.AppendLine("-c <redis connection string>     -Redis full connection string");
            sb.AppendLine("-p <pattern>     -Redis KEYS command pattern");
            sb.AppendLine("-b <bulk size>       -Number of items to process in batch");
            sb.AppendLine();
            sb.AppendLine("Usage Example:");
            sb.AppendLine("RedisCleaner.exe -c \"myredishost:6379,password=*****,abortConnect=False\" -p \"cache:*\" -b 100");
            sb.AppendLine();

            Console.WriteLine(sb);
        }

        static KeyValuePair<string, string> GetArgsValue(string name, string[] args)
        {
            KeyValuePair<string, string> keyValuePair = new KeyValuePair<string, string>();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == name)
                {
                    keyValuePair = new KeyValuePair<string, string>(name, args[i + 1]);
                    break;
                }
            }
            return keyValuePair;
        }
    }
}
