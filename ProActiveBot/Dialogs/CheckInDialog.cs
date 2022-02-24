
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using ProActiveBot.Bot.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ProActiveBot.Bot.Services;

namespace ProActiveBot.Bot.Dialogs
{
    public class CheckInDialog : ComponentDialog
    {
        protected readonly ILogger Logger;

        public CheckInDialog(IConfiguration configuration, ILogger<MainDialog> logger)
        {
            Logger = logger;

            AddDialog(new TextPrompt(nameof(TextPrompt)));

            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
               CheckInStepAsync
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }
        private async Task<DialogTurnResult> CheckInStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Context.Activity.Text == "gg")
            {
                var reply = MessageFactory.Attachment(GetCheckInCard());
                await stepContext.Context.SendActivityAsync(reply, cancellationToken);
            }
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

        private static Attachment GetCheckInCard()
        {
            // combine path for cross platform support
            string[] paths = { ".", "Resources", "adaptiveCard.json" };
            var adaptiveCardJson = File.ReadAllText(Path.Combine(paths));

            var adaptiveCardAttachment = new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(adaptiveCardJson),
            };
            return adaptiveCardAttachment;
        }


    }
}
