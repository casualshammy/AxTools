"F:\programs\ConfuserEx\Confuser.CLI.exe" "F:\Dropbox\Projects\AxTools\AxTools_x64\bin\x64\Release\AxTools.exe" -o "F:\Dropbox\Projects\AxTools\AxTools_x64\bin\x64\Release\distr"
copy /b "F:\Dropbox\Projects\AxTools\AxTools_x64\bin\x64\Release\distr\AxTools.exe" "C:\Program Files (x86)\AxTools\"
del "C:\Program Files (x86)\AxTools\*.dll"
copy /b "bin\x64\Release\*.dll" "C:\Program Files (x86)\AxTools\"
"C:\Program Files (x86)\AxTools\AxTools.exe"
pause