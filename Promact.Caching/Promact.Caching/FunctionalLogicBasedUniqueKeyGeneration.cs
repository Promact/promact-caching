using Promact.Core.Caching;
using System.IO.Hashing;


namespace Promact.Caching
{
    public class FunctionalLogicBasedUniqueKeyGeneration : ICachingUniqueKeyGenerationService
    {
        // Dictionary to store hashes
        private readonly Dictionary<string, string> _hashes;

        public FunctionalLogicBasedUniqueKeyGeneration()
        {
            _hashes = new Dictionary<string, string>();
        }
        public string GenerateUniqueKey(string key, string[] args)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }
            if (args.Length == 0)
            {
                throw new ArgumentNullException(nameof(args));
            }
            var typename = args[0];
            var methodName = args[1];
            var type = Type.GetType(typename);
            if (type == null)
            {
                throw new ArgumentException("Type not found");
            }
            var methodInfo = type.GetMethod(methodName);
            if (methodInfo == null)
            {
                throw new ArgumentException("Method not found");
            }
            var hashKey = $"{typename}-{methodName}";
            if (_hashes.ContainsKey(hashKey))
            {
                _hashes.TryGetValue(hashKey, out var val);
                return $"{key}-{val}";
            }
            var methodHash = ComputeMethodHash(type, methodName);
            var argsHash = CalculateHashForArrayOfString(args);
            _hashes[hashKey] = methodHash;
            return $"{key}-{methodHash}";

        }

        // Compute hash for a method
        private string ComputeMethodHash(Type type, string methodName)
        {
            // Construct key using type's full name and method name
            string key = $"{type.FullName}-{methodName}";
            if (_hashes.TryGetValue(key, out string hashString))
            {
                return hashString;
            }

            // Get method info and method body
            var methodInfo = type.GetMethod(methodName);
            var methodBody = methodInfo.GetMethodBody() ?? throw new ArgumentException("Method body cannot be null");

            // Get IL byte array from method body
            var ilBytes = methodBody.GetILAsByteArray() ?? throw new ArgumentException("IL byte array cannot be null");


            // Compute hash for the combined IL byte array
            // Use XXHash64 algorithm to compute hash as it is faster than SHA256
            var hash = XxHash64.Hash(ilBytes);
            _hashes[key] = hashString = Convert.ToBase64String(hash);
            return hashString;
        }



        // Compute hash for an array of strings
        private static string CalculateHashForArrayOfString(string[] args)
        {
            // Create a memory stream and binary writer
            using var stringStream = new MemoryStream();
            using var writer = new BinaryWriter(stringStream);
            // Write each argument to the stream
            foreach (var arg in args)
            {
                if (string.IsNullOrEmpty(arg))
                {
                    writer.Write(string.Empty);
                }
                else
                {
                    writer.Write(arg);
                }
            }
            // Compute hash for the stream
            // Use XXHash64 algorithm to compute hash as it is faster than SHA256
            var hash = XxHash64.Hash(stringStream.ToArray());
            return Convert.ToBase64String(hash);
        }
    }
}
