namespace Settings
{
    public static class Constants
    {
        public const int ReceiversCount = 6;

        /// <summary>
        /// Direction finding receivers count
        /// </summary>
        public const int DfReceiversCount = 5;

        public const int PhasesDifferencesCount = DfReceiversCount * (DfReceiversCount - 1) / 2;

        public const int BandwidthKhz = 30000;

        public const float SamplesGapKhz = 1f * ReceiverBandwidthKhz / (ReceiverSampleCount - 1);

        /// <summary>
        /// Count of samples in one band that server take from whole receiver's band
        /// </summary>
        public const int BandSampleCount = (BandwidthKhz * (ReceiverSampleCount - 1) - ReceiverSampleCount + 1) / ReceiverBandwidthKhz + 2;

        public const int DirectionsCount = 360;

        public const int DirectionStep = 1;

        public const int LiterCount = 9;

        public const float ReliabilityThreshold = 0.5f;

        /// <summary>
        /// Khz frequency of first sample in the first band
        /// </summary>
        public const int FirstBandMinKhz = 25000;

        public const int MinRadioJamFrequencyKhz = 30000;

        public const float ReceiverMinAmplitude = -130f;

        public const float DefaultThresholdValue = -80f;

        /// <summary>
        /// Real receiver's bandwidth
        /// </summary>
        public const int ReceiverBandwidthKhz = 50000;

        /// <summary>
        /// How much samples do receivers really "see" in one band
        /// </summary>
        public const int ReceiverSampleCount = 16384;

        public const int FhssChannelsCount = 4;

        /// <summary>
        /// intersection factor threshold for two fhss networks equality
        /// </summary>
        public const float FhssIntersectionThreshold = 0.7f;

        /// <summary>
        /// Size of peak that is permanently cut off from all scans in intelligence and fhss jamming modes
        /// </summary>
        public const int CenterPeakSampleWidth = 21;

        /// <summary>
        /// Time needed for fpga to collect one sample from air
        /// </summary>
        private const int FpgaSampleCollectionTimeNs = 20;

        public const int FpgaScanCollectionTimeNs = FpgaSampleCollectionTimeNs * ReceiverSampleCount;
    }
}
