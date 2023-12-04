namespace VoteOnIt.PollService.Services;

// TODO: fix this. temporary hack just to have a user ID available without hooking up the auth
internal class SingleUserProfileService(UserProfile profile) : IUserProfileService
{
    public UserProfile GetCurrentUser() => profile;
}
