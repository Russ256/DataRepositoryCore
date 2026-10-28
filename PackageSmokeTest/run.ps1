$ErrorActionPreference = 'Stop'
$repositoryRoot = Split-Path -Parent $PSScriptRoot
[xml]$sharedProperties = Get-Content (Join-Path $repositoryRoot 'Directory.Build.props') -Raw
$packageVersion = [string]$sharedProperties.Project.PropertyGroup.Version
$framework = [string]$sharedProperties.Project.PropertyGroup.TargetFramework
[xml]$repositoryProject = Get-Content (Join-Path $repositoryRoot 'DataRepositoryCore/DataRepositoryCore.csproj') -Raw
$efReference = $repositoryProject.SelectSingleNode("//PackageReference[@Include='Microsoft.EntityFrameworkCore']")
$entityFrameworkVersion = $efReference.GetAttribute('Version')
$runRoot = Join-Path ([System.IO.Path]::GetTempPath()) ("DataRepositoryCore-smoke-" + [guid]::NewGuid())
$feed = Join-Path $runRoot 'feed'
$consumer = Join-Path $runRoot 'consumer'
$packages = Join-Path $runRoot 'packages'
New-Item -ItemType Directory -Path $feed, $consumer | Out-Null
Copy-Item (Join-Path $PSScriptRoot 'PackageSmokeTest.csproj'), (Join-Path $PSScriptRoot 'Program.cs') -Destination $consumer

function Invoke-DotNet {
    & dotnet @args
    if ($LASTEXITCODE -ne 0) { throw "dotnet failed with exit code $LASTEXITCODE" }
}

foreach ($project in @('DataRepositoryCore.Abstractions', 'DataRepositoryCore')) {
    $projectPath = Join-Path $repositoryRoot "$project/$project.csproj"
    Invoke-DotNet pack $projectPath --configuration Release --output $feed '-p:GeneratePackageOnBuild=false'
    $packagePath = Join-Path $feed "$project.$packageVersion.nupkg"
    $archive = [System.IO.Compression.ZipFile]::OpenRead($packagePath)
    try {
        foreach ($entry in @('README.md', "lib/$framework/$project.xml")) {
            if ($null -eq $archive.GetEntry($entry)) { throw "$packagePath is missing $entry" }
        }
    } finally { $archive.Dispose() }
}

$consumerProject = Join-Path $consumer 'PackageSmokeTest.csproj'
Invoke-DotNet restore $consumerProject --packages $packages "-p:ConsumerTargetFramework=$framework" "-p:RepositoryPackageVersion=$packageVersion" "-p:EntityFrameworkVersion=$entityFrameworkVersion" "-p:RestoreAdditionalProjectSources=$feed"
foreach ($project in @('DataRepositoryCore.Abstractions', 'DataRepositoryCore')) {
    $packagePath = Join-Path $feed "$project.$packageVersion.nupkg"
    $hashPath = Join-Path $packages "$($project.ToLowerInvariant())/$($packageVersion.ToLowerInvariant())/$($project.ToLowerInvariant()).$($packageVersion.ToLowerInvariant()).nupkg.sha512"
    $hash = [Convert]::ToBase64String([System.Security.Cryptography.SHA512]::HashData([System.IO.File]::ReadAllBytes($packagePath)))
    if ((Get-Content $hashPath -Raw).Trim() -ne $hash) { throw "The consumer did not restore the freshly built $project package." }
}
Invoke-DotNet run --project $consumerProject --no-restore "-p:ConsumerTargetFramework=$framework" "-p:RepositoryPackageVersion=$packageVersion" "-p:EntityFrameworkVersion=$entityFrameworkVersion"
Write-Host "Package smoke test passed. Artifacts: $runRoot"
