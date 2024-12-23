using System;
using System.Collections.Generic;
using System.Linq;
using DspDataModel;

namespace LightningChartTestsApp
{
    public class PossibleFhssNetwork
    {
        public float MedianDirection => _directions.Count != 0 ? _directions.ElementAt(_directions.Count() / 2).Value : -1;

        public float StartFrequencyKhz { get; private set; } = float.MaxValue;
        public float EndFrequencyKhz { get; private set; } = float.MinValue;

        public int Count => _signals.Count;

        public IReadOnlyList<Signal> Signals => _signals;

        private readonly SortedList<int, float> _directions;
        private readonly List<Signal> _signals;

        public PossibleFhssNetwork(Signal signal)
        {
            _directions = new SortedList<int, float>(5000);//todo : to constants
            _signals = new List<Signal>(5000);
            AddSignal(signal);
        }

        private int counter = 0;

        public void AddSignal(Signal impulseSignal)
        {
            if (StartFrequencyKhz > impulseSignal.FrequencyKhz)
                StartFrequencyKhz = impulseSignal.FrequencyKhz;

            if (EndFrequencyKhz < impulseSignal.FrequencyKhz)
                EndFrequencyKhz = impulseSignal.FrequencyKhz;

            _directions.Add(counter, impulseSignal.Direction);
            counter++;
            _signals.Add(impulseSignal);
        }
    }

    public static class TestClass
    {
        public static IReadOnlyList<PossibleFhssNetwork> Test(IReadOnlyList<Signal> signals)
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            var list = signals.ToList();
            list.Sort((a, b) => a.FrequencyKhz >= b.FrequencyKhz ? 1 : -1);

            var possibleFhssNetworks = new List<PossibleFhssNetwork>(20);
            var directionThreshold = 15;
            var frequencyThresholdKhz = 2_000;
            var countThreshold = 20;
            foreach (var impulseSignal in list)
            {
                var network = possibleFhssNetworks.FirstOrDefault(n =>
                    GetAngle(n.MedianDirection, impulseSignal.Direction) <= directionThreshold &&
                    Math.Abs(n.EndFrequencyKhz - impulseSignal.FrequencyKhz) <= frequencyThresholdKhz);
                if (network != null)
                {
                    network.AddSignal(impulseSignal);
                }
                else
                {
                    possibleFhssNetworks.Add(new PossibleFhssNetwork(impulseSignal));
                }
            }

            var result = possibleFhssNetworks.Where(n => n.Count > countThreshold).ToList();
            var str = "";
            float c = 0;
            foreach (var n in result)
            {
                str += $"{n.StartFrequencyKhz} - {n.EndFrequencyKhz} : {n.MedianDirection}. Count is {n.Count}\r\n";
                c += n.Count;
            }
            watch.Stop();

            MessageLogger.Warning($"Spent {watch.Elapsed.TotalMilliseconds} ms for that.\r\n" +
                                  $"Found {possibleFhssNetworks.Count} possibilites.\r\n" +
                                  $"Filtered count is: {result.Count}\r\n" +
                                  $"{str}\r\n" +
                                  $"Bad results %:\r\n{100 * (1 - c / signals.Count):F4}%");
            return result;

            float GetAngle(float a, float b)
            {
                var max = Math.Max(a, b);
                var min = Math.Min(a, b);
                var alpha = max - min;
                var betta = 360 - alpha;
                return Math.Min(alpha, betta);
            }
        }
    }
}
