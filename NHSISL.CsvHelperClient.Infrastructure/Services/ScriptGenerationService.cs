// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ADotNet.Clients;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets.Tasks;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets.Tasks.SetupDotNetTaskV5s;

namespace NHSISL.CsvHelperClient.Infrastructure.Services
{
    internal class ScriptGenerationService
    {
        private readonly ADotNetClient adotNetClient;

        public ScriptGenerationService() =>
            adotNetClient = new ADotNetClient();

        public void GenerateBuildScript(string branchName, string projectName, string dotNetVersion)
        {
            var githubPipeline = new GithubPipeline
            {
                Name = "Build",

                OnEvents = new Events
                {
                    Push = new PushEvent
                    {
                        Branches = new string[] { branchName }
                    },

                    PullRequest = new PullRequestEvent
                    {
                        Types = new string[] { "opened", "synchronize", "reopened", "closed" }
                    }
                },

                EnvironmentVariables = new Dictionary<string, string>
                {
                    { "IS_RELEASE_CANDIDATE", EnvironmentVariables.IsGitHubReleaseCandidate() }
                },


                Jobs = new Dictionary<string, Job>
                {
                    {
                        "build",
                        new Job
                        {
                            Name = "Build",
                            RunsOn = BuildMachines.UbuntuLatest,

                            Steps = new List<GithubTask>
                            {
                                new GithubTask
                                {
                                    Name = "Enable long paths for Git",
                                    Run = "git config --global core.longpaths true"
                                },

                                new CheckoutTaskV5
                                {
                                    Name = "Check Out"
                                },

                                new SetupDotNetTaskV5
                                {
                                    Name = "Setup Dot Net Version",

                                    With = new TargetDotNetVersionV5
                                    {
                                        DotNetVersion = dotNetVersion
                                    }
                                },

                                new RestoreTask
                                {
                                    Name = "Restore",
                                },

                                new DotNetBuildTask
                                {
                                    Name = "Build"
                                },

                                new TestTask
                                {
                                    Name = "Test"
                                }
                            }
                        }
                    },
                    {
                        "add_tag",
                        new TagJobV2(
                            runsOn: BuildMachines.UbuntuLatest,
                            dependsOn: "build",
                            projectRelativePath: $"{projectName}/{projectName}.csproj",
                            githubToken: "${{ secrets.PAT_FOR_TAGGING }}",
                            branchName: branchName)
                    },
                    {
                        "publish",
                        new PublishJobV4(
                            runsOn: BuildMachines.UbuntuLatest,
                            dependsOn: "add_tag",
                            dotNetVersion: dotNetVersion,
                            nugetApiKey: "${{ secrets.NUGET_ACCESS }}")
                    }
                }
            };

            string buildScriptPath = "../../../../.github/workflows/build.yml";
            string directoryPath = Path.GetDirectoryName(buildScriptPath);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            adotNetClient.SerializeAndWriteToFile(
                githubPipeline,
                path: buildScriptPath);
        }

        public void GeneratePrLintScript(string branchName)
        {
            var githubPipeline = new GithubPipeline
            {
                Name = "PR Linter",

                OnEvents = new Events
                {
                    PullRequest = new PullRequestEvent
                    {
                        Types = new string[] { "opened", "edited", "synchronize", "reopened", "closed" },
                        Branches = new string[] { branchName }
                    }
                },

                Jobs = new Dictionary<string, Job>
                {
                    {
                        "label",
                        new LabelJobV3(runsOn: BuildMachines.UbuntuLatest)
                        {
                            Name = "Label"
                        }
                    },
                    {
                        "requireIssueOrTask",
                        new RequireIssueOrTaskJobV2(excludedAuthors: "dependabot[bot]")
                        {
                            Name = "Require Issue Or Task Association",
                        }
                    },
                    {
                        "setAuthorAsPrAssignee",
                        new SetAuthorAsPrAssigneeJobV2(runsOn: BuildMachines.UbuntuLatest)
                        {
                            Name = "Set Author As PR Assignee",
                        }
                    },
                }
            };

            string buildScriptPath = "../../../../.github/workflows/prLinter.yml";
            string directoryPath = Path.GetDirectoryName(buildScriptPath);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            adotNetClient.SerializeAndWriteToFile(
                githubPipeline,
                path: buildScriptPath);
        }
    }
}
