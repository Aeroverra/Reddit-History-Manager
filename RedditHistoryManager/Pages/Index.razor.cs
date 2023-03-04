using Microsoft.AspNetCore.Components;
using RedditHistoryManager.Services;

namespace RedditHistoryManager.Pages
{
    public partial class Index : ComponentBase, IAsyncDisposable
    {
        [Inject] public RedditHistoryService RedditHistory { get; set; } = null!;

        private CancellationTokenSource TokenSource = new CancellationTokenSource();
        private bool IsLoading = false;

        private async Task OnLoadCicked()
        {
            IsLoading = true;
            await Task.Run(() => RedditHistory.LoadCommentsAsync(TokenSource.Token, OnUpdate));
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