import sys,vsscripts

print '<Bitmap guid="guidDigits" href="Resources\\digits.png" usedList="%s"/>'%(",".join(["icon%02d"%i for i in range(vsscripts.max_num_scripts())]))

print '<GuidSymbol name="guidDigits" value="{0A811C08-2416-4CD9-88AE-F3C0E91C2ED3}">'
for i in range(vsscripts.max_num_scripts()):
    print '<IDSymbol name="icon%02d" value="%d"/>'%(i,i+1)
print '</GuidSymbol>'

for i in range(vsscripts.max_num_scripts()):
    print '''<Button guid="guidVSScriptsCmdSet" id="cmdIdScript%(n)02d" priority="0x0101" type="Button">
  <Parent guid="guidVSScriptsCmdSet" id="ScriptsGroup" />
  <Icon guid="guidDigits" id="icon%(n)02d" />
  <CommandFlag>DynamicVisibility</CommandFlag>
  <CommandFlag>TextChanges</CommandFlag>
  <Strings>
    <ButtonText>Script %(n)02d</ButtonText>
  </Strings>
</Button>'''%{"n":i}

for i in range(vsscripts.max_num_scripts()):
    print '<IDSymbol name="cmdIdScript%02d" value="0x%X"/>'%(i,vsscripts.base_id()+i)
    
