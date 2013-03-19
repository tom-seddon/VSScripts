import sys

for i in range(int(sys.argv[1])):
    print "public const int cmdidScript%d=0x%X;"%(i,512+i)
