using LibGit2Sharp;
using System.Text.RegularExpressions;

namespace ReleaseNotesGenerator
{
    public static class GitHelper
    {
        public static List<int> GetPrIds(string repoPath, string sinceTag)
        {
            using (var repo = new Repository(repoPath))
            {
                var filter = new CommitFilter
                {
                    IncludeReachableFrom = repo.Branches["main"],
                    ExcludeReachableFrom = repo.Tags[sinceTag]
                };

                var commits = repo.Commits.QueryBy(filter);

                var prIds = new List<int>();
                var prIdPattern = new Regex(@"Merged PR (\d+)");

                foreach (var commit in commits)
                {
                    var match = prIdPattern.Match(commit.MessageShort);
                    if (match.Success && int.TryParse(match.Groups[1].Value, out int prId))
                    {
                        prIds.Add(prId);
                    }
                }

                return prIds;
            }
        }
    }
}
