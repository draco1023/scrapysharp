#I "packages/FAKE/tools/"
#r "FakeLib.dll"

open Fake
open System
open System.IO

let buildDir = __SOURCE_DIRECTORY__ @@ "build"
let nugetsDir = __SOURCE_DIRECTORY__ @@ "NuGet"

ensureDirectory buildDir
ensureDirectory nugetsDir

Target "Clean"
    <| fun _ ->
        CleanDir buildDir
        CleanDir nugetsDir

Target "Packages"
    <| fun _ ->
        trace "Restoring packages"
        RestorePackages()

Target "BuildRelease"
    <| fun _ ->
        !! "*/**.csproj"
        |> MSBuildRelease buildDir "Build"
        |> Log "BuildTests-Output: "

Target "NuGet" (fun _ ->

    buildDir
    |> directoryInfo
    |> filesInDir
    |> Array.map(fun f -> f.FullName)
    |> CopyFiles nugetsDir

    let mustPublish = false
    let nugetAccessKey = ""
    let nugetsVersions name = 
        NuGetVersion.nextVersion <|
            fun arg -> 
                { arg 
                    with 
                        PackageName=name
                        DefaultVersion="0.1.0"
                        Increment=NuGetVersion.IncPatch
                }
    NuGet (fun p -> 
            { p with
                Authors = ["Romain Flechner"]
                Project = "ScrapySharp"
                OutputPath = nugetsDir
                AccessKey = nugetAccessKey
                Version = nugetsVersions "ScrapySharp"
                Publish = mustPublish
                Dependencies = getDependencies "ScrapySharp/packages.config"
                Properties = [("Configuration","Release")]
            }) "ScrapySharp.nuspec"
)


"Clean"
    ==> "Packages"
    ==> "BuildRelease"

RunTargetOrDefault "BuildRelease"


