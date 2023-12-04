using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Immutable;
using VoteOnIt.PollService.Services;

namespace VoteOnIt.PollService.Endpoints;

internal static class PollInfoEndpoints
{
    internal static void MapPollInfoEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/polls").WithTags("polls");

        group.MapGet("/", GetAllPolls)
        .WithName("GetAllPolls")
        .WithOpenApi()
        .Produces<PollDto[]>();

        group.MapGet("/{id}", GetPoll)
        .WithName("GetPollById")
        .WithOpenApi()
        .Produces<PollDto>();

        group.MapPut("/{id}", UpdatePoll)
        .WithName("UpdatePoll")
        .WithOpenApi();

        group.MapPost("/", CreatePoll)
        .WithName("CreatePoll")
        .WithOpenApi()
        .Produces<PollDto>();

        group.MapDelete("/{id}", DeletePoll)
        .WithName("DeletePoll")
        .WithOpenApi();
    }

    internal static async Task<IResult> GetAllPolls(IPollService svc, IUserProfileService user)
    {
        int userId = user.GetCurrentUser().UserId;

        var dtos = (await svc.GetPolls(userId))
                             .Select(p => PollDto.FromPoll(p));

        return TypedResults.Ok(dtos.ToArray());
    }

    internal static async Task<IResult> GetPoll(int id, IPollService svc, IUserProfileService user)
    {
        int userId = user.GetCurrentUser().UserId;
        var polls = (await svc.GetPolls(userId, new[] { id }));
        if (polls.Count == 0)
        {
            return Results.NotFound();
        }
        else
        {
            PollDto dto = PollDto.FromPoll(polls.First());
            return Results.Ok(dto);
        }
    }

    internal static async Task<IResult> UpdatePoll(int id, PollUpdateDto input, IPollService svc, IUserProfileService user)
    {
        int userId = user.GetCurrentUser().UserId;
        var poll = await svc.GetPollById(userId, id);

        if (poll == null)
        {
            return TypedResults.NotFound();
        }

        var newPoll = poll with
        {
            Name = input.Name,
            BallotMethod = input.Ballot.Method,
            Options = input.Ballot.Options,
        };

        await svc.UpdatePoll(newPoll, userId);

        PollDto dto = PollDto.FromPoll(newPoll);
        return TypedResults.Ok(dto);
    }

    internal static async Task<IResult> CreatePoll(PollUpdateDto newModel, IPollService svc, IUserProfileService user)
    {
        int userId = user.GetCurrentUser().UserId;
        var poll = await svc.AddPoll(newModel.Name, newModel.Ballot.Method, newModel.Ballot.Options, "Created", userId);
        PollDto pollDto = PollDto.FromPoll(poll);
        return TypedResults.Created($"/polls/{pollDto.Id}", pollDto);
    }

    internal static async Task<IResult> DeletePoll(int id, IPollService svc, IUserProfileService user)
    {
        int userId = user.GetCurrentUser().UserId;
        if (await svc.DeletePoll(id, userId))
        {
            return TypedResults.Ok(new { Id = id });
        }
        else
        {
            return TypedResults.NotFound();
        }
    }
}