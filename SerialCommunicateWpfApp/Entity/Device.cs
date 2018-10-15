using System;

namespace SerialCommunicateWpfApp.Entity {
    public class Device {
        public int ChildId { get; set; }
        public int AreaCode { get; set; }
        public DateTime DateTime { get; set; }
        public bool CurrentSwitch{ get; set; }
        public int Temperature { get; set; }
        public int Humidity { get; set; }
        public int Illumination  { get; set; }
        public int Dust { get; set; }
    }

    public static class DeviceChildId {
        public const int CURRENT = 0x00;
        public const int ENVIRONMENT = 0x01;
        public const int DUST = 0x02;
    }
}
