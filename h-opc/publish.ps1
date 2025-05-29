Write-Host "Building package..."
dotnet pack -c Release
if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

# Load API key from set-apikey.bat or set-apikey.ps1
# set-apikey.ps1
if (Test-Path ".\set-apikey.ps1") {
    & ".\set-apikey.ps1"
}

if ([string]::IsNullOrEmpty($env:NUGET_API_KEY)) {
    Write-Host "API key is not set. Please set it in set-apikey.ps1 or set-apikey.bat." -ForegroundColor Red
    exit 1
}

Write-Host "Extracting version number..."
$versionLine = Select-String -Path ".\h-opc-pro.csproj" -Pattern "<Version>(.*)</Version>"
if ($versionLine -eq $null) {
    Write-Host "Failed to extract version number from project file." -ForegroundColor Red
    exit 1
}

$version = $versionLine.Matches[0].Groups[1].Value

$nupkgPath = "..\publish\h-opc-pro.$version.nupkg"
if (-not (Test-Path $nupkgPath)) {
    Write-Host "Package file not found: $nupkgPath" -ForegroundColor Red
    exit 1
}

Write-Host "Pushing package version $version..."
dotnet nuget push $nupkgPath `
    --api-key $env:NUGET_API_KEY `
    --source https://api.nuget.org/v3/index.json

if ($LASTEXITCODE -ne 0) {
    Write-Host "Package push failed!" -ForegroundColor Red
    exit 1
} else {
    Write-Host "Package pushed successfully!" -ForegroundColor Green
}
