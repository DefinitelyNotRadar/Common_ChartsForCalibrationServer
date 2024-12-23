using System;

namespace LightningChartTestsApp
{
    public class Signal
    {
        public float FrequencyKhz { get; }
        public float Direction { get; }
        public float Reliability { get; }
        public float BandwidthKhz { get; }
        public float Amplitude { get; }
        public float StandardDeviation { get; }
        public float DiscardedDirectionsPart { get; }
        public float RelativeSubScanCount { get; }

        public Signal(
            float frequencyKhz, float direction, float reliability,
            float bandwidthKhz, float amplitude, float standardDeviation, float discardedDirectionsPart, float relativeSubScanCount)
        {
            FrequencyKhz = frequencyKhz;
            Direction = direction;
            Reliability = reliability;
            BandwidthKhz = bandwidthKhz;
            Amplitude = amplitude;
            StandardDeviation = standardDeviation;
            DiscardedDirectionsPart = discardedDirectionsPart;
            RelativeSubScanCount = relativeSubScanCount;
        }

        public Signal(string[] args)
        {
            FrequencyKhz = Single.Parse(args[0]);
            Direction = Single.Parse(args[1]);
            Reliability = Single.Parse(args[2]);
            BandwidthKhz = Single.Parse(args[3]);
            Amplitude = Single.Parse(args[4]);
            StandardDeviation = Single.Parse(args[5]);
            DiscardedDirectionsPart = Single.Parse(args[6]);
            RelativeSubScanCount = Single.Parse(args[7]);
        }
    }
}
