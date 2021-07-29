import os
import shutil
import zipfile
import glob

def zipdir(dir_path, output_path):
    zf = zipfile.ZipFile(output_path, mode="w")
    for root, dirs, files in os.walk(dir_path):
        for file in files:
            if file != output_path:
                p = os.path.join(root, file)
                zf.write(p, p[10:])
    zf.close()

buildpath = "../disaster5/bin/Release/netcoreapp3.1"
outputpath = "../builds/disaster"
datapath = "../../disaster_test"
print("distributing...")

if os.path.exists(outputpath): shutil.rmtree(outputpath)
if os.path.exists(outputpath+"_temp"): shutil.rmtree(outputpath+"_temp")
if os.path.exists(outputpath+"_win32"): shutil.rmtree(outputpath+"_win32")
if os.path.exists(outputpath+"_win64"): shutil.rmtree(outputpath+"_win64")
if os.path.exists(outputpath+"_linux32"): shutil.rmtree(outputpath+"_linux32")
if os.path.exists(outputpath+"_linux64"): shutil.rmtree(outputpath+"_linux64")
if os.path.exists(outputpath+"_osx"): shutil.rmtree(outputpath+"_osx")

if os.path.exists(outputpath+"_win64.zip"): os.remove(outputpath+"_win64.zip")

if os.path.exists(buildpath):
    shutil.copytree(buildpath, outputpath)
    shutil.copytree(buildpath, outputpath+"_temp")
    shutil.rmtree(outputpath+"/runtimes")

    print("copying resources")

    files = os.listdir("../res")
    for filename in files:
        shutil.copy("../res/"+filename, outputpath+"/"+filename)

    files = os.listdir(datapath)
    for filename in files:
        if filename[:4] != ".git":
            if os.path.isfile(datapath+"/"+filename):
                shutil.copy(datapath+"/"+filename, outputpath+"/data/"+filename)
            else:
                shutil.copytree(datapath+"/"+filename, outputpath+"/data/"+filename)


    print("creating versions")

    shutil.copytree(outputpath, outputpath+"_win32")
    shutil.copytree(outputpath, outputpath+"_win64")
    shutil.copytree(outputpath, outputpath+"_linux32")
    shutil.copytree(outputpath, outputpath+"_linux64")
    shutil.copytree(outputpath, outputpath+"_osx")

    shutil.copytree(outputpath+"_temp/runtimes/win-x64", outputpath+"_win64/runtimes/win-x64")
    shutil.copytree(outputpath+"_temp/runtimes/win-x86", outputpath+"_win32/runtimes/win-x86")
    shutil.copytree(outputpath+"_temp/runtimes/linux-x64", outputpath+"_linux64/runtimes/linux-x64")
    shutil.copytree(outputpath+"_temp/runtimes/linux-x86", outputpath+"_linux32/runtimes/linux-x86")
    shutil.copytree(outputpath+"_temp/runtimes/osx-x64", outputpath+"_osx/runtimes/osx-x64")

    print("zipping")
    
    zipdir(outputpath+"_win64", "../builds/disaster_win64.zip")
    zipdir(outputpath+"_win32", "../builds/disaster_win32.zip")
    zipdir(outputpath+"_linux64", "../builds/disaster_linux64.zip")
    zipdir(outputpath+"_linux32", "../builds/disaster_linux32.zip")
    zipdir(outputpath+"_osx", "../builds/disaster_osx.zip")

    print("cleanup")

    shutil.rmtree(outputpath+"_temp")
    shutil.rmtree(outputpath+"_win32")
    shutil.rmtree(outputpath+"_win64")
    shutil.rmtree(outputpath+"_linux32")
    shutil.rmtree(outputpath+"_linux64")
    shutil.rmtree(outputpath+"_osx")
    shutil.rmtree(outputpath)

    print("done!")
    
else:
    print("no build directory")
