using ProActiveBot.Bot.Models;

namespace ProActiveBot.Bot.Helpers
{
    public static class UIConstants
    {
        public static UISettings Teams { get; set; } =
            new UISettings(1000, 700, "Adaptive Card: Teams", ButtonIds.Teams, "Do on Teams");
        public static UISettings Web { get; set; } =
            new UISettings(510, 450, "Adaptive Card: Web", ButtonIds.Web, "Do on Web");
        public static UISettings Cancel { get; set; } =
            new UISettings(400, 200, "Cancel", ButtonIds.Cancel, "Cancel");
        public static UISettings Submit { get; set; } =
           new UISettings(400, 200, "Submit", ButtonIds.Submit, "Submit");
    }
}
