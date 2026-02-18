namespace SerialDataForward
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);


            builder.Services.AddWindowsService();

            builder.Services.Configure<SerialSettings>(
            builder.Configuration.GetSection("SerialSettings"));

            builder.Services.AddHostedService<Worker>();

            var host = builder.Build();
            host.Run();
        }
    }
}
