using VoteOnIt.PollService.Services;

namespace VoteOnIt.PollService.Endpoints;

public record struct BallotDto(string Method, string[] Options);

public record struct PollDto(int Id, string Name, BallotDto Ballot, string State)
{
    public static PollDto FromPoll(Poll poll) =>
        new PollDto(poll.Id, poll.Name, new BallotDto(poll.BallotMethod, poll.Options), poll.State);
}

public record struct PollUpdateDto(int Id, string Name, BallotDto Ballot);

public record struct PollCreateDto(string Name, BallotCreateDto Ballot);

public record struct BallotCreateDto(string Method, string[] Options);
