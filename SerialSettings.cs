using System;
using System.Collections.Generic;
using System.Text;

namespace SerialDataForward
{
    public class SerialSettings
    {
        public SerialPortConfig Input { get; set; } = new();
        public List<SerialPortConfig> Outputs { get; set; } = new();
    }

    public class SerialPortConfig
    {
        public string PortName { get; set; } = "";
        public int BaudRate { get; set; }
        public int DataBits { get; set; }
        public string Parity { get; set; } = "None";
        public string StopBits { get; set; } = "One";
    }

}
