import sys

for i in range(int(sys.argv[1])):
    print '<IDSymbol name="cmdIdScript%d" value="0x%X"/>'%(i,512+i)
    
