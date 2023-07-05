using System.Linq;
using System.Reflection.Metadata;

using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.EntityFramework;
using Nuke.Common.Utilities.Collections;

[GitHubActions("ci",
    GitHubActionsImage.UbuntuLatest,
    OnPushBranches = new[] { "main" },
    OnPullRequestBranches = new[] { "main" },
    InvokedTargets = new[] { nameof(TestWithCoverage) })]
class Build : NukeBuild
{
    /// <summary>
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode
    /// </summary>
    public static int Main() => Execute<Build>(x => x.Compile);

    [Solution]
    readonly Solution Solution;

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration _configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() => Solution.GetAllProjects("TestContainers101.Api*")
            .ForEach(project => DotNetTasks.DotNetClean(s => s
                .SetProject(project)
            )));

    Target Restore => _ => _
        .Executes(() => Solution.GetAllProjects("TestContainers101.Api*")
            .ForEach(project => DotNetTasks.DotNetRestore(s => s
                .SetProjectFile(project)
            )));

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() => Solution.GetAllProjects("TestContainers101.Api")
            .ForEach(project => DotNetTasks.DotNetBuild(s => s
                .SetProjectFile(project)
            )));

    Target Recompile => _ => _
        .DependsOn(Clean, Compile);

    Target Test => _ => _
        .DependsOn(Restore)
        .Executes(() => Solution.GetAllProjects("*.Tests")
            .ForEach(project => DotNetTasks.DotNetTest(s => s
                .SetProjectFile(project)
                .SetConfiguration(_configuration)
                .SetNoRestore(true)
            )));

    Target TestWithCoverage => _ => _
        .DependsOn(Restore)
        .Executes(() => Solution.GetAllProjects("*.Tests")
            .ForEach(project => DotNetTasks.DotNetTest(s => s
                .SetProjectFile(project)
                .SetConfiguration(_configuration)
                .SetCollectCoverage(true)
                .SetExcludeByFile("**/Migrations/*.cs")
                .SetNoRestore(true)
            )));

    Target DbUpdate => _ => _
        .DependsOn(Compile)
        .Executes(()
         => EntityFrameworkTasks.EntityFrameworkDatabaseUpdate(s => s
                .SetProject(Solution.GetAllProjects("TestContainers101.Api").First())
                .SetConfiguration(_configuration)
                .SetNoBuild(true)
            ));
}
