namespace VoteOnIt.PollService.Services;

public interface IPollService
{
    Task<IList<Poll>> GetPolls(int userId, IList<int>? ids = null);

    Task<Poll?> GetPollById(int userId, int pollId);

    Task<Poll> AddPoll(string name, string BallotMethod, string[] Options, string State, int userId);

    Task<bool> UpdatePoll(Poll poll, int userId);

    Task<bool> DeletePoll(int pollId, int userId);

    Task<int> Vote(int pollId, string ballotSelection, int userId);
}

public record Poll(int Id, string Name, string BallotMethod, string[] Options, string State, int OwnerId);
