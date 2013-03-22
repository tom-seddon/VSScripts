VSIXNAME:=VSScripts/bin/debug/VSScripts.vsix

.PHONY:default
default:
	$(error must specify target)

.PHONY:cpvsix
cpvsix:
	cp $(VSIXNAME) ./VSScripts-$(shell cmd /c vsixver.py $(VSIXNAME)).vsix

