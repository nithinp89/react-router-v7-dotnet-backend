
using System.Text;
using BackendApi.Core.Objects.Core;

namespace BackendApi.Core.Helpers
{
    public class KeyGenerator
    {
        private const string Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private static readonly int Base = Alphabet.Length;

        /// <summary>
        /// Generate Unqiue Record Key which is unique across tenant and object
        /// </summary>
        /// <param name="tenantId">Unique id of the tenant.</param>
        /// <param name="objectId">Unique object id.</param>
        /// <param name="rowId">Record Id from the concerned object table. It can be Standard or Custom.</param>
        /// <returns>Encoded Record Key.</returns>
        public static string GenerateRecordKey (int tenantId, int objectId, int rowId)
        {
            
            string key = string.Empty;

            if(tenantId < 1000 | tenantId > 17576)
            {
                throw new ArgumentOutOfRangeException("tenant_id", $"Tenant Id {tenantId} must be between 1000 and 10000.");
            }

            if (objectId < 100 | objectId > 1000)
            {
                throw new ArgumentOutOfRangeException("object_id", $"Object Id {tenantId} must be between 100 and 1000.");
            }

            key = EncodeToAlpha3Letters(tenantId) + EncodeToAlpha3Letters(objectId) + Base62Encode(rowId).PadLeft(6, '0');

            return key;
        }


        /// <summary>
        /// Get Tenant Id, Object Id and Record Id from the Key.
        /// </summary>
        /// <param name="record_key">Encoded Record Key.</param>
        /// <returns>Array of Int contains Tenant Id, Object Id and Record Id.</returns>
        public static RecordId GetRecordId(string record_key)
        {

            int tenant_id = DecodeFromAlpha3Letters(record_key.Substring(0, 3));
            int object_id = DecodeFromAlpha3Letters(record_key.Substring(3, 3));
            int row_id = Base62Decode(record_key.Substring(6, 6).TrimStart('0'));

            return new RecordId { TenantId = tenant_id, ObjectId = object_id, RowId = row_id };
        }


        // This is enough to encode numbers up to 17,576
        public static string EncodeToAlpha3Letters(int num)
        {
            num -= 1; // Make zero-based
            char[] chars = new char[3];
            for (int i = 2; i >= 0; i--)
            {
                chars[i] = (char)('A' + (num % 26));
                num /= 26;
            }
            return new string(chars);
        }

        public static int DecodeFromAlpha3Letters(string code)
        {
            int num = 0;
            foreach (char c in code)
            {
                num = num * 26 + (c - 'A');
            }
            return num + 1; // Convert back to 1-based index
        }

        public static string Base62Encode(int number)
        {
            if (number == 0) return "0";

            if (number < 0)
                throw new ArgumentOutOfRangeException("value", $"Value {number} must be non-negative.");

            StringBuilder sb = new StringBuilder();
            int value = number;

            while (value > 0)
            {
                int remainder = (int)(value % Base);
                sb.Insert(0, Alphabet[remainder]);
                value /= Base;
            }

            return sb.ToString();
        }

        // Decode a Base62 string back to an integer
        public static int Base62Decode(string base62String)
        {
            int value = 0;

            foreach (char c in base62String)
            {
                value = value * Base + Alphabet.IndexOf(c);
            }

            return value;
        }
    }
}
