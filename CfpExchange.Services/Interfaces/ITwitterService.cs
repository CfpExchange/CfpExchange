using System.Threading.Tasks;

using CfpExchange.Models;

namespace CfpExchange.Services.Interfaces
{
    public interface ITwitterService
    {
        Task SendTweetAsync(SendTweetMessage sendTweetMessage);
    }
}
