#tool "nuget:?package=coveralls.io&version=1.4.2"
#addin Cake.Git
#addin nuget:?package=Nuget.Core
#addin "nuget:?package=Cake.Coveralls&version=0.9.0"

using NuGet;


//////////////////////////////////////////////////////
//      CONSTANTS AND ENVIRONMENT VARIABLES         //
//////////////////////////////////////////////////////

var target = Argument("target", "Default");
var outputDir = "./output/";
var solutionPath = "./src/FortniteReplayReader.sln";
var project = "./src/FortniteReplayReader/FortniteReplayReader.csproj";
var projectObservable = "./src/FortniteReplayReader.Observable/FortniteReplayReader.Observable.csproj";
var testFolder = "./src/FortniteReplayReader.Test/";
var testProject = testFolder + "FortniteReplayReader.Test.csproj";
var coverageResultsFileName = "coverage.xml";
var currentBranch = Argument<string>("currentBranch", GitBranchCurrent("./").FriendlyName);
var isReleaseBuild = string.Equals(currentBranch, "master", StringComparison.OrdinalIgnoreCase);
var configuration = "Release";
var nugetApiKey = Argument<string>("nugetApiKey", null);
var coverallsToken = Argument<string>("coverallsToken", null);
var nugetSource = "https://api.nuget.org/v3/index.json";

//////////////////////////////////////////////////////
//                     TASKS                        //
//////////////////////////////////////////////////////


Task("Clean")
    .Does(() => {
        if (DirectoryExists(outputDir))
        {
            DeleteDirectory(
                outputDir, 
                new DeleteDirectorySettings {
                    Recursive = true,
                    Force = true
                }
            );
        }
        CreateDirectory(outputDir);
        DotNetCoreClean(solutionPath);
    });


Task("Restore")
    .Does(() => {
        DotNetCoreRestore(solutionPath);
    });

Task("BuildCore")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() => {
        DotNetCoreBuild(
            project,
            new DotNetCoreBuildSettings 
            {
                Configuration = configuration
            }
        );
    });
    
Task("BuildObservable")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() => {
        DotNetCoreBuild(
            projectObservable,
            new DotNetCoreBuildSettings 
            {
                Configuration = configuration
            }
        );
    });

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .IsDependentOn("BuildCore")
    .IsDependentOn("BuildObservable")
    .Does(() => {
        DotNetCoreBuild(
            solutionPath,
            new DotNetCoreBuildSettings 
            {
                Configuration = configuration
            }
        );
    });

Task("Test")
    .Does(() => {
        var settings = new DotNetCoreTestSettings
        {
            ArgumentCustomization = args => args.Append("/p:Include=\"[FortniteReplayReader.Test]*\"")
                                                .Append("/p:CollectCoverage=true")
                                                .Append("/p:CoverletOutputFormat=opencover")
                                                .Append("/p:CoverletOutput=./" + coverageResultsFileName)
        };
        DotNetCoreTest(testProject, settings);
        MoveFile(testFolder + coverageResultsFileName, outputDir + coverageResultsFileName);
    });

Task("UploadCoverage")
    .IsDependentOn("Test")
    .Does(() =>
    {
        CoverallsIo(outputDir + coverageResultsFileName, new CoverallsIoSettings()
        {
            RepoToken = coverallsToken
        });
    });

Task("Package")
    .Does(() => {
        var settings = new DotNetCorePackSettings
        {
            OutputDirectory = outputDir,
            NoBuild = true
        };
        DotNetCorePack(project, settings);
    });

Task("Publish")
    .IsDependentOn("Package")
    .Does(() => {
        var pushSettings = new DotNetCoreNuGetPushSettings 
        {
            Source = nugetSource,
            ApiKey = nugetApiKey
        };

        var pkgs = GetFiles(outputDir + "*.nupkg");
        foreach(var pkg in pkgs) 
        {
            if(!IsNuGetPublished(pkg)) 
            {
                Information($"Publishing \"{pkg}\".");
                DotNetCoreNuGetPush(pkg.FullPath, pushSettings);
            }
            else {
                Information($"Bypassing publishing \"{pkg}\" as it is already published.");
            }
            
        }
    });

//////////////////////////////////////////////////////
//                     TARGETS                      //
//////////////////////////////////////////////////////

Task("BuildAndTest")
    .IsDependentOn("Build")
    .IsDependentOn("Test");


if(isReleaseBuild)
{
    Information("Release build");
    Task("Complete")
        .IsDependentOn("Build")
        .IsDependentOn("Test")
        .IsDependentOn("UploadCoverage")
        .IsDependentOn("Publish");
}
else
{
    Information("Development build");
    Task("Complete")
        .IsDependentOn("Build")
        .IsDependentOn("Test")
        .IsDependentOn("UploadCoverage");
}

Task("Default")
    .IsDependentOn("Complete");


RunTarget(target);


//////////////////////////////////////////////////////
//                      HELPERS                     //
//////////////////////////////////////////////////////

private bool IsNuGetPublished(FilePath packagePath) {
    var package = new ZipPackage(packagePath.FullPath);

    var latestPublishedVersions = NuGetList(
        package.Id,
        new NuGetListSettings 
        {
            Prerelease = true
        }
    );

    return latestPublishedVersions.Any(p => package.Version.Equals(new SemanticVersion(p.Version)));
}