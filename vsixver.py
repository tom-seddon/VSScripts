import zipfile,xml.etree.ElementTree,sys,os,subprocess

z=zipfile.ZipFile(sys.argv[1],"r")

##########################################################################

f=z.open("extension.vsixmanifest","r")
data=f.read()
f.close()
del f

#xml.etree.ElementTree.register_namespace("","http://schemas.microsoft.com/developer/vsx-schema/2011")
x=xml.etree.ElementTree.XML(data)
#xml.etree.ElementTree.dump(x)
x2=x.find("{http://schemas.microsoft.com/developer/vsx-schema/2011}Metadata/{http://schemas.microsoft.com/developer/vsx-schema/2011}Identity")
manifest_version=x2.get("Version")

f=z.open("VSScripts.dll","r")
data=f.read()
f.close()
del f

##########################################################################

dll_version=None

tmp_name="./tmp_dll.dll"
f=open(tmp_name,"wb")
f.write(data)
f.close()
del f

try:
    subprocess.check_output("sigcheck.exe \"%s\" 2>NUL"%tmp_name,shell=True)
except subprocess.CalledProcessError,e:
    # sigcheck.exe returns non-zero exit code on success.  This is
    # making things NEEDLESSLY DIFFICULT.
    output=e.output

xs=[x for x in output.splitlines() if x.strip().lower().startswith("version:")]
if len(xs)>0:
    xs=xs[0].split()
    if len(xs)>0:
        dll_version=xs[-1]

z.close()
del z

os.unlink(tmp_name)

##########################################################################

if dll_version is not None:
    print dll_version
else:
    print manifest_version
