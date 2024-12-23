using System;

namespace LightningChartTestsApp
{
    public struct IqScan
    {
        public double[] Amplitudes { get; private set; }
        public double[] Phases { get; private set; }

        public IqScan(byte[] iqBytes)
        {
            Amplitudes = new double[256];
            Phases = new double[256];

            if (iqBytes.Length != 1)
            {
                for (int i = 0; i < Amplitudes.Length; i++)
                {
                    var realBytes = new byte[] { iqBytes[4 * i + 1], iqBytes[4 * i] };
                    var imgBytes = new byte[] { iqBytes[4 * i + 3], iqBytes[4 * i + 2] };
                    var real = BitConverter.ToInt16(realBytes, 0);
                    var imaginary = BitConverter.ToInt16(imgBytes, 0);
                    Amplitudes[i] = Math.Sqrt(real * real + imaginary * imaginary);
                    Phases[i] = Math.Atan(imaginary * 1.0d / real);
                }
            }
        }
    }
}
