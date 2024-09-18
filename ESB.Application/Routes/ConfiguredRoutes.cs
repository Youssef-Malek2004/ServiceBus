
namespace ESB.Application.Routes
{
    public sealed class ConfiguredRoutes 
    {
        public List<EsbRoute>? Routes { get; set; }
        public RoutesSettings? DefaultSettings { get; set; }
    }
}