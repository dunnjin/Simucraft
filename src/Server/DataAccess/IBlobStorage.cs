using System.IO;
using System.Threading.Tasks;

namespace Simucraft.Server.DataAccess
{
    public interface IBlobStorage
    {
        Task DeleteContainerAsync(string container);
        Task DeleteBlobAsync(string container, string blob);
        Task<string> SaveImageAsync(string container, string blob, Stream stream);
        Task<Stream> GetBlobAsync(string container, string id);
    }
}
