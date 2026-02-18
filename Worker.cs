using Microsoft.Extensions.Options;
using System.IO.Ports;

namespace SerialDataForward
{
    public class Worker : BackgroundService
    {

        private readonly ILogger<Worker> _logger;
        private readonly SerialSettings _settings;

        private SerialPort? _inputPort;
        private List<SerialPort> _outputPorts = new();

        public Worker(ILogger<Worker> logger,IOptions<SerialSettings> options)
        {
            _logger = logger;
            _settings = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _inputPort = CreatePort(_settings.Input);
                _inputPort.DataReceived += OnDataReceived;
                _inputPort.Open();

                foreach (var cfg in _settings.Outputs)
                {
                    var port = CreatePort(cfg);
                    port.Open();
                    _outputPorts.Add(port);
                }

                _logger.LogInformation("Serial Forward Service started.");

                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service start failed");
            }
        }
        private SerialPort CreatePort(SerialPortConfig cfg)
        {
            return new SerialPort(
                cfg.PortName,
                cfg.BaudRate,
                Enum.Parse<Parity>(cfg.Parity),
                cfg.DataBits,
                Enum.Parse<StopBits>(cfg.StopBits));
        }

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (_inputPort == null) return;

                string data = _inputPort.ReadExisting();

                foreach (var port in _outputPorts)
                {
                    if (port.IsOpen)
                        port.Write(data);
                }

                _logger.LogInformation("Data: {data}", data.Trim());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Read/Write error");
            }
        }



        public override void Dispose()
        {
            _inputPort?.Dispose();
            foreach (var port in _outputPorts)
                port.Dispose();

            base.Dispose();
        }
    }
}

