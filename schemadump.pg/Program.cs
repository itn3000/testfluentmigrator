using System;
using System.Collections.Generic;
using System.Linq;
using Npgsql;
using System.Data;

namespace schemadump.pg
{
    static class DtExtensions
    {
        public static IEnumerable<Dictionary<string, object>> ToDictionary(this DataTable dt)
        {
            foreach (var row in dt.Rows.OfType<DataRow>())
            {
                yield return dt.Columns.OfType<DataColumn>().Select(x => (n: x.ColumnName, v: row[x])).ToDictionary(x => x.n, x => x.v);
            }
        }
    }
    class Program
    {
        static NpgsqlConnection CreateConnection(string host, int port, string user, string pass, string dbname)
        {
            var cb = new NpgsqlConnectionStringBuilder();
            cb.Host = host;
            cb.Port = port;
            cb.Username = user;
            cb.Password = pass;
            cb.Database = dbname;
            return new NpgsqlConnection(cb.ToString());
        }
        static string[] GetSchemaCollectionNames(NpgsqlConnection con)
        {
            using (var dt = con.GetSchema())
            {
                return dt.ToDictionary().Select(dic => dic["CollectionName"] as string).ToArray();
            }
        }
        static void Main(string[] args)
        {
            // TODO: create schema dumper
            using (var con = CreateConnection(args[0], int.Parse(args[1]), args[2], args[3], args[4]))
            {
                con.Open();
                foreach (var collectionName in GetSchemaCollectionNames(con))
                {
                    Console.WriteLine($"collection = {collectionName}");
                    using (var schemaTable = con.GetSchema(collectionName))
                    {
                        foreach (var row in schemaTable.ToDictionary())
                        {
                            if(collectionName == "Columns" && 
                                ((string)row["table_schema"] == "information_schema" ||
                                (string)row["table_schema"] == "pg_catalog"))
                            {
                                continue;
                            }
                            foreach (var kv in row)
                            {
                                var vtypename = kv.Value != null ? kv.Value.GetType().ToString() : "unknown";
                                Console.WriteLine($"    {kv.Key} = {kv.Value}, {vtypename}");
                            }
                            Console.WriteLine();
                        }
                    }
                }
            }
        }
    }
}
