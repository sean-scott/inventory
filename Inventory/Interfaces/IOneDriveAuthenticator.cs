using System.Threading.Tasks;
using Microsoft.OneDrive.Sdk;

namespace Inventory.Interfaces
{
    public interface IOneDriveAuthenticator
    {
        Task<IOneDriveClient> LoginAsync();
    }
}