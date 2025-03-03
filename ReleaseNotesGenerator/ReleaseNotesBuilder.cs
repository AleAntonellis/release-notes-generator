using System.Text;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace ReleaseNotesGenerator
{
    public class ReleaseNotesBuilder
    {
        private readonly AdoHelper _adoHelper;

        public ReleaseNotesBuilder(AdoHelper adoHelper)
        {
            ArgumentNullException.ThrowIfNull(adoHelper);

            _adoHelper = adoHelper;
        }

        public async Task<string> BuildAsync(string repoPath, string sinceTag)
        {
            var prIds = GitHelper.GetPrIds(repoPath, sinceTag);
            var workItems = await _adoHelper.GetWorkItemsFromPrsAsync(prIds);

            var sb = new StringBuilder();
            sb.AppendLine("## Release Notes");
            sb.AppendLine("### Linked Work Items");

            var workItemsByParent = new Dictionary<WorkItem, List<WorkItem>>();
            var withoutParentWorkItems = new List<WorkItem>();

            foreach (var workItem in workItems)
            {
                if (!workItem.Id.HasValue)
                    continue;

                var parent = await _adoHelper.GetParentWorkItemAsync(workItem.Id.Value);

                if (parent == null)
                {
                    if (!withoutParentWorkItems.Any(w => w.Id == workItem.Id))
                        withoutParentWorkItems.Add(workItem);
                    continue;
                }

                if (workItemsByParent.TryGetValue(parent, out var userStories))
                {
                    if (!userStories.Any(w => w.Id == workItem.Id))
                        userStories.Add(workItem);
                }
                else
                {
                    workItemsByParent[parent] = [workItem];
                }
            }

            foreach (var parent in workItemsByParent)
            {
                sb.AppendLine($"- {parent.Key.Fields["System.WorkItemType"]} {parent.Key.Id}: {parent.Key.Fields["System.Title"]}");
                foreach (var workItem in parent.Value)
                {
                    sb.AppendLine($"  - {workItem.Fields["System.WorkItemType"]} {workItem.Id}: {workItem.Fields["System.Title"]}");
                }
            }

            sb.AppendLine("### Work Items Without Parent");

            foreach (var workItem in withoutParentWorkItems)
            {
                sb.AppendLine($"- {workItem.Fields["System.WorkItemType"]} {workItem.Id}: {workItem.Fields["System.Title"]}");
            }

            return sb.ToString();
        }
    }
}
