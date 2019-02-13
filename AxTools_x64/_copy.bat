cd /d "D:\sync\private\Projects\AxTools\AxTools_x64"
"C:\apps\ConfuserEx\Confuser.CLI.exe" "D:\sync\private\Projects\AxTools\AxTools_x64\bin\x64\Release\AxTools.exe" -o "D:\sync\private\Projects\AxTools\AxTools_x64\bin\x64\Release\distr2"
"C:\Program Files (x86)\Windows Kits\8.1\bin\x64\signtool.exe" sign /f "C:\Users\Axioma\Documents\certificates\Alex Sommen.pfx" /p 125521 /t http://timestamp.comodoca.com/authenticode "D:\sync\private\Projects\AxTools\AxTools_x64\bin\x64\Release\distr2\AxTools.exe"
"C:\Program Files (x86)\Windows Kits\8.1\bin\x64\signtool.exe" sign /f "C:\Users\Axioma\Documents\certificates\Alex Sommen.pfx" /as /fd sha256 /p 125521 /tr http://timestamp.comodoca.com/rfc3161 /td sha256 "D:\sync\private\Projects\AxTools\AxTools_x64\bin\x64\Release\distr2\AxTools.exe"
copy /b "D:\sync\private\Projects\AxTools\AxTools_x64\bin\x64\Release\distr2\AxTools.exe" "C:\Program Files (x86)\AxTools\"
del "C:\Program Files (x86)\AxTools\*.dll"
copy /b "bin\x64\Release\*.dll" "C:\Program Files (x86)\AxTools\"
start "" "C:\Program Files (x86)\AxTools\AxTools.exe"
pause