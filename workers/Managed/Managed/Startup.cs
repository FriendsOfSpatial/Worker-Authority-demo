using System;
using System.Reflection;
using System.Xml.Serialization;
using Improbable.Worker;

namespace Managed
{
    internal class Startup
    {
        private const string WorkerType = "Managed";

        public const string LoggerName = "Startup.cs";

        private const int ErrorExitStatus = 1;

        private const uint MaxFrameMiliseconds = 50;

        private static int Main(string[] args)
        {
            if (args.Length != 4) {
                PrintUsage();
                return ErrorExitStatus;
            }

            // Avoid missing component errors because no components are directly used in this project
            // and the GeneratedCode assembly is not loaded but it should be
            Assembly.Load("GeneratedCode");

            var connectionParameters = new ConnectionParameters
            {
                WorkerType = WorkerType,
                Network =
                {
                    ConnectionType = NetworkConnectionType.Tcp
                }
            };

            using (var connection = ConnectWithReceptionist(args[1], Convert.ToUInt16(args[2]), args[3], connectionParameters))
            using (var dispatcher = new Dispatcher())
            {
                var isConnected = true;

                dispatcher.OnDisconnect(op =>
                {
                    Console.Error.WriteLine("[disconnect] " + op.Reason);
                    isConnected = false;
                });

                dispatcher.OnLogMessage(op =>
                {
                    connection.SendLogMessage(op.Level, LoggerName, op.Message);
                    if (op.Level == LogLevel.Fatal)
                    {
                        Console.Error.WriteLine("Fatal error: " + op.Message);
                        Environment.Exit(ErrorExitStatus);
                    }
                });

                uint colorInt = uint.Parse(connection.GetWorkerId().Substring(connection.GetWorkerId().Length - 1));
                colorInt = (colorInt % 2) + 5;
                TurretProcessor processor = new TurretProcessor(colorInt, connection);

                dispatcher.OnAuthorityChange<Improbable.Demo.TurretInfo>(processor.OnTurretInfoComponentAuthorityChanged);
                dispatcher.OnAddComponent<Improbable.Demo.TurretInfo>(processor.OnTurretInfoComponentAdded);
                dispatcher.OnRemoveComponent<Improbable.Demo.TurretInfo>(processor.OnTurretInfoComponentRemoved);
                dispatcher.OnComponentUpdate<Improbable.Demo.TurretInfo>(processor.OnTurretInfoComponentUpdated);

                dispatcher.OnComponentUpdate<Improbable.Demo.CheckOutColor>(processor.OnCheckOutComponentUpdated);

                dispatcher.OnCommandResponse<Improbable.Demo.CheckOutColor.Commands.SendColorId>(processor.OnCommandSendColorIdResponse);
                dispatcher.OnCommandResponse<Improbable.Demo.CheckOutColor.Commands.SendAndUpdateColorId>(processor.OnCommandSendAndUpdateColorIdResponse);

                var maxWait = System.TimeSpan.FromMilliseconds(MaxFrameMiliseconds);
                var stopwatch = new System.Diagnostics.Stopwatch();
                while (isConnected)
                {
                    stopwatch.Reset();
                    stopwatch.Start();

                    var opList = connection.GetOpList(0);
                    dispatcher.Process(opList);

                    processor.Tick(MaxFrameMiliseconds);

                    stopwatch.Stop();
                    var waitFor = maxWait.Subtract(stopwatch.Elapsed);
                    System.Threading.Thread.Sleep(waitFor.Milliseconds > 0 ? waitFor : System.TimeSpan.Zero);
                }
            }

            // This means we forcefully disconnected
            return ErrorExitStatus;
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Usage: mono Managed.exe receptionist <hostname> <port> <worker_id>");
            Console.WriteLine("Connects to SpatialOS");
            Console.WriteLine("    <hostname>      - hostname of the receptionist to connect to.");
            Console.WriteLine("    <port>          - port to use");
            Console.WriteLine("    <worker_id>     - name of the worker assigned by SpatialOS.");
        }

        private static Connection ConnectWithReceptionist(string hostname, ushort port,
            string workerId, ConnectionParameters connectionParameters)
        {
            Connection connection;

            // You might want to change this to true or expose it as a command-line option
            // if using `spatial cloud connect external` for debugging
            connectionParameters.Network.UseExternalIp = false;

            using (var future = Connection.ConnectAsync(hostname, port, workerId, connectionParameters))
            {
                connection = future.Get();
            }

            connection.SendLogMessage(LogLevel.Info, LoggerName, "Successfully connected using the Receptionist");

            return connection;
        }
    }
}