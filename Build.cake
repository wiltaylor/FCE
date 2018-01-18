#tool "nuget:?package=GitVersion.CommandLine"
#tool "nuget:?package=xunit.runner.console"
/*****************************************************************************************************
Build Script for FCE
Author: Wil Taylor (Wil Taylor)
Created: 9/01/2018

Description:
Generates cake files and can add new build targets and tasks etc.
*****************************************************************************************************/

//Folder Variables
var RepoRootFolder = MakeAbsolute(Directory("."));
var ToolsFolder = RepoRootFolder + "/Tools";
var InstallFolder = RepoRootFolder + "/Install";

var target = Argument("target", "Default");

var nugetAPIKey = EnvironmentVariable("NUGETAPIKEY");
    
GitVersion version;

try{
    version = GitVersion(new GitVersionSettings{UpdateAssemblyInfo = true}); //This updates all AssemblyInfo files automatically.
        Information("##vso[task.setvariable variable=BuildVer]" + version.SemVer);
}
catch
{
    //Unable to get version.
}

Task("Default")
    .IsDependentOn("Restore")
    .IsDependentOn("Build");

Task("Restore")
    .IsDependentOn("FCE.Restore");

Task("Clean")
    .IsDependentOn("FCE.Clean");

Task("Build")
    .IsDependentOn("FCE.Build");

Task("Test")
    .IsDependentOn("FCE.Test");

Task("Package")
    .IsDependentOn("FCE.Package");

Task("Deploy")
    .IsDependentOn("FCE.Deploy");

Task("Version")
    .Does(() => {
        Information("Assembly Version: " + version.AssemblySemVer);
        Information("SemVer: " + version.SemVer);
        Information("Branch: " + version.BranchName);
        Information("Commit Date: " + version.CommitDate);
        Information("Build Metadata: " + version.BuildMetaData);
        Information("PreReleaseLabel: " + version.PreReleaseLabel);
        Information("FullBuildMetaData: " + version.FullBuildMetaData);
    });
/*****************************************************************************************************
FCE
*****************************************************************************************************/
Task("FCE.Restore")
    .Does(() => StartProcess("dotnet", "restore"));

Task("FCE.Clean")
    .IsDependentOn("FCE.Clean.Windowsx86");

Task("FCE.Clean.Windowsx86")
    .Does(() => { 
        CleanDirectory(RepoRootFolder + "/FlexibleConfigEngine/bin/Release/netcoreapp2.0/win7-x86");
    });

Task("FCE.Clean.Windowsx86.Debug")
    .Does(() => { 
        CleanDirectory(RepoRootFolder + "/FlexibleConfigEngine/bin/Debug/netcoreapp2.0/win7-x86");
    });
    
Task("FCE.Build")
    .IsDependentOn("FCE.Build.Windowsx86");

Task("FCE.Build.Windowsx86")
    .IsDependentOn("FCE.UpdateVersion")
    .IsDependentOn("FCE.Clean.Windowsx86")
    .Does(() => {
        var ret = StartProcess("dotnet", "publish -r win7-x86 -c release");

        Information("Ignore warnings about package version not being compatible with legacy clients. Can't supress them at the moment.");

        if(ret != 0)
            throw new Exception("Build failed!");
    });

Task("FCE.Build.Windowsx86.Debug")
    .IsDependentOn("FCE.UpdateVersion")
    .IsDependentOn("FCE.Clean.Windowsx86.Debug")
    .Does(() => {
        var ret = StartProcess("dotnet", "publish -r win7-x86 -c debug");

        Information("Ignore warnings about package version not being compatible with legacy clients. Can't supress them at the moment.");

        if(ret != 0)
            throw new Exception("Build failed!");
    });

Task("FCE.UpdateVersion")
    .Does(() => {
        var file = RepoRootFolder + "/FlexibleConfigEngine/FlexibleConfigEngine.csproj";
        XmlPoke(file, "/Project/PropertyGroup/Version", version.SemVer);
        XmlPoke(file, "/Project/PropertyGroup/AssemblyVersion", version.AssemblySemVer);
        XmlPoke(file, "/Project/PropertyGroup/FileVersion", version.AssemblySemVer);

        file = RepoRootFolder + "/FlexibleConfigEngine.Core/FlexibleConfigEngine.Core.csproj";
        XmlPoke(file, "/Project/PropertyGroup/Version", version.SemVer);
        XmlPoke(file, "/Project/PropertyGroup/AssemblyVersion", version.AssemblySemVer);
        XmlPoke(file, "/Project/PropertyGroup/FileVersion", version.AssemblySemVer);
    });

Task("FCE.Test")
    .IsDependentOn("FCE.Test.UnitTest");

Task("FCE.Test.UnitTest")
    .IsDependentOn("FCE.Build")
    .Does(() => {
        var ret = StartProcess("dotnet", "test FlexibleConfigUnitTest");

        if(ret != 0)
            throw new Exception("Test failed!");
    });

Task("FCE.Package")
    .IsDependentOn("FCE.Package.Zip");

Task("FCE.Package.Zip")
    .IsDependentOn("FCE.Build")
    .Does(() => Zip(RepoRootFolder + "/FlexibleConfigEngine/bin/Release/netcoreapp2.0/win7-x86/publish", RepoRootFolder + "/FlexibleConfigEngine/bin/Release/FCE-" + version.SemVer + "-Win.zip"))
    .Does(() => CopyFile(RepoRootFolder + "/FlexibleConfigEngine/bin/Release/FCE-" + version.SemVer + "-Win.zip",RepoRootFolder + "/BootstrapScript/fce.zip" ));

Task("FCE.Deploy");
/*****************************************************************************************************
End of script
*****************************************************************************************************/
RunTarget(target);
