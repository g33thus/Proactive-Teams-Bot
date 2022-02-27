using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ProActiveBot.Bot.Helpers;
using ProActiveBot.Bot.Models;
using System.Threading;
using System.Threading.Tasks;

namespace ProActiveBot.Bot.Dialogs
{
    public class CheckInDialog : LogoutDialog
    {
        protected readonly ILogger Logger;

        public CheckInDialog(IConfiguration configuration, ILogger<CheckInDialog> logger) : base(nameof(CheckInDialog), configuration["ConnectionName"])
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
            var value = stepContext?.Context?.Activity?.Value;
            if (value!=null)
            {
                var asJobject = JObject.FromObject(value);

                var cardValue = asJobject.ToObject<CardTaskFetchValue<string>>()?.Data;
                IMessageActivity reply = null;
                switch (cardValue)
                {
                    case ButtonIds.Teams:
                        reply = MessageFactory.Attachment(CardHelper.GetCheckInCard());
                        await stepContext.Context.SendActivityAsync(reply, cancellationToken);
                        break;
                    case ButtonIds.Submit:
                        reply = MessageFactory.Attachment(CardHelper.GetCheckInCard());

                        break;
                    default:
                        break;
                }
                if (reply != null) await stepContext.Context.SendActivityAsync(reply, cancellationToken);
            }
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }
}
