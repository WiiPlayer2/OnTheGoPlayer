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

var appVersion = new Version(0, 1);

Setup(ctx =>
{
    // Executed BEFORE the first task.
    Information("Running tasks...");

    var now = DateTimeOffset.UtcNow;
    appVersion = new Version(appVersion.Major, appVersion.Minor, (now.Year * 10000) + (now.Month * 100) + now.Day, (int)now.TimeOfDay.TotalSeconds);
    Information($"Building version {appVersion}...");
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
    CleanDirectory($"./OnTheGoPlayer/bin/{configuration}");
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
            .SetPlatformTarget(PlatformTarget.MSIL)
            .WithTarget("publish")
            .WithProperty("ApplicationVersion", appVersion.ToString(4)));
});

Task("Test")
.IsDependentOn("Build")
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
