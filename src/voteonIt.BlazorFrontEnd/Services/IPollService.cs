namespace VoteOnIt.BlazorFrontEnd.Services;

public interface IPollService
{
    Task<IList<PollData>> GetPolls();
    Task<PollData?> GetPoll(int id);
    Task UpdatePoll(PollUpdateData pollData);
    public Task<PollData> AddPoll(string name, string BallotMethod, IList<string> Options);
    Task DeletePoll(int id);
}

public record Ballot(string Method, IList<string> Options);
public record PollData(int Id, string Name, Ballot Ballot, string State);
public record PollCreateData(string Name, Ballot Ballot);
public record PollUpdateData(int Id, string Name, Ballot Ballot);
