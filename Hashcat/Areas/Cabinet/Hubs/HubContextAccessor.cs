using Microsoft.AspNetCore.SignalR;

namespace WebHashcat.Areas.Cabinet.Hubs
{
    public class HubContextAccessor
    {
        private readonly IServiceProvider _serviceProvider;

        public HubContextAccessor(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public IHubContext<BalanceHub> GetHubContext()
        {
            var scope = _serviceProvider.CreateScope();
            return scope.ServiceProvider.GetRequiredService<IHubContext<BalanceHub>>();
        }
    }
}