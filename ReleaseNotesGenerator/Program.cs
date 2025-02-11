using ReleaseNotesGenerator;

var repoPath = @"your repository local path";
var sinceTag = "the tag name ex. 1.0.0.0";

var adoUrl = "< the ADO link https://<ado link>/your-organization";
var adoUserName = "your ado username";
var personalAccessToken = "your personal access token";
var repositoryId = "the repository id";

var adoHelper = new AdoHelper(adoUrl, adoUserName, personalAccessToken, repositoryId);
var releaseNotesGenerator = new ReleaseNotesBuilder(adoHelper);

var releaseNotes = await releaseNotesGenerator.BuildAsync(repoPath, sinceTag);
Console.WriteLine(releaseNotes);
