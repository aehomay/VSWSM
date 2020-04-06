# VSWSM
The VSWSM (Visual Studio Windows Service Manager) is an extension for Visual Studio to manage windows services directly from the inside of Visual Studio IDE. 
The VSWSM extension allows starting/stopping Windows Services all in one go or one by one. If you stop a service, which has a dependency to other services the VSWSM will take care of all other dependent services and will stop them in a respected dependency order. 
The VSWSM will also bridge the Windows Event view windows services. Meaning that if a service has failed during it`s start/stop process by double clicking on the service you will be redirected to a window that will show all events that the service logged in the Windows Event\Application Events.

