using System.Threading.Tasks;

namespace Simucraft.Server.Services
{
    public interface IEmailService
    {
        Task EmailAsync(string subject, string body, string to);
    }
}
