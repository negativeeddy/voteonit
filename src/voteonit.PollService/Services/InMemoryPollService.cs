using Microsoft.AspNetCore.Mvc;

namespace VoteOnIt.PollService.Services;

internal class InMemoryPollService : IPollService
{
    int nextId = 3;

    readonly Dictionary<int, Poll> _polls;

    public InMemoryPollService(IList<Poll>? initialPolls = null)
    {
        if (initialPolls is null)
        {
            _polls = new Dictionary<int, Poll>();
        }
        else
        {
            _polls = new Dictionary<int, Poll>(
            initialPolls.Select(p => new KeyValuePair<int, Poll>(p.Id, p)));
            nextId = initialPolls.Max(p => p.Id) + 1;
        }
    }

    public Task<Poll> AddPoll(string name, string ballotMethod, string[] ballotOptions, string state, int userId)
    {
        int id = nextId++;
        Poll poll = new Poll(id, name, ballotMethod, ballotOptions, state, userId);
        _polls.Add(id, poll);
        return Task.FromResult(poll);
    }

    public Task<bool> DeletePoll(int pollId, int userId)
    {
        if (_polls.ContainsKey(pollId) && _polls[pollId].OwnerId == userId)
        {
            return Task.FromResult(_polls.Remove(pollId));
        }
        else
        {
            return Task.FromResult(false);
        }
    }

    public Task<IList<Poll>> GetPolls(int userId, IList<int>? ids = null)
    {
        var query = from p in _polls.Values
                    where p.OwnerId == userId
                    select p;

        if (ids is not null)
        {
            query = query.Where(p => ids.Contains(p.Id));
        }

        return Task.FromResult<IList<Poll>>(query.ToList());
    }

    public Task<int> Vote(int pollId, string ballot, int userId)
    {
        throw new NotImplementedException();
    }

    public Task<Poll?> GetPollById(int userId, int pollId)
    {
        if (_polls.ContainsKey(pollId) && _polls[pollId].OwnerId == userId)
        {
            return Task.FromResult<Poll?>(_polls[pollId]);
        }
        else
        {
            return Task.FromResult<Poll?>(null);
        }
    }

    public Task<bool> UpdatePoll(Poll poll, int userId)
    {
        if (_polls.ContainsKey(poll.Id) && _polls[poll.Id].OwnerId == userId)
        {
            _polls[poll.Id] = poll;
            return Task.FromResult(true);
        }
        else
        {
            return Task.FromResult(false);
        }
    }
}
