var builder = DistributedApplication.CreateBuilder(args);

var pollApiService = builder.AddProject<Projects.VoteOnIt_PollService>("pollApiService");

builder.AddProject<Projects.VoteOnIt_BlazorFrontEnd>("blazorFrontEnd")
       .WithReference(pollApiService);

builder.Build().Run();
