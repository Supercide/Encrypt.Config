#tool "nuget:?package=NUnit.ConsoleRunner"

var target = Argument("target", "Default");

//////////////////////////////////////////////////////////////////////
// VARIABLES
//////////////////////////////////////////////////////////////////////

string semVersion = EnvironmentVariable("BUILD_VERSION") ?? "1.0.0";
string version = string.Join(".", semVersion.Split('.').Take(3));

const string BUILD_CONFIG = "Release";
const string SOLUTION_PATH = "./Encrypt.Config.sln";
var artifactsDir = "./artifacts/";

//////////////////////////////////////////////////////////////////////
// PATCH 
//////////////////////////////////////////////////////////////////////

Task("Patch")
    .Does(() => {
        var file = "./SolutionInfo.cs";

        CreateAssemblyInfo(file, new AssemblyInfoSettings {
            Version = version,
            FileVersion = version,
            InformationalVersion = semVersion,
            });        
    });

///////////////////////////////////////////////////////////////////////////////
// PREPARE
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() => {
		var directoriesToClean = GetDirectories("./**/bin/")
                                    .Union(GetDirectories(artifactsDir));
	
		CleanDirectories(directoriesToClean);
    });

Task("NugetRestore")
    .IsDependentOn("Clean")
    .Does(() => 
    {
        NuGetRestore(SOLUTION_PATH);
    });

///////////////////////////////////////////////////////////////////////////////
// BUILD
///////////////////////////////////////////////////////////////////////////////

Task("Build")
    .IsDependentOn("NugetRestore")
    .IsDependentOn("Patch")
    .Does(() =>
    {
		MSBuild(SOLUTION_PATH, configurator => configurator
                    .SetConfiguration(BUILD_CONFIG)
                    .SetVerbosity(Verbosity.Minimal)                    
                    .SetMSBuildPlatform(MSBuildPlatform.x64)
                    .SetPlatformTarget(PlatformTarget.MSIL));
    });

///////////////////////////////////////////////////////////////////////////////
// TESTS
///////////////////////////////////////////////////////////////////////////////

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
    {
		var testAssemblies = GetFiles(string.Format("./tests/**/bin/{0}/*Tests.dll", BUILD_CONFIG));

        NUnit3(testAssemblies, new NUnit3Settings {
            NoResults = false
        });
    });

///////////////////////////////////////////////////////////////////////////////
// TESTS
///////////////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Test");

RunTarget(target);