using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

[RunInstaller(true)]
public class ClientInstaller : Installer
{
    private ServiceProcessInstaller processInstaller;
    private ServiceInstaller serviceInstaller;

    public ClientInstaller()
    {
        processInstaller = new ServiceProcessInstaller();
        serviceInstaller = new ServiceInstaller();

        processInstaller.Account = ServiceAccount.LocalSystem;
        serviceInstaller.StartType = ServiceStartMode.Manual;
        serviceInstaller.ServiceName = "Client";

        Installers.Add(serviceInstaller);
        Installers.Add(processInstaller);
    }
}