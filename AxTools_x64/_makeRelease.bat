"F:\programs\ConfuserEx\Confuser.CLI.exe" "F:\Dropbox\Projects\AxTools\AxTools_x64\bin\x64\Release\AxTools.exe" -o "F:\Dropbox\Projects\AxTools\AxTools_x64\bin\x64\Release\distr1"
del "F:\Dropbox\Projects\AxTools\AxTools_x64\bin\x64\Release\_update1.zip"
del "F:\Dropbox\Projects\AxTools\AxTools_x64\bin\x64\Release\_updater1.zip"
"C:\Program Files\7-Zip\7z.exe" a -tzip "F:\Dropbox\Projects\AxTools\AxTools_x64\bin\x64\Release\_update1.zip" -p3aTaTre6agA$-E+e "F:\Dropbox\Projects\AxTools\AxTools_x64\bin\x64\Release\distr1\*" -r -mx9
"C:\Program Files\7-Zip\7z.exe" a -tzip "F:\Dropbox\Projects\AxTools\AxTools_x64\bin\x64\Release\_updater1.zip" -p3aTaTre6agA$-E+e "F:\Dropbox\Projects\AxTools\axtools_updater_CSharp\bin\Release\*" -r -mx9
"F:\Dropbox\Projects\AxTools\AxTools_x64\bin\x64\Release\_update1.json"
pause
"F:\programs\WinSCP\WinSCP.exe" /console /script="F:\Dropbox\Projects\AxTools\AxTools_x64\_makeReleaseWinSCP.txt"