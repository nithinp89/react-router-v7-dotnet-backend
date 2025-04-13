using BackendApi.Core.Helpers;
using System.Text.Json;

namespace BackendApi.Api
{
    public class TestCases
    {
        public static void Run()
        {
            Console.WriteLine("TestCases.Run");
            Console.WriteLine("GenerateRecordKey");

            var record_key = KeyGenerator.GenerateRecordKey(1001, 101, 100100100);
            Console.WriteLine(record_key);

            int tenant_id = KeyGenerator.DecodeFromAlpha3Letters(record_key.Substring(0, 3));
            int object_id = KeyGenerator.DecodeFromAlpha3Letters(record_key.Substring(3, 3));
            int row_id = KeyGenerator.Base62Decode(record_key.Substring(6, 6).TrimStart('0'));
            Console.WriteLine(record_key.Substring(0, 3) + " " + record_key.Substring(3, 3) + " " + record_key.Substring(6, 6).TrimStart('0'));
            Console.WriteLine(tenant_id + " " + object_id + " " + row_id);
            Console.WriteLine(JsonSerializer.Serialize(KeyGenerator.GetRecordId("BMMADW06m0bk")));
        }
    }
}
