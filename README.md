# VSWSM
The VSWSM (Visual Studio Windows Service Manager) is an extension for Visual Studio to manage windows services directly from the inside of Visual Studio IDE. The VSWSM extension allows:

Starting or stopping multiple windows services in one go or one by one.
Searcing for a windows service by its name
Resolving dependencies of a windows service and stoping the dependent services before stopping the selected service
Showing all logged events by the service in the windows event page by double-clicking on the service
Allowing to search an event by the source of the event
Thus, if you stop a service, which has a dependency on other services the VSWSM will take care of all other dependent services and will stop them in respected order. The VSWSM will also bridge the windows event view with windows services, meaning that if a service has failed during its start/stop process by double-clicking on the service you will be redirected to a window that will show all events that the service logged in the Windows Event\Application Events.

For more information please visit the Visual Studio Marketplace
https://marketplace.visualstudio.com/items?itemName=AydinHomay.VSWSM04062020

