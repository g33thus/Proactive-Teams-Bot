// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProActiveBot.Bot.Helpers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ProActiveBot.Bot.Bots
{
    // This bot is derived (view DialogBot<T>) from the TeamsACtivityHandler class currently included as part of this sample.

    public class TeamsBot<T> : DialogBot<T> where T : Dialog
    {
        private static IConfiguration _config;
        public TeamsBot(ConversationState conversationState, UserState userState, T dialog, ILogger<DialogBot<T>> logger, IConfiguration config, ConcurrentDictionary<string, ConversationReference> conversationReferences)
            : base(conversationState, userState, dialog, logger, config, conversationReferences)
        {
            _config = config;   
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var teamsMember = await TeamsInfo.GetMemberAsync(turnContext, turnContext.Activity.From.Id, cancellationToken);
            foreach (var member in turnContext.Activity.MembersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    var reply = MessageFactory.Attachment(CardHelper.FirstTimeGreetingCard());
                    await turnContext.SendActivityAsync(reply, cancellationToken);
                    reply = MessageFactory.Attachment(CardHelper.CheckInGetIntroCard(_config.GetValue<string>("BaseUrl")));
                    await turnContext.SendActivityAsync(reply, cancellationToken);
                }
            }
        }

        protected override async Task OnTeamsSigninVerifyStateAsync(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
        {
            Logger.LogInformation("Running dialog with signin/verifystate from an Invoke Activity.");

            // The OAuth Prompt needs to see the Invoke Activity in order to complete the login process.

            // Run the Dialog with the new Invoke Activity.
            await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);
        }
    }
}
