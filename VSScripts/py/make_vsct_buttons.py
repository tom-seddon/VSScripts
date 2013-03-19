import sys

for i in range(int(sys.argv[1])):
    print '''<Button guid="guidVSScriptsCmdSet" id="cmdIdScript%(n)d" priority="0x0101" type="Button">
  <Parent guid="guidVSScriptsCmdSet" id="ScriptsGroup" />
  <Icon guid="guidImages" id="bmpPic1" />
  <CommandFlag>DynamicVisibility</CommandFlag>
  <CommandFlag>TextChanges</CommandFlag>
  <Strings>
    <ButtonText>Script %(n)d</ButtonText>
  </Strings>
</Button>'''%{"n":i}
