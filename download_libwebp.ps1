$url = "https://s3.amazonaws.com/resizer-dynamic-downloads/webp/0.5.2/x86_64/libwebp.dll"
$output = "$PSScriptRoot\src\Imazen.Test.Webp\x64\libwebp.dll"

Invoke-WebRequest -Uri $url -OutFile $output

$url = "https://s3.amazonaws.com/resizer-dynamic-downloads/webp/0.5.2/x86/libwebp.dll"
$output = "$PSScriptRoot\src\Imazen.Test.Webp\x86\libwebp.dll"

Invoke-WebRequest -Uri $url -OutFile $output