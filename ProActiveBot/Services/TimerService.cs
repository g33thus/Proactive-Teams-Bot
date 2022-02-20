using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProActiveBot.Bot.Services
{
    public class DailyTrigger
    {
        TimeSpan TriggerHour { get; }

        CancellationTokenSource CancellationToken { get; set; }

        Task RunningTask { get; set; }

        public DailyTrigger(int hour, int minute = 0, int second = 0)
        {
            TriggerHour = new TimeSpan(hour, minute, second);
            CancellationToken = new CancellationTokenSource();
            RunningTask = Task.Run(async () =>
            {
                while (true)
                {
                    var triggerTime = DateTime.Today + TriggerHour - DateTime.Now;
                    if (triggerTime < TimeSpan.Zero)
                        triggerTime = triggerTime.Add(new TimeSpan(24, 0, 0));
                    await Task.Delay(triggerTime, CancellationToken.Token);
                    //OnTimeTriggered?.Invoke();
                }
            }, CancellationToken.Token);
        }



    }
}
