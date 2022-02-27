
namespace ProActiveBot.Bot.Models
{
    public class CheckinValue
    {
        public string ButtonType { get; set; }
        public int Energy { get; set; }
        public int Motivation { get; set; }
        public int Feeling { get; set; }
        public int Work { get; set; }
        public int Mood { get; set; }
        public User User { get; set; }
    }

    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
