Push-Location
Set-Location $PSScriptRoot

# Move up to the plugin project directory
Set-Location ../PowershellHistory

$name = 'PowershellHistory'
$assembly = "Community.PowerToys.Run.Plugin.$name"
$pluginJsonPath = "./plugin.json"

if (-not (Test-Path $pluginJsonPath)) {
    Write-Error "❌ plugin.json not found at $pluginJsonPath"
    Exit 1
}

$version = "v$((Get-Content $pluginJsonPath | ConvertFrom-Json).Version)"
$archs = @('x64', 'arm64')
$tempDir = "./out/$name"

Write-Host "🔧 Building $name version $version"
Write-Host ""

# Clean previous archives
Remove-Item ./out/*.zip -Recurse -Force -ErrorAction Ignore

foreach ($arch in $archs) {
    Write-Host "🏗️ Building for architecture: $arch"
    $releasePath = "./bin/$arch/Release/net9.0-windows"

    dotnet build -c Release /p:Platform=$arch

    Remove-Item "$tempDir/*" -Recurse -Force -ErrorAction Ignore
    mkdir "$tempDir" -ErrorAction Ignore | Out-Null

    $items = @(
        "$releasePath/$assembly.deps.json",
        "$releasePath/$assembly.dll",
        "$releasePath/plugin.json",
        "$releasePath/Images"
    )

    Copy-Item $items "$tempDir" -Recurse -Force
    Compress-Archive "$tempDir" "./out/$name-$version-$arch.zip" -Force
}

Write-Host "📦 Creating GitHub release $version"
gh release create $version (Get-ChildItem ./out/*.zip)

Pop-Location