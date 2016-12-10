using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace seyana
{
    // seyana no zunou nannyade
    public class SeyanaBrain
    {
        private static SeyanaBrain SeyanaBrainIns = null;
        public static SeyanaBrain SeyanaBrainFactory
        {
            get
            {
                if (SeyanaBrainIns == null)
                {
                    SeyanaBrainIns = new SeyanaBrain();
                    return SeyanaBrainIns;
                }
                else return SeyanaBrainIns;
            }
        }
        private SeyanaBrain() {
            thinkTask = new Util.TaskManage(run);
            moveTask = new Util.TaskManage(move);
        }

        private const int FPS = 30;

        private Util.TaskManage thinkTask = null;
        private Util.TaskManage moveTask = null;

        private MainWindow syn = null;
        private SeyanaVoice voice = null;
        private SerifuWindow sw = null;
        private ebifry ebi = null;

        enum moveMode
        {
            STAND, EBI, RANDOMWALK, JUMP
        }
        private moveMode nowMoveMode;

        enum qtask
        {
            JUMP
        }
        private Queue<qtask> queue;

        public void init(MainWindow mw, SerifuWindow sw, ebifry ebi)
        {
            syn = mw;
            voice = new SeyanaVoice();
            this.sw = sw;
            this.ebi = ebi;
            moveTask.cancel();
            thinkTask.start();

            nowMoveMode = moveMode.STAND;

            queue = new Queue<qtask>();
        }

        /// <summary>
        /// 思考ルーチン
        /// </summary>
        private void run()
        {
            // ランダムウォークが発生する確率
            double randomWalkThreshold = 0.03;

            // エビフライを食べた後大人しくしている時間(秒)
            int manpukudo = 0;
            int ebisize = 12;

            thinkTask.getToken().ThrowIfCancellationRequested();

            while(true)
            {
                if (thinkTask.cancellationRequest()) thinkTask.getToken().ThrowIfCancellationRequested();

                // queue処理
                #region queue
                if (queue.Count != 0)
                {
                    var top = queue.Dequeue();
                    switch (top)
                    {
                        case qtask.JUMP:
                            {
                                switch(nowMoveMode)
                                {
                                    case moveMode.JUMP:
                                        {
                                            queue.Enqueue(qtask.JUMP);
                                            break;
                                        }
                                    default:
                                        {
                                            nowMoveMode = moveMode.JUMP;
                                            moveTask.start();
                                            break;
                                        }
                                }
                                break;
                            }
                    }
                }
                #endregion

                // エビフライ判定
                // 画面内にエビフライがあればそっちに向かう
                if (ebi.live) {
                    manpukudo = ebisize * FPS;
                    if (nowMoveMode != moveMode.EBI)
                    {
                        nowMoveMode = moveMode.EBI;
                        moveTask.start();
                    }
                }
                // ランダムウォーク判定
                // 移動中でないかつランダムウォーク判定に成功するとランダムウォークが起こる
                else if (moveTask.isCompleted() && Util.rnd.NextDouble() < randomWalkThreshold && manpukudo <= 0)
                {
                    randomWalk();
                }

                if (manpukudo > 0) manpukudo--;


                System.Threading.Thread.Sleep(1000 / FPS);
            }
        }

        private void randomWalk()
        {
            toX = Util.rnd.Next(MainWindow.width, Util.screenwidth - MainWindow.width);
            toY = Util.rnd.Next(MainWindow.height, Util.screenheight - MainWindow.height);
            nowMoveMode = moveMode.RANDOMWALK;

            moveTask.start();
        }

        private int toX, toY;

        /// <summary>
        /// seyana move
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void moveSeyana(int x, int y)
        {
            syn.setPositionInvoke(syn, x, y);
            syn.setPositionInvoke(sw, x, y - 60);
        }
        /// <summary>
        /// 移動ルーチン
        /// </summary>
        private void move()
        {
            moveTask.getToken().ThrowIfCancellationRequested();
            int x = MainWindow.x;
            int y = MainWindow.y;

            double speed = 10;

            double t = 0;

            while(true)
            {
                if (moveTask.cancellationRequest()) return;

                switch(nowMoveMode)
                {
                    case moveMode.STAND:
                        {
                            System.Threading.Thread.Sleep(3000);
                            return;
                        }
                    case moveMode.EBI:
                        {
                            int dx = (ebi.x + ebi.w / 2) - (x + MainWindow.width / 2);
                            int dy = (ebi.y + ebi.h / 2) - (y + MainWindow.height / 2);
                            double dst = Math.Sqrt(dx * dx + dy * dy);
                            if (dst > 10)
                            {
                                double dir = Math.Atan2(dy, dx);
                                x = (int)(x + speed * Math.Cos(dir));
                                y = (int)(y + speed * Math.Sin(dir));

                                if (Math.Cos(dir) > 0) syn.faceRight();
                                else syn.faceLeft();

                                moveSeyana(x, y);
                            }else
                            {
                                dst = Util.rnd.NextDouble() * 60;
                                double dir = Util.rnd.NextDouble() * 2 * Math.PI;
                                x = (int)((ebi.x + ebi.w / 2) + dst * Math.Cos(dir) - MainWindow.width / 2);
                                y = (int)((ebi.y + ebi.h / 2) + dst * Math.Sin(dir) - MainWindow.height / 2);

                                moveSeyana(x, y);
                                ebi.eaten();
                            }

                            if (!ebi.live) nowMoveMode = moveMode.STAND;

                            break;
                        }
                    case moveMode.RANDOMWALK:
                        {
                            double dx = toX - x;
                            double dy = toY - y;
                            double dir = Math.Atan2(dy, dx);
                            double dst = Math.Sqrt(dx * dx + dy * dy);

                            if (dst > 10)
                            {
                                x += (int)(speed * Math.Cos(dir));
                                y += (int)(speed * Math.Sin(dir));

                                if (Math.Cos(dir) > 0) syn.faceRight();
                                else syn.faceLeft();

                                moveSeyana(x, y);
                            }
                            else nowMoveMode = moveMode.STAND;

                            break;
                        }
                    case moveMode.JUMP:
                        {
                            // 重力加速度，ジャンプ力
                            double g = 1.2;
                            double v0 = 20;

                            double dx = 0;
                            double dy = - v0 + g * t;

                            x += (int)dx;
                            y += (int)dy;

                            if (g * t > 2 * v0) nowMoveMode = moveMode.STAND;
                            else moveSeyana(x, y);
                            break;
                        }
                }

                t++;
                System.Threading.Thread.Sleep(1000 / FPS);
            }
        }

        public void clicked()
        {
            queue.Enqueue(qtask.JUMP);
            queue.Enqueue(qtask.JUMP);
            syn.say("ｾﾔﾅｰ");
            voice.playSeyana();
        }
    }
}
