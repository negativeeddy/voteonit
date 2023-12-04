namespace VoteOnIt.PollService.Services;

public interface IUserProfileService
{
    UserProfile GetCurrentUser();
}

public record UserProfile(int UserId);


