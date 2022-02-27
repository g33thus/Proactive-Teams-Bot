using AdaptiveCards;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using ProActiveBot.Bot.Constants;
using System;
using System.Collections.Generic;
using System.IO;

namespace ProActiveBot.Bot.Helpers
{
    public static class CardHelper
    {
        public static Attachment SimpleTextCard(string msg)
        {
            var card = new AdaptiveCard(new AdaptiveSchemaVersion(1, 2))
            {
                Body = new List<AdaptiveElement>()
                    {
                        new AdaptiveTextBlock(){ Text=msg, Weight=AdaptiveTextWeight.Default, Size=AdaptiveTextSize.Default}
                    },

            };

            return new Attachment() { ContentType = AdaptiveCard.ContentType, Content = card };
        }
        public static Attachment CheckInGetIntroCard(string url)
        {
            var card = new AdaptiveCard(new AdaptiveSchemaVersion(1, 2))
            {
                Body = new List<AdaptiveElement>()
                {
                    new AdaptiveTextBlock() { Text = StringConstants.CheckInIntro, Weight = AdaptiveTextWeight.Default, Size = AdaptiveTextSize.Default }
                },
                Actions = new List<AdaptiveAction>()
                {
                    new AdaptiveSubmitAction()
                    {
                        Id = UIConstants.Teams.Id,
                        DataJson =  "{ \"ButtonType\" : \""+UIConstants.Teams.Id+"\" }",
                        Title= UIConstants.Teams.ButtonTitle
                    },
                    new AdaptiveOpenUrlAction
                    {
                        Id = UIConstants.Web.Id,
                        Url=new Uri(url),
                        Title= UIConstants.Web.ButtonTitle
                    }
                }
            };

            return new Attachment() { ContentType = AdaptiveCard.ContentType, Content = card };
        }
        public static Attachment GetCheckInCard()
        {
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
