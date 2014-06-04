// ------------------------------------------------------------------------------
// -- Copyright ERTMS Solutions
// -- Licensed under the EUPL V.1.1
// -- http://joinup.ec.europa.eu/software/page/eupl/licence-eupl
// --
// -- This file is part of ERTMSFormalSpec software and documentation
// --
// --  ERTMSFormalSpec is free software: you can redistribute it and/or modify
// --  it under the terms of the EUPL General Public License, v.1.1
// --
// -- ERTMSFormalSpec is distributed in the hope that it will be useful,
// -- but WITHOUT ANY WARRANTY; without even the implied warranty of
// -- MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// --
// ------------------------------------------------------------------------------
using System;
using System.Configuration;
using System.IO;
using System.Windows.Forms;
using System.ServiceModel;
using System.ServiceModel.Description;
using GUI.IPCInterface;
using DataDictionary;
using System.Threading;

namespace ERTMSFormalSpecs
{
    public static class ErtmsFormalSpecGui
    {
        /// <summary>
        /// The EFS IPC service
        /// </summary>
        private static ServiceHost host = null;

        /// <summary>
        /// Hosts the EFS IPC service
        /// </summary>
        /// <returns>The hosting service</returns>
        private static void HostEFSService()
        {
            host = new ServiceHost(EFSService.INSTANCE);
            try
            {
                host.Open();
            }
            catch (CommunicationException exception)
            {
                Console.WriteLine("An exception occurred: {0}", exception.Message);
                host.Abort();
            }
        }

        /// <summary>
        /// Closes the EFS IPC host service
        /// </summary>
        private static void CloseEFSService()
        {
            if (host != null)
            {
                // Close the ServiceHostBase to shutdown the service.
                host.Close();
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                log4net.Config.XmlConfigurator.Configure(new FileInfo("logconfig.xml"));

                System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (config.AppSettings != null && config.AppSettings.Settings != null)
                {
                    foreach (KeyValueConfigurationElement keyValue in config.AppSettings.Settings)
                    {
                        if (keyValue.Key == "LockOpenedFiles")
                        {
                            DataDictionary.Util.PleaseLockFiles = !(keyValue.Value.CompareTo("false") == 0);
                        }
                    }
                }

                GUI.MainWindow window = new GUI.MainWindow();

                {
                    // TRICKY SECTION
                    // This thread is mandatory otherwise WCF does not create a new thread to handle the service requests. 
                    // Since the call to Cycle is blocking, creating such threads is mandatory
                    Thread thread = new Thread((ThreadStart)HostEFSService);
                    thread.Start();
                }

                Application.Run(window);
                CloseEFSService();
            }
            finally
            {
                DataDictionary.Util.UnlockAllFiles();
            }


            DataDictionary.EFSSystem.INSTANCE.Stop();
            GUI.SynchronizerList.Stop();
        }
    }
}
