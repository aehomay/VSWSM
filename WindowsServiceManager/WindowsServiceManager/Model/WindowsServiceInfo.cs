using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace WindowsServiceManager.Model
{
    /// <summary>
    /// This class represent windows service information.
    /// </summary>
    public class WindowsServiceInfo
    {
        /// <summary>
        /// Gets or sets the name that identifies the service that this instance references.
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// Gets or sets the name of the computer on which this service resides.
        /// </summary>
        public string MachineName { get; set; }

        /// <summary>
        /// Gets or sets a friendly name for the service.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets the status of the service that is referenced by this instance.
        /// </summary>
        public ServiceControllerStatus Status { get; set; }

        public ServiceController Controller { get; private set; }

        /// <summary>
        /// Gets the type of service that this object references.
        /// </summary>
        public ServiceType ServiceType { get; set; }

        /// <summary>
        /// Gets the handle for the service.
        /// </summary>
        public SafeHandle ServiceHandle { get; set; }

 
    }
}
