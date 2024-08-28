using ESB.Configurations.AbstractClasses;

namespace ESB.Configurations.Routes
{
    public sealed class ConfiguredRoutes 
    {
        public List<EsbRoute>? Routes { get; set; }
        public RoutesSettings? DefaultSettings { get; set; }
    }
}