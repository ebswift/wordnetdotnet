/*  Copyright @ 1993 MIT */
/* Written by Eric Brill */

#include <stdio.h>
#include <fcntl.h>
#include <string.h>
#include <malloc.h>
#include "lex.h"
#include "darray.h"
#include "registry.h"
#include "memory.h"
#include "useful.h"

#define GUESSNUMWORDS 100000 /* guess at number of words in LEXICON*/
#define NUMTAGS 100  
#define NUMWDS  300
#define THRESHOLD 0.0
#define MAXTAGLEN 70
#define THRESHOLD2 18
#define THRESHOLD3 20

char staart[] = "STAART";
Darray correct_tag_corpus,word_corpus,guess_tag_corpus;
float localbest;
char localbestthing[100],localdif[20];
char *wrong,*right;
char *tempforcreate;
char globalstring[100];

/*    argv 0 = correct
           1 = guess
	   2 = file for rules
	   3 = lexicon file from lexical rule corpus
*/

/************************************************************/

int is_tagged_with(tag,tagseq)
     char *tag,*tagseq;
/* returns 1 if tag is contained in tagseq
   For example, tag = ab  tagseq = st/ab/gg/rt
   is_tagged_with(tag,tagseq) == 1 */
{
  char *temp,*temp2;
  temp = mystrdup(tagseq);
  temp2 = strtok(temp,"_");
  while (temp2 != NULL &&
	 strcmp(temp2,tag) != 0) 
    temp2 = strtok(NULL,"_");
  if (temp2 == NULL ){ free(temp); return(0);}
  else { free(temp); return(1); }
}


char *first_tag(tagstr)
     char *tagstr;
/* Tag is either x or x/y/z */
/* in either case, returns x */
{
  char *temp,*temp2,*temp3;
  temp  = mystrdup(tagstr);
  temp2 = strtok(temp,"_");
  temp3 = mystrdup(temp2);
  free(temp); 
  return(temp3);
}

char *first_tag_nospace(tagstr)
     char *tagstr;
/* Tag is either x or x/y/z */
/* in either case, returns x */
{
  char *temp,*temp2;
  temp  = mystrdup(tagstr);
  temp2 = strtok(temp,"_");
  strcpy(globalstring,temp2);
  free(temp); 
  return(globalstring);
}


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
void check_counts(goodhash,badhash,label)
     Registry *goodhash,*badhash;
     char *label;
{
  Darray tempkey,tempval,temp2key,temp2val;
  int tempcount,*hashtempstr,hashtempval;
  float hashtempval2;
  float  tempbest;
  int FREEFLAG;
  
  FREEFLAG = 0;

  if (strcmp(label,"PREVBIGRAM") == 0 ||
      strcmp(label,"NEXTBIGRAM") == 0 ||
      strcmp(label,"WDPREVTAG") == 0 ||
      strcmp(label,"WDNEXTTAG") == 0 ||
      strcmp(label,"RBIGRAM") == 0 ||
      strcmp(label,"LBIGRAM") == 0 ||
      strcmp(label,"NEXTTAG") == 0 ||
      strcmp(label,"NEXT2TAG") == 0 ||
      strcmp(label,"NEXT1OR2TAG") == 0 ||
      strcmp(label,"NEXT1OR2OR3TAG") == 0 ||
      strcmp(label,"PREVTAG") == 0 ||
      strcmp(label,"PREV2TAG") == 0 ||
      strcmp(label,"PREV1OR2TAG") == 0 ||
      strcmp(label,"PREV1OR2OR3TAG") == 0 ||
      strcmp(label,"SURROUNDTAG") == 0) 
    FREEFLAG = 1;

  tempkey = Darray_create();
  Darray_hint(tempkey,10,Registry_entry_count(*goodhash));
  tempval = Darray_create();
  Darray_hint(tempval,10,Registry_entry_count(*goodhash));
  temp2key = Darray_create();
  Darray_hint(temp2key,10,Registry_entry_count(*badhash));
  temp2val = Darray_create();
  Darray_hint(temp2val,10,Registry_entry_count(*badhash));

  Registry_fetch_contents(*goodhash,tempkey,tempval);
  Registry_fetch_contents(*badhash,temp2key,temp2val);
  for (tempcount=0;tempcount<Darray_len(tempkey);++tempcount) {

      hashtempstr  = (int *)Registry_get(*badhash,Darray_get(tempkey,tempcount));
      if (hashtempstr == NULL) 
	hashtempval  =  0;
      else
	hashtempval = *hashtempstr;
      hashtempval2 = (float)hashtempval;
      hashtempval2 +=1.0;
       if (*(int *)Darray_get(tempval,tempcount) > THRESHOLD2) {
      if ((tempbest = (float)*(int *)Darray_get(tempval,tempcount)/hashtempval2)
	  > localbest &&
	  tempbest < 1.0) {
	localbest = tempbest;
	sprintf(localdif,"%d %f",*(int *)Darray_get(tempval,tempcount),hashtempval2);
	sprintf(localbestthing,"%s %s %s %s",wrong,right,label,Darray_get(tempkey,tempcount)); 
      }}
      free(Darray_get(tempval,tempcount));
      if (FREEFLAG)
	      free(Darray_get(tempkey,tempcount));
	     
    }
    for (tempcount=0;tempcount<Darray_len(temp2key);++tempcount) {
       free(Darray_get(temp2val,tempcount)); 
       if (FREEFLAG)
	        free(Darray_get(temp2key,tempcount)); 
    }
    Darray_destroy(tempval);
    Darray_destroy(temp2val);
    Darray_destroy(tempkey);
    Darray_destroy(temp2key);
    Registry_destroy(*goodhash);
    Registry_destroy(*badhash);
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

char onewdbfr[512],onewdaft[512];
char onetagbfr[512],twotagbfr[512],onetagaft[512],twotagaft[512];
char *freshcharvar;
Darray errorlist,temperrorkey,temperrorval;
Registry errorlistcount,SEENTAGGING,WORDS;

FILE *correct_file, *guess_file, *error_list,*correct_out;
char line[5000];  /* input line buffer */
char **split_ptr,**split_ptr2;

char *tempforfree,*tempforfree2;
char wdpair[1024],*wdpair2;
char *tempstr,*tempstr2;
float CONTINUE = 10000.0;
int count,count2,count3,numwrong,lengthcount;
char globalprint[500];
char systemcall[500];
char forpasting[500];
char forpasting2[500];
float globalbest = 0.0;
char flag[20];
Registry currentwd,currentwd2;
Registry always,always2;
Registry wdnexttag,wdnexttag2,wdprevtag,wdprevtag2;
Registry rbigram,lbigram,rbigram2,lbigram2;
Registry next1tag,next1tag2,prev1tag,prev1tag2;
Registry next1or2tag,next1or2tag2,prev1or2tag,prev1or2tag2;
Registry next1or2or3tag,next1or2or3tag2,prev1or2or3tag,prev1or2or3tag2;
Registry next1wd,next1wd2,prev1wd,prev1wd2;
Registry next1or2wd,next1or2wd2,prev1or2wd,prev1or2wd2;
Registry nextbigram,nextbigram2,prevbigram,prevbigram2;
Registry surroundtag,surroundtag2;
Registry next2tag,next2tag2,prev2tag,prev2tag2;
Registry next2wd,next2wd2,prev2wd,prev2wd2;
char globaldif[20];
int printscore;
FILE *allowedmovefile;
char **perl_split_ptr,**perl_split_ptr2,*atempstr,atempstr2[1024];
char space[500];

SEENTAGGING = Registry_create(Registry_strcmp,Registry_strhash);
Registry_size_hint(SEENTAGGING,GUESSNUMWORDS);
WORDS = Registry_create(Registry_strcmp,Registry_strhash);
Registry_size_hint(WORDS,GUESSNUMWORDS);

allowedmovefile = fopen(argv[4], "r");
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


system("/bin/rm AANEWRESTRJUNKKK");
correct_tag_corpus = Darray_create();
Darray_hint(correct_tag_corpus,100,400000);
word_corpus = Darray_create();
Darray_hint(word_corpus,100,400000);


correct_file = fopen(argv[1],"r");


while(fgets(line,sizeof(line),correct_file) != NULL) {
  Darray_addh(correct_tag_corpus,staart);
  Darray_addh(correct_tag_corpus,staart);
  Darray_addh(word_corpus,staart);
  Darray_addh(word_corpus,staart);
  line[strlen(line)-1] = '\0';
  split_ptr = perl_split_independent(line);
  while (*split_ptr != NULL) {
    Darray_addh(word_corpus,*split_ptr);
    while ((*(++*split_ptr)) != '/') {
    }
    **split_ptr = '\0';
    Darray_addh(correct_tag_corpus,++*split_ptr);
    ++split_ptr;
  }
}
fclose(correct_file);


printf("READ IN CORRECT FILE\n");




while(CONTINUE > THRESHOLD) {

  guess_tag_corpus = Darray_create();
  Darray_hint(guess_tag_corpus,100,400000);
  guess_file  = fopen(argv[2],"r");
  while(fgets(line,sizeof(line),guess_file) != NULL) {
    
    Darray_addh(guess_tag_corpus,staart);
    Darray_addh(guess_tag_corpus,staart);
    line[strlen(line)-1] = '\0';
    split_ptr = perl_split_independent(line); 
    split_ptr2 = split_ptr;
    while (*split_ptr != NULL) {
      tempstr = strtok(*split_ptr,"/");
      tempstr = strtok(NULL,"/");
      tempstr2  = mystrdup(tempstr);
      Darray_addh(guess_tag_corpus,tempstr2);
      free(*split_ptr);
      ++split_ptr;
    }
    free(split_ptr2);
  }
  fclose(guess_file);

printf("READ IN BAD FILE\n");

  errorlist = Darray_create();
  Darray_hint(errorlist,10,500);
  temperrorkey = Darray_create();
  temperrorval = Darray_create();
  Darray_hint(temperrorkey,10,500);
  Darray_hint(temperrorval,10,500);

  init_hash(&errorlistcount,500);


  printscore=0;
  for(count=0;count<Darray_len(guess_tag_corpus);++count) {
    if
      (! is_tagged_with(Darray_get(correct_tag_corpus,count),Darray_get(guess_tag_corpus,count))) { 
	++printscore; 
	freshcharvar =
	  mystrdup(first_tag_nospace(Darray_get(guess_tag_corpus,count)));
	sprintf(forpasting,"%s %s",freshcharvar,
		                       Darray_get(correct_tag_corpus,count));
	increment_array_create(&errorlistcount,forpasting);
      } 
  }

  error_list = fopen("AANEWRESTRJUNKKK","a");
  Registry_fetch_contents(errorlistcount,temperrorkey,temperrorval);  
  for (count=0;count<Darray_len(temperrorkey);++count) {
    if (*(int *)Darray_get(temperrorval,count) > THRESHOLD)
      /*Darray_addh(errorlist,tempstr);*/
      fprintf(error_list,"%d %s\n",*(int *)Darray_get(temperrorval,count),
	                    Darray_get(temperrorkey,count));
    free(Darray_get(temperrorval,count));
    free(Darray_get(temperrorkey,count));
  }
  fclose(error_list);
  Darray_destroy(temperrorval);
  Darray_destroy(temperrorkey);
  Registry_destroy(errorlistcount);

  printf("NUM ERRORS: %d\n",printscore);
/* shoud sort error list !!!!!!!*/
  
  system("cat AANEWRESTRJUNKKK | sort -rn > AANEWRESTRJUNKKK2");
  system("mv AANEWRESTRJUNKKK2 AANEWRESTRJUNKKK");
  
  error_list = fopen("AANEWRESTRJUNKKK","r");
  while(fgets(line,sizeof(line),error_list) != NULL) {
    line[strlen(line)-1] = '\0';
    tempstr = mystrdup(line);
    Darray_addh(errorlist,tempstr); 
  }
  fclose(error_list);
  system("/bin/rm AANEWRESTRJUNKKK");

  globalbest= 0;
  strcpy(globalprint,"");
  
  
  for (count=0;count<Darray_len(errorlist);++count) {

    localbest =0;
    strcpy(localbestthing,"");
    /*printf("ERROR LIST GUY: %s\n",Darray_get(errorlist,count));      */

    split_ptr = perl_split_independent(Darray_get(errorlist,count));
/*printf("ERRORLISTGUY: %s %s %s\n",split_ptr[0],split_ptr[1],split_ptr[2]);*/
    wrong = split_ptr[1];
    right = split_ptr[2];
    numwrong = atoi(split_ptr[0]);
    if (numwrong > THRESHOLD3) {
      
      printf("WRONG,RI: %s %s\n",wrong,right);
      printf("GLOBALBEST, GLOBALPRINT, GLOBALDIF: %f %s %s\n",globalbest,globalprint,globaldif);

      init_hash(&always,NUMTAGS/2);
      init_hash(&always2,NUMTAGS/2);
      init_hash(&rbigram,(NUMWDS*NUMWDS)/4);
      init_hash(&lbigram,(NUMWDS*NUMWDS)/4);
      init_hash(&rbigram2,(NUMWDS*NUMWDS)/4);
      init_hash(&lbigram2,(NUMWDS*NUMWDS)/4);
      init_hash(&wdnexttag,(NUMWDS*NUMTAGS)/4);
      init_hash(&wdnexttag2,(NUMWDS*NUMTAGS)/4);
      init_hash(&wdprevtag,(NUMWDS*NUMTAGS)/4);
      init_hash(&wdprevtag2,(NUMWDS*NUMTAGS)/4);
      init_hash(&next1tag,NUMTAGS/2);
      init_hash(&next1tag2,NUMTAGS/2);
      init_hash(&prev1tag,NUMTAGS/2);
      init_hash(&prev1tag2,NUMTAGS/2);
      init_hash(&next1or2tag,NUMTAGS/2);
      init_hash(&next1or2tag2,NUMTAGS/2);
      init_hash(&prev1or2tag,NUMTAGS/2);
      init_hash(&prev1or2tag2,NUMTAGS/2);
      init_hash(&next1wd,NUMWDS/2);
      init_hash(&next1wd2,NUMWDS/2);
      init_hash(&prev1wd,NUMWDS/2);
      init_hash(&prev1wd2,NUMWDS/2);
      init_hash(&currentwd,NUMWDS/2);
      init_hash(&currentwd2,NUMWDS/2);
      init_hash(&next1or2wd,NUMWDS/2);
      init_hash(&next1or2wd2,NUMWDS/2);
      init_hash(&prev1or2wd,NUMWDS/2);
      init_hash(&prev1or2wd2,NUMWDS/2);
      init_hash(&next1or2or3tag,NUMTAGS/2);
      init_hash(&next1or2or3tag2,NUMTAGS/2);
      init_hash(&prev1or2or3tag,NUMTAGS/2);
      init_hash(&prev1or2or3tag2,NUMTAGS/2);
      init_hash(&nextbigram,NUMTAGS);
      init_hash(&nextbigram2,NUMTAGS);
      init_hash(&prevbigram,NUMTAGS);
      init_hash(&prevbigram2,NUMTAGS);
      init_hash(&surroundtag,NUMTAGS);
      init_hash(&surroundtag2,NUMTAGS);
      init_hash(&next2tag,NUMTAGS/2);
      init_hash(&next2tag2,NUMTAGS/2);
      init_hash(&prev2tag,NUMTAGS/2);
      init_hash(&prev2tag2,NUMTAGS/2);
      init_hash(&next2wd,NUMWDS/2);
      init_hash(&next2wd2,NUMWDS/2);
      init_hash(&prev2wd,NUMWDS/2);
      init_hash(&prev2wd2,NUMWDS/2);


      lengthcount = Darray_len(correct_tag_corpus);
      for(count2=0;count2<lengthcount;++count2){
	sprintf(atempstr2,"%s %s",Darray_get(word_corpus,count2),right);
	if (Registry_get(WORDS,Darray_get(word_corpus,count2)) &&
	    ! Registry_get(SEENTAGGING,atempstr2)) 
	  strcpy(flag,"NOMATCH");
	else if 
	  (strcmp(Darray_get(correct_tag_corpus,count2),right) == 0 &&
	   (strcmp
	    (first_tag_nospace(Darray_get(guess_tag_corpus,count2)),wrong) 
	    ==  0)
	    &&
	    (! is_tagged_with(right,Darray_get(guess_tag_corpus,count2))))
	  strcpy(flag,"BADMATCH");
	else if
	  (strcmp(Darray_get(correct_tag_corpus,count2),right) != 0 &&
	   (strcmp
	    (first_tag_nospace(Darray_get(guess_tag_corpus,count2)),wrong) 
	    ==  0)
	   &&
	   (! is_tagged_with(right,Darray_get(guess_tag_corpus,count2))))
	  strcpy(flag,"GOODMATCH");
	else 
	  strcpy(flag,"NOMATCH");
	
	if (strcmp(flag,"BADMATCH") == 0) {
	  increment_array(&always,"DUMMY");
	  increment_array(&currentwd,Darray_get(word_corpus,count2),0);
	  if (count2 != lengthcount-1) {
	    strcpy(onewdaft,Darray_get(word_corpus,count2+1));
	    strcpy(onetagaft,
		   first_tag_nospace(Darray_get(guess_tag_corpus,count2+1)));
	    sprintf(wdpair,"%s %s",Darray_get(word_corpus,count2),
		                  Darray_get(word_corpus,count2+1));
    	    increment_array_create(&rbigram,wdpair);
	    sprintf(wdpair,"%s %s",Darray_get(word_corpus,count2),
		    first_tag_nospace(Darray_get(guess_tag_corpus,count2+1)));
	    increment_array_create(&wdnexttag,wdpair);
	    increment_array_create(&next1or2tag,
			    first_tag_nospace(
				Darray_get(guess_tag_corpus,count2+1)));
	    increment_array_create(&next1or2or3tag,
			    first_tag_nospace(
				Darray_get(guess_tag_corpus,count2+1)));
	    increment_array_create(&next1tag,
			    first_tag_nospace(Darray_get(guess_tag_corpus,count2+1)));
	    increment_array(&next1wd,Darray_get(word_corpus,count2+1));
	    increment_array(&next1or2wd,Darray_get(word_corpus,count2+1));
	  }
	  if (count2 < lengthcount-2) {
	    strcpy(twotagaft,first_tag_nospace(Darray_get(guess_tag_corpus,count2+2)));
	    strcpy(forpasting2,
		   first_tag_nospace(Darray_get(guess_tag_corpus,count2+1)));
	    sprintf(forpasting,"%s %s",forpasting2,
		                       first_tag_nospace(Darray_get(guess_tag_corpus,count2+2)));
	    increment_array_create(&nextbigram,forpasting);
	    increment_array_create(&next2tag,first_tag_nospace(Darray_get(guess_tag_corpus,count2+2)));
	    increment_array(&next2wd,Darray_get(word_corpus,count2+2));
	    if
	      (strcmp(first_tag_nospace(Darray_get(guess_tag_corpus,count2+2)),                 onetagaft) != 0)
	    {
	      increment_array_create(&next1or2tag,
		  first_tag_nospace(Darray_get(guess_tag_corpus,count2+2)));
	      
	      increment_array_create(&next1or2or3tag,
		  first_tag_nospace(Darray_get(guess_tag_corpus,count2+2)));
	    }
	    if (strcmp(Darray_get(word_corpus,count2+2),onewdaft) != 0)
	      increment_array(&next1or2wd,Darray_get(word_corpus,count2+2));
	  }
	  if (count2 < lengthcount-3) {
	    if (strcmp(first_tag_nospace(Darray_get(guess_tag_corpus,count2+3)),onetagaft) != 0
		&&
		strcmp(first_tag_nospace(Darray_get(guess_tag_corpus,count2+3)),twotagaft) != 0)
	      increment_array_create(&next1or2or3tag,
		 first_tag_nospace(Darray_get(guess_tag_corpus,count2+3)));
	  }
	  if (count2 != 0) {
	    strcpy(onewdbfr,Darray_get(word_corpus,count2-1));
	    strcpy(onetagbfr,first_tag_nospace(Darray_get(guess_tag_corpus,count2-1)));
	    sprintf(wdpair,"%s %s",Darray_get(word_corpus,count2-1),
		                  Darray_get(word_corpus,count2));
    	    increment_array_create(&lbigram,wdpair);
	    sprintf(wdpair,"%s %s",first_tag_nospace(Darray_get(guess_tag_corpus,count2-1)),
		                  Darray_get(word_corpus,count2));
    	    increment_array_create(&wdprevtag,wdpair);
	    increment_array_create(&prev1tag,first_tag_nospace(Darray_get(guess_tag_corpus,count2-1)));
	    increment_array(&prev1wd,Darray_get(word_corpus,count2-1));
	    increment_array(&prev1or2wd,Darray_get(word_corpus,count2-1));
	    increment_array_create(&prev1or2tag,
			    first_tag_nospace(Darray_get(guess_tag_corpus,count2-1)));
	    increment_array_create(&prev1or2or3tag,
			    first_tag_nospace(Darray_get(guess_tag_corpus,count2-1)));
	    if (count2 < lengthcount-1) {
	      strcpy(forpasting2,
		     first_tag_nospace(Darray_get(guess_tag_corpus,count2-1)));
	      sprintf(forpasting,"%s %s",forpasting2,
		                       first_tag_nospace(Darray_get(guess_tag_corpus,count2+1)));
	      increment_array_create(&surroundtag,forpasting);
	    }
	  }
	  if (count2 > 1) {
	    strcpy(twotagbfr,first_tag_nospace(Darray_get(guess_tag_corpus,count2-2)));
	    strcpy(forpasting2,
		   first_tag_nospace(Darray_get(guess_tag_corpus,count2-2)));
	    sprintf(forpasting,"%s %s",forpasting2,
		                       first_tag_nospace(Darray_get(guess_tag_corpus,count2-1)));
	    increment_array_create(&prevbigram,forpasting);
	    increment_array_create(&prev2tag,first_tag_nospace(Darray_get(guess_tag_corpus,count2-2)));
	    increment_array(&prev2wd,Darray_get(word_corpus,count2-2));
	    if (strcmp(first_tag_nospace(
		 Darray_get(guess_tag_corpus,count2-2)),onetagbfr) != 0){ 
	      increment_array_create(&prev1or2tag,
				     first_tag_nospace(Darray_get(guess_tag_corpus,count2-2)));
	      increment_array_create(&prev1or2or3tag,
				     first_tag_nospace(Darray_get(guess_tag_corpus,count2-2)));
	    }
	    if (strcmp(Darray_get(word_corpus,count2-2),onewdbfr) !=
		0)
	      increment_array(&prev1or2wd,Darray_get(word_corpus,count2-2));
	  }
	  if (count2 > 2) {
	     if (strcmp(first_tag_nospace(Darray_get(guess_tag_corpus,count2-3)),onetagbfr) != 0
		&&
		strcmp(first_tag_nospace(Darray_get(guess_tag_corpus,count2-3)),twotagbfr) != 0)
	       increment_array_create(&prev1or2or3tag,
			    first_tag_nospace(Darray_get(guess_tag_corpus,count2-3)));
	  }
	}


	else if (strcmp(flag,"GOODMATCH") == 0) {
	  increment_array(&always2,"DUMMY");
	  increment_array(&currentwd2,Darray_get(word_corpus,count2),0);
	  if (count2 != lengthcount-1) {
	    strcpy(onewdaft,Darray_get(word_corpus,count2+1));
	    strcpy(onetagaft,first_tag_nospace(Darray_get(guess_tag_corpus,count2+1)));
            sprintf(wdpair,"%s %s",Darray_get(word_corpus,count2),
		                  Darray_get(word_corpus,count2+1));
    	    increment_array_create(&rbigram2,wdpair);
	    sprintf(wdpair,"%s %s",Darray_get(word_corpus,count2),
		                  first_tag_nospace(Darray_get(guess_tag_corpus,count2+1)));
    	    increment_array_create(&wdnexttag2,wdpair);
	    increment_array_create(&next1tag2,first_tag_nospace(Darray_get(guess_tag_corpus,count2+1)));
	    increment_array(&next1wd2,Darray_get(word_corpus,count2+1));
	    increment_array(&next1or2wd2,Darray_get(word_corpus,count2+1));
	    increment_array_create(&next1or2tag2,
			    first_tag_nospace(Darray_get(guess_tag_corpus,count2+1)));
	    increment_array_create(&next1or2or3tag2,
			    first_tag_nospace(Darray_get(guess_tag_corpus,count2+1)));
	  }
	  if (count2 < lengthcount-2) {
	    strcpy(twotagaft,first_tag_nospace(Darray_get(guess_tag_corpus,count2+2)));
	    strcpy(forpasting2,
		   first_tag_nospace(Darray_get(guess_tag_corpus,count2+1)));
	    sprintf(forpasting,"%s %s",forpasting2,
		                       first_tag_nospace(Darray_get(guess_tag_corpus,count2+2)));
	    increment_array_create(&nextbigram2,forpasting);
	    increment_array_create(&next2tag2,first_tag_nospace(Darray_get(guess_tag_corpus,count2+2)));
	    increment_array(&next2wd2,Darray_get(word_corpus,count2+2));
	    if (strcmp(first_tag_nospace(Darray_get(guess_tag_corpus,count2+2)),onetagaft) !=0) {
	      increment_array_create(&next1or2tag2,
		     first_tag_nospace(Darray_get(guess_tag_corpus,count2+2)));
	      increment_array_create(&next1or2or3tag2,
		    first_tag_nospace(Darray_get(guess_tag_corpus,count2+2)));
	    }
	    if (strcmp(Darray_get(word_corpus,count2+2),onewdaft) !=0)
	      increment_array(&next1or2wd2,Darray_get(word_corpus,count2+2));
	  }
	  if (count2 < lengthcount-3) {
	    if (strcmp(first_tag_nospace(Darray_get(guess_tag_corpus,count2+3)),onetagaft) !=0 
		&&
		strcmp(first_tag_nospace(Darray_get(guess_tag_corpus,count2+3)),twotagaft) !=0 )
	      increment_array_create(&next1or2or3tag2,
			    first_tag_nospace(Darray_get(guess_tag_corpus,count2+3)));
	  }
	  if (count2 != 0) {
	    strcpy(onewdbfr,Darray_get(word_corpus,count2-1));
	    strcpy(onetagbfr,Darray_get(guess_tag_corpus,count2-1));
	    sprintf(wdpair,"%s %s",Darray_get(word_corpus,count2-1),
		                  Darray_get(word_corpus,count2));
    	    increment_array_create(&lbigram2,wdpair);
	    sprintf(wdpair,"%s %s",first_tag_nospace(Darray_get(guess_tag_corpus,count2-1)),
		                  Darray_get(word_corpus,count2));
    	    increment_array_create(&wdprevtag2,wdpair);
	    increment_array_create(&prev1tag2,first_tag_nospace(Darray_get(guess_tag_corpus,count2-1)));
	    increment_array(&prev1wd2,Darray_get(word_corpus,count2-1));
	    increment_array(&prev1or2wd2,Darray_get(word_corpus,count2-1));
	    increment_array_create(&prev1or2tag2,
			    first_tag_nospace(Darray_get(guess_tag_corpus,count2-1)));
	    increment_array_create(&prev1or2or3tag2,
			    first_tag_nospace(Darray_get(guess_tag_corpus,count2-1)));
	    if (count2 < lengthcount-1) {
	      strcpy(forpasting2,
		     first_tag_nospace(Darray_get(guess_tag_corpus,count2-1)));
	      sprintf(forpasting,"%s %s",forpasting2,
		                       first_tag_nospace(Darray_get(guess_tag_corpus,count2+1)));
      increment_array_create(&surroundtag2,forpasting);
	    }
	  }
	  if (count2 >1 ) { 
	    strcpy(twotagbfr,first_tag_nospace(Darray_get(guess_tag_corpus,count2-2)));
	    strcpy(forpasting2,
		   first_tag_nospace(Darray_get(guess_tag_corpus,count2-2)));
	    sprintf(forpasting,"%s %s",forpasting2,
		                       first_tag_nospace(Darray_get(guess_tag_corpus,count2-1)));
	    increment_array_create(&prevbigram2,forpasting);
	    increment_array_create(&prev2tag2,first_tag_nospace(Darray_get(guess_tag_corpus,count2-2)));
	    increment_array(&prev2wd2,Darray_get(word_corpus,count2-2));
	    if (strcmp(first_tag_nospace(Darray_get(guess_tag_corpus,count2-2)),onetagbfr) != 0){
	      increment_array_create(&prev1or2tag2,
			    first_tag_nospace(Darray_get(guess_tag_corpus,count2-2)));
	      increment_array_create(&prev1or2or3tag2,
			    first_tag_nospace(Darray_get(guess_tag_corpus,count2-2)));
	    }
	    if (strcmp(Darray_get(word_corpus,count2-2),onewdbfr) != 0)
	      increment_array(&prev1or2wd2,Darray_get(word_corpus,count2-2));
	  }
	  if (count2 > 2) {
	    if (strcmp(first_tag_nospace(Darray_get(guess_tag_corpus,count2-3)),onetagbfr) != 0 
		&&
		strcmp(first_tag_nospace(Darray_get(guess_tag_corpus,count2-3)),twotagbfr) != 0)
	      increment_array_create(&prev1or2or3tag2,
			    first_tag_nospace(Darray_get(guess_tag_corpus,count2-3)));
	  }
	}
      }  


check_counts(&always,&always2,"ALWAYS");
check_counts(&prev1tag,&prev1tag2,"PREVTAG");
check_counts(&next1tag,&next1tag2,"NEXTTAG");
check_counts(&next1or2tag,&next1or2tag2,"NEXT1OR2TAG");
check_counts(&prev1or2tag,&prev1or2tag2,"PREV1OR2TAG");
check_counts(&next1wd,&next1wd2,"NEXTWD");
check_counts(&currentwd,&currentwd2,"CURRENTWD");
check_counts(&prev1wd,&prev1wd2,"PREVWD");
check_counts(&rbigram,&rbigram2,"RBIGRAM");
check_counts(&lbigram,&lbigram2,"LBIGRAM");
check_counts(&wdnexttag,&wdnexttag2,"WDNEXTTAG");
check_counts(&wdprevtag,&wdprevtag2,"WDPREVTAG");
check_counts(&next1or2wd,&next1or2wd2,"NEXT1OR2WD");
check_counts(&prev1or2wd,&prev1or2wd2,"PREV1OR2WD");
check_counts(&next1or2or3tag,&next1or2or3tag2,"NEXT1OR2OR3TAG");
check_counts(&prev1or2or3tag,&prev1or2or3tag2,"PREV1OR2OR3TAG");
check_counts(&prevbigram,&prevbigram2,"PREVBIGRAM");
check_counts(&nextbigram,&nextbigram2,"NEXTBIGRAM");
check_counts(&surroundtag,&surroundtag2,"SURROUNDTAG");
check_counts(&next2tag,&next2tag2,"NEXT2TAG");
check_counts(&prev2tag,&prev2tag2,"PREV2TAG");
check_counts(&next2wd,&next2wd2,"NEXT2WD");
check_counts(&prev2wd,&prev2wd2,"PREV2WD");



    if (localbest > globalbest) {
      globalbest = localbest;
      strcpy(globaldif,localdif);
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
  sprintf(systemcall,"cat %s | fix-kbest-rule-learn \'%s\' %s > aanewmynewtagggs",
	  argv[2],globalprint,argv[4]);
  system(systemcall);
  for (count=0;count<strlen(globalprint);++count)
    if (*(globalprint+count) == '\b') 
      *(globalprint+count) = '\''; 
  sprintf(systemcall,"mv aanewmynewtagggs %s",argv[2]);
  system(systemcall);
  correct_out = fopen(argv[3],"a");
  fprintf(correct_out,"%s\n",globalprint);
/*  fprintf(correct_out,"%d %s %s\n",globalbest,globalprint,globaldif);*/
  fclose(correct_out);
  CONTINUE = globalbest; 
  for (count=0;count<Darray_len(guess_tag_corpus);++count)
    if (strcmp((tempstr=Darray_get(guess_tag_corpus,count)),"STAART") != 0)
      free(tempstr);
  Darray_destroy(guess_tag_corpus);
  for (count=0;count<Darray_len(errorlist);++count)
    free(Darray_get(errorlist,count));
  Darray_destroy(errorlist);
	 
}

}
