using System.IO;
using Core.Configurations;

namespace PlayGround
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var parser = new ConfigParser();
            
            var fileStream = new FileStream("config.yaml", FileMode.Open);
            parser.Parse(fileStream);
        }
    }
}