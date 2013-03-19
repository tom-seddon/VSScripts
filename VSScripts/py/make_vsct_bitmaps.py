import sys

print '<Bitmap guid="guidDigits" href="Resources\\digits.png" usedList="%s"/>'%(",".join(["digit%d"%i for i in range(int(sys.argv[1]))]))

print '<GuidSymbol name="guidDigits" value="{0A811C08-2416-4CD9-88AE-F3C0E91C2ED3}">'
for i in range(int(sys.argv[1])):
    print '<IDSymbol name="digit%d" value="%d"/>'%(i,i+1)
print '</GuidSymbol>'
