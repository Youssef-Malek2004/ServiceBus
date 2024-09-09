namespace ESB.Application.Interfaces;

public interface IAuthorizer<in TAuthClient>
{
    public Task AuthenticateAsync(TAuthClient client);   
}