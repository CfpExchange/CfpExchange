using System.Threading.Tasks;

using CfpExchange.Common.Messages;

namespace CfpExchange.Common.Services.Interfaces
{
    public interface ITwitterService
    {
        Task SendTweetAsync(SendTweetMessage sendTweetMessage);
    }
}
