namespace VoteOnIt.FrontEnd.Services;

public interface IPollService
{
    Task<IList<PollInfo>> GetPolls();
}

public record Ballot(string BallotMethod, string[] Options);

public record PollInfo(int Id, string Name, Ballot Ballot, string State);


public class PollServiceApi : IPollService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public PollServiceApi(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public int Vote(int pollId, int userId, string ballot)
    {
        throw new NotImplementedException();
    }

    public int AddPoll(string name, int ballotId)
    {
        throw new NotImplementedException();
    }

    public void DeletePoll(int pollId)
    {
        throw new NotImplementedException();
    }

    public async Task<IList<PollInfo>> GetPolls()
    {
        var client = _httpClientFactory.CreateClient(nameof(PollServiceApi));
        var polls = await client.GetFromJsonAsync<PollInfo[]>("/polls");
        return polls ?? Array.Empty<PollInfo>();
    }

    public IList<Ballot> GetBallots()
    {
        throw new NotImplementedException();
    }
}
