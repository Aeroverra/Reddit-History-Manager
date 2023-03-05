using Microsoft.AspNetCore.Components;
using Reddit.Controllers;
using RedditHistoryManager.Services;

namespace RedditHistoryManager.Pages
{
    public partial class Index : ComponentBase, IAsyncDisposable
    {
        [Inject] public RedditHistoryService RedditHistory { get; set; } = null!;

        private List<string> KnownSubreddits => RedditHistory.LoadedComments.ToList().Where(x=>x != null).Select(x => x.Subreddit).Distinct().OrderBy(x => x).ToList();

        private CancellationTokenSource TokenSource = new CancellationTokenSource();
        private bool IsLoading = false;

        /// <summary>
        /// Time, UpVotes
        /// </summary>
        private string OrderBy = "Time";

        private string SubReddit = "All";

        private string Search = "";

        private IReadOnlyList<Comment> GetFilteredComments()
        {
            var commentsQuery = RedditHistory.LoadedComments.ToList().AsQueryable();
           
            if (OrderBy == "Time")
            {
                commentsQuery = commentsQuery.OrderByDescending(x => x.Created);
            }
            else if (OrderBy == "UpVotes")
            {
                commentsQuery = commentsQuery.OrderByDescending(x => x.UpVotes);
            }

            if (SubReddit != "All")
            {
                commentsQuery = commentsQuery.Where(x => x.Subreddit == SubReddit);
            }

            if (Search != "")
            {
                commentsQuery = commentsQuery.Where(x => x.Body.ToLower().Contains(Search.ToLower()));
            }

            return commentsQuery.ToList();

        }

        private async Task DeleteComment(Comment comment)
        {
            await Task.Run(async () => await RedditHistory.DeleteCommentAsync(comment));
        }

        private async Task OnLoadCicked()
        {
            IsLoading = true;
            await Task.Run(async () => await RedditHistory.LoadCommentsAsync(TokenSource.Token, OnUpdate));
        }

        private async Task OnUpdate(bool complete)
        {
            if (complete)
            {
                IsLoading = false;
            }
            await InvokeAsync(StateHasChanged);
        }

        private void OnStopLoadClicked()
        {
            Console.WriteLine("stoped");
            IsLoading = false;
            TokenSource.Cancel();
            TokenSource.Dispose();
            TokenSource = new CancellationTokenSource();
        }

        public ValueTask DisposeAsync()
        {
            TokenSource.Cancel();
            TokenSource.Dispose();
            return ValueTask.CompletedTask;
        }
    }
}