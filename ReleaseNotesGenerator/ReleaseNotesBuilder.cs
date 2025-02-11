using System.Text;

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

            var addedWorkItems = new HashSet<int>();

            foreach (var workItem in workItems)
            {
                if (!workItem.Id.HasValue || !addedWorkItems.Add(workItem.Id.Value))
                {
                    continue;
                }
                sb.AppendLine($"- {workItem.Fields["System.WorkItemType"]} {workItem.Id}: {workItem.Fields["System.Title"]}");
            }

            return sb.ToString();
        }
    }
}
