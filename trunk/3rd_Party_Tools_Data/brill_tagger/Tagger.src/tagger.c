#define		R_OK		1
#define		X_OK		1

#include <stdlib.h>
#include <io.h>

/* Copyright @ 1993 MIT and University of Pennsylvania */
/* Written by Eric Brill */
/*THIS SOFTWARE IS PROVIDED "AS IS", AND M.I.T. MAKES NO REPRESENTATIONS 
OR WARRANTIES, EXPRESS OR IMPLIED.  By way of example, but not 
limitation, M.I.T. MAKES NO REPRESENTATIONS OR WARRANTIES OF 
MERCHANTABILITY OR FITNESS FOR ANY PARTICULAR PURPOSE OR THAT THE USE OF 
THE LICENSED SOFTWARE OR DOCUMENTATION WILL NOT INFRINGE ANY THIRD PARTY 
PATENTS, COPYRIGHTS, TRADEMARKS OR OTHER RIGHTS.   */


#include <stdio.h>
#include <string.h>
//#include <unistd.h>
#include "..\Utils.src\useful.h"
#define START_PROG "sst.exe"
#define END_PROG "fst.exe"
  

main(argc, argv)
  int             argc;
  char           **argv;
{
  int corpussize = 0;
  int linenums = 0;
  int tagnums = 0;
  char line[2048];
  FILE *fp,*CORPUS;
  FILE *lexicon;
  char COMMAND[1024];
  int c;
  int splitnum;
  char deflt[20];
  char *wdlistname; 
  /*extern */char *optarg;
  extern int optind;
  char *intermed;
  int SPLIT,WORDLIST;
  int START_ONLY_FLAG,FINAL_ONLY_FLAG;
  int temp,count;
  int numlines;
  int INTERMED;
  char tempname1[L_tmpnam];
  char **tempargv;
  int tempargc;

  if (access(START_PROG,X_OK) != 0 ||
      access(END_PROG,X_OK) != 0) {
  fprintf(stderr,"YOU MUST RUN THIS PROGRAM IN THE SAME DIRECTORY AS %s and %s\n",
	 START_PROG,END_PROG);
  fprintf(stderr,"AND %s and %s MUST HAVE EXECUTE PERMISSION SET\n",START_PROG,
	 END_PROG);
  exit(0);
}


  SPLIT =0;
  WORDLIST=0;
  INTERMED=0;
  START_ONLY_FLAG = 0;
  FINAL_ONLY_FLAG = 0;
  

  tempargc=1;
  tempargv=argv;
  ++tempargv;
  while(tempargc<argc && *tempargv[0] != '-') {
    ++tempargc;
    ++tempargv;
  }
  if (tempargc<argc && *tempargv[0] == '-' && argc!= 2 && tempargc<5) {
    fprintf(stderr,"ARGUMENTS MUST COME BEFORE OPTIONS\n");
    exit(0);
  }
  if (tempargc < argc && *tempargv[0] == '-') {
    tempargv--;
    tempargc = argc-tempargc+1; }
  else {
    tempargc =argc;
    tempargv=argv;}
  while( (c = getopt(tempargc, tempargv, "s:w:i:SFh")) != -1) {
    switch (c) {
    case 'w': 
      WORDLIST = 1;
      wdlistname = mystrdup(optarg);
      if (access(wdlistname,R_OK) != 0) {
	fprintf(stderr,"COULD NOT OPEN %s\n",wdlistname);
	exit(0);}
      break;
    case 'i':
      INTERMED=1;
      intermed = mystrdup(optarg);
      break;
    case 's':
      SPLIT = 1;
      splitnum = atoi(optarg);
      if (splitnum < 100) {
	fprintf(stderr,"TOO SMALL A NUMBER TO SPLIT ON.  WILL NOT BE EFFICIENT\n");
	exit(0);
      }
      break;
    case 'S':
       START_ONLY_FLAG = 1;
       break;
    case 'F':
        FINAL_ONLY_FLAG = 1;
        break;
    case 'h':
    case '?':
      printf("usage: %s LEXICON CORPUS-TO-TAG BIGRAMS LEXICALRULEFILE CONTEXTUALRULEFILE [-w WORDLIST] [-s SPLITNUMBER] [-i INTERMEDFILE] -S -F \n",argv[0]);
      exit(0);
      break;
    }
  }
  
  temp = (2*SPLIT + 2*WORDLIST +2*INTERMED + FINAL_ONLY_FLAG + 
	  START_ONLY_FLAG);
  if (argc < (6+temp)) {
    fprintf(stderr,"TOO FEW ARGUMENTS\n");
    exit(0);}
  if (argc > (6+temp)) {
    fprintf(stderr,"TOO MANY ARGUMENTS\n");
    exit(0);}
  for(count=1;count<argc-temp;++count) {
    if (access(argv[count],R_OK) != 0) {
      fprintf(stderr,"COULD NOT OPEN %s\n",argv[count]);
      exit(0);}
  }
  if (INTERMED &&  SPLIT) {
    fprintf(stderr,"Intermediate Files Not Permitted With Split option\n");
    exit(0);
  }
  if ( (START_ONLY_FLAG && FINAL_ONLY_FLAG) ||
       ((START_ONLY_FLAG || FINAL_ONLY_FLAG) && INTERMED) ||
       ( FINAL_ONLY_FLAG && WORDLIST) || 
       ( START_ONLY_FLAG && SPLIT)) {
    fprintf(stderr,"This set of options does not make sense.\n");
    exit(0);
  }

  if (! START_ONLY_FLAG) {
    CORPUS = fopen(argv[2],"r");
    while(fgets(line,sizeof(line),CORPUS) != NULL) {
      if (not_just_blank(line)){
	line[strlen(line) - 1] = '\0';
	corpussize +=2;
	corpussize += num_words(line) + 1;}
    }
    fclose(CORPUS);
    
    lexicon = fopen(argv[1],"r");
    while(fgets(line,sizeof(line),lexicon) != NULL) {
      if (not_just_blank(line)) {
	++linenums;
	line[strlen(line) - 1] = '\0';
	tagnums += num_words(line);
      }
    }
    fclose(lexicon);
  }


  if (! SPLIT) {
    if (! WORDLIST) {
      if (! INTERMED) { /* -intermed,-split,-wordlist */
	if (START_ONLY_FLAG) 
	  sprintf(COMMAND,"%s %s %s %s %s",
		START_PROG, argv[1], argv[2], argv[3], argv[4]);
	else if (FINAL_ONLY_FLAG)
	  sprintf(COMMAND,"cat %s | %s %s %s %d %d %d",
		  argv[2],
		  END_PROG, argv[5], argv[1],corpussize,linenums,tagnums);
	else
	  sprintf(COMMAND,"%s %s %s %s %s | %s %s %s %d %d %d",
		START_PROG, argv[1], argv[2], argv[3], argv[4],
		END_PROG, argv[5], argv[1],corpussize,linenums,tagnums);
      }
      else { /* +intermed  -split -wordlist*/
	sprintf(COMMAND,"%s %s %s %s %s | tee %s |  %s %s %s %d %d %d",
		START_PROG, argv[1], argv[2], argv[3], argv[4],
		intermed,
		END_PROG, argv[5], argv[1],corpussize,linenums,tagnums);
      }
    }
    else /* - split + wordlist */   {
      if (! INTERMED)  /* -intermed */
	if (START_ONLY_FLAG)
	  sprintf(COMMAND,"%s %s %s %s %s %s",
		  START_PROG, argv[1], argv[2], argv[3], argv[4], wdlistname);
	else
	    sprintf(COMMAND,"%s %s %s %s %s %s | %s %s %s %d %d %d",
		  START_PROG, argv[1], argv[2], argv[3], argv[4], wdlistname,
		  END_PROG, argv[5], argv[1],corpussize,linenums,tagnums);
      else  /* + intermed */
	sprintf(COMMAND,"%s %s %s %s %s %s | tee %s |  %s %s %s %d %d %d",
		START_PROG, argv[1], argv[2], argv[3], argv[4],
		wdlistname,
		intermed,
		END_PROG, argv[5], argv[1],corpussize,linenums,tagnums);
    }
    system(COMMAND);
  }
  else {  /* + split -intermed */
    numlines=0;
    CORPUS = fopen(argv[2],"r");
    tmpnam(tempname1);
    if (! WORDLIST) { /* - wordlist */
      if (FINAL_ONLY_FLAG)
	sprintf(COMMAND,"cat %s | %s %s %s %d %d %d",
		tempname1, 
		END_PROG, argv[5], argv[1],corpussize,linenums,tagnums);
      else 
	sprintf(COMMAND,"%s %s %s %s %s | %s %s %s %d %d %d",
		START_PROG, argv[1], tempname1, argv[3], argv[4],
		END_PROG, argv[5], argv[1],corpussize,linenums,tagnums);
    }
    else { /* + wordlist */
      sprintf(COMMAND,"%s %s %s %s %s %s | %s %s %s %d %d %d",
	      START_PROG, argv[1], tempname1, argv[3], argv[4], wdlistname,
	      END_PROG, argv[5], argv[1], corpussize, linenums, tagnums); 
    }
    fp = fopen(tempname1,"w");
    while(fgets(line,sizeof(line),CORPUS) != NULL) {
      if (not_just_blank(line)) {
	++numlines;
	fputs(line,fp);
	if ((numlines % splitnum) == 0) {
	  fclose(fp);
	  system(COMMAND);
	  fp = fopen(tempname1,"w");
	}
      }
    }
    if ((numlines %splitnum) != 0) {
      fclose(fp);
      system(COMMAND);
    }
  }
}


