using System.ComponentModel;
using System.Runtime.InteropServices;

namespace PingPong.Utils {
    class Timer {

        [DllImport("KERNEL32")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);

        private long start;

        private long stop;

        private readonly long frequency;

        private readonly decimal multiplier = new decimal(1.0e9);

        public Timer() {
            if (QueryPerformanceFrequency(out frequency) == false) {
                // Frequency not supported
                throw new Win32Exception();
            }
        }

        public void Start() {
            QueryPerformanceCounter(out start);
        }

        public void Stop() {
            QueryPerformanceCounter(out stop);
        }

        public double Nanoseconds {
            get {
                return (stop - start) * (double) multiplier / frequency;
            }
        }

        public double Milliseconds {
            get {
                return Nanoseconds / 1000000.0;
            }
        }

        public double Seconds {
            get {
                return Nanoseconds / 1000000000.0;
            }
        }

    }
}
