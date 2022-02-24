
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using ProActiveBot.Bot.Models;

namespace ProActiveBot.Bot.Controllers
{
    [Route("api/notify")]
    [ApiController]
    public class NotifyController : ControllerBase
    {
        private readonly IBotFrameworkHttpAdapter _adapter;
        private readonly string _appId;
        private readonly ConcurrentDictionary<string, ConversationReference> _conversationReferences;

        public NotifyController(IBotFrameworkHttpAdapter adapter, IConfiguration configuration, ConcurrentDictionary<string, ConversationReference> conversationReferences)
        {
            _adapter = adapter;
            _conversationReferences = conversationReferences;
            _appId = configuration["MicrosoftAppId"] ?? string.Empty;
        }

        public async Task<IActionResult> Get()
        {
            foreach (var conversationReference in _conversationReferences.Values)
            {
                await ((BotAdapter)_adapter).ContinueConversationAsync(_appId, conversationReference, BotCallback, default);
            }

            // Let the caller know proactive messages have been sent
            return new ContentResult()
            {
                Content = "<html><body><h1>Proactive messages have been sent.</h1></body></html>",
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.OK,
            };
        }

        private async Task BotCallback(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            try
            {
                var reply = MessageFactory.Attachment(GetIntroCard());
                await turnContext.SendActivityAsync(reply, cancellationToken);

            }
            catch (Exception ex)
            {
            }
        }
        private static Attachment GetIntroCard()
        {
            // Create an Adaptive Card with an AdaptiveSubmitAction for each Task Module
            var card = new AdaptiveCard(new AdaptiveSchemaVersion(1, 2))
            {
                Body = new List<AdaptiveElement>()
                    {
                        new AdaptiveTextBlock(){ Text="How are you gettin on today? Take a second to check in.", Weight=AdaptiveTextWeight.Default, Size=AdaptiveTextSize.Normal}
                    },
                Actions = new[] { TaskModuleUIConstants.AdaptiveCard, TaskModuleUIConstants.CustomForm, TaskModuleUIConstants.YouTube }
                            .Select(cardType => new AdaptiveSubmitAction() { Title = cardType.ButtonTitle, Data = new AdaptiveCardTaskFetchValue<string>() { Data = cardType.Id } })
                            .ToList<AdaptiveAction>(),
            };

            return new Attachment() { ContentType = AdaptiveCard.ContentType, Content = card };
        }
        //private async Task MessageAllMembersAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        //{
        //    var teamsChannelId = turnContext.Activity.TeamsGetChannelId();
        //    var serviceUrl = turnContext.Activity.ServiceUrl;
        //    var credentials = new MicrosoftAppCredentials(_appId, _appPassword);
        //    ConversationReference conversationReference = null;

        //    var members = await GetPagedMembers(turnContext, cancellationToken);

        //    foreach (var teamMember in members)
        //    {
        //        var proactiveMessage = MessageFactory.Text($"Hello {teamMember.GivenName} {teamMember.Surname}. I'm a Teams conversation bot.");

        //        var conversationParameters = new ConversationParameters
        //        {
        //            IsGroup = false,
        //            Bot = turnContext.Activity.Recipient,
        //            Members = new ChannelAccount[] { teamMember },
        //            TenantId = turnContext.Activity.Conversation.TenantId,
        //        };

        //        await ((CloudAdapter)turnContext.Adapter).CreateConversationAsync(
        //            credentials.MicrosoftAppId,
        //            teamsChannelId,
        //            serviceUrl,
        //            credentials.OAuthScope,
        //            conversationParameters,
        //            async (t1, c1) =>
        //            {
        //                conversationReference = t1.Activity.GetConversationReference();
        //                await ((CloudAdapter)turnContext.Adapter).ContinueConversationAsync(
        //                    _appId,
        //                    conversationReference,
        //                    async (t2, c2) =>
        //                    {
        //                        await t2.SendActivityAsync(proactiveMessage, c2);
        //                    },
        //                    cancellationToken);
        //            },
        //            cancellationToken);
        //    }

        //    await turnContext.SendActivityAsync(MessageFactory.Text("All messages have been sent."), cancellationToken);
        //}
    }
}
