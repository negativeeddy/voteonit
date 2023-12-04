using System.ComponentModel.DataAnnotations;

namespace VoteOnIt.BlazorFrontEnd.Services;

public interface IPollService
{
    Task<IList<PollData>> GetPolls();
    Task<PollData?> GetPoll(int id);
    Task UpdatePoll(PollUpdateData pollData);
    public int AddPoll(PollUpdateData pollData);
}

public record Ballot(string Method, IList<string> Options);

public record PollData(int Id, string Name, Ballot Ballot, string State);
public record PollUpdateData(int Id, string Name, Ballot Ballot);

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
}
