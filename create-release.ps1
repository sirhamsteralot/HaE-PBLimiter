# Fetch the current git tag
$gitTag = git describe --tags --abbrev=0
if (-not $gitTag) {
    Write-Host "No tags found in the repository." -ForegroundColor Red
    exit 1
}

# Ask for Plugin Name and GUID
$pluginName = "HaE-PBLimiter"
$pluginGuid = "c1905f3f-86ac-4c54-96d2-32f71f677f6e"

# Get repository name
$repoName = git remote get-url origin | ForEach-Object { ($_ -split "/")[-1] } | ForEach-Object { $_ -replace ".git$", "" }

# Create the folder in the releases directory with the tag as the name
$releaseFolder = "releases/$gitTag"
if (-not (Test-Path $releaseFolder)) {
    New-Item -Path $releaseFolder -ItemType Directory | Out-Null
}

# Generate the manifest.xml content
$manifestContent = @"
<?xml version="1.0"?>
<PluginManifest xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <Name>$pluginName</Name>
  <Guid>$pluginGuid</Guid>
  <Repository>$repoName</Repository>
  <Version>$gitTag</Version>
</PluginManifest>
"@

# Save manifest.xml in the release folder
$manifestPath = "$releaseFolder/manifest.xml"
$manifestContent | Out-File -FilePath $manifestPath -Encoding UTF8

# Define the source files and destination folder
$sourceDll = "HaE PBLimiter/obj/x64/Release/HaE_PBLimiter.dll"
$sourcePdb = "HaE PBLimiter/obj/x64/Release/HaE_PBLimiter.pdb"
$destinationFolder = $releaseFolder

# Check if the source files exist, and copy them to the release folder
if ((Test-Path $sourceDll) -and (Test-Path $sourcePdb)) {
    Copy-Item -Path $sourceDll -Destination $destinationFolder
    Copy-Item -Path $sourcePdb -Destination $destinationFolder
    Write-Host "Files copied successfully!" -ForegroundColor Green
} else {
    Write-Host "One or both of the files do not exist." -ForegroundColor Red
}

# Confirm creation
Write-Host "Release folder '$releaseFolder' and manifest.xml created successfully!" -ForegroundColor Green