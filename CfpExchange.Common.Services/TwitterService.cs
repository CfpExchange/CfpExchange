﻿using System;
using System.Text;
using System.Threading.Tasks;

using LinqToTwitter;

using CfpExchange.Common.Helpers;
using CfpExchange.Common.Messages;
using CfpExchange.Common.Services.Interfaces;

namespace CfpExchange.Common.Services
{
    public class TwitterService :  ITwitterService
    {
        public async Task SendTweetAsync(SendTweetMessage sendTweetMessage)
        {
            var auth = new SingleUserAuthorizer
            {
                CredentialStore = new SingleUserInMemoryCredentialStore
                {
                    ConsumerKey = SettingsHelper.GetEnvironmentVariable("TwitterConsumerKey"),
                    ConsumerSecret = SettingsHelper.GetEnvironmentVariable("TwitterConsumerSecret"),
                    OAuthToken = SettingsHelper.GetEnvironmentVariable("TwitterOAuthToken"),
                    OAuthTokenSecret = SettingsHelper.GetEnvironmentVariable("TwitterOAuthTokenSecret")
                }
            };

            await auth.AuthorizeAsync();
            var ctx = new TwitterContext(auth);
            var tweetMessage = BuildTweet(sendTweetMessage);

            if (sendTweetMessage.EventLocationLatitude > 0 && sendTweetMessage.EventLocationLongitude > 0)
                await ctx.TweetAsync(tweetMessage, sendTweetMessage.EventLocationLatitude, sendTweetMessage.EventLocationLongitude, true);
            else
                await ctx.TweetAsync(tweetMessage);
        }

        #region Private methods

        private static string BuildTweet(SendTweetMessage sendTweetMessage)
        {
            var tweetMessageBuilder = new StringBuilder();

            tweetMessageBuilder.Append($"\U0001F4E2 New CFP: {sendTweetMessage.EventName}");
            if (!string.IsNullOrWhiteSpace(sendTweetMessage.TwitterHandle))
            {
                var twitterHandle = sendTweetMessage.TwitterHandle;
                if (!twitterHandle.StartsWith('@'))
                {
                    twitterHandle = $"@{twitterHandle}";
                }
                tweetMessageBuilder.Append($" ({twitterHandle})");
            }
            tweetMessageBuilder.AppendLine();
            tweetMessageBuilder.AppendLine($"\U000023F3 Closes: {sendTweetMessage.CfpEndDate.ToLongDateString()}");
            if (sendTweetMessage.EventStartDate != default)
            {
                tweetMessageBuilder.Append($"\U0001F5D3 Event: {sendTweetMessage.EventStartDate:MMM dd}");
                if (sendTweetMessage.EventEndDate.Date != default && sendTweetMessage.EventStartDate.Date != sendTweetMessage.EventEndDate.Date)
                {
                    tweetMessageBuilder.Append($"- { sendTweetMessage.EventEndDate:MMM dd}");
                }
            }
            tweetMessageBuilder.AppendLine();
            tweetMessageBuilder.AppendLine($"#cfp #cfpexchange {sendTweetMessage.UrlToCfp}");

            return tweetMessageBuilder.ToString(0, Math.Min(tweetMessageBuilder.Length, 280));
        }

        #endregion
    }
}
