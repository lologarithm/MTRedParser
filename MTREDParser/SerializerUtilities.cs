using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MTREDParser
{
    class SerializerUtilities
    {
        public static void SerializeHistory(Dictionary<long, List<MTREDWorkerStats>> dictionary, Stream stream)
        {
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(dictionary.Count);
            foreach (var kvp in dictionary)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value.Count);
                foreach (MTREDWorkerStats stat in kvp.Value)
                {
                    writer.Write(stat.name);
                    writer.Write(stat.rsolved);
                    writer.Write(stat.mhash);
                }
            }
            writer.Flush();
            writer.Close();
        }

        public static Dictionary<long, List<MTREDWorkerStats>> DeserializeHistory(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);
            int count = reader.ReadInt32();
            var dictionary = new Dictionary<long, List<MTREDWorkerStats>>(count);
            for (int n = 0; n < count; n++)
            {
                long key = reader.ReadInt64();
                var list_count = reader.ReadInt32();
                List<MTREDWorkerStats> worker_list = new List<MTREDWorkerStats>(list_count);
                for (int m = 0; m < list_count; m++)
                {
                    MTREDWorkerStats stats = new MTREDWorkerStats();
                    stats.name = reader.ReadString();
                    stats.rsolved = reader.ReadDouble();
                    stats.mhash = reader.ReadDouble();
                    worker_list.Add(stats);
                }

                dictionary.Add(key, worker_list);
            }
            reader.Close();
            return dictionary;
        }

        public static void SerializeBlocks(Dictionary<string, SolvedBlock> dictionary, Stream stream)
        {
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(dictionary.Count);
            foreach (var kvp in dictionary)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value.amount);
                writer.Write(kvp.Value.duration);
                writer.Write(kvp.Value.shares);
                writer.Write(kvp.Value.solved_time.ToFileTimeUtc());
                writer.Write(kvp.Value.confirmed);

                writer.Write(kvp.Value.worker_shares.Count);
                foreach (MTREDWorkerStats stat in kvp.Value.worker_shares)
                {
                    writer.Write(stat.name);
                    writer.Write(stat.rsolved);
                    writer.Write(stat.mhash);
                }
            }
            writer.Flush();
            writer.Close();
        }

        public static Dictionary<string, SolvedBlock> DeserializeBlocks(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);
            int count = reader.ReadInt32();
            Dictionary<string, SolvedBlock> dictionary = new Dictionary<string, SolvedBlock>(count);
            for (int n = 0; n < count; n++)
            {
                string key = reader.ReadString();
                SolvedBlock sb = new SolvedBlock();
                sb.hash = key;
                sb.amount = reader.ReadDouble();
                sb.duration = reader.ReadString();
                sb.shares = reader.ReadInt64();
                sb.solved_time = DateTime.FromFileTimeUtc(reader.ReadInt64());
                sb.confirmed = reader.ReadBoolean();

                var list_count = reader.ReadInt32();
                List<MTREDWorkerStats> worker_list = new List<MTREDWorkerStats>(list_count);
                for (int m = 0; m < list_count; m++)
                {
                    MTREDWorkerStats stats = new MTREDWorkerStats();
                    stats.name = reader.ReadString();
                    stats.rsolved = reader.ReadDouble();
                    stats.mhash = reader.ReadDouble();
                    worker_list.Add(stats);
                }
                sb.worker_shares = worker_list;

                dictionary.Add(key, sb);
            }
            reader.Close();
            return dictionary;
        }
    }
}
