param($certFile, $pw, $buildDir, $timestampUrl)
 
Write-Host "****************************************"
Write-Host "****************************************"
Write-Host "*** S I G N I N G O U P U T ***"
Write-Host "****************************************"
Write-Host "****************************************"
 
Write-Host $buildDir
Write-Host $certFile
 
$exe = Get-ChildItem -Path "C:\program files (x86)\Windows Kits" -Recurse -Include signtool.exe | select -First 1
Write-Host $exe
 
ForEach ($file in (Get-ChildItem $buildDir -Filter ISynergy* -Recurse))
{
  if ($file.Extension -eq ".dll")
  {
    Write-Host $file.FullName
    &$exe sign /f $certFile /p $pw -tr $timestampUrl $file.FullName
  }
  elseif ($file.Extension -eq ".exe")
  {
    Write-Host $file.FullName
    &$exe sign /f $certFile /p $pw -tr $timestampUrl $file.FullName
  }
}