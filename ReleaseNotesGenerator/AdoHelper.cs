using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace ReleaseNotesGenerator
{
    public class AdoHelper
    {
        private readonly string _adoUrl;
        private readonly string _adoUsername;
        private readonly string _personalAccessToken;
        private readonly string _repositoryId;

        public AdoHelper(string adoUrl, string adoUsername, string personalAccessToken, string repositoryId)
        {
            _adoUrl = adoUrl;
            _adoUsername = adoUsername;
            _personalAccessToken = personalAccessToken;
            _repositoryId = repositoryId;
        }

        public async Task<List<WorkItem>> GetWorkItemsFromPrsAsync(List<int> prIds)
        {
            var credentials = new VssBasicCredential(_adoUsername, _personalAccessToken);
            var connection = new VssConnection(new Uri(_adoUrl), credentials);
            var workItemTrackingClient = await connection.GetClientAsync<WorkItemTrackingHttpClient>();
            var gitClient = await connection.GetClientAsync<GitHttpClient>();

            var workItems = new List<WorkItem>();
            foreach (var prId in prIds)
            {
                var workItemRefs = await gitClient.GetPullRequestWorkItemRefsAsync(_repositoryId, prId);

                if (workItemRefs != null)
                {
                    var workItemIds = workItemRefs.Select(wir => int.Parse(wir.Id)).ToArray();
                    var workItemsBatch = await workItemTrackingClient.GetWorkItemsAsync(workItemIds);
                    workItems.AddRange(workItemsBatch);
                }
            }

            return workItems;
        }

        public async Task<WorkItem?> GetParentWorkItemAsync(int workItemId)
        {
            var credentials = new VssBasicCredential(_adoUsername, _personalAccessToken);
            var connection = new VssConnection(new Uri(_adoUrl), credentials);
            var workItemTrackingClient = await connection.GetClientAsync<WorkItemTrackingHttpClient>();

            var workItem = await workItemTrackingClient.GetWorkItemAsync(workItemId, expand: WorkItemExpand.Relations);

            if (workItem.Relations != null)
            {
                var parentRelation = workItem.Relations.FirstOrDefault(r => r.Rel == "System.LinkTypes.Hierarchy-Reverse");
                if (parentRelation != null)
                {
                    var parentId = int.Parse(parentRelation.Url.Split('/').Last());
                    var parentWorkItem = await workItemTrackingClient.GetWorkItemAsync(parentId);
                    return parentWorkItem;
                }
            }

            return default;
        }
    }
}
