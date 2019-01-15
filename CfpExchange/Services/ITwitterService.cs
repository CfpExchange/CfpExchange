using System.Threading.Tasks;
using CfpExchange.Models;

namespace CfpExchange.Services
{
    public interface ITwitterService
    {
        Task PostNewCfpTweet(Cfp cfpToAdd, string urlToCfp);
    }
}