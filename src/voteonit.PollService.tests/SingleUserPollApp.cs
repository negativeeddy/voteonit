using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VoteOnIt.PollService.Endpoints;
using VoteOnIt.PollService.Services;

namespace VoteOnIt.PollService.Tests;

class SingleUserPollApp(IList<Poll>? initialPolls = null) : WebApplicationFactory<Program>
{
    public const int UserId = 0;

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddSingleton<IPollService>(new InMemoryPollService(initialPolls));
            services.AddSingleton<IUserProfileService>(new SingleUserProfileService(new UserProfile(UserId)));
        });

        return base.CreateHost(builder);
    }
}