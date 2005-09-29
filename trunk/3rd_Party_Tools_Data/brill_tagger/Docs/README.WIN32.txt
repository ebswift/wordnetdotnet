
The Win32 edition of Eric Brill's rule based tagger is made available
by Paul Maddox. Full copyright is retained by the original author and
is included in COPYRIGHT.txt.

This cross-compilation was produced by Paul Maddox. Please send any
matters regarding this edition to paulmdx@hotmail.com.

======================================================================


This document was produced to provide a bridge between Eric's original
documentation and the Win32 edition. Some of the information shown
below has been borrowed from the original documents with any
necessary ammendments made. For a full overview of the taggers
capabilities and its methodology it is worth reading the original
documentation in the Docs directory.

Paul Maddox. 5 February, 2001.


======================================================================

Q U I C K   S T A R T

The following is designed for users who want to get tagging straight
away, and don't care too much about the works and full capabilites.

*COMPILATION*

The compiler used is Microsoft Visual C++ 6.0. The project files have
not been tried with earlier versions.

First open the project workspace Tagger.dsw.

Next click the [Build] + [Set Active Configuration] menu option. Then
choose "tagger - Win32 Release".

Press F7 and let it go! It will come up with a lot of warnings, mainly
due to harmless cross-compilation factors. These can be completely
ignored.

This should make three executables in the newly created Release
directory where you unzipped the files: tagger.exe, sst.exe, fst.exe.

*TAGGING*

Copy tagger.exe, fst.exe and sst.exe from the Release driectory into
the Data directory.

I have written a couple of simple batch files to allow users to tag
documents easily without worrying about what data files they need to
use. I recommend reading the documentation further down to get a more
complete view of how the software works.

tag-br.bat  -- Tag using data trained from the BROWN corpus
tag-wsj.bat -- Tag using data trained from the WALL STREET JOURNAL
               corpus

Both batch files can be used as so:

  tag-br.bat inputfile.txt taggedfile.txt
  
  tag-wsj.bat inputfile.txt taggedfile.txt

======================================================================

F U L L   P A R A M A T E R S

The following contains the full paramaters for the tagger.exe program.
Both sst.exe and fst.exe are executed through this program. This text
has been lifted from README.QUICK.txt.

To execute the program open a DOS prompt and type:

tagger LEXICON YOUR-CORPUS BIGRAMS LEXICALRULEFULE CONTEXTUALRULEFILE 

where YOUR-CORPUS is the file name of the corpus you wish to have
tagged, and the other files are all provided with the tagger.

Options (which are typed after the file names) are:

-h             :: help

-w wordlist    :: provide an extra set of words beyond those in LEXICON.
                  For more information about this, read README.LONG.

-i filename    :: writes intermediate results from start state tagger
                  into filename

-s number      :: processes the corpus to be tagged "number" lines at
                  a time.  This should be specified if memory problems
                  result from trying to process too large a corpus at
                  once.  For more information about this, read
                  README.LONG.txt.

-S             :: use start state tagger only.

-F             :: use final state tagger only.  In this case,
                  YOUR-CORPUS is a tagged corpus, whose taggings will
                  be changed according to the final-state-tagger
                  contextual rules.  YOUR-CORPUS should be a tagged
                  corpus ONLY when using this option.

	
The tagger writes to standard output.

======================================================================

O M I S S I O N S

My own reason for the cross-compilation was to provide a Win32 app
that I could tag with. My intention was not to reproduce the
functionality of the unix version. Hence, there are a number of small
omissions.

1) The learning code has not been implemented. The code is provided
   in the Learner.src directory for completeness. This may be an
   addition I make in the future.

2) The N-Best tagger code has not been added to the project. As this
   code was still in development, it is unlikely it will be part of
   the Win32 edition unless there is a demand for it.

======================================================================
