namespace ESB.Configurations.Interfaces;

public interface IAdapter<in TRequestMessage, TResponseMessage, TContext> 
{
    void Initialize(); //Initialize basic Configurations
    void Reload(); //Reload Any changes in the Configs
    
    public Task<TResponseMessage> SendMessageAsync(TRequestMessage requestMessage);
    public Task HandleIncomingRequest(TContext context);

    //event EventHandler<AdapterEventArgs> OnStopped;
}