/*  Copyright @ 1993 MIT and University of Pennsylvania*/
/* Written by Eric Brill */
/*THIS SOFTWARE IS PROVIDED "AS IS", AND M.I.T. MAKES NO REPRESENTATIONS 
OR WARRANTIES, EXPRESS OR IMPLIED.  By way of example, but not 
limitation, M.I.T. MAKES NO REPRESENTATIONS OR WARRANTIES OF 
MERCHANTABILITY OR FITNESS FOR ANY PARTICULAR PURPOSE OR THAT THE USE OF 
THE LICENSED SOFTWARE OR DOCUMENTATION WILL NOT INFRINGE ANY THIRD PARTY 
PATENTS, COPYRIGHTS, TRADEMARKS OR OTHER RIGHTS.   */


#include <stdio.h>
#include <string.h>
#include <fcntl.h>
#include <malloc.h>
#include "lex.h"
#include "darray.h"
#include "registry.h"
#include "memory.h"
#include "useful.h"
#define MAXTAGLEN 256  /* max char length of pos tags */
#define MAXWORDLEN 256 /* max char length of words */
#define MAXAFFIXLEN 5  /* max length of affixes being considered */


main(argc, argv)
	int             argc;
	char           *argv[];
{
        
        char *tempruleptr;
	char *atempptr;
	FILE           *wordlist,*lexicon,*corpus,*rulefile,*bigrams;
	char line[5000],*linecopy;      /* input line buffer */
	Registry wordlist_hash,lexicon_hash,ntot_hash,bigram_hash;
	Registry good_right_hash,good_left_hash,tag_hash;
	Darray rule_array,tag_array_key,tag_array_val;
	char word[MAXWORDLEN],tag[MAXTAGLEN],*word2,*tag2,
	   *tempstr,*tempstr2;
	char **perl_split_ptr,**temp_perl_split_ptr,**therule,**therule2;
	char bigram1[MAXWORDLEN],bigram2[MAXWORDLEN];
	char noun[10],proper[10];
	int count,count2,count3,rulesize,tempcount;
	char
	  tempstr_space[MAXWORDLEN+MAXAFFIXLEN],bigram_space[MAXWORDLEN*2];
	int EXTRAWDS=0;
	int numlexiconentries=0;
	int numwordentries=0;

/***********************************************************************/


/* lexicon hash stores the most likely tag for all known words.
   we can have a separate wordlist and lexicon file because unsupervised
   learning    can add to wordlist, while not adding to lexicon.  For
   example, if a big    untagged corpus is about to be tagged, the wordlist
   can be extended to    include words in that corpus, while the lexicon
   remains static.    Lexicon is file of form: 
             word t1 t2 ... tn 
   where t1 is the most likely tag for the word, and t2...tn are alternate
   tags, in no particular order. */
	lexicon = fopen(argv[1],"r");  /* read through once to get size */
	while(fgets(line,sizeof(line),lexicon) != NULL) {
	  if (not_just_blank(line)){
	    line[strlen(line) - 1] = '\0';
	    numlexiconentries += num_words(line);
	  }
	}
	fclose(lexicon);
/* just need word and most likely tag from lexicon (first tag entry) */
	lexicon_hash = Registry_create(Registry_strcmp,Registry_strhash);
	Registry_size_hint(lexicon_hash,numlexiconentries);
	lexicon = fopen(argv[1],"r");
	while(fgets(line,sizeof(line),lexicon) != NULL) {
	  if (not_just_blank(line)){
	    line[strlen(line) - 1] = '\0';
	    sscanf(line,"%s%s",word,tag);
	    word2 = mystrdup(word);
	    tag2 = mystrdup(tag);
	    Registry_add(lexicon_hash,(char *)word2,(char *)tag2);
	  }
	}
	fclose(lexicon);
	fprintf(stderr,"START STATE TAGGER::LEXICON READ\n");


/* Wordlist_hash contains a list of words.  This is used 
   for tagging unknown words in the "add prefix/suffix" and
   "delete prefix/suffix" rules.  This contains words not in LEXICON. */
	
	if (argc==6) {
	  EXTRAWDS=1;
	  wordlist = fopen(argv[5],"r");
	  while(fgets(line,sizeof(line),wordlist) != NULL) {
	    if (not_just_blank(line)) 
	      ++numwordentries;
	  }
	  fclose(wordlist);
	  wordlist_hash = Registry_create(Registry_strcmp,Registry_strhash);
	  Registry_size_hint(wordlist_hash,numwordentries);
	  wordlist = fopen(argv[5],"r");
	  /* read in list of words */
	  while(fgets(line,sizeof(line),wordlist) != NULL) {
	    if (not_just_blank(line)) {
	      line[strlen(line) - 1] = '\0';
	      word2 = mystrdup(line);
	      Registry_add(wordlist_hash,(char *)word,(char *)1);
	    }
	  }
	  fclose(wordlist);
	  fprintf(stderr,"START STATE TAGGER:: WORDLIST READ\n");
	}

/*********************************************************/
	/* Read in corpus to be tagged.  Actually, just record word list, */
	/* since each word will get the same tag, regardless of context. */
	 ntot_hash = Registry_create(Registry_strcmp,Registry_strhash);
	 corpus = fopen(argv[2],"r");
	 while(fgets(line,sizeof(line),corpus) != NULL) {
           if (not_just_blank(line)){
	     line[strlen(line) - 1] = '\0';
	     perl_split_ptr = perl_split_independent(line);
	     temp_perl_split_ptr = perl_split_ptr;
	     while (*temp_perl_split_ptr != NULL) { 
	       if (Registry_get(lexicon_hash,(char *)*temp_perl_split_ptr) ==
		   NULL) {
		 if (Registry_add(ntot_hash,(char *)*temp_perl_split_ptr,
				  (char *)1) ==   Bool_FALSE) {
		   free(*temp_perl_split_ptr);
		 }
	       }
	       else {
		 free(*temp_perl_split_ptr);}
	       ++temp_perl_split_ptr;
	     }
	     free(perl_split_ptr);
	   }
	 }
	 fclose(corpus);
	 fprintf(stderr,"START STATE TAGGER:: CORPUS READ\n");  

/* read in rule file */
	rule_array = Darray_create();
	good_right_hash = Registry_create(Registry_strcmp,Registry_strhash);
	good_left_hash = Registry_create(Registry_strcmp,Registry_strhash);
	  
	rulefile = fopen(argv[4],"r");
	while(fgets(line,sizeof(line),rulefile) != NULL) {
	  if (not_just_blank(line)){
	    line[strlen(line) - 1] = '\0';
	    linecopy = mystrdup(line);
	    Darray_addh(rule_array,linecopy);	   
	    perl_split_ptr = perl_split(line);
	    temp_perl_split_ptr = perl_split_ptr;
	    if (strcmp(perl_split_ptr[1],"goodright") == 0) {
	      tempruleptr = mystrdup(perl_split_ptr[0]);
	      Registry_add(good_right_hash,tempruleptr,(char *)1); }
	    else if (strcmp(perl_split_ptr[2],"fgoodright") == 0) {
	      tempruleptr = mystrdup(perl_split_ptr[1]);
	      Registry_add(good_right_hash,tempruleptr,(char *)1); }
	    else if (strcmp(perl_split_ptr[1],"goodleft") == 0) {
	      tempruleptr = mystrdup(perl_split_ptr[0]);
	      Registry_add(good_left_hash,tempruleptr,(char *)1); }
	    else if (strcmp(perl_split_ptr[2],"fgoodleft") == 0) {
	      tempruleptr = mystrdup(perl_split_ptr[1]);
	      Registry_add(good_left_hash,tempruleptr,(char *)1); }
	    free(*temp_perl_split_ptr);
	    free(temp_perl_split_ptr);
	  }
	}
	  fclose(rulefile);
	  fprintf(stderr,"START STATE TAGGER:: RULEFILE READ\n");



	  /* read in bigram file */
	bigram_hash = Registry_create(Registry_strcmp,Registry_strhash);
	
	bigrams = fopen(argv[3],"r");
	while(fgets(line,sizeof(line),bigrams) != NULL) {
	  if (strlen(line) > 1) {
	    line[strlen(line) - 1] = '\0';
	    sscanf(line,"%s%s",bigram1,bigram2);
	    if (Registry_get(good_right_hash,bigram1) &&
		Registry_get(ntot_hash,bigram2)) {
	      sprintf(bigram_space,"%s %s",bigram1,bigram2);
	      linecopy = mystrdup(bigram_space);
	      Registry_add(bigram_hash,linecopy,(char *)1);
	    }
	    if (Registry_get(good_left_hash,bigram2) &&
		Registry_get(ntot_hash,bigram1)) {
	      sprintf(bigram_space,"%s %s",bigram1,bigram2);
	      linecopy = mystrdup(bigram_space);
	      Registry_add(bigram_hash,linecopy,(char *)1);
	    }
	  }
	}
	fclose(bigrams);
	  fprintf(stderr,"START STATE TAGGER:: BIGRAMS READ\n");	

	
        tag_array_key = Darray_create();
	tag_array_val = Darray_create();
	Registry_fetch_contents(ntot_hash,tag_array_key,tag_array_val);

/********** START STATE ALGORITHM
  YOU CAN USE OR EDIT ONE OF THE TWO START STATE ALGORITHMS BELOW, 
  # OR REPLACE THEM WITH YOUR OWN ************************/


	strcpy(noun,"NN");
	strcpy(proper,"NNP");
	
/* UNCOMMENT THIS AND COMMENT OUT START STATE 2 IF ALL UNKNOWN WORDS
   SHOULD INITIALLY BE ASSUMED TO BE TAGGED WITH "NN".
   YOU CAN ALSO CHANGE "NN" TO A DIFFERENT TAG IF APPROPRIATE. */

      /*** START STATE 1 ***/
/*   for (count=0; count < Darray_len(tag_array_val);++count) 
	Darray_set(tag_array_val,count,noun); */

/* THIS START STATE ALGORITHM INITIALLY TAGS ALL UNKNOWN WORDS WITH TAG 
   "NN" (singular common noun) UNLESS THEY BEGIN WITH A CAPITAL LETTER, 
   IN WHICH CASE THEY ARE TAGGED WITH "NNP" (singular proper noun)
   YOU CAN CHANGE "NNP" and "NN" TO DIFFERENT TAGS IF APPROPRIATE.*/
       
     /*** START STATE 2 ***/

      for (count=0; count < Darray_len(tag_array_val);++count) 
	{
	  if (((char *)Darray_get(tag_array_key,count))[0] >='A' && 
	      ((char *)Darray_get(tag_array_key,count))[0] <= 'Z') 
	    Darray_set(tag_array_val,count,proper); 
	  else
	    Darray_set(tag_array_val,count,noun); }



/******************* END START STATE ALGORITHM ****************/
	for (count=0;count < Darray_len(rule_array);++count) {
	  tempstr = (char *)Darray_get(rule_array,count);
/*fprintf(stderr,"RULE IS: %s\n",tempstr);*/
fprintf(stderr,"s");
	  therule = perl_split_independent(tempstr);
/* we don't worry about freeing "rule" space, as this is a small fraction
   of total memory used */
	  therule2 = &therule[1];
	  rulesize=0;
	  perl_split_ptr = therule;
	  while(*(++perl_split_ptr) != NULL) {
	    ++rulesize;}

	  if (strcmp(therule[1],"char") == 0) {
	    for (count2=0;count2<Darray_len(tag_array_key);++count2) {
	      if (strcmp(Darray_get(tag_array_val,count2),therule[rulesize-1]) !=0) {
		if(strpbrk(Darray_get(tag_array_key,count2), therule[0]) !=
		   NULL) {
		  Darray_set(tag_array_val,count2,therule[rulesize-1]);
		}
	      }
	    }
	  }
	  else if (strcmp(therule2[1],"fchar") == 0) { 
	    for (count2=0;count2<Darray_len(tag_array_key);++count2) {
	      if (strcmp(Darray_get(tag_array_val,count2),therule[0]) ==0) {
		if(strpbrk(Darray_get(tag_array_key,count2), therule2[0]) !=
		   NULL) {
		  Darray_set(tag_array_val,count2,therule[rulesize-1]);
		}
	      }
	    }
	  }
	  else if (strcmp(therule[1],"deletepref") == 0) {
	    for (count2=0;count2<Darray_len(tag_array_key);++count2) {
	      if (strcmp(Darray_get(tag_array_val,count2),therule[rulesize-1])
		  !=0) {
		tempstr = Darray_get(tag_array_key,count2);
		for (count3=0;count3<atoi(therule[2]);++count3) {
		  if (tempstr[count3] != therule[0][count3])
		    break;}
		if (count3 == atoi(therule[2])) {
		  tempstr += atoi(therule[2]);
		  if (Registry_get(lexicon_hash,(char *)tempstr) != NULL ||
		      (EXTRAWDS &&
		       Registry_get(wordlist_hash,(char *)tempstr) != NULL)){
		    Darray_set(tag_array_val,count2,therule[rulesize-1]);}
		}
	      }
	    }
	  }
	  
	  else if (strcmp(therule2[1],"fdeletepref") == 0) {
	    for (count2=0;count2<Darray_len(tag_array_key);++count2) {
	      if (strcmp(Darray_get(tag_array_val,count2),therule[0]) == 0){ 
		tempstr=Darray_get(tag_array_key,count2);
		for (count3=0;count3<atoi(therule2[2]);++count3) {
		  if (tempstr[count3] != therule2[0][count3])
		    break;}
		if (count3 == atoi(therule2[2])) {
		  tempstr += atoi(therule2[2]);
		  if (Registry_get(lexicon_hash,(char *)tempstr) != NULL ||
		      (EXTRAWDS &&
		       Registry_get(wordlist_hash,(char *)tempstr) != NULL)){
		    
		    Darray_set(tag_array_val,count2,therule[rulesize-1]);}
		}
	      }
	    }
	  }
	  
	 
	  else if (strcmp(therule[1],"haspref") == 0) {
	    for (count2=0;count2<Darray_len(tag_array_key);++count2) {
	      if (strcmp(Darray_get(tag_array_val,count2),therule[rulesize-1])
		  !=0) {
		tempstr = Darray_get(tag_array_key,count2);
		for (count3=0;count3<atoi(therule[2]);++count3) {
		  if (tempstr[count3] != therule[0][count3])
		    break;}
		if (count3 == atoi(therule[2])) {
		    Darray_set(tag_array_val,count2,therule[rulesize-1]);}
	      }
	    }
	  }
	  
	  else if (strcmp(therule2[1],"fhaspref") == 0) {
	    for (count2=0;count2<Darray_len(tag_array_key);++count2) {
	      if (strcmp(Darray_get(tag_array_val,count2),therule[0]) == 0){ 
		tempstr=Darray_get(tag_array_key,count2);
		for (count3=0;count3<atoi(therule2[2]);++count3) {
		  if (tempstr[count3] != therule2[0][count3])
		    break;}
		if (count3 == atoi(therule2[2])) {
		    Darray_set(tag_array_val,count2,therule[rulesize-1]);}
	      }
	    }
	  }

	  
	  else if (strcmp(therule[1],"deletesuf") == 0) {
	    for (count2=0;count2<Darray_len(tag_array_key);++count2) {
	      if (strcmp(Darray_get(tag_array_val,count2),therule[rulesize-1])
		  !=0) {
		tempstr = Darray_get(tag_array_key,count2);
		tempcount=strlen(tempstr)-atoi(therule[2]);
		for (count3=tempcount;
		     count3<strlen(tempstr); ++count3) {
		  if (tempstr[count3] != therule[0][count3-tempcount])
		    break;}
		if (count3 == strlen(tempstr)) {
		  tempstr2 = mystrdup(tempstr);
		  tempstr2[tempcount] = '\0';
		  if (Registry_get(lexicon_hash,(char *)tempstr2) != NULL ||
		      (EXTRAWDS &&
		       Registry_get(wordlist_hash,(char *)tempstr2) != NULL)) {

		    Darray_set(tag_array_val,count2,therule[rulesize-1]);}
		  free(tempstr2);
		}
	      }
	    }
	  }
	  
	  else if (strcmp(therule2[1],"fdeletesuf") == 0) {
	    for (count2=0;count2<Darray_len(tag_array_key);++count2) {
	      if (strcmp(Darray_get(tag_array_val,count2),therule[0]) == 0){ 
		tempstr=Darray_get(tag_array_key,count2);
		tempcount=strlen(tempstr)-atoi(therule2[2]);
		for (count3=tempcount;
		     count3<strlen(tempstr); ++count3) {
		  if (tempstr[count3] != therule2[0][count3-tempcount])
		    break;}
		if (count3 == strlen(tempstr)){
		  tempstr2 = mystrdup(tempstr);
		  tempstr2[tempcount] = '\0';
		  if (Registry_get(lexicon_hash,(char *)tempstr2) != NULL||
		      (EXTRAWDS &&
		       Registry_get(wordlist_hash,(char *)tempstr2) != NULL)) {

		    Darray_set(tag_array_val,count2,therule[rulesize-1]);}
		}
	      }
	    }
	  }
	  else if (strcmp(therule[1],"hassuf") == 0) {
	    for (count2=0;count2<Darray_len(tag_array_key);++count2) {
	      if (strcmp(Darray_get(tag_array_val,count2),therule[rulesize-1])
		  !=0) {
		tempstr = Darray_get(tag_array_key,count2);
		tempcount=strlen(tempstr)-atoi(therule[2]);
		for (count3=tempcount;
		     count3<strlen(tempstr); ++count3) {
		  if (tempstr[count3] != therule[0][count3-tempcount])
		    break;}
		if (count3 == strlen(tempstr)) {

		    Darray_set(tag_array_val,count2,therule[rulesize-1]);}
	      }
	    }
	  }
	  
	  else if (strcmp(therule2[1],"fhassuf") == 0) {
	    for (count2=0;count2<Darray_len(tag_array_key);++count2) {
	      if (strcmp(Darray_get(tag_array_val,count2),therule[0]) == 0){ 
		tempstr=Darray_get(tag_array_key,count2);
		tempcount = strlen(tempstr)-atoi(therule2[2]);
		for (count3=tempcount;
		     count3<strlen(tempstr); ++count3) {
		  if (tempstr[count3] != therule2[0][count3-tempcount])
		    break;}
		if (count3 == strlen(tempstr)){

		    Darray_set(tag_array_val,count2,therule[rulesize-1]);}
	      }
	    }
	  }
	  
	  else if (strcmp(therule[1],"addpref") == 0) {
	    for (count2=0;count2<Darray_len(tag_array_key);++count2) {
	      if (strcmp(Darray_get(tag_array_val,count2),therule[rulesize-1])
		  == 0){
		sprintf(tempstr_space,"%s%s",
			therule[0],Darray_get(tag_array_key,count2));
		if (Registry_get(lexicon_hash,(char *)tempstr_space) != NULL
		    ||
		    (EXTRAWDS &&
		       Registry_get(wordlist_hash,(char *)tempstr_space) != NULL)) {

		  Darray_set(tag_array_val,count2,therule[rulesize-1]);}
	      }
	    }
	  }

	   else if (strcmp(therule2[1],"faddpref") == 0) {
	    for (count2=0;count2<Darray_len(tag_array_key);++count2) {
	      if (strcmp(Darray_get(tag_array_val,count2),therule[rulesize-1]) == 0){
		sprintf(tempstr_space,"%s%s",therule2[0],
			Darray_get(tag_array_key,count2));
		if (Registry_get(lexicon_hash,(char *)tempstr_space) != NULL
		    ||
		    (EXTRAWDS &&
		       Registry_get(wordlist_hash,(char *)tempstr_space) != NULL)) {

		  Darray_set(tag_array_val,count2,therule[rulesize-1]);}
	      }
	    }
	  }


	   else if (strcmp(therule[1],"addsuf") == 0) {
	    for (count2=0;count2<Darray_len(tag_array_key);++count2) {
	      if (strcmp(Darray_get(tag_array_val,count2),therule[rulesize-1])
		  !=0) {
		sprintf(tempstr_space,"%s%s",
		       Darray_get(tag_array_key,count2),
		       therule[0]);
		if (Registry_get(lexicon_hash,(char *)tempstr_space) != NULL
		    ||
		    (EXTRAWDS &&
		       Registry_get(wordlist_hash,(char *)tempstr_space) != NULL)){

		    Darray_set(tag_array_val,count2,therule[rulesize-1]);}
	      }
	    }
	  }
	  
	  
	   else if (strcmp(therule2[1],"faddsuf") == 0) {
	    for (count2=0;count2<Darray_len(tag_array_key);++count2) {
	      if (strcmp(Darray_get(tag_array_val,count2),therule[0])
		  ==0) {
		sprintf(tempstr_space,"%s%s",
			Darray_get(tag_array_key,count2),
			therule2[0]);
		if (Registry_get(lexicon_hash,(char *)tempstr_space) != NULL
		    ||
		   (EXTRAWDS &&
		       Registry_get(wordlist_hash,(char *)tempstr_space) != NULL)){

		    Darray_set(tag_array_val,count2,therule[rulesize-1]);}
	      }
	    }
	  }

	  
	   else if (strcmp(therule[1],"goodleft") == 0) {
	     for (count2=0;count2<Darray_len(tag_array_key);++count2) {
	       if (strcmp(Darray_get(tag_array_val,count2),therule[rulesize-1])
		   !=0) {
		 sprintf(bigram_space,"%s %s",
			Darray_get(tag_array_key,count2),therule[0]);
		 if (Registry_get(bigram_hash,(char *)bigram_space) != NULL) {

		   Darray_set(tag_array_val,count2,therule[rulesize-1]);}
	       }
	     }
	   }

	   else if (strcmp(therule2[1],"fgoodleft") == 0) {
	     for (count2=0;count2<Darray_len(tag_array_key);++count2) {
	       if (strcmp(Darray_get(tag_array_val,count2),therule[0])
		   ==0) {
		 sprintf(bigram_space,"%s %s",Darray_get(tag_array_key,count2),therule2[0]);
		 if (Registry_get(bigram_hash,(char *)bigram_space) != NULL) {

		   Darray_set(tag_array_val,count2,therule[rulesize-1]);}
	       }
	     }
	   }
	  
	  else if (strcmp(therule[1],"goodright") == 0) {
	     for (count2=0;count2<Darray_len(tag_array_key);++count2) {
	       if (strcmp(Darray_get(tag_array_val,count2),therule[rulesize-1])
		   !=0) {
		 sprintf(bigram_space,"%s %s",therule[0],Darray_get(tag_array_key,count2));
		 if (Registry_get(bigram_hash,(char *)bigram_space) != NULL) {

		   Darray_set(tag_array_val,count2,therule[rulesize-1]);}
	       }
	     }
	   }

	   else if (strcmp(therule2[1],"fgoodright") == 0) {
	     for (count2=0;count2<Darray_len(tag_array_key);++count2) {
	       if (strcmp(Darray_get(tag_array_val,count2),therule[0])
		   ==0) {
		 sprintf(bigram_space,"%s %s",therule2[0],Darray_get(tag_array_key,count2));
		 if (Registry_get(bigram_hash,(char *)bigram_space) != NULL) {

		   Darray_set(tag_array_val,count2,therule[rulesize-1]);}
	       }
	     }
	   }

 
	}
	fprintf(stderr,"\n");
	

	/* now go from darray to hash table */
		  
	tag_hash = Registry_create(Registry_strcmp,Registry_strhash);
	for (count=0;count<Darray_len(tag_array_key);++count) {
	  Registry_add(tag_hash,
		       (char *)Darray_get(tag_array_key,count),
		       (char *)Darray_get(tag_array_val,count)); }
	corpus = fopen(argv[2],"r");
	while(fgets(line,sizeof(line),corpus) != NULL) {
	  if (not_just_blank(line)) {
	    line[strlen(line) - 1] = '\0';
	    perl_split_ptr = perl_split(line);
	    temp_perl_split_ptr = perl_split_ptr;
	    count=-1;
	    while (*temp_perl_split_ptr != NULL) {
	      count++;
	      if ((atempptr = strchr(*temp_perl_split_ptr,'/')) != NULL
		  && *(atempptr+1) == '/') {
		/* a word can be pretagged by putting two slashes between the */
		/* word and the tag ::  The boy//NN ate . */
		/* if a word is pretagged, we just spit out the pretagging */
		printf("%s ",*temp_perl_split_ptr);}
	      else if
		((tempstr = Registry_get(lexicon_hash,*temp_perl_split_ptr))
		 != NULL) {
		  printf("%s/%s ",*temp_perl_split_ptr,tempstr);}
	      else {
		printf("%s/%s ",*temp_perl_split_ptr,
		       Registry_get(tag_hash,*temp_perl_split_ptr)); }
	      ++temp_perl_split_ptr;
	    }
	    free(*perl_split_ptr);
	    free(perl_split_ptr);
	    if (count >=0) { printf("\n");}
	  }
	}
}


