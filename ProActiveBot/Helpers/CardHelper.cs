using AdaptiveCards;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ProActiveBot.Bot.Helpers
{
    public static class CardHelper
    {
        public static Attachment FirstTimeGreetingCard()
        {
            var card = new AdaptiveCard(new AdaptiveSchemaVersion(1, 2))
            {
                Body = new List<AdaptiveElement>()
                    {
                        new AdaptiveTextBlock(){ Text="Hi, there! Welcome!", Weight=AdaptiveTextWeight.Default, Size=AdaptiveTextSize.Normal}
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
                    new AdaptiveTextBlock() { Text = "How are you gettin on today? Take a second to check in.", Weight = AdaptiveTextWeight.Default, Size = AdaptiveTextSize.Normal }
                },
                Actions = new List<AdaptiveAction>()
                {
                    new AdaptiveSubmitAction()
                    {
                        Id = UIConstants.Teams.Id,
                        DataJson =  "{ \"Data\" : \""+UIConstants.Teams.Id+"\" }",
                        Title= UIConstants.Teams.Title,
                    },
                    new AdaptiveOpenUrlAction
                    {
                        Id = UIConstants.Web.Id,
                        Url=new Uri(url),
                        Title= UIConstants.Web.Title,
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
