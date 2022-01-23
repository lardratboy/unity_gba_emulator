namespace GarboDev
{
    //using System;
    //using System.Runtime.InteropServices;

    /// <summary>
    /// A high resolution query performance timer.
    /// </summary>
    public class HighPerformanceTimer
    {

#if false
        #region Imported Methods
        /// <summary>
        /// The current system ticks (count).
        /// </summary>
        /// <param name="lpPerformanceCount">Current performance count of the system.</param>
        /// <returns>False on failure.</returns>
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        /// <summary>
        /// Ticks per second (frequency) that the high performance counter performs.
        /// </summary>
        /// <param name="lpFrequency">Frequency the higher performance counter performs.</param>
        /// <returns>False if the high performance counter is not supported.</returns>
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);
        #endregion
#endif

        #region Member Variables
        private long startTime = 0;
        #endregion

        public HighPerformanceTimer()
        {
        }

        #region Methods
        public void Start()
        {
            // Record when the timer was started.
            this.startTime = HighPerformanceTimer.Counter;
        }
        #endregion

        #region Static Properties
        private static long frequency;

        static HighPerformanceTimer()
        {
            HighPerformanceTimer.frequency = 1000;
            
//            QueryPerformanceFrequency(out HighPerformanceTimer.frequency);
        }

        /// <summary>
        /// Gets the frequency that this HighPerformanceTimer performs at.
        /// </summary>
        public static long Frequency
        {
            get
            {
                return HighPerformanceTimer.frequency;
            }
        }

        /// <summary>
        /// Gets the current system ticks.
        /// </summary>
        public static long Counter
        {
            get
            {
#if true
                //return (long)(UnityEngine.Time.realtimeSinceStartup * 1000);
                return System.DateTime.Now.Millisecond;
#else
                long ticks = 0;
                QueryPerformanceCounter(out ticks);
                return ticks;
#endif
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the tick count of when this HighPerformanceTimer was started.
        /// </summary>
        public long StartTime
        {
            get
            {
                return this.startTime;
            }
        }

        public long Elapsed
        {
            get
            {
                return HighPerformanceTimer.Counter - this.startTime;
            }
        }

        public double ElapsedSeconds
        {
            get
            {
                return ((double)this.Elapsed) / ((double)HighPerformanceTimer.Frequency);
            }
        }
        #endregion
    }
}
