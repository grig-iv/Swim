using System.IO;
using Optional;

namespace Core.Configurations
{
    public interface IConfigLocator
    {
        Option<Stream> FindConfig();
    }
}