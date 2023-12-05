var builder = DistributedApplication.CreateBuilder(args);

var pollApiService = builder.AddProject<Projects.VoteOnIt_PollService>("poll-api-service");

builder.AddProject<Projects.VoteOnIt_BlazorFrontEnd>("blazor-front-end")
       .WithReference(pollApiService);

builder.Build().Run();
