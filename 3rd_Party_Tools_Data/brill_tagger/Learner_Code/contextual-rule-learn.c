/*  Copyright @ 1993 MIT and University of Pennsylvania*/
/* Written by Eric Brill */

#include <stdio.h>
#include <fcntl.h>
#include <string.h>
#include <malloc.h>
#include <unistd.h>
#include "lex.h"
#include "darray.h"
#include "registry.h"
#include "memory.h"
#include "useful.h"

#define THRESHOLD 2  /* SPECIFY THE THRESHOLD FOR LEARNING.  WHEN NO RULES */
		     /* CAN BE FOUND WHOSE IMPROVEMENT IS GREATER THAN THE */
		     /* THRESHOLD, LEARNING STOPS */

/* NUMTAGS and NUMWDS are (roughly) guesses of the max number of words
   or tags that could appear to the right of a particular tag */

#define NUMTAGS 100  
#define NUMWDS  300


void implement_change();
char staart[] = "STAART";
char **correct_tag_corpus,**word_corpus,**guess_tag_corpus;
int correct_tag_corpus_index,word_corpus_index,guess_tag_corpus_index;
int localbest;
char localbestthing[100];
char *wrong,*right;
char *tempforcreate;

/*    argv 0 = correct
           1 = guess
	   2 = file for rules
	   3 = lexicon file from lexical rule corpus
*/
/********************************************************************/
void increment_array(thehash,thestr) 
     Registry *thehash;
     char  *thestr;
{
  int *numforhash;


  if ((numforhash = (int *)Registry_get(*thehash,thestr)) != NULL) {
    (*numforhash)++; 
  }
  else {
    numforhash = (int *)malloc(sizeof(int));
    *numforhash = 1;
    Registry_add(*thehash,thestr, (int *)numforhash); 
  }
}
/***********************************************************************/
void increment_array_create(thehash,thestr)
     Registry *thehash;
     char  *thestr;
{
  int *numforhash;


  if ((numforhash = (int *)Registry_get(*thehash,thestr)) != NULL) {
    (*numforhash)++; 
  }
  else {
    numforhash = (int *)malloc(sizeof(int));
    *numforhash = 1;
    tempforcreate = malloc((1+strlen(thestr))*sizeof(char));
    strcpy(tempforcreate,thestr);
    Registry_add(*thehash,tempforcreate,(int *)numforhash); 
  }
}
/***********************************************************************/
/********************************************************************/
void decrement_array(thehash,thestr)
     Registry *thehash;
     char  *thestr;
{
  int *numforhash;


  if ((numforhash = (int *)Registry_get(*thehash,thestr)) != NULL) {
    (*numforhash)--; 
  }
  else {
    numforhash = (int *)malloc(sizeof(int));
    *numforhash = -1;
    Registry_add(*thehash,thestr, (int *)numforhash); 
  }
}
/***********************************************************************/
void decrement_array_create(thehash,thestr)
     Registry *thehash;
     char  *thestr;
{
  int *numforhash;


  if ((numforhash = (int *)Registry_get(*thehash,thestr)) != NULL) {
    (*numforhash)--; 
  }
  else {
    numforhash = (int *)malloc(sizeof(int));
    *numforhash = -1;
    tempforcreate = malloc((1+strlen(thestr))*sizeof(char));
    strcpy(tempforcreate,thestr);
    Registry_add(*thehash,tempforcreate,(int *)numforhash); 
  }
}
/***********************************************************************/
void check_counts(goodhash,label)
     Registry *goodhash;
     char *label;
{
  Darray tempkey,tempval;
  int tempcount,*hashtempstr,hashtempval,tempbest;
  int FREEFLAG;
  
  FREEFLAG = 0;

  if (strcmp(label,"PREVBIGRAM") == 0 ||
      strcmp(label,"NEXTBIGRAM") == 0 ||
      strcmp(label,"WDPREVTAG") == 0 ||
      strcmp(label,"WDNEXTTAG") == 0 ||
      strcmp(label,"WDAND2AFT") == 0 ||
      strcmp(label,"WDAND2BFR") == 0 ||
      strcmp(label,"WDAND2TAGAFT") == 0 ||
      strcmp(label,"WDAND2TAGBFR") == 0 ||
      strcmp(label,"RBIGRAM") == 0 ||
      strcmp(label,"LBIGRAM") == 0 ||
      strcmp(label,"SURROUNDTAG") == 0) 
    FREEFLAG = 1;

  tempkey = Darray_create();
  Darray_hint(tempkey,10,Registry_entry_count(*goodhash));
  tempval = Darray_create();
  Darray_hint(tempval,10,Registry_entry_count(*goodhash));

  Registry_fetch_contents(*goodhash,tempkey,tempval);
  for (tempcount=0;tempcount<Darray_len(tempkey);++tempcount) {
    if ( (tempbest = *(int *)Darray_get(tempval,tempcount)) > localbest) {
      localbest = tempbest;
      sprintf(localbestthing,"%s %s %s %s",wrong,right,label,Darray_get(tempkey,tempcount)); 
    }
    free(Darray_get(tempval,tempcount));
    if (FREEFLAG)
      free(Darray_get(tempkey,tempcount));
  }
  Darray_destroy(tempval);
  Darray_destroy(tempkey);
  Registry_destroy(*goodhash);
}

/**********************************************************************/

void init_hash(thehash,numhint)
     Registry *thehash;
     int numhint;
{
  *thehash =  Registry_create(Registry_strcmp,Registry_strhash);
  Registry_size_hint(*thehash,numhint);
}
/***********************************************************************/


main(argc, argv)

     int             argc;
     char           *argv[];
{



Darray errorlist,temperrorkey,temperrorval;
Registry errorlistcount,SEENTAGGING,WORDS;

FILE *correct_file, *guess_file, *error_list,*correct_out;
char line[5000];  /* input line buffer */
char **split_ptr,**split_ptr2;

char tempfile1[128],tempfile2[128];


char wdpair[1024],*wdpair2;
char *tempstr,*tempstr2;
int CONTINUE = 10000;
int count,count2,count3,numwrong,lengthcount;
char globalprint[500];
char systemcall[500];
char forpasting[500];
int globalbest = 0;
char flag[20];
Registry curwd,wdand2aft,wdand2bfr;
Registry wdand2tagaft,wdand2tagbfr;
Registry wdnexttag,wdprevtag;
Registry rbigram,lbigram;
Registry next1tag,prev1tag;
Registry next1or2tag,prev1or2tag;
Registry next1or2or3tag,prev1or2or3tag;
Registry next1wd,prev1wd;
Registry next1or2wd,prev1or2wd;
Registry nextbigram,prevbigram;
Registry surroundtag;
Registry next2tag,prev2tag;
Registry next2wd,prev2wd;
int printscore;
FILE *allowedmovefile;
char **perl_split_ptr,**perl_split_ptr2,*atempstr,atempstr2[1024];
char space[500];
char onewdbfr[512],onewdaft[512];
char onetagbfr[512],twotagbfr[512],onetagaft[512],twotagaft[512];
int linenums,tagnums;
int words_in_good,words_in_bad;

linenums=tagnums=0; /* used to count number of words/tags in lexicon, for */
		    /* sizing the associative arrays */

words_in_bad = words_in_good = 0;


tmpnam(tempfile1);
tmpnam(tempfile2);


/* just count number of words,tags for sizing arrays */
allowedmovefile = fopen(argv[4],"r");
while(fgets(line,sizeof(line),allowedmovefile) != NULL) {
  if (not_just_blank(line)) {
    ++linenums;
    line[strlen(line) - 1] = '\0';
    tagnums += num_words(line);
  }
}
fclose(allowedmovefile);

/*SEENTAGGING is a hash table of word/tag pairs, from the lexicon*/
SEENTAGGING = Registry_create(Registry_strcmp,Registry_strhash);
Registry_size_hint(SEENTAGGING,tagnums);
/* WORDS is a hash table of all words in the lexicon */
WORDS = Registry_create(Registry_strcmp,Registry_strhash);
Registry_size_hint(WORDS,linenums);

allowedmovefile = fopen(argv[4], "r"); /* read in the lexicon to record all */
				       /* tags seen with all words in the */
				       /* lexicon */
while(fgets(line,sizeof(line),allowedmovefile) != NULL) {
  if (not_just_blank(line)) {
    line[strlen(line) - 1] = '\0';
    perl_split_ptr = perl_split(line);
    perl_split_ptr2 = perl_split_ptr;
    ++perl_split_ptr;
    atempstr= mystrdup(*perl_split_ptr2);
    Registry_add(WORDS,atempstr,(char *)1);
    while(*perl_split_ptr != NULL) {
      sprintf(space,"%s %s",*perl_split_ptr2,
	      *perl_split_ptr);
      atempstr=mystrdup(space);
      Registry_add(SEENTAGGING,atempstr,(char *)1);
      ++perl_split_ptr; }
    free(*perl_split_ptr2);
    free(perl_split_ptr2);
  }
}
fclose(allowedmovefile);

/* read through the gold standard once just for sizing array */
correct_file = fopen(argv[1],"r");
while(fgets(line,sizeof(line),correct_file) != NULL) {
  if (not_just_blank(line)) {
    line[strlen(line) - 1] = '\0';
    words_in_good += 2; /* add two start of sent tokens */
    words_in_good += num_words(line) + 1;
  }
}
fclose(correct_file);


correct_tag_corpus = (char **)malloc(sizeof(char *) * words_in_good);
word_corpus = (char **)malloc(sizeof(char *) * words_in_good);

/* read in the manually tagged gold standard corpus*/
correct_file = fopen(argv[1],"r");
correct_tag_corpus_index = 0;
word_corpus_index = 0;
while(fgets(line,sizeof(line),correct_file) != NULL) {
  if (not_just_blank(line)) {
    correct_tag_corpus[correct_tag_corpus_index++] = staart;
    correct_tag_corpus[correct_tag_corpus_index++] = staart;
    word_corpus[word_corpus_index++] = staart;
    word_corpus[word_corpus_index++] = staart;
    line[strlen(line)-1] = '\0';
    split_ptr = perl_split_independent(line);
    perl_split_ptr2 = split_ptr;
    while (*split_ptr != NULL) {
      word_corpus[word_corpus_index++] = *split_ptr;
      /*Darray_addh(word_corpus,*split_ptr);*/
      /*  while ((*(++*split_ptr)) != '/') { }*/
      *split_ptr = strrchr(*split_ptr,'/');
      **split_ptr = '\0';
      correct_tag_corpus[correct_tag_corpus_index++] = ++*split_ptr;
      /*Darray_addh(correct_tag_corpus,++*split_ptr);*/
      ++split_ptr;
    }
    free(perl_split_ptr2);
  }
}
fclose(correct_file);

printf("READ IN CORRECT FILE\n");

/* read through dummy-tagged file once to size an array*/
guess_file = fopen(argv[2],"r");
while(fgets(line,sizeof(line),guess_file) != NULL) {
  if (not_just_blank(line)) {
    line[strlen(line) - 1] = '\0';
    words_in_bad += 2; /* add two start of sent tokens */
    words_in_bad += num_words(line) + 1;
  }
}
fclose(guess_file);

/* the gold standard and the dummy-tagged corpus had better have the same
   number of words */
if (words_in_good != words_in_bad) {
  printf("%s and %s do not have the same number of words\n",argv[1],argv[2]);
  exit(0);
}
  
/* read in the guess corpus*/
  guess_tag_corpus = (char **)malloc(sizeof(char *) * words_in_bad);
  guess_file  = fopen(argv[2],"r"); /* read in our guess-tag corpus */
  guess_tag_corpus_index = 0;
  while(fgets(line,sizeof(line),guess_file) != NULL) {
    if (not_just_blank(line)) { 
      guess_tag_corpus[guess_tag_corpus_index++] = staart;
      guess_tag_corpus[guess_tag_corpus_index++] = staart;
      line[strlen(line)-1] = '\0';
      split_ptr = perl_split_independent(line); 
      split_ptr2 = split_ptr;
      while (*split_ptr != NULL) {
	tempstr = strrchr(*split_ptr,'/');
	tempstr++;
	tempstr2  = mystrdup(tempstr);
	guess_tag_corpus[guess_tag_corpus_index++] = tempstr2;
	free(*split_ptr);
	++split_ptr;
      }
      free(split_ptr2);
    }
  }
  fclose(guess_file);

printf("READ IN BAD FILE\n");



while(CONTINUE > THRESHOLD) { /* this is the rule learning loop.
				 we continue learning rules until the score
				 of the best rule found drops below the
				 THRESHOLD */

/* generate and sort confusion matrix ( tag x confused for tag y 
   with count z */
  printf("\n");
  printf("==========BEGINNING A LEARNING ITERATION===========\n");
  errorlist = Darray_create();
  Darray_hint(errorlist,10,500);
  temperrorkey = Darray_create();
  temperrorval = Darray_create();
  Darray_hint(temperrorkey,10,500);
  Darray_hint(temperrorval,10,500);

  init_hash(&errorlistcount,500);


  printscore=0;
  for(count=0;count<guess_tag_corpus_index;++count) {
    if
      (strcmp(guess_tag_corpus[count],correct_tag_corpus[count]) 
        != 0) { 
	++printscore; 
	sprintf(forpasting,"%s %s",guess_tag_corpus[count],
		                       correct_tag_corpus[count]);
	increment_array_create(&errorlistcount,forpasting);
      } 
  }

  error_list = fopen(tempfile1,"a");
  Registry_fetch_contents(errorlistcount,temperrorkey,temperrorval);  
  for (count=0;count<Darray_len(temperrorkey);++count) {
    if (*(int *)Darray_get(temperrorval,count) > THRESHOLD)
      fprintf(error_list,"%d %s\n",*(int *)Darray_get(temperrorval,count),
	                    Darray_get(temperrorkey,count));
    free(Darray_get(temperrorval,count));
    free(Darray_get(temperrorkey,count));
  }
  fclose(error_list);
  Darray_destroy(temperrorval);
  Darray_destroy(temperrorkey);
  Registry_destroy(errorlistcount);

  printf("NUMBER OF ERRORS: %d\n",printscore);

/* DUMB DUMB DUMB*/
  sprintf(systemcall,"cat %s | sort -rn > %s",tempfile1,tempfile2);
  system(systemcall);
  sprintf(systemcall,"mv %s %s",tempfile2,tempfile1);
  system(systemcall);
  
  error_list = fopen(tempfile1,"r");
  while(fgets(line,sizeof(line),error_list) != NULL) {
    line[strlen(line)-1] = '\0';
    tempstr = mystrdup(line);
    Darray_addh(errorlist,tempstr); 
  }
  fclose(error_list);
  sprintf(systemcall,"/bin/rm %s",tempfile1);
  system(systemcall);

  globalbest= 0;
  strcpy(globalprint,"");
  
  /* for each pair of tags in the confusion matrix . . .*/
  for (count=0;count<Darray_len(errorlist);++count) {

    localbest =0;
    strcpy(localbestthing,"");

    split_ptr = perl_split_independent(Darray_get(errorlist,count));

    wrong = split_ptr[1]; /* this is the bad tag in the confusion matrix */
    right = split_ptr[2]; /* this is the good tag */
    numwrong = atoi(split_ptr[0]); /* this is the number of confusions */
    if (numwrong > globalbest) {
      
      printf("From confusion matrix, tagged as: %s should be: %s\n",wrong,right);
      printf("Best rule found for this iteration so far: %d %s\n",globalbest,globalprint);
      init_hash(&curwd,NUMWDS/2);
      init_hash(&wdand2aft,(NUMWDS*NUMWDS)/4);
      init_hash(&wdand2bfr,(NUMWDS*NUMWDS)/4);
      init_hash(&wdand2tagaft,(NUMWDS*NUMTAGS)/4);
      init_hash(&wdand2tagbfr,(NUMWDS*NUMTAGS)/4);
      init_hash(&rbigram,(NUMWDS*NUMWDS)/4);
      init_hash(&lbigram,(NUMWDS*NUMWDS)/4);
      init_hash(&wdnexttag,(NUMWDS*NUMTAGS)/4);
      init_hash(&wdprevtag,(NUMWDS*NUMTAGS)/4);
      init_hash(&next1tag,NUMTAGS/2);
      init_hash(&prev1tag,NUMTAGS/2);
      init_hash(&next1or2tag,NUMTAGS/2);
      init_hash(&prev1or2tag,NUMTAGS/2);
      init_hash(&next1wd,NUMWDS/2);
      init_hash(&prev1wd,NUMWDS/2);
      init_hash(&next1or2wd,NUMWDS/2);
      init_hash(&prev1or2wd,NUMWDS/2);
      init_hash(&next1or2or3tag,NUMTAGS/2);
      init_hash(&prev1or2or3tag,NUMTAGS/2);
      init_hash(&nextbigram,NUMTAGS);
      init_hash(&prevbigram,NUMTAGS);
      init_hash(&surroundtag,NUMTAGS);
      init_hash(&next2tag,NUMTAGS/2);
      init_hash(&prev2tag,NUMTAGS/2);
      init_hash(&next2wd,NUMWDS/2);
      init_hash(&prev2wd,NUMWDS/2);

/* a proposed transformation tag X --> tag Y can have 3 outcomes.
   if neither X nor Y is the correct tag, then the transformation
   has no effect.  If X is correct, then the change is detremental.
   if Y is correct, then it is a good change. */

      lengthcount = correct_tag_corpus_index;
      for(count2=0;count2<lengthcount;++count2){
	sprintf(atempstr2,"%s %s",word_corpus[count2],right);
	if (Registry_get(WORDS,word_corpus[count2]) &&
	    ! Registry_get(SEENTAGGING,atempstr2)) 
	  strcpy(flag,"NOMATCH");
	else if (strcmp(guess_tag_corpus[count2],wrong) == 0 &&
	    strcmp(correct_tag_corpus[count2],right) == 0 ) 
	  strcpy(flag,"BADMATCH");
	else if (strcmp(guess_tag_corpus[count2],wrong) == 0 &&
	    strcmp(correct_tag_corpus[count2],wrong) == 0 ) 
	  strcpy(flag,"GOODMATCH");
	else 
	  strcpy(flag,"NOMATCH");


/* ok, the terminology is a bit screwed up.  If we have a "BADMATCH", that */
/* means the current tagging is bad, and the transformation will fix it */
/* ( a good thing ) */
	if (strcmp(flag,"BADMATCH") == 0) {
	  increment_array(&curwd,word_corpus[count2]);
	  if (count2 != lengthcount-1) {
	    strcpy(onewdaft,word_corpus[count2+1]);
	    strcpy(onetagaft,guess_tag_corpus[count2+1]);
	    sprintf(wdpair,"%s %s",word_corpus[count2],
		                 word_corpus[count2+1]);
    	    increment_array_create(&rbigram,wdpair);
	    sprintf(wdpair,"%s %s",word_corpus[count2],
		                  guess_tag_corpus[count2+1]);
	    increment_array_create(&wdnexttag,wdpair);
	    increment_array(&next1or2tag,
			    guess_tag_corpus[count2+1]);
	    increment_array(&next1or2or3tag,
			    guess_tag_corpus[count2+1]);
	    increment_array(&next1tag,guess_tag_corpus[count2+1]);
	    increment_array(&next1wd,word_corpus[count2+1]);
	    increment_array(&next1or2wd,word_corpus[count2+1]);
	  }
	  if (count2 < lengthcount-2) {
	    strcpy(twotagaft,guess_tag_corpus[count2+2]);
	    sprintf(forpasting,"%s %s",guess_tag_corpus[count2+1],
		                       guess_tag_corpus[count2+2]);
	    increment_array_create(&nextbigram,forpasting);
	    sprintf(forpasting,"%s %s",word_corpus[count2],
		                       word_corpus[count2+2]);
	    increment_array_create(&wdand2aft,forpasting);
	    sprintf(forpasting,"%s %s",word_corpus[count2],
		                       guess_tag_corpus[count2+2]);
	    increment_array_create(&wdand2tagaft,forpasting);
	    increment_array(&next2tag,guess_tag_corpus[count2+2]);
	    increment_array(&next2wd,word_corpus[count2+2]);
	    if (strcmp(guess_tag_corpus[count2+2],onetagaft) != 0)
	    { 
	      increment_array(&next1or2tag,
			      guess_tag_corpus[count2+2]);
	      increment_array(&next1or2or3tag,
			      guess_tag_corpus[count2+2]);
	    }
	    if (strcmp(word_corpus[count2+2],onewdaft) != 0)
	      increment_array(&next1or2wd,word_corpus[count2+2]);
	  }
	  if (count2 < lengthcount-3) {
	    if (strcmp(guess_tag_corpus[count2+3],onetagaft) != 0
		&&
		strcmp(guess_tag_corpus[count2+3],twotagaft) != 0)
	    increment_array(&next1or2or3tag,
			    guess_tag_corpus[count2+3]);
	  }
	  if (count2 != 0) {
	    strcpy(onewdbfr,word_corpus[count2-1]);
	    strcpy(onetagbfr,guess_tag_corpus[count2-1]);
	    sprintf(wdpair,"%s %s",word_corpus[count2-1],
		                  word_corpus[count2]);
    	    increment_array_create(&lbigram,wdpair);
	    sprintf(wdpair,"%s %s",guess_tag_corpus[count2-1],
		                  word_corpus[count2]);
    	    increment_array_create(&wdprevtag,wdpair);
	    increment_array(&prev1tag,guess_tag_corpus[count2-1]);
	    increment_array(&prev1wd,word_corpus[count2-1]);
	    increment_array(&prev1or2wd,word_corpus[count2-1]);
	    increment_array(&prev1or2tag,
			    guess_tag_corpus[count2-1]);
	    increment_array(&prev1or2or3tag,
			    guess_tag_corpus[count2-1]);
	    if (count2 < lengthcount-1) {
	      sprintf(forpasting,"%s %s",guess_tag_corpus[count2-1],
		                       guess_tag_corpus[count2+1]);
	      increment_array_create(&surroundtag,forpasting);
	    }
	  }
	  if (count2 > 1) {
	    strcpy(twotagbfr,guess_tag_corpus[count2-2]);
	    sprintf(forpasting,"%s %s",guess_tag_corpus[count2-2],
		                       guess_tag_corpus[count2-1]);
	    increment_array_create(&prevbigram,forpasting);
	    sprintf(forpasting,"%s %s",word_corpus[count2-2],
		                       word_corpus[count2]);
	    increment_array_create(&wdand2bfr,forpasting);
	    sprintf(forpasting,"%s %s",guess_tag_corpus[count2-2],
		                       word_corpus[count2]);
	    increment_array_create(&wdand2tagbfr,forpasting);
	    increment_array(&prev2tag,guess_tag_corpus[count2-2]);
	    increment_array(&prev2wd,word_corpus[count2-2]);
	    if (strcmp(guess_tag_corpus[count2-2],onetagbfr) !=
		0){ 
	      increment_array(&prev1or2tag,
			     guess_tag_corpus[count2-2]);
	      increment_array(&prev1or2or3tag,
			      guess_tag_corpus[count2-2]);
	    }
	    if (strcmp(word_corpus[count2-2],onewdbfr) !=
		0)
	      increment_array(&prev1or2wd,word_corpus[count2-2]);
	  }
	  if (count2 > 2) {
	    if (strcmp(guess_tag_corpus[count2-3],onetagbfr) != 0
		&&
		strcmp(guess_tag_corpus[count2-3],twotagbfr) != 0)
	    increment_array(&prev1or2or3tag,
			    guess_tag_corpus[count2-3]);
	  }
	}

/* this is the case where the current tagging is correct, and therefore the */
/* transformation will hurt */
	else if (strcmp(flag,"GOODMATCH") == 0) {
	  decrement_array(&curwd,word_corpus[count2]);
	  if (count2 != lengthcount-1) {
	    strcpy(onewdaft,word_corpus[count2+1]);
	    strcpy(onetagaft,guess_tag_corpus[count2+1]);
            sprintf(wdpair,"%s %s",word_corpus[count2],
		                  word_corpus[count2+1]);
    	    decrement_array_create(&rbigram,wdpair);
	    sprintf(wdpair,"%s %s",word_corpus[count2],
		                  guess_tag_corpus[count2+1]);
    	    decrement_array_create(&wdnexttag,wdpair);
	    decrement_array(&next1tag,guess_tag_corpus[count2+1]);
	    decrement_array(&next1wd,word_corpus[count2+1]);
	    decrement_array(&next1or2wd,word_corpus[count2+1]);
	    decrement_array(&next1or2tag,
			    guess_tag_corpus[count2+1]);
	    decrement_array(&next1or2or3tag,
			    guess_tag_corpus[count2+1]);
	  }
	  if (count2 < lengthcount-2) {
	    strcpy(twotagaft,guess_tag_corpus[count2+2]);
	    sprintf(forpasting,"%s %s",guess_tag_corpus[count2+1],
		                       guess_tag_corpus[count2+2]);
	    decrement_array_create(&nextbigram,forpasting);
	    sprintf(forpasting,"%s %s",word_corpus[count2],
		                       word_corpus[count2+2]);
	    decrement_array_create(&wdand2aft,forpasting);
	    sprintf(forpasting,"%s %s",word_corpus[count2],
		                       guess_tag_corpus[count2+2]);
	    decrement_array_create(&wdand2tagaft,forpasting);
	    decrement_array(&next2tag,guess_tag_corpus[count2+2]);
	    decrement_array(&next2wd,word_corpus[count2+2]);
	    if (strcmp(guess_tag_corpus[count2+2],onetagaft) !=0) {
	      decrement_array(&next1or2tag,
			      guess_tag_corpus[count2+2]);
	      decrement_array(&next1or2or3tag,
			      guess_tag_corpus[count2+2]); }
	    if (strcmp(word_corpus[count2+2],onewdaft) !=0)
	      decrement_array(&next1or2wd,word_corpus[count2+2]);
	  }
	  if (count2 < lengthcount-3) {
	    if (strcmp(guess_tag_corpus[count2+3],onetagaft) !=0 
		&&
		strcmp(guess_tag_corpus[count2+3],twotagaft) !=0 )
	      decrement_array(&next1or2or3tag,
			      guess_tag_corpus[count2+3]);
	  }
	  if (count2 != 0) {
	    strcpy(onewdbfr,word_corpus[count2-1]);
	    strcpy(onetagbfr,guess_tag_corpus[count2-1]);
	    sprintf(wdpair,"%s %s",word_corpus[count2-1],
		                  word_corpus[count2]);
    	    decrement_array_create(&lbigram,wdpair);
	    sprintf(wdpair,"%s %s",guess_tag_corpus[count2-1],
		                  word_corpus[count2]);
    	    decrement_array_create(&wdprevtag,wdpair);
	    decrement_array(&prev1tag,guess_tag_corpus[count2-1]);
	    decrement_array(&prev1wd,word_corpus[count2-1]);
	    decrement_array(&prev1or2wd,word_corpus[count2-1]);
	    decrement_array(&prev1or2tag,
			    guess_tag_corpus[count2-1]);
	    decrement_array(&prev1or2or3tag,
			    guess_tag_corpus[count2-1]);
	    if (count2 < lengthcount-1) {
	      sprintf(forpasting,"%s %s",guess_tag_corpus[count2-1],
		                       guess_tag_corpus[count2+1]);
	      decrement_array_create(&surroundtag,forpasting);
	    }
	  }
	  if (count2 >1 ) { 
	    strcpy(twotagbfr,guess_tag_corpus[count2-2]);
	    sprintf(forpasting,"%s %s",guess_tag_corpus[count2-2],
		                       guess_tag_corpus[count2-1]);
	    decrement_array_create(&prevbigram,forpasting);
	    sprintf(forpasting,"%s %s",word_corpus[count2-2],
		                       word_corpus[count2]);
	    decrement_array_create(&wdand2bfr,forpasting);
	    sprintf(forpasting,"%s %s",guess_tag_corpus[count2-2],
		                       word_corpus[count2]);
	    decrement_array_create(&wdand2tagbfr,forpasting);
	    decrement_array(&prev2tag,guess_tag_corpus[count2-2]);
	    decrement_array(&prev2wd,word_corpus[count2-2]);
	    if (strcmp(guess_tag_corpus[count2-2],onetagbfr) != 0)
	    { 
	      decrement_array(&prev1or2tag,
			      guess_tag_corpus[count2-2]);
	      decrement_array(&prev1or2or3tag,
			      guess_tag_corpus[count2-2]); }
	    if (strcmp(word_corpus[count2-2],onewdbfr) != 0)
	      decrement_array(&prev1or2wd,word_corpus[count2-2]);
	  }
	  if (count2 > 2) {
	    if (strcmp(guess_tag_corpus[count2-3],onetagbfr) != 0 
		&&
		strcmp(guess_tag_corpus[count2-3],twotagbfr) != 0)
	    decrement_array(&prev1or2or3tag,
			    guess_tag_corpus[count2-3]);
	  }
	}
      }  

/* now we go through all of the contexts we've recorded, and see which */
/* contextual trigger provides the biggest win */
check_counts(&prev1tag,"PREVTAG");
check_counts(&next1tag,"NEXTTAG");
check_counts(&next1or2tag,"NEXT1OR2TAG");
check_counts(&prev1or2tag,"PREV1OR2TAG");
check_counts(&next1wd,"NEXTWD");
check_counts(&prev1wd,"PREVWD");
check_counts(&rbigram,"RBIGRAM");
check_counts(&lbigram,"LBIGRAM");
check_counts(&wdnexttag,"WDNEXTTAG");
check_counts(&wdprevtag,"WDPREVTAG");
check_counts(&next1or2wd,"NEXT1OR2WD");
check_counts(&prev1or2wd,"PREV1OR2WD");
check_counts(&next1or2or3tag,"NEXT1OR2OR3TAG");
check_counts(&prev1or2or3tag,"PREV1OR2OR3TAG");
check_counts(&prevbigram,"PREVBIGRAM");
check_counts(&nextbigram,"NEXTBIGRAM");
check_counts(&surroundtag,"SURROUNDTAG");
check_counts(&next2tag,"NEXT2TAG");
check_counts(&prev2tag,"PREV2TAG");
check_counts(&next2wd,"NEXT2WD");
check_counts(&prev2wd,"PREV2WD");
check_counts(&curwd,"CURWD");
check_counts(&wdand2aft,"WDAND2AFT");
check_counts(&wdand2bfr,"WDAND2BFR");
check_counts(&wdand2tagaft,"WDAND2TAGAFT");
check_counts(&wdand2tagbfr,"WDAND2TAGBFR");



    if (localbest > globalbest) {
      globalbest = localbest;
      strcpy(globalprint,localbestthing);}
    }
  }
  free(split_ptr[0]);
  free(split_ptr[1]);
  free(split_ptr[2]);
  free(split_ptr);
  for (count=0;count<strlen(globalprint);++count)
    if (*(globalprint+count) == '\'') 
      *(globalprint+count) = '\b'; 
  implement_change(globalprint,guess_tag_corpus,word_corpus,
		   word_corpus_index,&WORDS,&SEENTAGGING);
  for (count=0;count<strlen(globalprint);++count)
    if (*(globalprint+count) == '\b') 
      *(globalprint+count) = '\''; 
  correct_out = fopen(argv[3],"a");
  fprintf(correct_out,"%s\n",globalprint);
  fclose(correct_out);
  CONTINUE = globalbest; 
  for (count=0;count<Darray_len(errorlist);++count)
    free(Darray_get(errorlist,count));
  Darray_destroy(errorlist);
	 
}
}


/*************  The code below is for updating the corpus by applying
                the learned rule ***************************************/

void change_the_tag(theentry,thetag,theposition)
  char **theentry, *thetag;
  int theposition;

{
  free(theentry[theposition]);
  theentry[theposition] = mystrdup(thetag);
}


void implement_change(thearg,thetagcorpus,thewordcorpus,corpuslen,
		 wordreg,seentaggingreg)
char *thearg;
char **thetagcorpus,**thewordcorpus;
int corpuslen;
Registry *wordreg,*seentaggingreg;
{

	char            line[5000];	/* input line buffer */
	char          **split_ptr;
	int             count,tempcount1,tempcount2,ccount;
	char            old[100], *new, when[300], tag[100], lft[100],
	                rght[100], 
	                prev1[100], prev2[100], next1[100], next2[100], 
	                curtag[100],
	                tempstr[300],word[300];
	char **perl_split_ptr,**perl_split_ptr2,*atempstr,atempstr2[256],
	      space[500],curwd[100];

	strcpy(line,thearg);
	for (ccount=0;ccount<strlen(line);++ccount) /*patch for sing quote*/
	  if (*(line+ccount) == '\b') 
	    *(line+ccount) = '\''; 
	if (strlen(line) > 1) {
	  split_ptr = perl_split(line);
	  strcpy(old, split_ptr[0]);
	  new = mystrdup(split_ptr[1]);
	  strcpy(when, split_ptr[2]);
	  
	  if (strcmp(when, "NEXTTAG") == 0 ||
	      strcmp(when, "NEXT2TAG") == 0 ||
	      strcmp(when, "NEXT1OR2TAG") == 0 ||
	      strcmp(when, "NEXT1OR2OR3TAG") == 0 ||
	      strcmp(when, "PREVTAG") == 0 ||
	      strcmp(when, "PREV2TAG") == 0 ||
	      strcmp(when, "PREV1OR2TAG") == 0 ||
	      strcmp(when, "PREV1OR2OR3TAG") == 0) {
	    strcpy(tag, split_ptr[3]);
	  } 
	  else if (strcmp(when, "NEXTWD") == 0 ||
		   strcmp(when,"CURWD") == 0 || 
		   strcmp(when, "NEXT2WD") == 0 ||
		   strcmp(when, "NEXT1OR2WD") == 0 ||
		   strcmp(when, "NEXT1OR2OR3WD") == 0 ||
		   strcmp(when, "PREVWD") == 0 ||
		   strcmp(when, "PREV2WD") == 0 ||
		   strcmp(when, "PREV1OR2WD") == 0 ||
		   strcmp(when, "PREV1OR2OR3WD") == 0) {
	    strcpy(word, split_ptr[3]);
	  }
	  else if (strcmp(when, "SURROUNDTAG") == 0) {
	    strcpy(lft, split_ptr[3]);
	    strcpy(rght, split_ptr[4]);
	  } else if (strcmp(when, "PREVBIGRAM") == 0) {
	    strcpy(prev1, split_ptr[3]);
	    strcpy(prev2, split_ptr[4]);
	  } else if (strcmp(when, "NEXTBIGRAM") == 0) {
	    strcpy(next1, split_ptr[3]);
	    strcpy(next2, split_ptr[4]);
	  } else if (strcmp(when,"LBIGRAM") == 0||
		     strcmp(when,"WDPREVTAG") == 0) {
	    strcpy(prev1,split_ptr[3]);
	    strcpy(word,split_ptr[4]); 
	  } else if (strcmp(when,"RBIGRAM") == 0 ||
		     strcmp(when,"WDNEXTTAG") == 0) {
	    strcpy(word,split_ptr[3]);
	    strcpy(next1,split_ptr[4]); 
	  } else if (strcmp(when,"WDAND2BFR")== 0 ||
		     strcmp(when,"WDAND2TAGBFR")== 0) {
	    strcpy(prev2,split_ptr[3]);
	    strcpy(word,split_ptr[4]);}
	  else if (strcmp(when,"WDAND2AFT")== 0 ||
		   strcmp(when,"WDAND2TAGAFT")== 0) {
	    strcpy(next2,split_ptr[4]);
	    strcpy(word,split_ptr[3]);}
	  
	  for (count = 0; count <corpuslen; ++count) {
	    strcpy(curtag, thetagcorpus[count]);
	    if (strcmp(curtag, old) == 0) {
	      strcpy(curwd,thewordcorpus[count]);
	      sprintf(atempstr2,"%s %s",curwd,new);
	      
	      if (! Registry_get(*wordreg,curwd) ||
		  Registry_get(*seentaggingreg,atempstr2)) {
		
		if (strcmp(when, "SURROUNDTAG") == 0) {
		  if (count < corpuslen-1 && count > 0) {
		    if (strcmp(lft, thetagcorpus[count - 1]) == 0 &&
			strcmp(rght, thetagcorpus[count + 1]) == 0) 
		      change_the_tag(thetagcorpus,new,count);
		  }
		} else if (strcmp(when, "NEXTTAG") == 0) {
		  if (count < corpuslen-1) {
		    if (strcmp(tag, thetagcorpus[count + 1]) == 0) 
		      change_the_tag(thetagcorpus,new,count);
		  }
		}  
		else if (strcmp(when, "CURWD") == 0) {
		  if (strcmp(word, thewordcorpus[count]) == 0)
		      change_the_tag(thetagcorpus,new,count);
		}  
		else if (strcmp(when, "NEXTWD") == 0) {
		  if (count < corpuslen-1) {
		    if (strcmp(word, thewordcorpus[count + 1]) == 0)
		      change_the_tag(thetagcorpus,new,count);
		  }
		}  else if (strcmp(when, "RBIGRAM") == 0) {
		  if (count < corpuslen-1) {
		    if (strcmp(word, thewordcorpus[count]) ==
			0 &&
			strcmp(next1, thewordcorpus[count+1]) ==
			0)
		    change_the_tag(thetagcorpus,new,count);
		  }
		} 
		else if (strcmp(when, "WDNEXTTAG") == 0) {
		  if (count < corpuslen-1) {
		    if (strcmp(word, thewordcorpus[count]) ==
			0 &&
			strcmp(next1, thetagcorpus[count+1]) ==
			0)
		      change_the_tag(thetagcorpus,new,count);
		  }
		}
		
		else if (strcmp(when, "WDAND2AFT") == 0) {
		  if (count < corpuslen-2) {
		    if (strcmp(word, thewordcorpus[count]) ==
			0 &&
			strcmp(next2,thewordcorpus[count+2]) ==
			0)
		    change_the_tag(thetagcorpus,new,count);
		  }
		}
		else if (strcmp(when, "WDAND2TAGAFT") == 0) {
		  if (count < corpuslen-2) {
		    if (strcmp(word, thewordcorpus[count]) ==
			0 &&
			strcmp(next2, thetagcorpus[count+2]) ==
			0)
		      change_the_tag(thetagcorpus,new,count);
		  }
		}
		else if (strcmp(when, "NEXT2TAG") == 0) {
		  if (count < corpuslen-2) {
		    if (strcmp(tag,thetagcorpus[count + 2]) == 0)
		      change_the_tag(thetagcorpus,new,count);
		  }
		} else if (strcmp(when, "NEXT2WD") == 0) {
		  if (count < corpuslen-2) {
		    if (strcmp(word, thewordcorpus[count + 2]) == 0)
		      change_the_tag(thetagcorpus,new,count);
		  }
		} else if (strcmp(when, "NEXTBIGRAM") == 0) {
		  if (count < corpuslen-2) {
		    if
		      (strcmp(next1, thetagcorpus[count + 1]) == 0 &&
		       strcmp(next2, thetagcorpus[count + 2]) == 0)
			change_the_tag(thetagcorpus,new,count);
		  }
		} else if (strcmp(when, "NEXT1OR2TAG") == 0) {
		  if (count < corpuslen-1) {
		    if (count < corpuslen-2) 
		      tempcount1 = count+2;
		    else 
		      tempcount1 = count+1;
		    if
		    (strcmp(tag, thetagcorpus[count + 1]) == 0 ||
		     strcmp(tag, thetagcorpus[tempcount1]) == 0)
		      change_the_tag(thetagcorpus,new,count);
		  }
		}  else if (strcmp(when, "NEXT1OR2WD") == 0) {
		  if (count < corpuslen-1) {
		    if (count < corpuslen-2) 
		      tempcount1 = count+2;
		    else 
		      tempcount1 = count+1;
		    if
		    (strcmp(word, thewordcorpus[count + 1]) == 0 ||
		     strcmp(word, thewordcorpus[tempcount1]) == 0)
		      change_the_tag(thetagcorpus,new,count);
		  }
		}   else if (strcmp(when, "NEXT1OR2OR3TAG") == 0) {
		  if (count < corpuslen-1) {
		    if (count < corpuslen-2)
		      tempcount1 = count+2;
		    else 
		      tempcount1 = count+1;
		    if (count < corpuslen-3)
		      tempcount2 = count+3;
		    else 
		      tempcount2 =count+1;
		    if
		      (strcmp(tag, thetagcorpus[count + 1]) == 0 ||
		       strcmp(tag, thetagcorpus[tempcount1]) == 0 ||
		       strcmp(tag, thetagcorpus[tempcount2]) == 0)
			change_the_tag(thetagcorpus,new,count);
		  }
		} else if (strcmp(when, "NEXT1OR2OR3WD") == 0) {
		  if (count < corpuslen-1) {
		    if (count < corpuslen-2)
		      tempcount1 = count+2;
		    else 
		      tempcount1 = count+1;
		    if (count < corpuslen-3)
		      tempcount2 = count+3;
		    else 
		      tempcount2 =count+1;
		    if
		      (strcmp(word, thewordcorpus[count + 1]) == 0 ||
		       strcmp(word, thewordcorpus[tempcount1]) == 0 ||
		       strcmp(word, thewordcorpus[tempcount2]) == 0)
			change_the_tag(thetagcorpus,new,count);
		  }
		}  else if (strcmp(when, "PREVTAG") == 0) {
		  if (count > 0) {
		    if (strcmp(tag, thetagcorpus[count - 1]) == 0) {
		      change_the_tag(thetagcorpus,new,count);
		    }
		  }
		} else if (strcmp(when, "PREVWD") == 0) {
		  if (count > 0) {
		    if (strcmp(word, thewordcorpus[count - 1]) == 0) {
		      change_the_tag(thetagcorpus,new,count);
		    }
		  }
		}  else if (strcmp(when, "LBIGRAM") == 0) {
		  if (count > 0) {
		    if (strcmp(word, thewordcorpus[count]) ==
			0 &&
			strcmp(prev1, thewordcorpus[count-1]) ==
			0)
		      change_the_tag(thetagcorpus,new,count);
		  }
		}
		else if (strcmp(when, "WDPREVTAG") == 0) {
		  if (count > 0) {
		    if (strcmp(word, thewordcorpus[count]) ==
			0 &&
			strcmp(prev1, thetagcorpus[count-1]) ==
			0)
		      change_the_tag(thetagcorpus,new,count);
		  }
		}
		else if (strcmp(when, "WDAND2BFR") == 0) {
		  if (count > 1) {
		    if (strcmp(word, thewordcorpus[count]) ==
			0 &&
			strcmp(prev2, thewordcorpus[count-2]) ==
			0)
		      change_the_tag(thetagcorpus,new,count);
		  }
		}
		else if (strcmp(when, "WDAND2TAGBFR") == 0) {
		  if (count > 1) {
		    if (strcmp(word, thewordcorpus[count]) ==
			0 &&
			strcmp(prev2, thetagcorpus[count-2]) ==
			0)
		      change_the_tag(thetagcorpus,new,count);
		  }
		}
		else if (strcmp(when, "PREV2TAG") == 0) {
		  if (count > 1) {
		    if (strcmp(tag, thetagcorpus[count - 2]) == 0)
		      change_the_tag(thetagcorpus,new,count);
		  }
		} else if (strcmp(when, "PREV2WD") == 0) {
		  if (count > 1) {
		    if (strcmp(word, thewordcorpus[count - 2]) == 0)
		      change_the_tag(thetagcorpus,new,count);
		  }
		} else if (strcmp(when, "PREV1OR2TAG") == 0) {
		  if (count > 0) {
		    if (count > 1) 
		      tempcount1 = count-2;
		    else
		      tempcount1 = count-1;
		    if (strcmp(tag, thetagcorpus[count - 1]) == 0 ||
			strcmp(tag, thetagcorpus[tempcount1]) == 0)
		      change_the_tag(thetagcorpus,new,count);
		  }
		} else if (strcmp(when, "PREV1OR2WD") == 0) {
		  if (count > 0) {
		    if (count > 1) 
		      tempcount1 = count-2;
		    else
		      tempcount1 = count-1;
		    if (strcmp(word, thewordcorpus[count - 1]) == 0 ||
			strcmp(word, thewordcorpus[tempcount1]) == 0)
		      change_the_tag(thetagcorpus,new,count);
		  }
		} else if (strcmp(when, "PREV1OR2OR3TAG") == 0) {
		  if (count > 0) {
		    if (count>1) 
		      tempcount1 = count-2;
		    else 
		      tempcount1 = count-1;
		    if (count >2) 
		      tempcount2 = count-3;
		    else 
		      tempcount2 = count-1;
		    if (strcmp(tag, thetagcorpus[count - 1]) == 0 ||
			strcmp(tag, thetagcorpus[tempcount1]) == 0 ||
			strcmp(tag, thetagcorpus[tempcount2]) == 0)
		      change_the_tag(thetagcorpus,new,count);
		  }
		} else if (strcmp(when, "PREV1OR2OR3WD") == 0) {
		  if (count > 0) {
		    if (count>1) 
		      tempcount1 = count-2;
		    else 
		      tempcount1 = count-1;
		    if (count >2) 
		      tempcount2 = count-3;
		    else 
		      tempcount2 = count-1;
		    if (strcmp(word, thewordcorpus[count - 1]) == 0 ||
			strcmp(word, thewordcorpus[tempcount1]) == 0 ||
			strcmp(word, thewordcorpus[tempcount2]) == 0)
		      change_the_tag(thetagcorpus,new,count);
		  }
		} else if (strcmp(when, "PREVBIGRAM") == 0) {
		  if (count > 1) {
		    if (strcmp(prev1, thetagcorpus[count - 2]) == 0 &&
			strcmp(prev2, thetagcorpus[count - 1]) == 0)
		      change_the_tag(thetagcorpus,new,count);
		  }
		}
		else 
		  fprintf(stderr,
			  "ERROR: %s is not an allowable transform type\n",
			  when);
	      }
	    }
	    }
	    free(split_ptr);
	  free(new);
	  }
      }
