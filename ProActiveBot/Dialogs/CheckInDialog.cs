using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProActiveBot.Bot.Constants;
using ProActiveBot.Bot.Helpers;
using ProActiveBot.Bot.Models;
using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ProActiveBot.Bot.Dialogs
{
    public class CheckInDialog : ComponentDialog
    {
        protected readonly ILogger Logger;
        private readonly IConfiguration _configuration;
        private readonly ConcurrentDictionary<string, ConversationReference> _conversationReferences;
        public CheckInDialog(IConfiguration configuration, ILogger<CheckInDialog> logger, ConcurrentDictionary<string, ConversationReference> conversationReferences)
        {
            Logger = logger;
            _configuration = configuration;
            _conversationReferences = conversationReferences;

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
            if (value != null)
            {
                var asJobject = JObject.FromObject(value);

                var cardValue = asJobject.ToObject<CheckinValue>();
                IMessageActivity reply = null;
                switch (cardValue?.ButtonType)
                {
                    case ButtonIds.Teams:
                        reply = MessageFactory.Attachment(CardHelper.GetCheckInCard());
                        await stepContext.Context.SendActivityAsync(reply, cancellationToken);
                        break;
                    case ButtonIds.Submit:
                        cardValue.User = GetUser(stepContext?.Context?.Activity);
                        reply = PostDataToDB(cardValue);
                        break;
                    default:
                        break;
                }
                if (reply != null) await stepContext.Context.SendActivityAsync(reply, cancellationToken);
            }
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
        private User GetUser(Activity activity)
        {
            var conversationReference = activity.GetConversationReference();
            return new() { Id = conversationReference.User.AadObjectId, Name = conversationReference.User.Name };
        }
        private IMessageActivity PostDataToDB(CheckinValue checkInData)
        {
            DayOfWeek day = DateTime.Now.DayOfWeek;
            if ((day == DayOfWeek.Saturday) || (day == DayOfWeek.Sunday))
            {
                return MessageFactory.Attachment(CardHelper.SimpleTextCard(StringConstants.CheckedInTimeOut));
            }
            if (CheckIfCheckedIn(checkInData.User.Id))
            {
                HttpContent dataToPost = GetCheckinFormData(checkInData);
                var response = HTTPRequestHelper.Post(_configuration.GetValue<string>("ApiUrl"), dataToPost);
                if (response.IsSuccessStatusCode)
                {
                    return MessageFactory.Attachment(CardHelper.SimpleTextCard(StringConstants.Completed));
                }
                else return MessageFactory.Attachment(CardHelper.SimpleTextCard(StringConstants.PostDataFailure));

            }
            else
            {
                return MessageFactory.Attachment(CardHelper.SimpleTextCard(StringConstants.AlreadyCheckedIn));
            }
            
        }
        private HttpContent GetCheckinFormData(CheckinValue checkInData)
        {
            checkInData.ButtonType = null;
            var json = JsonConvert.SerializeObject(checkInData, Formatting.Indented);
            return new StringContent(json);
        }

        //CHECK THIS IN OLD
        private bool CheckIfCheckedIn(string userId)
        {
            return false;
        }
    }
}
