using System.Threading.Tasks;

namespace Marketplace.Server.Services
{
    public interface IUnturnedItemsIconService
    {
        Task<byte[]> GetIconAsync(int itemId);
        Task UpdateIconAsync(int itemId, byte[] icon);
    }
}