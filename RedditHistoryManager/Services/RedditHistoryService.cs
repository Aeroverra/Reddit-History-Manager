using Reddit;
using Reddit.Controllers;

namespace RedditHistoryManager.Services
{
    public class RedditHistoryService
    {
        private readonly string _appId;
        private readonly string _appSecret;
        private readonly string _refreshToken;
        private readonly RedditClient _redditClient;
        private readonly List<Comment> _comments = new List<Comment>();

        public IReadOnlyList<Comment> LoadedComments => _comments;

        public RedditHistoryService(IConfiguration configuration)
        {
            _appId = configuration["Reddit:AppId"] ?? "";
            _appSecret = configuration["Reddit:AppSecret"] ?? "";
            _refreshToken = configuration["Reddit:RefreshToken"] ?? "";

            if (_appId == "" || _appSecret == "" || _refreshToken == "")
            {
                throw new ArgumentNullException("Invalid Reddit Configuration");
            }

            _redditClient = new RedditClient(appId: _appId, appSecret: _appSecret, refreshToken: _refreshToken);
        }

        public async Task LoadCommentsAsync(CancellationToken token, Func<bool, Task>? callback)
        {
            await Task.Delay(1);

            string after = _comments.LastOrDefault()?.Fullname ?? "";

            while (token.IsCancellationRequested == false)
            {
                var comments = _redditClient.Account.Me.GetCommentHistory(sort: "new", limit: 100, after: after);

                if (comments.Count == 0) { break; }

                _comments.AddRange(comments);

                after = _comments.Last().Fullname;

                if (callback != null)
                {
                    await callback.Invoke(false);
                }
            }

            if (callback != null)
            {
                await callback.Invoke(true);
            }
        }

    }
}
