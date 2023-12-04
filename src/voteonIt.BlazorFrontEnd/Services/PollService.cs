namespace VoteOnIt.BlazorFrontEnd.Services;

public class PollServiceApi : IPollService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public PollServiceApi(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public int AddPoll(PollUpdateData pollData)
    {
        throw new NotImplementedException();
    }

    public async Task<IList<PollData>> GetPolls()
    {
        var client = _httpClientFactory.CreateClient(nameof(PollServiceApi));
        var polls = await client.GetFromJsonAsync<PollData[]>("/polls");
        return polls ?? Array.Empty<PollData>();
    }

    public async Task<PollData?> GetPoll(int id)
    {
        var client = _httpClientFactory.CreateClient(nameof(PollServiceApi));
        var poll = await client.GetFromJsonAsync<PollData?>($"/polls/{id}");
        return poll;
    }

    public async Task UpdatePoll(PollUpdateData pollData)
    {
        var client = _httpClientFactory.CreateClient(nameof(PollServiceApi));
        var response = await client.PutAsJsonAsync($"polls/{pollData.Id}", pollData, CancellationToken.None);
        response.EnsureSuccessStatusCode();
    }

    public async Task<PollData> AddPoll(string name, string ballotMethod, IList<string> options)
    {
        PollCreateData poll = new PollCreateData(name, new Ballot(ballotMethod, options));
        var client = _httpClientFactory.CreateClient(nameof(PollServiceApi));
        var response = await client.PostAsJsonAsync("polls", poll, CancellationToken.None);
        response.EnsureSuccessStatusCode();
        var newPoll = await response.Content.ReadFromJsonAsync<PollData>();
        return newPoll;
    }

    public async Task DeletePoll(int id)
    {
        var client = _httpClientFactory.CreateClient(nameof(PollServiceApi));
        var response = await client.DeleteAsync($"polls/{id}", CancellationToken.None);
        response.EnsureSuccessStatusCode();
    }
}
