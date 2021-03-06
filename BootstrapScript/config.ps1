param(
    [Parameter(ParameterSetName='Version')]
    [switch]$Version,
    
    [Parameter(ParameterSetName='Apply')]
    [switch]$Apply,
    
    [Parameter(ParameterSetName='Test')]
    [switch]$Test,

    [Parameter(ParameterSetName='Validate')]
    [switch]$Validate,

    [Parameter(ParameterSetName='Gather')]
    [switch]$Gather,

    [Parameter(ParameterSetName='Clean')]
    [switch]$Clean,

    [Parameter(ParameterSetName='Gather')]
    [Parameter(ParameterSetName='Validate')]
    [Parameter(ParameterSetName='Test')]
    [Parameter(ParameterSetName='Apply')]
    [string]$Script,

    [Parameter(ParameterSetName='Test')]
    [Parameter(ParameterSetName='Apply')]
    [string]$Settings,

    [Parameter(ParameterSetName='Gather')]
    [string]$Output
    
)

$zip64 = "https://github.com/wiltaylor/FCE/releases/download/v0.1.4/FCE-0.1.4-Win64.zip"
$zip32 = "https://github.com/wiltaylor/FCE/releases/download/v0.1.4/FCE-0.1.4-Win86.zip"

if(Test-Path "C:\Program Files (x86)")
{
    $ZipURL = $zip64
}else{
    $ZipURL = $zip32
}

$FCEEXE = ".\FCE\FlexibleConfigEngine.exe"

if(!(Test-Path .\FCE) -and $Clean -ne $true)
{
    if(!(Test-Path .\fce.zip))
    {
        Invoke-WebRequest -Uri $ZipURL -OutFile .\fce.zip
    }

    Expand-Archive -Path fce.zip -DestinationPath .\FCE
    Remove-Item .\fce.zip -Force
}

if($Version)
{
    &$FCEEXE version
}

if($Apply)
{
    $para = "apply "

    if($Script -ne "") {$para += "-s $script "}
    if($Settings -ne "") {$para += "-c $Settings "}

    $LASTEXITCODE = (Start-Process $FCEEXE -ArgumentList $para -Wait -NoNewWindow -PassThru).ExitCode
}

if($Test)
{
    $para = "test "

    if($Script -ne "") {$para += "-s $script "}
    if($Settings -ne "") {$para += "-c $Settings "}

    $LASTEXITCODE = (Start-Process $FCEEXE -ArgumentList $para -Wait -NoNewWindow -PassThru).ExitCode
}

if($Validate)
{
    $para = "valid "
    if($Script -ne "") {$para += "-s $script "}

    $LASTEXITCODE = (Start-Process $FCEEXE -ArgumentList $para -Wait -NoNewWindow -PassThru).ExitCode
}

if($Gather)
{
    $para = "gather "

    if($Script -ne "") {$para += "-s $script "}
    if($Settings -ne "") {$para += "-o $Output "}

    $LASTEXITCODE = (Start-Process $FCEEXE -ArgumentList $para -Wait -NoNewWindow -PassThru).ExitCode
}

if($Clean)
{
    Remove-Item .\Packages -Force -Recurse -ErrorAction SilentlyContinue
    Remove-Item .\FCE -Force -Recurse -ErrorAction SilentlyContinue
    Remove-Item *.log -Force -ErrorAction SilentlyContinue
    Remove-Item .\fce.zip -Force -ErrorAction SilentlyContinue
}

exit $LASTEXITCODE