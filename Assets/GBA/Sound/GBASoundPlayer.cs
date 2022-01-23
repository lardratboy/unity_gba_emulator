namespace GarboDev
{
    using System;
    using System.IO;
    using System.Threading;
    //using System.Windows.Forms;
    //using Microsoft.DirectX.DirectSound;

    public delegate void PullAudio(short[] buffer, int length);

    public class GBASoundPlayer : IDisposable
    {
        //private Device soundDevice;
        //private SecondaryBuffer soundBuffer;
        private int samplesPerUpdate;
        private AutoResetEvent[] fillEvent = new AutoResetEvent[2];
        private Thread thread;
        private PullAudio pullAudio;
        private short channels;
        private bool halted;
        private bool running;

        public GBASoundPlayer(/*Control owner, */PullAudio pullAudio, short channels)
        {
            this.channels = channels;
            this.pullAudio = pullAudio;

#if false
            this.soundDevice = new Device();
            this.soundDevice.SetCooperativeLevel(owner, CooperativeLevel.Priority);

            // Set up our wave format to 44,100Hz, with 16 bit resolution
            WaveFormat wf = new WaveFormat();
            wf.FormatTag = WaveFormatTag.Pcm;
            wf.SamplesPerSecond = 44100;
            wf.BitsPerSample = 16;
            wf.Channels = channels;
            wf.BlockAlign = (short)(wf.Channels * wf.BitsPerSample / 8);
            wf.AverageBytesPerSecond = wf.SamplesPerSecond * wf.BlockAlign;
#endif

            this.samplesPerUpdate = 512;

#if false
            // Create a buffer with 2 seconds of sample data
            BufferDescription bufferDesc = new BufferDescription(wf);
            bufferDesc.BufferBytes = this.samplesPerUpdate * wf.BlockAlign * 2;
            bufferDesc.ControlPositionNotify = true;
            bufferDesc.GlobalFocus = true;

            this.soundBuffer = new SecondaryBuffer(bufferDesc, this.soundDevice);

            Notify notify = new Notify(this.soundBuffer);
#endif

            fillEvent[0] = new AutoResetEvent(false);
            fillEvent[1] = new AutoResetEvent(false);

#if false
            // Set up two notification events, one at halfway, and one at the end of the buffer
            BufferPositionNotify[] posNotify = new BufferPositionNotify[2];
            posNotify[0] = new BufferPositionNotify();
            posNotify[0].Offset = bufferDesc.BufferBytes / 2 - 1;
            posNotify[0].EventNotifyHandle = fillEvent[0].SafeWaitHandle.DangerousGetHandle();
            posNotify[1] = new BufferPositionNotify();
            posNotify[1].Offset = bufferDesc.BufferBytes - 1;
            posNotify[1].EventNotifyHandle = fillEvent[1].SafeWaitHandle.DangerousGetHandle();

            notify.SetNotificationPositions(posNotify);
#endif

#if false
            this.thread = new Thread(new ThreadStart(SoundPlayback));
            this.thread.Priority = ThreadPriority.Highest;
#endif

            this.Pause();
            this.running = true;

            this.thread.Start();
        }

        public void Pause()
        {
            if (this.halted) return;

            this.halted = true;

#if false
            this.soundBuffer.Stop();
#endif

            if (null != this.thread)
            {
                Monitor.Enter(this.thread);
            }
        }

        public void Resume()
        {
            if (!this.halted) return;

            this.halted = false;

#if false
            this.soundBuffer.Play(0, BufferPlayFlags.Looping);
#endif

            if (null != this.thread)
            {
                Monitor.Pulse(this.thread);
                Monitor.Exit(this.thread);
            }
        }

        private void SoundPlayback()
        {
            if (null == this.thread)
            {
                return;
            }

            lock (this.thread)
            {
                if (!this.running) return;

                // Set up the initial sound buffer to be the full length
                int bufferLength = this.samplesPerUpdate * 2 * this.channels;
                short[] soundData = new short[bufferLength];

                // Prime it with the first x seconds of data
                this.pullAudio(soundData, soundData.Length);

#if false
                this.soundBuffer.Write(0, soundData, LockFlag.None);

                // Start it playing
                this.soundBuffer.Play(0, BufferPlayFlags.Looping);

                int lastWritten = 0;
#endif

#if false
                while (this.running)
                {
                    if (this.halted)
                    {
                        Monitor.Pulse(this.thread);
                        Monitor.Wait(this.thread);
                    }

                    // Wait on one of the notification events with a 3ms timeout
                    WaitHandle.WaitAny(this.fillEvent, 3, true);

                    // Get the current play position (divide by two because we are using 16 bit samples)
#if false
                    if (this.soundBuffer != null)
                    {
                        int tmp = this.soundBuffer.PlayPosition / 2;

                        // Generate new sounds from lastWritten to tmp in the sound buffer
                        if (tmp == lastWritten)
                        {
                            continue;
                        }
                        else
                        {
                            soundData = new short[(tmp - lastWritten + bufferLength) % bufferLength];
                        }

                        this.pullAudio(soundData, soundData.Length);

                        // Write in the generated data
                        soundBuffer.Write(lastWritten * 2, soundData, LockFlag.None);

                        // Save the position we were at
                        lastWritten = tmp;
                    }
#endif
                }

#endif
            }
        }

        public void Dispose()
        {
            this.running = false;
            this.Resume();

#if false
            if (this.soundBuffer != null)
            {
                this.soundBuffer.Dispose();
                this.soundBuffer = null;
            }
            if (this.soundDevice != null)
            {
                this.soundDevice.Dispose();
                this.soundDevice = null;
            }
#endif

        }
    }
}