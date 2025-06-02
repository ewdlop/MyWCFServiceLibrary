using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using WcfServiceLibrary;
using System.IO; // For StreamWriter

namespace WcfServiceHost
{
    class Program
    {
        private static string logFilePath = "host_exceptions.log";

        static void LogException(string message, Exception ex = null)
        {
            try
            {
                File.AppendAllText(logFilePath, $"{DateTime.Now}: {message}\n");
                if (ex != null)
                {
                    File.AppendAllText(logFilePath, $"{DateTime.Now}: Exception: {ex.ToString()}\n\n");
                }
                Console.WriteLine(message); // Also write to console
                if (ex != null) Console.WriteLine($"Exception details: {ex.ToString()}");
            }
            catch (Exception logEx)
            {
                Console.WriteLine($"Failed to write to log file: {logEx.Message}");
            }
        }

        static void Main(string[] args) {
            ServiceHost selfHost = null;
            try
            {
                LogException("Service host starting...");
                // Step 1: Create a URI to serve as the base address. Use a different port.
                Uri baseAddress = new Uri("http://localhost:8001/WcfService/");
                LogException($"Base address: {baseAddress}");

                // Step 2: Create a ServiceHost instance.
                selfHost = new ServiceHost(typeof(CalculatorService), baseAddress);
                LogException("ServiceHost instance created.");

                // Step 3: Add a service endpoint.
                selfHost.AddServiceEndpoint(typeof(ICalculator), new WSHttpBinding(), "CalculatorService");
                LogException("Service endpoint added: ICalculator with WSHttpBinding at 'CalculatorService'.");

                // Step 4: Enable metadata exchange.
                ServiceMetadataBehavior smb = selfHost.Description.Behaviors.Find<ServiceMetadataBehavior>();
                if (smb == null)
                {
                    smb = new ServiceMetadataBehavior();
                    selfHost.Description.Behaviors.Add(smb);
                }
                smb.HttpGetEnabled = true;
                // smb.HttpsGetEnabled = false; // Ensure only HTTP if not configured for HTTPS
                LogException("Metadata exchange enabled (HttpGetEnabled=true).");

            // Enable including exception details in faults for debugging
            ServiceDebugBehavior sdb = selfHost.Description.Behaviors.Find<ServiceDebugBehavior>();
            if (sdb == null)
            {
                sdb = new ServiceDebugBehavior();
                selfHost.Description.Behaviors.Add(sdb);
            }
            sdb.IncludeExceptionDetailInFaults = true;
            LogException("ServiceDebugBehavior.IncludeExceptionDetailInFaults enabled.");

                // Step 5: Start the service.
                LogException("Attempting to open ServiceHost...");
                selfHost.Open();
                LogException("ServiceHost opened successfully. The service is ready and should stay running.");
                // Keep the service running until the process is terminated externally.
                // For a real service, use a more robust mechanism like a Windows Service or systemd unit.
                // For this environment, we'll just let the Main thread block.
                System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
            }
            catch (CommunicationException ce) {
                LogException("A CommunicationException occurred during service hosting.", ce);
            }
            catch (TimeoutException te) {
                LogException("A TimeoutException occurred during service hosting.", te);
            }
            catch (InvalidOperationException ioe) {
                LogException("An InvalidOperationException occurred during service hosting.", ioe);
            }
            catch (Exception ex) { // Catch all other exceptions
                LogException("An unexpected exception occurred during service hosting.", ex);
            }
            finally
            {
                if (selfHost != null)
                {
                    try
                    {
                        if (selfHost.State == CommunicationState.Faulted)
                        {
                            LogException("ServiceHost is in a faulted state. Aborting...");
                            selfHost.Abort();
                        }
                        else if (selfHost.State != CommunicationState.Closed)
                        {
                            LogException("Closing ServiceHost...");
                            selfHost.Close();
                            LogException("ServiceHost closed.");
                        }
                    }
                    catch (Exception exClose)
                    {
                        LogException("Exception occurred while closing/aborting ServiceHost.", exClose);
                        if (selfHost.State != CommunicationState.Closed) // Ensure abort if close fails
                        {
                           selfHost.Abort(); // Abort on close failure
                        }
                    }
                }
                LogException("Service host shut down.");
            }
        }
    }
}
