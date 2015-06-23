using System;
using UIKit;
using Xamarin;

namespace Dojo
{
    public class Application
    {
        // This is the main entry point of the application.
        private static void Main(string[] args)
        {
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            Insights.HasPendingCrashReport += (sender, isStartupCrash) =>
            {
                if (isStartupCrash)
                {
                    Insights.PurgePendingCrashReports().Wait();
                }
            };
            Insights.Initialize("eeb71246a815e0d405a338047f5a308003a4514d");
            UIApplication.Main(args, null, "AppDelegate");
        }
    }
}
