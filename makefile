VSIXNAME:=VSScripts/bin/release/VSScripts.vsix
MKDIR:=mkdir.exe -p
CP:=cp

.PHONY:default
default:
	$(error must specify target)

.PHONY:installer
installer:
	$(MKDIR) ./installers
	$(CP) $(VSIXNAME) ./installers/VSScripts-$(shell cmd /c vsixver.py $(VSIXNAME)).vsix
