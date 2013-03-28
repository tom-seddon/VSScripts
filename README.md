VSScripts
=========

The VSScripts extension package adds a simple macro facility to Visual
Studio 2012 for inserting and transforming text. It runs a command
line program from within the IDE, feeding it the selection via stdin,
and captures the output. Then it replaces the selection with the
output.

You can write your macros in any language you like, pretty
much. Often, you won't need anything fancier than a simple batch file.

(If you've ever used the User Scripts facility of Xcode 3.x, or emacs's
`shell-command-on-region`, this should be familiar.)

installation
============

installing pre-built version
----------------------------

You'll need Visual Studio 2012. Update 1 might be required.

Look in `installers` folder for .vsix file with largest numbers
attached.

Double click that .vsix file, and follow the instructions.

building it yourself
--------------------

You'll need the Visual Studio 2012 SDK, which you can get from here:

http://www.microsoft.com/en-gb/download/details.aspx?id=30668

Once you have that installed, load SLN into Visual Studio 2012, build,
and run - it will run the experimental version of Visual Studio.

Once you're happy, find the VSIX file in the `bin\Debug` or
`bin\Release` folder and double-click to install.

quick walkthrough
=================

Once you have the VSScripts extension installed, run Visual
Studio 2012.

Note new SCRIPTS menu.

![images/SCRIPTSMenu.png](images/SCRIPTSMenu.png?raw=true)

It might not look like much.

![images/SCRIPTSMenuEmpty.png](images/SCRIPTSMenuEmpty.png)

Click `Scripts...` to reveal the Scripts dialog.

![images/ScriptsDialog.png](images/ScriptsDialog.png)

Click `Add...` to add a new script.

![images/AddScript00DialogEmpty.png](images/AddScript00DialogEmpty.png)

You get to specify details straight away - let's use one of the
examples supplied by setting things up as follows:

<dl>
<dt><code>Command Line</code></dt>
<dd>use the `...` button to find the <code>examples\InsertArrow.bat</code> file from the distribution.</dd>
<dt><code>Name</code></dt>
<dd><code>Insert -></code></dd>
<dt><code>stdout</code></dt>
<dd>leave this at the default of <code>ReplaceSelection</code>.</dd>
<dt><code>stderr</code></dt>
<dd>leave this at the default of <code>LastLineToStatusBar</code>.</dd>
<dt><code>stdin</code></dt>
<dd>leave this at the default of <code>None</code>.</dd>
</dl>

You're done! Click `OK`. Your script was added.

![images/ScriptsDialogWithScript00.png](images/ScriptsDialogWithScript00.png)

Click `OK` to get back to the text editor, and put the caret somewhere you can see it.

![images/SourceFileBeforeInsertingArrow.png](images/SourceFileBeforeInsertingArrow.png)

Visit the scripts menu - note your script is there!

![images/SCRIPTSMenuWithInsertArrow.png](images/SCRIPTSMenuWithInsertArrow.png)

Select it!

![images/SourceFileAfterInsertingArrow.png](images/SourceFileAfterInsertingArrow.png)

Result! (Even if the compiler doesn't agree.)

Nearly done. Let's assign a keyboard shortcut.

Look in the `SCRIPTS` menu - note the `00` icon. You can also see a
`0` next to the script's name, if you visit the scripts dialog. And it
also said `Script 00` when adding it. This is trying to tell you
something: that this is script 00.

(Don't forget this, because the options dialog is modal.)

Go to the options dialog, keyboard section, and enter `script00` in
the `Show commands containing:` box. There'll probably be one command
matching it: `Scripts.Script00`.

![images/ToolsOptionsKeyboardWithScript00Selected.png](images/ToolsOptionsKeyboardWithScript00Selected.png)

`Scripts.Script00` is the one that runs Script 00 (hopefully this is
not confusing).

Assign it a keyboard shortcut. You're also best off setting it to be
usable in the text editor only, but it's up to you.

![images/ToolsOptionsKeyboardAssigningKeyboardShortcut.png](images/ToolsOptionsKeyboardAssigningKeyboardShortcut.png)

script settings summary
=======================

Command line
------------

This can be anything that you can type at the command prompt, pretty
much. The command line is executed via `CMD /C`, so you can use
builtin commands (`SET`, `VER`, `ECHO`, etc.), or invoke batch files,
or run scripts with file type assocations (e.g., python scripts), and
so on.

`stdout` and `stderr` settings
------------------------------

<dl>
<dt>Discard</dt>
<dd>the output is discarded.</dd>
<dt>ReplaceSelection</dt>
<dd>the output replaces the current selection. (The last newline is discarded, if there is one, so if you really want the result to end with a newline then you must print an extra one. This is a .NET limitation.)</dd>
<dt>FirstLineToStatusBar</dt>
<dd>the first non-empty line of the output is displayed on the status bar.</dd>
<dt>LastLineToStatusBar</dt>
<dd>the last non-empty line of the output is displayed on the status bar.</dd>
<dt>ReplaceOutputWindow</dt>
<dd>the output is sent to an output window pane called <code>VSScripts</code>, replacing the previous contents.</dd>
<dt>AppendToOutputWindow</dt>
<dd>the output is appended to an output window pane called <code>VSScripts</code>.</dd>
</dl>

`stdin` settings
----------------

<dl>
<dt>None</dt>
<dd>no input is supplied.</dd>
<dt>CurrentLine</dt>
<dd>the contents of the current line, excluding line ending, is sent.</dd>
<dt>Selection</dt>
<dd>the contents of the selection is sent.</dd>
</dl>

* making your own scripts

You can write scripts in any language. Just set up the stdin, stdout
and stderr options as appropriate for its behaviour.

There are a few pieces of information supplied via environment
variables:

<dl>
<dt>FullPath</dt>
<dd>full path of file being edited.</dd>
<dt>Filename</dt>
<dd>name of file being edited, no path or extension.</dd>
<dt>Extension</dt>
<dd>extension of name of file being edited, with a leading <code>.</code>.</dd>
<dt>Directory</dt>
<dd>directory of file being edited, excluding drive letter.</dd>
<dt>RootDir</dt>
<dd>drive letter of file being edited, with trailing separator.</dd>
<dt>TabSize</dt>
<dd>width of a tab, in spaces.</dd>
</dl>

bugs/problems
=============

- Box selection behaviour is far from perfect. It works well enough
  for my purposes though and I'm sick of poking at it for now.

- Should be cleverer about box selections. With box selection active,
  if output includes a newline, should replace box with new output. If
  output doesn't include a newline, and box selection is 0 chars wide,
  should treat box selection as multiple cursors. That should probably
  cater for most cases. At the moment it's easy to end up with a big
  mess.

- Needs more stdin/stdout/stderr options.

- Would be nice to be able to rearrange the scripts order.

- Would be nice to be able to name the commands dynamically rather
  than have the stupid numbers - not sure Visual Studio will do this.

- Should be able to supply dynamic information to scripts via command
  line.

- Should be able to handle I/O via temp files, if the script would
  need that.

(etc., etc.)
