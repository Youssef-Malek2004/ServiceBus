namespace ESB.Application.Interfaces;

public interface IAdapter<in TRequestMessage, TResponseMessage, in TAuthorization, in TContext> 
{
    void Initialize(); //Initialize basic Configurations
    void Reload(); //Reload Any changes in the Configs
    
    public Task<TResponseMessage> SendMessageAsync(TRequestMessage requestMessage);
    public Task<string> HandleIncomingRequest(TContext context, TAuthorization authorization); 

    //event EventHandler<AdapterEventArgs> OnStopped;
}