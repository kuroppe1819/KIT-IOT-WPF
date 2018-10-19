using System;

namespace SerialCommunicateWpfApp.Entity {
    public class Device {
        public const int EMPTY = 9100;
        public int ChildId { get; set; }
        public int AreaCode { get; set; }
        public DateTime DateTime { get; set; }
        public bool CurrentSwitch { get; set; } = false;
        public int Temperature { get; set; } = EMPTY;
        public int Humidity { get; set; } = EMPTY;
        public int Illumination { get; set; } = EMPTY;
        public int Dust { get; set; } = EMPTY;
    }

    public static class DeviceChildId {
        public const int CURRENT = 0x00;
        public const int ENVIRONMENT = 0x01;
        public const int DUST = 0x02;
    }
}
