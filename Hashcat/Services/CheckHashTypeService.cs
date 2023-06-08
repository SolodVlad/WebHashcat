using WebHashcat.Enums;
using System.Text.RegularExpressions;

namespace WebHashcat.Services
{
    public partial class CheckHashTypeService
    {
        public HashType GetHashType(string hash)
        {
            if (hash.Length == 32 && MyRegex().IsMatch(hash)) return (HashType)Enum.Parse(typeof(HashType), "MD5");
            else if (hash.Length == 40 && MyRegex().IsMatch(hash)) return (HashType)Enum.Parse(typeof(HashType), "SHA1");
            else if (hash.Length == 64 && MyRegex().IsMatch(hash)) return (HashType)Enum.Parse(typeof(HashType), "SHA256");
            else if (hash.Length == 96 && MyRegex().IsMatch(hash)) return (HashType)Enum.Parse(typeof(HashType), "SHA384");
            else if (hash.Length == 128 && MyRegex().IsMatch(hash)) return (HashType)Enum.Parse(typeof(HashType), "SHA512");
            else return HashType.None;
        }

        [GeneratedRegex("^[0-9a-fA-F]+$")]
        private static partial Regex MyRegex();
    }
}
