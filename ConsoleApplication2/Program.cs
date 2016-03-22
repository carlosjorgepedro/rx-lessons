using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {
            var ticks = Observable.Interval(TimeSpan.FromMilliseconds(250))
                .ObserveOn(NewThreadScheduler.Default)
                .SubscribeOn(NewThreadScheduler.Default);

            var subscription = ticks.Subscribe(tick => WriteWithColor($"{new string('-', 50)} : {tick}", ConsoleColor.Cyan), () => WriteLine("Terminated tick subscription"));

            var bufferedTicks = ticks.Buffer(TimeSpan.FromSeconds(3), 15, NewThreadScheduler.Default);

            var bufferSubscription = bufferedTicks.Subscribe((tickList) => WriteWithColor($"{new string('*', 100)} : {tickList.Count}", ConsoleColor.DarkYellow), () => WriteLine("Terminated list subscription"));

            ReadKey();
            subscription.Dispose();

            WriteWithColor("stop subscription to ticks", ConsoleColor.Red);

            ReadKey();
            bufferSubscription.Dispose();
            ReadKey();
            WriteLine("End");
        }

        public static void WriteWithColor(string message, ConsoleColor color)
        {
            var originalColor = ForegroundColor;
            ForegroundColor = color;
            WriteLine(message);
            ForegroundColor = originalColor;
        }
    }
}
