using VoteOnIt.PollService.Services;
using VoteOnIt.ServiceDefaults;
using VoteOnIt.PollService.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

builder.Services.AddSingleton<IPollService, InMemoryPollService>();
builder.Services.AddSingleton<IUserProfileService>(new SingleUserProfileService(new UserProfile(0)));

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
//    var svc = app.Services.GetRequiredService<IPollService>();
//    await svc.AddPoll("Movie Night", "Basic", ["Toy Story", "Batman", "E.T.", "Star Wars"], "Created", 0);
//    await svc.AddPoll("Game Night", "Basic", ["Monopoly", "Risk", "Settlers of Catan", "Carcassonne"], "Created", 0);
//    await svc.AddPoll("Dinner Options", "Basic", ["Whataburger", "Ruth's Chris", "Jason's Deli"], "Created", 0);
//}

app.UseExceptionHandler();
app.MapDefaultEndpoints();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPollInfoEndpoints();

app.Map("/", () => Results.Ok("Poll API"));

app.Run();

public partial class Program { }
