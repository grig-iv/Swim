using System.IO;
using System.Text;
using Core.Configurations;
using Optional.Unsafe;

namespace Core.Test.Configurations
{
    public static class ConfigParserExtension
    {
        public static TParsedConfig Parse<TParsedConfig>(this ConfigParser parser, string config)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(config)))
            {
                return parser
                    .Parse(stream)
                    .GetConfig<TParsedConfig>()
                    .ValueOrFailure("Parsed config not found");
            }
        }
    }
}