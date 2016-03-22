using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace SignalRExample
{
    public class DataHub : Hub
    {
        public void Request()
        {
            var ticks = Observable
                .Interval(TimeSpan.FromMilliseconds(500));

            ticks.Subscribe((tick) =>
            {
                Clients.All.sendData(tick);
            });



    
        }
    }
}