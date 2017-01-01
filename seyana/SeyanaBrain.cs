﻿using System;
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

        public const int FPS = 30;
        public static double scale = 1;
        // ランダムウォークが発生する確率
        public static double randomWalkThreshold = 0.02;

        private Util.TaskManage thinkTask = null;
        private Util.TaskManage moveTask = null;

        private MainWindow syn = null;
        private SeyanaVoice voice = null;
        private SerifuWindow sw = null;
        private ebifry ebi = null;
        private Clock clk = null;

        enum moveMode
        {
            STAND, EBI, RANDOMWALK, JUMP, ARABURI
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
            clk = new Clock();
            clk.Show();

            nowMoveMode = moveMode.STAND;

            queue = new Queue<qtask>();

            speed = 8;

            moveSeyana(MainWindow.x, MainWindow.y);
            moveTask.start();
            thinkTask.start();
        }

        /// <summary>
        /// 思考ルーチン
        /// </summary>
        private void run()
        {
            // エビフライを食べた後大人しくしている時間(秒)
            int manpukudo = 0;
            int ebisize = 12;

            thinkTask.getToken().ThrowIfCancellationRequested();

            while(true)
            {
                if (thinkTask.cancellationRequest()) return;

                // queue処理
                #region queue
                if (queue.Count != 0)
                {
                    var top = queue.Peek();
                    switch (top)
                    {
                        case qtask.JUMP:
                            {
                                switch(nowMoveMode)
                                {
                                    case moveMode.JUMP:
                                        {
                                            break;
                                        }
                                    default:
                                        {
                                            queue.Dequeue();
                                            nowMoveMode = moveMode.JUMP;
                                            move_t = 0;
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
                    if (nowMoveMode != moveMode.EBI && nowMoveMode != moveMode.ARABURI)
                    {
                        nowMoveMode = moveMode.EBI;
                    }else
                    {
                        int dx = (MainWindow.x + MainWindow.w / 2) - (ebi.x + ebi.w / 2);
                        int dy = (MainWindow.y + MainWindow.h / 2) - (ebi.y + ebi.h / 2);
                        if (dx * dx + dy * dy < 75 * 75) ebi.eaten();
                    }
                }
                // ランダムウォーク判定
                // 移動中でないかつランダムウォーク判定に成功するとランダムウォークが起こる
                else if (nowMoveMode == moveMode.STAND && Util.rnd.NextDouble() < randomWalkThreshold && manpukudo <= 0)
                {
                    randomWalk();
                }

                if (manpukudo > 0) manpukudo--;

                System.Threading.Thread.Sleep(1000 / FPS);
            }
        }

        private void randomWalk()
        {
            toX = Util.rnd.Next(MainWindow.w, Util.screenwidth - MainWindow.w);
            toY = Util.rnd.Next(MainWindow.h, Util.screenheight - MainWindow.h);
            nowMoveMode = moveMode.RANDOMWALK;
        }

        private int toX, toY;
        private int centX, centY;
        double move_t = 0;
        public double speed { private set; get; }

        /// <summary>
        /// seyana move
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void moveSeyana(int x, int y)
        {
            moveOnlySeyana(x, y);
            syn.setPositionInvoke(sw, x + MainWindow.w / 2 - sw.w / 2, y +MainWindow.h / 4 - sw.h);
            syn.setPositionInvoke(clk, x + MainWindow.w / 2 - clk.w / 2, y + MainWindow.h);
        }
        private void moveOnlySeyana(int x, int y)
        {
            syn.setPositionInvoke(syn, x, y);
        }
        /// <summary>
        /// 移動ルーチン
        /// </summary>
        private void move()
        {
            moveTask.getToken().ThrowIfCancellationRequested();
            int x = MainWindow.x;
            int y = MainWindow.y;
            
            while(true)
            {
                if (moveTask.cancellationRequest()) return;

                switch (nowMoveMode)
                {
                    case moveMode.STAND:
                        {
                            //System.Threading.Thread.Sleep(Util.rnd.Next(100, 500));
                            syn.faceToCursor();
                            moveSeyana(MainWindow.x, MainWindow.y);
                            break;
                        }
                    case moveMode.EBI:
                        {
                            int dx = (ebi.x + ebi.w / 2) - (x + MainWindow.w / 2);
                            int dy = (ebi.y + ebi.h / 2) - (y + MainWindow.h / 2);
                            double dst = Math.Sqrt(dx * dx + dy * dy);
                            if (dst > 70)
                            {
                                double dir = Math.Atan2(dy, dx);
                                x = (int)(x + speed * Math.Cos(dir));
                                y = (int)(y + speed * Math.Sin(dir));

                                if (Math.Cos(dir) > 0) syn.faceRight();
                                else syn.faceLeft();

                                moveSeyana(x, y);
                            }else
                            {
                                centX = ebi.x + ebi.w / 2;
                                centY = ebi.y + ebi.h / 2;
                                nowMoveMode = moveMode.ARABURI;
                            }

                            sw.hide();
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

                            sw.hide();
                            break;
                        }
                    case moveMode.JUMP:
                        {
                            // 重力加速度，ジャンプ力
                            double g = 1.2;
                            double v0 = 20;

                            double dx = 0;
                            double dy = - v0 + g * move_t;

                            x += (int)dx;
                            y += (int)dy;

                            if (g * move_t > 2 * v0) nowMoveMode = moveMode.STAND;
                            else moveOnlySeyana(x, y);
                            break;
                        }
                    case moveMode.ARABURI:
                        {
                            if (!(centX == ebi.x + ebi.w / 2 && centY == ebi.y + ebi.h / 2))
                            {
                                nowMoveMode = moveMode.EBI;
                                break;
                            }
                            double dst = Util.rnd.NextDouble() * 60;
                            double dir = Util.rnd.NextDouble() * 2 * Math.PI;
                            x = (int)(centX + dst * Math.Cos(dir) - MainWindow.w / 2);
                            y = (int)(centY + dst * Math.Sin(dir) - MainWindow.h / 2);

                            moveOnlySeyana(x, y);

                            if (!ebi.live) nowMoveMode = moveMode.STAND;

                            sw.hide();
                            break;
                        } 
                    default: break;
                } 

                move_t++;
                System.Threading.Thread.Sleep((int)(1000 / FPS / (nowMoveMode ==moveMode.EBI ? 1.7 : 1.0)));
            }
        }

        /// <summary>
        /// 終了時の処理
        /// syn -> brain
        /// </summary>
        public void close()
        {
            thinkTask.cancel();
            moveTask.cancel();
            clk.end();
        }

        /// <summary>
        /// セヤナーがクリックされたときの処理
        /// syn -> brain
        /// </summary>
        public void clicked()
        {
            queue.Enqueue(qtask.JUMP);
            queue.Enqueue(qtask.JUMP);
            syn.say("ｾﾔﾅｰ");
            voice.playSeyana();
        }

        /// <summary>
        /// タイマーをセットするやで
        /// brain -> clock
        /// </summary>
        /// <param name="h">時間</param>
        /// <param name="m">分</param>
        /// <param name="s">秒</param>
        public void setTimer(int h, int m, int s)
        {
            clk.setTimer(h, m, s);
        }
        /// <summary>
        /// タイマー終了時の処理
        /// clock -> brain
        /// </summary>
        public void endTimer()
        {
            syn.say("ｾﾔﾅｰ");
            voice.playSeyana();
            nowMoveMode = moveMode.ARABURI;
        }

        /// <summary>
        /// コンフィグウィンドウを開く命令
        /// syn -> brain
        /// </summary>
        public void openConfig()
        {
            var cw = new ConfigWindow(this);
            cw.ShowDialog();
        }
        /// <summary>
        /// コンフィグウィンドウが閉じられたときの処理
        /// </summary>
        /// <param name="ce">OKが押されたのかキャンセルされたのか</param>
        /// <param name="cw">コンフィグ内容</param>
        public void closeConfig(ConfigWindow.ConfigEvent ce, ConfigWindow cw)
        {
            if(ce == ConfigWindow.ConfigEvent.OKEVENT)
            {
                scale = cw.scale;
                speed = cw.speed;
                randomWalkThreshold = cw.randomWalkThreashold;
                syn.setScale();
                moveSeyana(MainWindow.x, MainWindow.y);
            }

            cw.Close();
        }
    }
}
