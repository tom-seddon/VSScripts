#!env python
import sys

for i in range(len(sys.argv)):
    print "sys.argv[%d]=\"%s\""%(i,sys.argv[i])
