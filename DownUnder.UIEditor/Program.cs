using DownUnder.Widgets;
using System;

namespace TheGame
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = new MainWindow())
                game.Run();
        }
    }
#endif
}


//using DownUnder;
//using DownUnder.UI;
//using DownUnder.Widgets;
//using System;
//using System.Collections.Concurrent;
//using System.Diagnostics;
//using System.Threading;
//using System.Threading.Tasks;
//using static DownUnder.UI.DWindow;

//namespace ProjectGame
//{
//#if WINDOWS || LINUX
//    /// <summary>
//    /// http://community.monogame.net/t/multiple-windows/2655/2
//    /// </summary>
//    public static class Program
//    {
//        private static BlockingCollection<DWindow> _windows = new BlockingCollection<DWindow>();
//        private static AutoResetEvent _stay_alive = new AutoResetEvent(false);

//        [STAThread]
//        static void Main()
//        {
//            SpawnWindowFromMain(new MainWindow());

//            // Keep the main UI thread alive until we decide we are done.
//            _stay_alive.WaitOne();
//        }

//        public static void SpawnWindowFromMain(DWindow new_window, DWindow parent = null)
//        {
//            StartSTATask((Action)(() =>
//            {
//                using (DWindow window = new MainWindow(new_window.Layout, null))
//                {
//                    //window.SpawnWindowDelegate = new WindowSpawn(SpawnWindowFromMain);
//                    window.Parent = parent;
//                    _windows.Add(window);
//                    window.Exiting += HandleGameExiting;
//                    window.Run();
//                }
//            }
//            )
//            //, CancellationToken.None
//            //, TaskCreationOptions.None
//            //, TaskScheduler.FromCurrentSynchronizationContext()
//            );
//        }

//        public static Task StartSTATask(Action func)
//        {
//            var tcs = new TaskCompletionSource<object>();
//            var thread = new Thread(() =>
//            {
//                try
//                {
//                    func();
//                    tcs.SetResult(null);
//                }
//                catch (Exception e)
//                {
//                    tcs.SetException(e);
//                }
//            });
//            thread.SetApartmentState(ApartmentState.STA);
//            thread.Start();
//            return tcs.Task;
//        }

//        private static void HandleGameExiting(object sender, EventArgs e)
//        {
//            DWindow window = sender as DWindow;
//            if (window != null)
//            {
//                _windows.TryTake(out DWindow gameToRemove);

//                if (gameToRemove == null)
//                {
//                    Console.WriteLine("Failed to remove closing window.");
//                    return;
//                }

//                // If no more windows exist lets close everything down.
//                if (_windows.Count == 0)
//                    _stay_alive.Set();
//            }
//        }
//    }
//#endif
//}
