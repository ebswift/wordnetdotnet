/* Copyright @ 1993 MIT and University of Pennsylvania */
/* Written by Eric Brill */
/*THIS SOFTWARE IS PROVIDED "AS IS", AND M.I.T. MAKES NO REPRESENTATIONS 
OR WARRANTIES, EXPRESS OR IMPLIED.  By way of example, but not 
limitation, M.I.T. MAKES NO REPRESENTATIONS OR WARRANTIES OF 
MERCHANTABILITY OR FITNESS FOR ANY PARTICULAR PURPOSE OR THAT THE USE OF 
THE LICENSED SOFTWARE OR DOCUMENTATION WILL NOT INFRINGE ANY THIRD PARTY 
PATENTS, COPYRIGHTS, TRADEMARKS OR OTHER RIGHTS.   */

#include <stdio.h>
#include <fcntl.h>
#include <malloc.h>
#include <string.h>
#include <errno.h>
#include "lex.h"
#include "darray.h"
#include "registry.h"
#include "memory.h"
#include "useful.h"
#define MAXTAGLEN 256  /* max char length of pos tags */
#define MAXWORDLEN 256 /* max char length of words */

#define RESTRICT_MOVE 1   /* if this is set to 1, then a rule "change a tag */
			  /* from x to y" will only apply to a word if:
			     a) the word was not in the training set or
			     b) the word was tagged with y at least once in
			     the training set  
			     When training on a very small corpus, better
			     performance might be obtained by setting this to
			     0, but for most uses it should be set to 1 */
			     

char            staart[] = "STAART";


void change_the_tag(theentry,thetag,theposition)
  char **theentry, *thetag;
  int theposition;

{
  free(theentry[theposition]);
  theentry[theposition] = mystrdup(thetag);
}



main(argc, argv)
	int             argc;
	char           *argv[];
{

        FILE           *allowedmovefile;
	FILE           *changefile;

	Registry SEENTAGGING,WORDS;
	char            *atempstrptr;
	char            line[5000];	/* input line buffer */
	char            space[500];
	int             linenums,tagnums;
	char            **word_corpus_array,**tag_corpus_array;
	int            word_corpus_array_index,tag_corpus_array_index;
	char          **split_ptr,**split_ptr2;
	int             corpus_size;
	int             corpussize;
	int             count,tempcount1,tempcount2;
	char            old[MAXTAGLEN], new[MAXTAGLEN], when[50],
	                tag[MAXTAGLEN], lft[MAXTAGLEN], rght[MAXTAGLEN],
	                prev1[MAXTAGLEN], prev2[MAXTAGLEN], next1[MAXTAGLEN],
	                next2[MAXTAGLEN], curtag[MAXTAGLEN],
	                curwd[MAXWORDLEN],
	                tempstr[MAXWORDLEN],word[MAXWORDLEN],**perl_split_ptr,
	                **perl_split_ptr2,*atempstr,atempstr2[256];

	corpussize = atoi(argv[3]);
	linenums = atoi(argv[4]);
	tagnums = atoi(argv[5]);
	word_corpus_array = (char **)malloc(sizeof(char *) * corpussize);
	tag_corpus_array = (char **)malloc(sizeof(char *) * corpussize);
	word_corpus_array_index = tag_corpus_array_index = 0;


/* read in corpus from stdin.  This corpus has already been tagged (output of */
/* the start state tagger) */


	while (gets(line) != NULL) {
	  if (not_just_blank(line)){  
	    word_corpus_array[word_corpus_array_index++] = staart;
	    word_corpus_array[word_corpus_array_index++] = staart;
	    tag_corpus_array[tag_corpus_array_index++] = staart;
	    tag_corpus_array[tag_corpus_array_index++] = staart;
	    if (line[strlen(line)-1] == ' ')
	      line[strlen(line) - 1] = '\0';
	    split_ptr = perl_split(line);
	    split_ptr2 = split_ptr;
	    while (*split_ptr != NULL) {
	      atempstrptr = strrchr(*split_ptr,'/');
	      if (atempstrptr == NULL) {
		perror("Every word in the input to the final state tagger must be tagged\n");
		return(0);}
	      *atempstrptr = '\0';
	      ++atempstrptr;
	      word_corpus_array[word_corpus_array_index++] =
		mystrdup(*split_ptr);
	      tag_corpus_array[tag_corpus_array_index++] = mystrdup(atempstrptr);
	      ++split_ptr;
	    }
	    free(*split_ptr2);
	    free(split_ptr2);
	  }
	}
fprintf(stderr,"FINAL STATE TAGGER:: READ IN OUTPUT FROM START\n");	

	if (RESTRICT_MOVE){  /* if in restrict-move mode (described above), */
	  /* then we need to read in the lexicon file to see allowable */
	  /* taggings for seen words*/
	  


	  SEENTAGGING = Registry_create(Registry_strcmp,Registry_strhash);
	  Registry_size_hint(SEENTAGGING,tagnums);
	  WORDS = Registry_create(Registry_strcmp,Registry_strhash);
	  Registry_size_hint(WORDS,linenums);
	  allowedmovefile = fopen(argv[2], "r");
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
	}

fprintf(stderr,"FINAL STATE TAGGER:: READ IN LEXICON\n");
/* read in rule file, and process each rule */
	changefile = fopen(argv[1], "r");
	corpus_size = tag_corpus_array_index - 1;
	while (fgets(line, sizeof(line), changefile) != NULL) {
	  if (not_just_blank(line)) {
	    line[strlen(line) - 1] = '\0';
	    split_ptr = perl_split(line);
	    strcpy(old, split_ptr[0]);
	    strcpy(new, split_ptr[1]);
	    strcpy(when, split_ptr[2]);
/*	    fprintf(stderr,"OLD: %s NEW: %s WHEN: %s\n", old, new, when);*/
	    fprintf(stderr,"f");
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
	    }else if (strcmp(when,"LBIGRAM") == 0||
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
	    
	    
	    for (count = 0; count <= corpus_size; ++count) {
	      strcpy(curtag, tag_corpus_array[count]);
	      if (strcmp(curtag, old) == 0) {
		strcpy(curwd,word_corpus_array[count]);
		sprintf(atempstr2,"%s %s",curwd,new);

		if (! RESTRICT_MOVE || 
		    ! Registry_get(WORDS,curwd) ||
		    Registry_get(SEENTAGGING,atempstr2)) {

		  if (strcmp(when, "SURROUNDTAG") == 0) {
		    if (count < corpus_size && count > 0) {
		      if (strcmp(lft, tag_corpus_array[count - 1]) == 0 &&
			  strcmp(rght, tag_corpus_array[count + 1]) == 0)
			change_the_tag(tag_corpus_array, new, count);
		    }
		  } else if (strcmp(when, "NEXTTAG") == 0) {
		    if (count < corpus_size) {
		      if (strcmp(tag,tag_corpus_array[count + 1]) == 0)
			change_the_tag(tag_corpus_array, new, count);
		    }
		  }  
		  else if (strcmp(when, "CURWD") == 0) {
		    if (strcmp(word, word_corpus_array[count]) == 0)
		      change_the_tag(tag_corpus_array, new, count);
		  } 
		  else if (strcmp(when, "NEXTWD") == 0) {
		    if (count < corpus_size) {
		      if (strcmp(word, word_corpus_array[count + 1]) == 0)
			change_the_tag(tag_corpus_array, new, count);
		    }
		  } 
		   else if (strcmp(when, "RBIGRAM") == 0) {
		  if (count < corpus_size) {
		    if (strcmp(word, word_corpus_array[count]) ==
			0 &&
			strcmp(next1, word_corpus_array[count+1]) ==
			0)
		      change_the_tag(tag_corpus_array, new, count);
		  }
		} 
		  else if (strcmp(when, "WDNEXTTAG") == 0) {
		  if (count < corpus_size) {
		    if (strcmp(word, word_corpus_array[count]) ==
			0 &&
			strcmp(next1, tag_corpus_array[count+1]) ==
			0)
		      change_the_tag(tag_corpus_array, new, count);
		  }
		}

		  else if (strcmp(when, "WDAND2AFT") == 0) {
		  if (count < corpus_size-1) {
		    if (strcmp(word, word_corpus_array[count]) ==
			0 &&
			strcmp(next2, word_corpus_array[count+2]) ==
			0)
		      change_the_tag(tag_corpus_array, new, count);
		  }
		}
		  else if (strcmp(when, "WDAND2TAGAFT") == 0) {
		  if (count < corpus_size-1) {
		    if (strcmp(word, word_corpus_array[count]) ==
			0 &&
			strcmp(next2, tag_corpus_array[count+2]) ==
			0)
		      change_the_tag(tag_corpus_array, new, count);
		  }
		}

		  else if (strcmp(when, "NEXT2TAG") == 0) {
		    if (count < corpus_size - 1) {
		      if (strcmp(tag, tag_corpus_array[count + 2]) == 0)
			change_the_tag(tag_corpus_array, new, count);
		    }
		  } else if (strcmp(when, "NEXT2WD") == 0) {
		    if (count < corpus_size - 1) {
		      if (strcmp(word, word_corpus_array[count + 2]) == 0)
			change_the_tag(tag_corpus_array, new, count);
		    }
		  } else if (strcmp(when, "NEXTBIGRAM") == 0) {
		    if (count < corpus_size - 1) {
		      if
			(strcmp(next1, tag_corpus_array[count + 1]) == 0 &&
			 strcmp(next2, tag_corpus_array[count + 2]) == 0)
			  change_the_tag(tag_corpus_array, new, count);
		    }
		  } else if (strcmp(when, "NEXT1OR2TAG") == 0) {
		    if (count < corpus_size) {
		      if (count < corpus_size-1) 
			tempcount1 = count+2;
		      else 
			tempcount1 = count+1;
		      if
			(strcmp(tag, tag_corpus_array[count + 1]) == 0 ||
			 strcmp(tag, tag_corpus_array[tempcount1]) == 0)
			  change_the_tag(tag_corpus_array, new, count);
		    }
		  }  else if (strcmp(when, "NEXT1OR2WD") == 0) {
		    if (count < corpus_size) {
		      if (count < corpus_size-1) 
			tempcount1 = count+2;
		      else 
			tempcount1 = count+1;
		      if
			(strcmp(word, word_corpus_array[count + 1]) == 0 ||
			 strcmp(word, word_corpus_array[tempcount1]) == 0)
			  change_the_tag(tag_corpus_array, new, count);
		    }
		  }   else if (strcmp(when, "NEXT1OR2OR3TAG") == 0) {
		    if (count < corpus_size) {
		      if (count < corpus_size -1)
			tempcount1 = count+2;
		      else 
			tempcount1 = count+1;
		      if (count < corpus_size-2)
			tempcount2 = count+3;
		      else 
			tempcount2 =count+1;
		      if
			(strcmp(tag, tag_corpus_array[count + 1]) == 0 ||
			 strcmp(tag, tag_corpus_array[tempcount1]) == 0 ||
			 strcmp(tag, tag_corpus_array[tempcount2]) == 0)
			  change_the_tag(tag_corpus_array, new, count);
		    }
		  } else if (strcmp(when, "NEXT1OR2OR3WD") == 0) {
		    if (count < corpus_size) {
		      if (count < corpus_size -1)
			tempcount1 = count+2;
		      else 
			tempcount1 = count+1;
		      if (count < corpus_size-2)
			tempcount2 = count+3;
		      else 
			tempcount2 =count+1;
		      if
			(strcmp(word, word_corpus_array[count + 1]) == 0 ||
			 strcmp(word, word_corpus_array[tempcount1]) == 0 ||
			 strcmp(word, word_corpus_array[tempcount2]) == 0)
			  change_the_tag(tag_corpus_array, new, count);
		    }
		  }  else if (strcmp(when, "PREVTAG") == 0) {
		    if (count > 0) {
		      if (strcmp(tag, tag_corpus_array[count - 1]) == 0) {
			change_the_tag(tag_corpus_array, new, count);
		      }
		    }
		  } else if (strcmp(when, "PREVWD") == 0) {
		    if (count > 0) {
		      if (strcmp(word, word_corpus_array[count - 1]) == 0) {
			change_the_tag(tag_corpus_array, new, count);
		      }
		    }
		  } 
		   else if (strcmp(when, "LBIGRAM") == 0) {
		  if (count > 0) {
		    if (strcmp(word, word_corpus_array[count]) ==
			0 &&
		      strcmp(prev1, word_corpus_array[count-1]) ==
			0)
		      change_the_tag(tag_corpus_array, new, count);
		  }
		}
		  else if (strcmp(when, "WDPREVTAG") == 0) {
		  if (count > 0) {
		    if (strcmp(word, word_corpus_array[count]) ==
			0 &&
			strcmp(prev1, tag_corpus_array[count-1]) ==
			0)
		      change_the_tag(tag_corpus_array, new, count);
		  }
		}
		  else if (strcmp(when, "WDAND2BFR") == 0) {
		  if (count > 1) {
		    if (strcmp(word, word_corpus_array[count]) ==
			0 &&
			strcmp(prev2, word_corpus_array[count-2]) ==
			0)
		      change_the_tag(tag_corpus_array, new, count);
		  }
		}
		  else if (strcmp(when, "WDAND2TAGBFR") == 0) {
		  if (count > 1) {
		    if (strcmp(word, word_corpus_array[count]) ==
			0 &&
			strcmp(prev2, tag_corpus_array[count-2]) ==
			0)
		      change_the_tag(tag_corpus_array, new, count);
		  }
		}

		  else if (strcmp(when, "PREV2TAG") == 0) {
		    if (count > 1) {
		      if (strcmp(tag, tag_corpus_array[count - 2]) == 0)
			change_the_tag(tag_corpus_array, new, count);
		    }
		  } else if (strcmp(when, "PREV2WD") == 0) {
		    if (count > 1) {
		      if (strcmp(word, word_corpus_array[count - 2]) == 0)
			change_the_tag(tag_corpus_array, new, count);
		    }
		  } else if (strcmp(when, "PREV1OR2TAG") == 0) {
		    if (count > 0) {
		      if (count > 1) 
			tempcount1 = count-2;
		      else
			tempcount1 = count-1;
		      if (strcmp(tag, tag_corpus_array[count - 1]) == 0 ||
			  strcmp(tag, tag_corpus_array[tempcount1]) == 0)
			change_the_tag(tag_corpus_array, new, count);
		    }
		  } else if (strcmp(when, "PREV1OR2WD") == 0) {
		    if (count > 0) {
		      if (count > 1) 
			tempcount1 = count-2;
		      else
			tempcount1 = count-1;
		      if (strcmp(word, word_corpus_array[count - 1]) == 0 ||
			  strcmp(word, word_corpus_array[tempcount1]) == 0)
			change_the_tag(tag_corpus_array, new, count);
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
		      if (strcmp(tag, tag_corpus_array[count - 1]) == 0 ||
			  strcmp(tag, tag_corpus_array[tempcount1]) == 0 ||
			  strcmp(tag, tag_corpus_array[tempcount2]) == 0)
			change_the_tag(tag_corpus_array, new, count);
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
		      if (strcmp(word, word_corpus_array[count - 1]) == 0 ||
			  strcmp(word, word_corpus_array[tempcount1]) == 0 ||
			  strcmp(word, word_corpus_array[tempcount2]) == 0)
			change_the_tag(tag_corpus_array, new, count);
		    }
		  } else if (strcmp(when, "PREVBIGRAM") == 0) {
		    if (count > 1) {
		      if (strcmp(prev2, tag_corpus_array[count - 1]) == 0 &&
			  strcmp(prev1, tag_corpus_array[count - 2]) == 0)
			change_the_tag(tag_corpus_array, new, count);
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
	  }
	}
fprintf(stderr,"\n");
	for (count = 0; count <= corpus_size; ++count) {
	  strcpy(tempstr, tag_corpus_array[count]);
	  if (strcmp(tempstr, "STAART") == 0 &&
	      strcmp(tag_corpus_array[count + 1], "STAART") == 0 &&
	      count !=0)
	    printf("\n");
	  else if (strcmp(tempstr, "STAART") != 0) {
	    printf("%s/%s ", word_corpus_array[count], tempstr);
	  }
	}
	printf("\n");
      }
