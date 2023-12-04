using System.Net.Http.Json;
using System.Net;
using VoteOnIt.PollService.Endpoints;
using System.Text.Json.Serialization;
using System.Text.Json;
using VoteOnIt.PollService.Services;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using static System.Net.Mime.MediaTypeNames;

namespace VoteOnIt.PollService.Tests;

public partial class PollServiceTests
{
    [Fact]
    public async Task GetPolls_Empty()
    {
        using var app = CreateAppWithNoPolls();
        var client = app.CreateClient();
        var polls = await client.GetFromJsonAsync<List<PollDto>>("/polls");

        Assert.NotNull(polls);
        Assert.Empty(polls);
    }

    [Fact]
    public async Task GetPolls_InitializedWithPolls()
    {
        using var app = CreateAppWithPolls();
        var client = app.CreateClient();

        var polls = await client.GetFromJsonAsync<List<PollDto>>("/polls");

        Assert.NotNull(polls);
        Assert.Equal(3, polls.Count);
    }

    [Fact]
    public async Task CreateAPoll_ToEmptyDb()
    {
        using var app = CreateAppWithNoPolls();
        var client = app.CreateClient();

        string pollTitle = "Operation Dinner Out!";
        var payload = new
        {
            name = pollTitle,
            ballot = new
            {
                method = "Basic",
                options = new string[] {
                        "Whataburger",
                        "Ruth's Chris",
                        "Jason's Deli"
                    }
            }
        };

        // create the poll
        var response = await client.PostAsJsonAsync("/polls", payload);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        // ensure the correct poll was created at the indicated location
        string location = response.Headers.Single(h => h.Key == "Location").Value.Single();
        var poll = await client.GetFromJsonAsync<PollDto>(location);

        Assert.Equal(pollTitle, poll.Name);
        Assert.Equal("Created", poll.State);
    }

    [Fact]
    public async Task CreateAPoll_WithExistingPolls()
    {

        using var app = CreateAppWithPolls();
        var client = app.CreateClient();

        {
            // check 3 polls exist
            var polls = await client.GetFromJsonAsync<List<PollDto>>("/polls");
            Assert.NotNull(polls);
            Assert.Equal(3, polls.Count);
        }

        {
            // add a new poll
            string pollTitle = "Operation Dinner Out!";
            var payload = new
            {
                name = pollTitle,
                ballot = new
                {
                    method = "Basic",
                    options = new string[] {
                        "Whataburger",
                        "Ruth's Chris",
                        "Jason's Deli"
                    }
                }
            };

            var response = await client.PostAsJsonAsync("/polls", payload);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            // check 4 polls exist
            var polls = await client.GetFromJsonAsync<List<PollDto>>("/polls");
            Assert.NotNull(polls);
            Assert.Equal(4, polls.Count);
        }
    }

    [Fact]
    public async Task UpdateAPoll_ThatDoesNotExist()
    {
        using var app = CreateAppWithPolls();
        var client = app.CreateClient();

        int pollId = 4; // init poll data does not contain ID 4
        {
            // check pollId does not exist
            var result = await client.GetAsync($"/polls/{pollId}");
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }


        {
            // update poll 2
            string json = $$"""
                            {
                                "id": {{pollId}},
                                "name": "Movie Night 2",
                                "ballot": {
                                    "method": "Basic",
                                    "options": [
                                        "Up",
                                        "Toy Story",
                                        "Batman",
                                        "E.T.",
                                        "Star Wars"
                                    ]
                                }
                            }
                            """;
            var response = await client.PutAsync($"/polls/{pollId}", new StringContent(json, new MediaTypeHeaderValue("application/json")));
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }

    [Fact]
    public async Task UpdateAPoll_ThatDoesExist()
    {
        using var app = CreateAppWithPolls();
        var client = app.CreateClient();

        int pollId = 2;
        {
            // check 2 polls exist
            var poll = await client.GetFromJsonAsync<PollDto>($"/polls/{pollId}");
            Assert.Equal("Movie Night", poll.Name);
            Assert.Equal(4, poll.Ballot.Options.Length);
            Assert.Equal("Toy Story", poll.Ballot.Options[0]);
        }

        {
            // update poll 2
            string json = $$"""
                            {
                                "id": {{pollId}},
                                "name": "Movie Night 2",
                                "ballot": {
                                    "method": "Basic",
                                    "options": [
                                        "Up",
                                        "Toy Story",
                                        "Batman",
                                        "E.T.",
                                        "Star Wars"
                                    ]
                                }
                            }
                            """;
            var response = await client.PutAsync($"/polls/{pollId}", new StringContent(json, new MediaTypeHeaderValue("application/json")));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        }

        {
            // check 2 polls has new values
            var poll = await client.GetFromJsonAsync<PollDto>($"/polls/{pollId}");
            Assert.Equal("Movie Night 2", poll.Name);
            Assert.Equal(5, poll.Ballot.Options.Length);
            Assert.Equal("Up", poll.Ballot.Options[0]);
            Assert.Equal("Toy Story", poll.Ballot.Options[1]);
        }
    }

    [Fact]
    public async Task DeleteAPoll_ThatDoesNotExist()
    {
        using var app = CreateAppWithPolls();
        var client = app.CreateClient();

        int pollId = 4; // init poll data does not contain ID 4
        {
            // check pollId does not exist
            var result = await client.GetAsync($"/polls/{pollId}");
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        {
            // attempt to delete poll pollId
            var result = await client.DeleteAsync($"/polls/{pollId}");
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }
    }

    [Fact]
    public async Task DeleteAPoll_ThatDoesExist()
    {
        using var app = CreateAppWithPolls();
        var client = app.CreateClient();

        int pollId = 3; // init poll data does contain ID 3

        {
            // check pollId does exist
            var result = await client.GetAsync($"/polls/{pollId}");
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        {
            // attempt to delete poll 3
            var result = await client.DeleteAsync($"/polls/{pollId}");
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        {
            // check pollId does not exist
            var result = await client.GetAsync($"/polls/{pollId}");
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }
    }

    private SingleUserPollApp CreateAppWithPolls()
    {
        var initialPolls = JsonSerializer.Deserialize<List<Poll>>(threePolls,
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        // initialize with 3 polls
        return new SingleUserPollApp(initialPolls);
    }

    private SingleUserPollApp CreateAppWithNoPolls()
    {
        return new SingleUserPollApp();
    }

    string threePolls = """
    [
        {
          "id": 1,
          "name": "Dinner Options",
          "ballotmethod": "Basic",
            "options": [
              "Whataburger",
              "Ruth's Chris",
              "Jason's Deli"
            ],
          "state": "Created",
          "owner": 0
        },
        {
          "id": 2,
          "name": "Movie Night",
          "ballotmethod": "Basic",
            "options": [
              "Toy Story",
              "Batman",
              "E.T.",
              "Star Wars"
            ],
          "state": "Created",
          "owner": 0
        },
        {
          "id": 3,
          "name": "Game Night",
          "ballotmethod": "Basic",
            "options": [
              "Monopoly",
              "Risk",
              "Settlers of Catan",
              "Carcassonne"
            ],
          "state": "Created",
          "owner": 0
        }
    ]
    """;
}