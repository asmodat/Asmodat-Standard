using System;
using System.Threading;
using AsmodatStandard.Extensions.Types;
using System.Threading.Tasks;

namespace AsmodatStandard.Types
{
    public partial class TickTimeout : ICloneable
    {
        private readonly object locker = new object();

        public object Clone()
        {
            TickTimeout obj = new TickTimeout(this.Timeout, this.Unit, this.Enabled);
            obj.Start = this.Start.Copy();
            obj.Forced = this.Forced;
            obj.TiggerSpan = this.TiggerSpan.Copy();
            return obj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Timeout">timeout</param>
        /// <param name="Unit">timeout unit</param>
        /// <param name="Start">timer start time</param>
        /// <param name="Enabled">defines if timeout shoud be enabled</param>
        public TickTimeout(long Timeout, TickTime.Unit Unit, TickTime Start, bool Enabled = true)
        {
            this.Start = Start;
            this.Timeout = Timeout;
            this.Unit = Unit;
            this.Enabled = Enabled;
        }

        /// <summary>
        /// Start time is set to now
        /// </summary>
        /// <param name="Timeout"></param>
        /// <param name="Unit"></param>
        public TickTimeout(long Timeout, TickTime.Unit Unit, bool Enabled)
        {
            this.Start = TickTime.Now;
            this.Timeout = Timeout;
            this.Unit = Unit;
            this.Enabled = Enabled;
        }

        /// <summary>
        /// this constructor enables timeout if it is possible (timeout > 0), else timeout is disabled
        /// </summary>
        /// <param name="Timeout"></param>
        /// <param name="Unit"></param>
        public TickTimeout(long Timeout, TickTime.Unit Unit)
        {
            this.Start = TickTime.Now;
            this.Timeout = Timeout;
            this.Unit = Unit;
            this.Enabled = (Timeout > 0);
        }

        public void SetTimeout(long Timeout) {
            lock (locker)
                this.Timeout = Timeout; 
        }
        public void SetTimeout(long Timeout, TickTime.Unit Unit)
        {
            lock (locker)
            {
                this.Timeout = Timeout;
                this.Unit = Unit;
            }
        }

        public bool Enabled { get; set; } = true;

        private TickTime _TiggerSpan = TickTime.Default;
        public TickTime TiggerSpan { get { return _TiggerSpan; } private set { _TiggerSpan = value; } }

        public bool IsTriggered
        {
            get
            {
                if (!Enabled)
                    return false;

                if (Forced || Start.Timeout(Timeout, Unit))
                {
                    if (TiggerSpan.IsDefault)
                        TiggerSpan = Start.TickSpan.Copy();

                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Returns true and resets timeout if it is triggered, else returns false
        /// </summary>
        /// <returns></returns>
        public bool TryEnter()
        {
            lock (locker)
                if (IsTriggered)
                {
                    this.Reset();
                    return true;
                }

            return false;
        }

        /// <summary>
        /// returns start time span in Unit's ((long)this.Start.TimeSpan / (long)this.Unit;)
        /// </summary>
        public decimal Span  {  get => Start.Span(Unit); }

        /// <summary>
        /// Reset start time to Now, and disables forcing to timeout,
        /// reset does not enable timer if it is disabled, 
        /// for this purpuse use Reset extention method with enable field
        /// </summary>
        public void Reset()
        {
            this.Reset(this.Enabled);
        }

        public void Reset(bool enable)
        {
            _TiggerSpan.SetDefault();
            Forced = false;
            _Start.SetNow();
            this.Enabled = enable;
        }

        /// <summary>
        /// Waits until timeout is triggered
        /// </summary>
        public void Wait(int intensity_ms = 1)
        {
            while (!IsTriggered)
                Thread.Sleep(intensity_ms);
        }

        public async Task WaitAsync(int intensity_ms = 1)
        {
            while (!IsTriggered)
                await Task.Delay(intensity_ms);
        }

        /// <summary>
        /// time in unit untill 
        /// </summary>
        public decimal TimeoutsIn() => this.Timeout - this.Span;

        /// <summary>
        /// Defines if timout result should be forced to true
        /// </summary>
        public bool Forced { get; set; } = false;

        private TickTime _Start = TickTime.Default;
        public TickTime Start { get { return _Start; } private set { _Start = value; } }

        public long Timeout { get; private set; }
        public TickTime.Unit Unit { get; private set; }
    }
}