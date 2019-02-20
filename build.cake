#addin nuget:?package=Cake.Git&version=0.19.0
#tool nuget:?package=NUnit.ConsoleRunner&version=3.9.0

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Build");
var configuration = Argument("configuration", "Debug");

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(ctx =>
{
   // Executed BEFORE the first task.
   Information("Running tasks...");
});

Teardown(ctx =>
{
   // Executed AFTER the last task.
   Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("Cleanup")
.Does(() => {
    DeleteFiles($"./OnTheGoPlayer.Test/bin/{configuration}/TestResult.xml");
    DeleteFiles($"./{configuration}-*.zip");
});

Task("Restore")
.IsDependentOn("Cleanup")
.Does(() => {
    NuGetRestore("./OnTheGoPlayer.sln");
});

Task("Build")
.IsDependentOn("Restore")
.Does(() => {
    MSBuild("./OnTheGoPlayer.sln", config =>
        config.SetConfiguration(configuration)
            .SetVerbosity(Verbosity.Minimal)
            .SetPlatformTarget(PlatformTarget.MSIL));
});

Task("Test")
.IsDependentOn("Build")
.ContinueOnError()
.Does(() =>
{
    NUnit3($"./OnTheGoPlayer.Test/bin/{configuration}/OnTheGoPlayer.Test.dll",
        new NUnit3Settings
        {
            WorkingDirectory = $"./OnTheGoPlayer.Test/bin/{configuration}",
        });
});

Task("Pack")
.IsDependentOn("Test")
.Does(() => {
    var lastCommit = GitLogTip("./");
    Zip($"./OnTheGoPlayer/bin/{configuration}", $"./{configuration}-{lastCommit.Sha}.zip");
});

RunTarget(target);
