using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.Utility;

namespace PRADA_Vayne.MyInitializer
{
    public static partial class PRADALoader
    {
        public static void ShowNotifications()
        {
            var notify = new Notification("PRADA Vayne", "PRADA Vayne baby " +
                "/n back in force " +
                "/n to carry ur games" +
                "/n myo and THE GUCCI" +
                "/n as always" +
                "/n wish u have fun" +
                "/n and remember" +
                "/n u dont need no luck.....");
            Notifications.Add(notify);
            /*DelayAction.Add(3000, () =>
            {
                Notifications.AddNotification("PRADA Vayne baby", 10000);
                Notifications.AddNotification("back in force", 10000);
                Notifications.AddNotification("to carry ur games", 10000);
                Notifications.AddNotification("myo and THE GUCCI,", 10000);
                Notifications.AddNotification("as always,", 10000);
                Notifications.AddNotification("wish u have fun,", 10000);
                Notifications.AddNotification("and remember,", 10000);
                Notifications.AddNotification("u dont need no luck", 10000);
            });*/
        }
    }
}