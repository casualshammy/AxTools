import os, shutil
from subprocess import call

pluginsFolder = r"F:\sync\private\Projects\AxTools\WoWPlugins"
outputFolder = "F:\\sync\\private\\Projects\\AxTools\\_webservice\\plugins"

def main():
    for entry in os.listdir(outputFolder):
        fullPath = os.path.join(outputFolder, entry)
        if (os.path.isfile(fullPath) and entry.endswith(".zip")):
            os.remove(fullPath)
    for entry in os.listdir(pluginsFolder):
        fullPath = os.path.join(pluginsFolder, entry)
        if (os.path.isdir(fullPath)):
            print("Processing folder " + fullPath)
            unneccessaryDirs = [ fullPath + "\\bin", fullPath + "\\obj" ]
            for uDir in unneccessaryDirs:
                if (os.path.isdir(uDir)):
                    shutil.rmtree(uDir)
            call("type NUL && \"C:\\Program Files\\7-Zip\\7z.exe\" a -tzip \"{0}\\{1}.zip\" \"{2}\\\" -r -mx9".format(outputFolder, entry, fullPath), shell=True);

try:
    main()
except Exception as k:
    print(k)
    input()
