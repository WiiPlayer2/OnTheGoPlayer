#addin nuget:?package=Cake.Git&version=0.19.0
#tool nuget:?package=NUnit.ConsoleRunner&version=3.9.0

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Build");
var configuration = Argument("configuration", "Release");

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

var appVersion = new Version(0, 1);

Setup(ctx =>
{
    // Executed BEFORE the first task.
    Information("Running tasks...");

    var now = DateTimeOffset.UtcNow;
    var epochStart = new DateTimeOffset(2019, 1, 1, 0, 0, 0, TimeSpan.Zero);
    var timeSinceEpochStart = now - epochStart;
    var daysSinceEpochStart = (int)timeSinceEpochStart.TotalDays;
    var minutesSinceMidnight = (int)timeSinceEpochStart.Subtract(TimeSpan.FromDays(daysSinceEpochStart)).TotalMinutes;
    appVersion = new Version(appVersion.Major, appVersion.Minor, daysSinceEpochStart, minutesSinceMidnight);
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
    CleanDirectory($"./OnTheGoPlayer/obj/{configuration}");
    DeleteFiles($"./OnTheGoPlayer.Test/bin/{configuration}/TestResult.xml");
    DeleteFiles($"./{configuration}-*.zip");
});

Task("Restore")
.IsDependentOn("Cleanup")
.Does(() => {
    NuGetRestore("./OnTheGoPlayer.sln");
});

Task("BuildPublish")
.IsDependentOn("Restore")
.Does(() => {
    MSBuild("./OnTheGoPlayer.sln", config =>
        config.SetConfiguration(configuration)
            .SetVerbosity(Verbosity.Minimal)
            .SetPlatformTarget(PlatformTarget.MSIL)
            .WithTarget("publish")
            .WithProperty("ApplicationVersion", appVersion.ToString(4)));
});

Task("BuildTest")
.IsDependentOn("Restore")
.Does(() => {
    MSBuild("./OnTheGoPlayer.Test/OnTheGoPlayer.Test.csproj", config =>
        config.SetConfiguration(configuration)
            .SetVerbosity(Verbosity.Minimal));
});

Task("Test")
.IsDependentOn("BuildTest")
.Does(() =>
{
    NUnit3($"./OnTheGoPlayer.Test/bin/{configuration}/OnTheGoPlayer.Test.dll",
        new NUnit3Settings
        {
            WorkingDirectory = $"./OnTheGoPlayer.Test/bin/{configuration}",
        });
});

Task("Pack")
.IsDependentOn("BuildPublish")
.IsDependentOn("Test")
.Does(() => {
    var lastCommit = GitLogTip("./");
    Zip($"./OnTheGoPlayer/bin/{configuration}", $"./{configuration}-{lastCommit.Sha}.zip");
});

RunTarget(target);
