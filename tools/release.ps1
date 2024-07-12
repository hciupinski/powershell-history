Push-Location
Set-Location $PSScriptRoot

$name = 'PowershellHistory'
$assembly = "Community.PowerToys.Run.Plugin.$name"
$projectPath = "../$name"
$pluginPath = "$projectPath/plugin.json"
$version = "v$((Get-Content $pluginPath | ConvertFrom-Json).Version)"
$archs = @('x64', 'arm64')
$tempDir = '../out/temp'

git tag $version
git push --tags

Remove-Item ../out/*.zip -Recurse -Force -ErrorAction Ignore
foreach ($arch in $archs) {
	$releasePath = "$projectPath/bin/$arch/Release/net8.0-windows"

	dotnet build $projectPath -c Release /p:Platform=$arch

	Remove-Item "$tempDir/*" -Recurse -Force -ErrorAction Ignore
	mkdir "$tempDir" -ErrorAction Ignore
	$items = @(
		"$releasePath/$assembly.deps.json",
		"$releasePath/$assembly.dll",
		"$releasePath/plugin.json",
		"$releasePath/Images"
	)
	Copy-Item $items "$tempDir" -Recurse -Force
	Compress-Archive "$tempDir" "../out/$name-$version-$arch.zip" -Force
}

gh release create $version (Get-ChildItem ../out/*.zip)
Pop-Location
