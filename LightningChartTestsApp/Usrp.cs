using System;
using System.Diagnostics;
using USRPInteropAssembly;

namespace LightningChartTestsApp
{
    public struct Args
    {
        public bool Result { get; private set; }
        public double MillisecondsTaken { get; private set; }

        public Args(bool result, double millisecondsTaken)
        {
            Result = !result;
            MillisecondsTaken = millisecondsTaken;
        }
    }

    public class Usrp
    {
        public event EventHandler<Args> OnResult;
        public event EventHandler<string> OnError;

        private const int Level1 = 0;
        private const int Level2 = 0;
        private const int SampleRate = 100_000_000;
        private const int NumberOfSamples = 1024;//32768;
        private const string Name = "RIO0";

        private NiUsrpRio__32Session _session;
        private Rx__32channels _channels;

        private readonly Stopwatch _controlWatch = new Stopwatch();
        private bool _isInitialized = false;

        public void OpenAndInit()
        {
            try
            {
                if (_isInitialized)
                {
                    throw new ArgumentException("Already initialized");
                }

                _controlWatch.Restart();
                USRPLabVIEWExports.open_and_init2(2400_000_000, 2400_000_000, Level1, Level2, Name, SampleRate, NumberOfSamples,
                    out _session, out _channels, out var result);
                _controlWatch.Stop();
                OnResult?.Invoke(this, new Args(result, _controlWatch.Elapsed.TotalMilliseconds));

                //first session read
                _controlWatch.Restart();
                USRPLabVIEWExports.read_spectrum(_session, true, _channels, out var newSession, out var data, out result);
                _controlWatch.Stop();
                OnResult?.Invoke(this, new Args(result, _controlWatch.Elapsed.TotalMilliseconds));

                _isInitialized = true;
            }
            catch (Exception e)
            {
                OnError?.Invoke(this, $"{e}\r\n{e.StackTrace}");
            }
        }

        public void Close()
        {
            if (!_isInitialized)
            {
                throw new ArgumentException("Try opening usrp before closing it!");
            }
            _controlWatch.Restart();
            USRPLabVIEWExports.close(_session, out var result);
            _controlWatch.Stop();
            OnResult?.Invoke(this, new Args(result, _controlWatch.Elapsed.TotalMilliseconds));
            _isInitialized = false;
        }

        public double[,] Read()
        {
            if (!_isInitialized)
            {
                throw new ArgumentException("Try opening usrp before reading it!");
            }
            _controlWatch.Restart();
            USRPLabVIEWExports.read_spectrum(_session, false, _channels, out var newSession, out var data, out var result);
            _controlWatch.Stop();
            OnResult?.Invoke(this, new Args(result, _controlWatch.Elapsed.TotalMilliseconds));
            return data;
        }

        public void SetFrequency(double frequencyMhz)
        {
            if (!_isInitialized)
            {
                throw new ArgumentException("Try opening usrp before changing it frequency!");
            }
            var frequencyHz = frequencyMhz * 1_000_000;
            _controlWatch.Restart();
            USRPLabVIEWExports.reconfig(_session, SampleRate, NumberOfSamples, frequencyHz, Level1, frequencyHz, Level2, out _session, out var result);
            _controlWatch.Stop();
            OnResult?.Invoke(this, new Args(result, _controlWatch.Elapsed.TotalMilliseconds));
        }
    }
}
