using ESB.Configurations.AbstractClasses;

namespace ESB.Configurations.Interfaces;

public interface IConfigurator<out TAbstractConfiguration> // -> To be Injected for each object -> Configurator for Engine and such
{
    public TAbstractConfiguration InitializeConfiguration(); //Initialize the Configuration into the Config obj
    public TAbstractConfiguration ReloadConfiguration(); //Reload the Config of the Obj
}