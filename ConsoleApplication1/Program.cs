using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using static System.Console;
using static System.Threading.Thread;
namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            WriteLine($"Started on thread {CurrentThread.ManagedThreadId}");
            var numbers =
                GetNumbers(0, 20)
                .ToObservable()
                .ObserveOn(NewThreadScheduler.Default)
                .SubscribeOn(NewThreadScheduler.Default);

            var subscription = numbers.Subscribe(
                number => WriteLine($"{number} on thread {CurrentThread.ManagedThreadId}"),
                () => WriteLine("Terminated"));

            WriteLine("Press key to dispose...");
            ReadKey();

            subscription.Dispose();
            WriteLine("Disposed");
            Console.ReadKey();
        }

        public static IEnumerable<int> GetNumbers(int start, int count)
        {
            for (var i = start; i < count; i++)
            {
                WriteLine($"Generate Number {i} on thread {CurrentThread.ManagedThreadId}");
                Sleep(50);
                yield return i;
            }
        }
    }
}
