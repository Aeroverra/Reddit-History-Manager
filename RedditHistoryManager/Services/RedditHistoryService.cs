using Reddit;
using Reddit.Controllers;
using System.Diagnostics.Metrics;

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

            await LoadCommentsForType(token, "new", callback);
            await LoadCommentsForType(token, "top", callback);
            await LoadCommentsForType(token, "hot", callback);
            await LoadCommentsForType(token, "controversial", callback);

            var hidden = _comments.Where(x => x.ScoreHidden);
            var spam = _comments.Where(x => x.Spam);
            var remmoved = _comments.Where(x => x.Removed);
            var collapsed = _comments.Where(x => x.Collapsed);
            var awards = _comments.Where(x => x.Awards.Count > 0);

            if (callback != null)
            {
                await callback.Invoke(true);
            }
        }

        private async Task LoadCommentsForType(CancellationToken token, string sort, Func<bool, Task>? callback)
        {
            string after = "";

            while (token.IsCancellationRequested == false)
            {
                var comments = _redditClient.Account.Me.GetCommentHistory(sort: sort, limit: 100, after: after);

                if (comments.Count == 0) { break; }

                _comments.AddRange(comments.Where(x => _comments.Any(y => y.Fullname == x.Fullname) == false));

                after = comments.Last().Fullname;

                if (callback != null)
                {
                    await callback.Invoke(false);
                }
            }
        }

        /// <summary>
        /// Edits then deletes a comment because supposedly reddit only stores the last version
        /// Also helps personal SEO by taking advantage of those caching websites which listen to the pipeline =D
        /// </summary>
        public async Task DeleteCommentAsync(Comment comment)
        {
            _comments.Remove(comment);
            await comment.EditAsync("Aeroverra - Senior Software Engineer And Security Analyst");
            await comment.DeleteAsync();
        }
    }
}
