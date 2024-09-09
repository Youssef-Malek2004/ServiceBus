namespace ESB.Application.Interfaces;

public interface IApiClient<in TRequestMessage, TResponseMessage> 
{
    void Initialize();
    Task<TResponseMessage> SendMessageAsync(TRequestMessage requestMessage);
}