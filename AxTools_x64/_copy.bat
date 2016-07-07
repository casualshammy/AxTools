"F:\programs\ConfuserEx\Confuser.CLI.exe" "F:\Dropbox\Projects\AxTools\AxTools_x64\bin\x64\Release\AxTools.exe" -o "F:\Dropbox\Projects\AxTools\AxTools_x64\bin\x64\Release\distr"
"C:\Program Files (x86)\Windows Kits\8.0\bin\x86\signtool.exe" sign /f "C:\Users\Axioma\Documents\certificates\Alex Sommen.pfx" /p 125521 /t http://timestamp.comodoca.com/authenticode "F:\Dropbox\Projects\AxTools\AxTools_x64\bin\x64\Release\distr\AxTools.exe"
copy /b "F:\Dropbox\Projects\AxTools\AxTools_x64\bin\x64\Release\distr\AxTools.exe" "C:\Program Files (x86)\AxTools\"
del "C:\Program Files (x86)\AxTools\*.dll"
copy /b "bin\x64\Release\*.dll" "C:\Program Files (x86)\AxTools\"
"C:\Program Files (x86)\AxTools\AxTools.exe"
pause