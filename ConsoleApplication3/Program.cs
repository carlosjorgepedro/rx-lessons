using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApplication3
{
    class Program
    {
        static void Main(string[] args)
        {
            var start = DateTime.UtcNow;
            Func<string, dynamic> lineToTrack = line =>
            {
                var array = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                return new
                {
                    Latitude = array[1],
                    Longitude = array[2],
                    Data = array[3],
                    Time = array[4],
                    Altitude = double.Parse(array[5])
                };
            };

            Action<Object, ConsoleColor> printWithColor = (text, color) =>
            {
                var original = Console.ForegroundColor;
                Console.ForegroundColor = color;
                Console.WriteLine($"Thread : {Thread.CurrentThread.ManagedThreadId}\t{text}");
                Console.ForegroundColor = original;
            };

            var files = Directory.GetFiles(@"./Data/")
                .ToObservable()
                .ObserveOn(NewThreadScheduler.Default)
                .SubscribeOn(NewThreadScheduler.Default);


            files.Subscribe(
                name => printWithColor(name, ConsoleColor.DarkBlue),
                err => printWithColor(err.Message, ConsoleColor.Red),
                () => printWithColor("Opened All Files", ConsoleColor.Cyan));

            var fileLines = from f in files
                            select File.ReadLines(f)
                                .ToObservable(NewThreadScheduler.Default)
                                .SubscribeOn(NewThreadScheduler.Default)
                                .SkipWhile(line =>
                                 {
                                     var regex = new Regex(@"^(.*)LATITUDE(\s+)LONGITUDE(\s+)DATE(\s+)TIME(\s+)ALT(.+)$");
                                     return !regex.IsMatch(line);
                                 })
                                .Skip(1);


            var trackpoints = from lines in fileLines
                              from line in lines
                              select lineToTrack(line);



            var handledTP = trackpoints.OnErrorResumeNext(trackpoints);

            handledTP
                .SubscribeOn(NewThreadScheduler.Default)
                .Subscribe(tp => printWithColor(tp, ConsoleColor.White),
                //(err) => printWithColor(err.Message, ConsoleColor.DarkMagenta),
                () => Console.WriteLine($"All Terminaterd after {DateTime.UtcNow - start}"));


            Console.ReadLine();
        }
    }
}
