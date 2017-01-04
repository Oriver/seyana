using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace seyana
{
    public static class Util
    {
        public class TaskManage
        {
            private Task t;
            private CancellationTokenSource cts;
            private Action act;

            public TaskManage(Action act)
            {
                this.act = act;
            }

            public void start()
            {
                cancel();

                cts = new CancellationTokenSource();
                t = Task.Factory.StartNew(act, cts.Token);
            }

            public void cancel()
            {
                if (t == null || t.IsCompleted) return;

                try
                {
                    cts.Cancel();
                    //t.Wait();
                }
                catch (TaskCanceledException) { }
            }

            public bool isCompleted()
            {
                return t == null || t.IsCompleted;
            }

            public bool cancellationRequest()
            {
                return cts.IsCancellationRequested;
            }

            public CancellationToken getToken()
            {
                return cts.Token;
            }
        }

        public static int screenwidth = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width;
        public static int screenheight= System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height;

        public static Random rnd = new Random();

        public struct rect
        {
            public int x { get; }
            public int y { get; }
            public int w { get; }
            public int h { get; }
            public rect(int x, int y, int w, int h)
            {
                this.x = x;
                this.y = y;
                this.w = w;
                this.h = h;
            }
        }

        public static bool isInScreen(rect p)
        {
            return p.x > 0 && p.x + p.w < screenwidth && p.y > 0 && p.y + p.h < screenheight;
        }
    }
}
