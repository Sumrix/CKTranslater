using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;

using Timer = System.Timers.Timer;

namespace CKTranslator.Core
{
    /// <summary>
    ///     Таймер вызывающий по очереди подписанные методы.
    ///     <para>
    ///         Используется в <see cref="Web.Queries.Query{TInput,TOutput}" /> Query, что бы
    ///         все запросы вызывались по очереди и с промежутком времени.
    ///     </para>
    /// </summary>
    public class QueueTimer
    {
        private readonly List<EventHandler> eventHandlers;
        private readonly Timer timer;

        /// <summary>
        ///     Инициализирует таймер
        /// </summary>
        /// <param name="intervals">Время между тиками в миллисекундах</param>
        public QueueTimer(double intervals)
        {
            this.eventHandlers = new List<EventHandler>();
            this.timer = new Timer(intervals);
            this.timer.Elapsed += this.Timer_Elapsed;
        }

        public event EventHandler Tick
        {
            add
            {
                lock (this.eventHandlers)
                {
                    this.eventHandlers.Add(value);
                    this.timer.Start();
                }
            }
            remove
            {
                lock (this.eventHandlers)
                {
                    this.eventHandlers.Remove(value);
                }
            }
        }

        /// <summary>
        ///     Ждать моей очереди
        /// </summary>
        public void WaitMyTurn()
        {
            EventWaitHandle handle = new AutoResetEvent(false);
            this.Tick += (_, _) => handle.Set();
            handle.WaitOne();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            EventHandler? eventHandler = null;
            //Console.WriteLine("Tick");

            lock (this.eventHandlers)
            {
                if (this.eventHandlers.Count == 0)
                {
                    this.timer.Stop();
                }
                else
                {
                    eventHandler = this.eventHandlers[0];
                    this.eventHandlers.RemoveAt(0);
                }
            }

            eventHandler?.Invoke(this, EventArgs.Empty);
        }
    }
}