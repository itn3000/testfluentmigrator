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
        static NpgsqlConnection CreateConnection()
        {
            var cb = new NpgsqlConnectionStringBuilder();
            cb.Host = "localhost";
            cb.Port = 5432;
            cb.Username = "postgres";
            cb.Password = "intercom";
            cb.Database = "fmtest1";
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
            using (var con = CreateConnection())
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
