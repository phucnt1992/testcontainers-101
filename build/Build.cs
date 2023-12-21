using System.Linq;
using System.Reflection.Metadata;

using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.EntityFramework;
using Nuke.Common.Tools.Npm;
using Nuke.Common.Tools.ReportGenerator;
using Nuke.Common.Utilities.Collections;

[GitHubActions("ci",
    GitHubActionsImage.UbuntuLatest,
    AutoGenerate = false,
    OnPushBranches = new[] { "main" },
    OnPullRequestBranches = new[] { "main" },
    InvokedTargets = new[] { nameof(Init), nameof(Lint), nameof(TestWithCoverage) })]
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

    readonly string ProjectName = "TestContainers101.*";
    string ProjectPrefix => $"{ProjectName}*";

    readonly string TestProjectPostfix = "*.Tests";
    readonly string TestWithCoverageSupportProjectPostfix = "*.Api.Tests";
    readonly string TestWithoutCoverageSupportProjectPostfix = "*.Function.Tests";
    readonly string CoverageFolderName = "coverage";
    string CoveragePrefix => $"{CoverageFolderName}.*";
    string CoverageReportFile => $"coverage.xml";

    Target Init => _ => _
        .Executes(() => NpmTasks.NpmCi());

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() => Solution.GetAllProjects(ProjectPrefix)
            .ForEach(project => DotNetTasks.DotNetClean(s => s
                .SetProject(project)
            )));

    Target Reset => _ => _
        .Before(Clean)
        .Executes(() => RootDirectory.GlobDirectories(CoverageFolderName).DeleteDirectories())
        .Executes(() => RootDirectory.GlobFiles(CoveragePrefix).DeleteFiles());

    Target Restore => _ => _
        .Executes(() => Solution.GetAllProjects(ProjectPrefix)
            .ForEach(project => DotNetTasks.DotNetRestore(s => s
                .SetProjectFile(project)
            )));

    Target Lint => _ => _
        .After(Init)
        .Executes(() =>
        {
            NpmTasks.NpmRun(s => s
                .SetCommand("lint"));

            Solution.GetAllProjects(ProjectPrefix)
            .ForEach(project =>
            {
                DotNetTasks.DotNetBuild(s => s
                    .SetProjectFile(project)
                    .AddWarningsAsErrors());
            });
        });

    Target Format => _ => _
        .DependsOn(Restore)
        .After(Lint)
        .Executes(() => Solution.GetAllProjects(ProjectPrefix)
            .ForEach(project => DotNetTasks.DotNetFormat(s => s
                .SetProject(project)
            )));

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() => Solution.GetAllProjects(ProjectName)
            .ForEach(project => DotNetTasks.DotNetBuild(s => s
                .SetProjectFile(project)
                .SetNoRestore(true)
            )));

    Target Recompile => _ => _
        .DependsOn(Clean, Compile);

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() => Solution.GetAllProjects(TestProjectPostfix)
            .ForEach(project => DotNetTasks.DotNetTest(s => s
                .SetProjectFile(project)
                .SetConfiguration(_configuration)
                .SetNoRestore(true)
            )));

    Target TestWithCoverage => _ => _
        .DependsOn(Compile)
        .After(Lint)
        .Executes(() =>
        {
            Solution.GetAllProjects(TestWithCoverageSupportProjectPostfix)
            .ForEach(project => DotNetTasks.DotNetTest(s => s
                .SetProjectFile(project)
                .SetConfiguration(_configuration)
                .SetCollectCoverage(true)
                .SetCoverletOutputFormat(CoverletOutputFormat.opencover)
                .SetCoverletOutput(RootDirectory / CoverageFolderName / CoverageReportFile)
                .SetExcludeByFile("**/Migrations/*.cs%2C**/Function/*.cs")
                .SetNoRestore(true)
            ));
        });

    Target TestWithoutCoverage => _ => _
        .DependsOn(Compile)
        .After(Lint)
        .Executes(() =>
        {
            Solution.GetAllProjects(TestWithoutCoverageSupportProjectPostfix)
            .ForEach(project => DotNetTasks.DotNetTest(s => s
                .SetProjectFile(project)
                .SetConfiguration(_configuration)
                .SetNoRestore(true)
            ));
        });

    Target CI => _ => _
        .DependsOn(Init, Lint, TestWithCoverage, TestWithoutCoverage);

    Target GenerateHtmlTestReport => _ => _
        .DependsOn(TestWithCoverage)
        .Executes(() => ReportGeneratorTasks.ReportGenerator(s => s
                .SetReports(RootDirectory / CoverageFolderName / CoverageReportFile)
                .SetTargetDirectory(RootDirectory / CoverageFolderName)
                .SetReportTypes(ReportTypes.HtmlInline)
        ));

    Target DbUpdate => _ => _
        .DependsOn(Compile)
        .Executes(()
         => EntityFrameworkTasks.EntityFrameworkDatabaseUpdate(s => s
                .SetProject(Solution.GetAllProjects(ProjectName).First())
                .SetConfiguration(_configuration)
                .SetNoBuild(true)
            ));
}
