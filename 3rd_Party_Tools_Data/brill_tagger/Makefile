#  Makefile for Transformation Rule-based Part of Speech Tagger

SHELL = /bin/sh
RM    = /bin/rm -f

#HOMEDIR  = .

SRCDIR     = ./Utils
INCLUDEDIR = ./Utils
LIBDIR     = ./Utils
BINDIR     = ./Bin_and_Data
T_DIR      = ./Tagger_Code
L_DIR      = ./Learner_Code


SRC = $(SRCDIR)/lex.c \
      $(SRCDIR)/useful.c \
      $(SRCDIR)/darray.c \
      $(SRCDIR)/registry.c \
      $(SRCDIR)/memory.c

OBJ = $(LIBDIR)/lex.o \
      $(LIBDIR)/useful.o \
      $(LIBDIR)/darray.o \
      $(LIBDIR)/registry.o \
      $(LIBDIR)/memory.o


C_INCL    =  -I${INCLUDEDIR} 


FINAL = my_tagger

L_TARGETS = contextual-rule-learn \
	    kbest-contextual-rule-learn \
	    fix-kbest-rule-learn

T_TARGETS = final-state-tagger \
	    start-state-tagger \
	    nbest-tagger \
            tagger


EXECS = ${L_TARGETS} ${T_TARGETS}


CFLAGS = -O -I. ${C_INCL}

${FINAL} : ${EXECS}

$(LIBDIR)/lex.o : $(SRCDIR)/lex.c
	gcc -c -o $@ $<

$(LIBDIR)/useful.o : $(SRCDIR)/useful.c
	gcc -c -o $@ $<

$(LIBDIR)/darray.o : $(SRCDIR)/darray.c
	gcc -c -o $@ $<

$(LIBDIR)/registry.o : $(SRCDIR)/registry.c
	gcc -c -o $@ $<

$(LIBDIR)/memory.o : $(SRCDIR)/memory.c
	gcc -c -o $@ $<

${T_TARGETS} : ${OBJ} 
	gcc $(CFLAGS) ${OBJ} -o $(BINDIR)/$@ $(T_DIR)/$@.c

${L_TARGETS} : ${OBJ}
	gcc $(CFLAGS) ${OBJ} -o $(BINDIR)/$@ $(L_DIR)/$@.c



