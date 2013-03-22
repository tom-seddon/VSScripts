import zipfile,xml.etree.ElementTree,sys

z=zipfile.ZipFile(sys.argv[1],"r")

f=z.open("extension.vsixmanifest","r")
data=f.read()
f.close()
del f

z.close()
del z

#xml.etree.ElementTree.register_namespace("","http://schemas.microsoft.com/developer/vsx-schema/2011")
x=xml.etree.ElementTree.XML(data)
#xml.etree.ElementTree.dump(x)
x2=x.find("{http://schemas.microsoft.com/developer/vsx-schema/2011}Metadata/{http://schemas.microsoft.com/developer/vsx-schema/2011}Identity")
sys.stdout.write(x2.get("Version"))
