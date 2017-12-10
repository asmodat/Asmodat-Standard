using System;
using System.Threading;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Diagnostics;

namespace AsmodatStandard.Threading
{
    public partial class AsyncTimer : IDisposable
    {
        private bool disposed = false;
        private static readonly object _locker = new object();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                Stop();
                _syncAction = null;
                _asyncFunction = null;
            }

            disposed = true;
        }
        
        public bool Enabled { get => _cancellationTokenSource?.Token.IsCancellationRequested == false; }
        private int _period = 0;

        private CancellationTokenSource _cancellationTokenSource = null;
        private Action _syncAction;
        private Func<Task> _asyncFunction;
        public AsyncTimer(Expression<Action> syncAction, int period, bool autostart, int startDelay_ms = 0)
        {
            _syncAction = syncAction.Compile();
            _period = period;

            if (autostart)
                Start(startDelay_ms);
        }

        public AsyncTimer(Func<Task> asyncFunction, int period, bool autostart, int startDelay_ms = 0)
        {
            _asyncFunction = asyncFunction;
            _period = period;

            if (autostart)
                Start(startDelay_ms);
        }

        private async Task Peacemaker(int startDelay_ms = 0)
        {
            try
            {
                await Task.Delay(startDelay_ms, _cancellationTokenSource.Token);

                var sw = Stopwatch.StartNew();
                while (Enabled)
                {
                    if (_syncAction != null)
                        await Task.Run(_syncAction, _cancellationTokenSource.Token);
                    else if (_asyncFunction != null)
                        await _asyncFunction();
                    else
                        break;
                    
                    await Task.Delay((int)Math.Max(0, _period - sw.ElapsedMilliseconds), _cancellationTokenSource.Token);
                    sw.Restart();
                }
            }
            finally
            {
                _cancellationTokenSource?.Cancel();
            }
        }
       
        public void Start(int delay_ms = 0)
        {
            if (Enabled)
                return;
            
            _cancellationTokenSource = new CancellationTokenSource();
            Task.Run(() => Peacemaker(delay_ms), _cancellationTokenSource.Token);
        }
        
        public void Stop()
        {
            if (!Enabled)
                return;

            _cancellationTokenSource?.Cancel();
        }
    }
}