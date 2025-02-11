# Git & Ado Release Notes Generator

This is a console application written in C# that generates release notes from Git history and Azure DevOps (ADO) pull request details. It identifies the pull requests merged into the main branch since the last release and lists the ADO work items linked to those pull requests

## Features

- Fetches commit history from a Git repository.
- Extracts pull request IDs from commit messages.
- Retrieves work items linked to pull requests from Azure DevOps.
- Generates release notes with commit messages and linked work items.

## Prerequisites

- .NET SDK
- Azure DevOps Personal Access Token (PAT) with necessary permissions
- Git repository with commit messages containing pull request IDs

## Installation

1. Clone the repository: 
 ```bash
   git clone https://github.com/AleAntonellis/release-notes-generator.git
   cd release-notes-generator
 ```
2. Install the required NuGet packages:
```bash
dotnet add package LibGit2Sharp
dotnet add package Microsoft.TeamFoundationServer.Client
 ```

## Usage

1. Update the `Program.cs` file with your repository path, Azure DevOps URL, personal access token, and repository ID:
```C#
var repoPath = @"your repository local path";
var sinceTag = "the tag name ex. 1.0.0.0";
var adoUrl = "< the ADO link https://<ado link>/your-organization";
var adoUserName = "your ado username";
var personalAccessToken = "your personal access token";
var repositoryId = "the repository id";
```
2. Run the application:
```bash
dotnet run
 ```
3. The release notes will be generated and printed to the console.

## Customization
You can change the Regex to get the PR id from the commit message in `GitHelper.cs` at

```C#
var prIdPattern = new Regex(@"Merged PR (\d+)");
```
You can change easily the Release Note information in `ReleaseNotesBuilder.cs` at

```C#
sb.AppendLine($"- {workItem.Fields["System.WorkItemType"]} {workItem.Id}: {workItem.Fields["System.Title"]}");
```
