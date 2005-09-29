/* Copyright @ 1993 MIT */
/* Written by Eric Brill */
/*THIS SOFTWARE IS PROVIDED "AS IS", AND M.I.T. MAKES NO REPRESENTATIONS 
OR WARRANTIES, EXPRESS OR IMPLIED.  By way of example, but not 
limitation, M.I.T. MAKES NO REPRESENTATIONS OR WARRANTIES OF 
MERCHANTABILITY OR FITNESS FOR ANY PARTICULAR PURPOSE OR THAT THE USE OF 
THE LICENSED SOFTWARE OR DOCUMENTATION WILL NOT INFRINGE ANY THIRD PARTY 
PATENTS, COPYRIGHTS, TRADEMARKS OR OTHER RIGHTS.   */

#include <stdio.h>
#include <fcntl.h>
#include <string.h>
#include <malloc.h>
#include "lex.h"
#include "darray.h"
#include "registry.h"
#include "memory.h"
#include "useful.h"
#define MAXWORD 100
#define GUESSNUMWORDS 100000 /* guess at number of words in LEXICON*/

char            start[] = "STAART";




char globalstring[100];
char gs2[100];

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
  if (temp2 == NULL ) { free(temp); return(0);}
  else { free(temp); return(1);}
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
 
main(argc, argv)
	int             argc;
	char           *argv[];
{

        char *curtagcpy;
        FILE           *allowedmovefile,*changefile;
	Registry SEENTAGGING,WORDS;
	char            line[5000];	/* input line buffer */
	Darray          word_corpus_array;
	Darray          tag_corpus_array;
	char          **split_ptr;
	int             corpus_size = 0;
	int             count,tempcount1,tempcount2,ccount,tempnum;
	char            old[10], *new, when[300], tag[100], lft[100], rght[100],
	                prev1[100], prev2[100], next1[100], next2[100], curtag[100],
	                tempstr[300],word[300];
	char **perl_split_ptr,**perl_split_ptr2,*atempstr,atempstr2[256],
	      space[500],curwd[100];

	word_corpus_array = Darray_create();
	tag_corpus_array = Darray_create();


	changefile = fopen(argv[1],"r");


	while (gets(line) != NULL) {
	  if (strlen(line) > 1){
	    Darray_addh(word_corpus_array, start);
	    Darray_addh(word_corpus_array, start);
	    Darray_addh(tag_corpus_array,start);
	    Darray_addh(tag_corpus_array, start);
	    corpus_size++;
	    split_ptr = perl_split(line);
	    while (*split_ptr != NULL) {
	      Darray_addh(word_corpus_array, *split_ptr);
	      while ((*(++*split_ptr)) != '/') {
	      }
	      **split_ptr = '\0';
	      Darray_addh(tag_corpus_array, ++*split_ptr);
	      ++split_ptr;
	    }
	  }
	}

	  SEENTAGGING = Registry_create(Registry_strcmp,Registry_strhash);
	  Registry_size_hint(SEENTAGGING,GUESSNUMWORDS);
	  WORDS = Registry_create(Registry_strcmp,Registry_strhash);
	  Registry_size_hint(WORDS,GUESSNUMWORDS);
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



	corpus_size = Darray_len(tag_corpus_array) - 1;
	while(fgets(line,sizeof(line),changefile) != NULL) {
	  if (strlen(line) > 1) {
	    line[strlen(line)-1] = '\0';
	    split_ptr = perl_split(line);
	    strcpy(old, split_ptr[0]);
	    new = (char *) malloc(sizeof(char) * 30);
	    strcpy(new, split_ptr[1]);
	    strcpy(when, split_ptr[2]);
	    fprintf(stderr,"x");
fprintf(stderr,"OLD: %s NEW: %s WHEN: %s\n",split_ptr[0],split_ptr[1],split_ptr[2]);
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
		strcmp(when, "NEXT2WD") == 0 ||
		strcmp(when, "NEXT1OR2WD") == 0 ||
		strcmp(when, "CURRENTWD") == 0 ||
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
	    }

		     
	    
	    for (count = 0; count <= corpus_size; ++count) {
	      strcpy(curtag, (char *) Darray_get(tag_corpus_array, count));
	      if (strcmp(first_tag_nospace(curtag),old) == 0
		  && ! is_tagged_with(new,curtag)) {
		strcpy(curwd,(char *)Darray_get(word_corpus_array,count));
		sprintf(atempstr2,"%s %s",curwd,new);
		
		if (! Registry_get(WORDS,curwd) ||
		    Registry_get(SEENTAGGING,atempstr2)) {
		  if (strcmp(when,"ALWAYS") == 0) {
		    strcat(curtag,"_");
		    strcat(curtag,new);
		    curtagcpy = mystrdup(curtag);
		    Darray_set(tag_corpus_array, count, curtagcpy); 
		  }
		  
		  else if (strcmp(when, "SURROUNDTAG") == 0) {
		    if (count < corpus_size && count > 0) {
		    if (strcmp(lft, first_tag_nospace(Darray_get(tag_corpus_array, count - 1))) == 0 &&
			strcmp(rght, first_tag_nospace(Darray_get(tag_corpus_array, count + 1))) == 0){
		      strcat(curtag,"_");
		      strcat(curtag,new);
		      curtagcpy = mystrdup(curtag);
		      Darray_set(tag_corpus_array, count, curtagcpy); }
		  }
		} else if (strcmp(when, "NEXTTAG") == 0) {
		  if (count < corpus_size) {
		    if (strcmp(tag, first_tag_nospace(Darray_get(tag_corpus_array, count + 1))) == 0){
		      strcat(curtag,"_");
		      strcat(curtag,new);
		      curtagcpy = mystrdup(curtag);
		      Darray_set(tag_corpus_array, count, curtagcpy);}
		  }
		}  else if (strcmp(when, "NEXTWD") == 0) {
		  if (count < corpus_size) {
		    if (strcmp(word, Darray_get(word_corpus_array, count + 1)) == 0){
		      strcat(curtag,"_");
		      strcat(curtag,new);
		      curtagcpy = mystrdup(curtag);
		      Darray_set(tag_corpus_array, count, curtagcpy);}
		  }
		}  else if (strcmp(when, "RBIGRAM") == 0) {
		  if (count < corpus_size) {
		    if (strcmp(word, Darray_get(word_corpus_array, count)) ==
			0 &&
			strcmp(next1, Darray_get(word_corpus_array, count+1)) ==
			0){
		      strcat(curtag,"_");
		      strcat(curtag,new);
		      curtagcpy = mystrdup(curtag);
		      Darray_set(tag_corpus_array, count, curtagcpy);}
		  }
		} 
		else if (strcmp(when, "WDNEXTTAG") == 0) {
		  if (count < corpus_size) {
		    if (strcmp(word, Darray_get(word_corpus_array, count)) ==
			0 &&
			strcmp(next1, first_tag_nospace(Darray_get(tag_corpus_array, count+1))) ==
			0){
		      strcat(curtag,"_");
		      strcat(curtag,new);
		      curtagcpy = mystrdup(curtag);
		      Darray_set(tag_corpus_array, count, curtagcpy);}
		  }
		}
		  else if (strcmp(when, "CURRENTWD") == 0) {
		  if (count < corpus_size) {
		    if (strcmp(word, Darray_get(word_corpus_array, count)) ==0) {
		      strcat(curtag,"_");
		      strcat(curtag,new);
		      curtagcpy = mystrdup(curtag);
		      Darray_set(tag_corpus_array, count, curtagcpy);
		  }}
		}
		else if (strcmp(when, "NEXT2TAG") == 0) {
		  if (count < corpus_size - 1) {
		    if (strcmp(tag, first_tag_nospace(Darray_get(tag_corpus_array, count + 2))) == 0){
		      strcat(curtag,"_");
		      strcat(curtag,new);
		      curtagcpy = mystrdup(curtag);
		      Darray_set(tag_corpus_array, count, curtagcpy);}
		  }
		} else if (strcmp(when, "NEXT2WD") == 0) {
		  if (count < corpus_size - 1) {
		    if (strcmp(word, Darray_get(word_corpus_array, count + 2)) == 0){
		      strcat(curtag,"_");
		      strcat(curtag,new);
		      curtagcpy = mystrdup(curtag);
		      Darray_set(tag_corpus_array, count, curtagcpy);}
		  }
		} else if (strcmp(when, "NEXTBIGRAM") == 0) {
		  if (count < corpus_size - 1) {
		    if
		      (strcmp(next1, first_tag_nospace(Darray_get(tag_corpus_array, count + 1))) == 0 &&
		       strcmp(next2, first_tag_nospace(Darray_get(tag_corpus_array, count + 2))) == 0){
			strcat(curtag,"_");
		      strcat(curtag,new);
		      curtagcpy = mystrdup(curtag);
			Darray_set(tag_corpus_array, count, curtagcpy);}
		  }
		} else if (strcmp(when, "NEXT1OR2TAG") == 0) {
		  if (count < corpus_size) {
		    if (count < corpus_size-1) 
		      tempcount1 = count+2;
		    else 
		      tempcount1 = count+1;
		    if
		    (strcmp(tag, first_tag_nospace(Darray_get(tag_corpus_array, count + 1))) == 0 ||
		     strcmp(tag, first_tag_nospace(Darray_get(tag_corpus_array, tempcount1))) == 0){
		      strcat(curtag,"_");
		      strcat(curtag,new);
		      curtagcpy = mystrdup(curtag);
		      Darray_set(tag_corpus_array, count, curtagcpy);}
		  }
		}  else if (strcmp(when, "NEXT1OR2WD") == 0) {
		  if (count < corpus_size) {
		    if (count < corpus_size-1) 
		      tempcount1 = count+2;
		    else 
		      tempcount1 = count+1;
		    if
		    (strcmp(word, Darray_get(word_corpus_array, count + 1)) == 0 ||
		     strcmp(word, Darray_get(word_corpus_array, tempcount1)) == 0){
		      strcat(curtag,"_");
		      strcat(curtag,new);
		      curtagcpy = mystrdup(curtag);
		      Darray_set(tag_corpus_array, count, curtagcpy);}
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
		      (strcmp(tag, first_tag_nospace(Darray_get(tag_corpus_array, count + 1))) == 0 ||
		       strcmp(tag, first_tag_nospace(Darray_get(tag_corpus_array, tempcount1))) == 0 ||
		       strcmp(tag, first_tag_nospace(Darray_get(tag_corpus_array, tempcount2))) == 0){
			strcat(curtag,"_");
		      strcat(curtag,new);
		      curtagcpy = mystrdup(curtag);
			Darray_set(tag_corpus_array, count, curtagcpy);}
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
		      (strcmp(word, Darray_get(word_corpus_array, count + 1)) == 0 ||
		       strcmp(word, Darray_get(word_corpus_array, tempcount1)) == 0 ||
		       strcmp(word, Darray_get(word_corpus_array, tempcount2)) == 0){
			strcat(curtag,"_");
		      strcat(curtag,new);
		      curtagcpy = mystrdup(curtag);
			Darray_set(tag_corpus_array, count, curtagcpy);}
		  }
		}  else if (strcmp(when, "PREVTAG") == 0) {
		  if (count > 0) {
		    if (strcmp(tag, first_tag_nospace(Darray_get(tag_corpus_array, count - 1))) == 0) {
		      strcat(curtag,"_");
		      strcat(curtag,new);
		      curtagcpy = mystrdup(curtag);
		      Darray_set(tag_corpus_array, count, curtagcpy);
		    }
		  }
		} else if (strcmp(when, "PREVWD") == 0) {
		  if (count > 0) {
		    if (strcmp(word, Darray_get(word_corpus_array, count - 1)) == 0) {
		      strcat(curtag,"_");
		      strcat(curtag,new);
		      curtagcpy = mystrdup(curtag);
		      Darray_set(tag_corpus_array, count, curtagcpy);
		    }
		  }
		}  else if (strcmp(when, "LBIGRAM") == 0) {
		  if (count > 0) {
		    if (strcmp(word, Darray_get(word_corpus_array, count)) ==
			0 &&
			strcmp(prev1, Darray_get(word_corpus_array, count-1)) ==
			0){
		      strcat(curtag,"_");
		      strcat(curtag,new);
		      curtagcpy = mystrdup(curtag);
		      Darray_set(tag_corpus_array, count, curtagcpy);}
		  }
		}
		else if (strcmp(when, "WDPREVTAG") == 0) {
		  if (count > 0) {
		    if (strcmp(word, Darray_get(word_corpus_array, count)) ==
			0 &&
			strcmp(prev1, first_tag_nospace(Darray_get(tag_corpus_array, count-1))) ==
			0){
		      strcat(curtag,"_");
		      strcat(curtag,new);
		      curtagcpy = mystrdup(curtag);
		      Darray_set(tag_corpus_array, count, curtagcpy);}
		  }
		}
		else if (strcmp(when, "PREV2TAG") == 0) {
		  if (count > 1) {
		    if (strcmp(tag, first_tag_nospace(Darray_get(tag_corpus_array, count - 2))) == 0){
		      strcat(curtag,"_");
		      strcat(curtag,new);
		      curtagcpy = mystrdup(curtag);
		      Darray_set(tag_corpus_array, count, curtagcpy);}
		  }
		} else if (strcmp(when, "PREV2WD") == 0) {
		  if (count > 1) {
		    if (strcmp(word, Darray_get(word_corpus_array, count - 2)) == 0){
		      strcat(curtag,"_");
		      strcat(curtag,new);
		      curtagcpy = mystrdup(curtag);
		      Darray_set(tag_corpus_array, count, curtagcpy);}
		  }
		} else if (strcmp(when, "PREV1OR2TAG") == 0) {
		  if (count > 0) {
		    if (count > 1) 
		      tempcount1 = count-2;
		    else
		      tempcount1 = count-1;
		    if (strcmp(tag, first_tag_nospace(Darray_get(tag_corpus_array, count - 1))) == 0 ||
			strcmp(tag, first_tag_nospace(Darray_get(tag_corpus_array, tempcount1))) == 0){
		      strcat(curtag,"_");
		      strcat(curtag,new);
		      curtagcpy = mystrdup(curtag);
		      Darray_set(tag_corpus_array, count, curtagcpy);}
		  }
		} else if (strcmp(when, "PREV1OR2WD") == 0) {
		  if (count > 0) {
		    if (count > 1) 
		      tempcount1 = count-2;
		    else
		      tempcount1 = count-1;
		    if (strcmp(word, Darray_get(word_corpus_array, count - 1)) == 0 ||
			strcmp(word, Darray_get(word_corpus_array, tempcount1)) == 0){
		      strcat(curtag,"_");
		      strcat(curtag,new);
		      curtagcpy = mystrdup(curtag);
		      Darray_set(tag_corpus_array, count, curtagcpy);}
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
		    if (strcmp(tag, first_tag_nospace(Darray_get(tag_corpus_array, count - 1))) == 0 ||
			strcmp(tag, first_tag_nospace(Darray_get(tag_corpus_array, tempcount1))) == 0 ||
			strcmp(tag, first_tag_nospace(Darray_get(tag_corpus_array, tempcount2))) == 0){
		      strcat(curtag,"_");
		      strcat(curtag,new);
		      curtagcpy = mystrdup(curtag);
		      Darray_set(tag_corpus_array, count, curtagcpy);}
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
		    if (strcmp(word, Darray_get(word_corpus_array, count - 1)) == 0 ||
			strcmp(word, Darray_get(word_corpus_array, tempcount1)) == 0 ||
			strcmp(word, Darray_get(word_corpus_array, tempcount2)) == 0){
		      strcat(curtag,"_");
		      strcat(curtag,new);
		      curtagcpy = mystrdup(curtag);
		      Darray_set(tag_corpus_array, count, curtagcpy);}
		  }
		} else if (strcmp(when, "PREVBIGRAM") == 0) {
		  if (count > 1) {
		    if (strcmp(prev1, first_tag_nospace(Darray_get(tag_corpus_array, count - 2))) == 0 &&
			strcmp(prev2, first_tag_nospace(Darray_get(tag_corpus_array, count - 1))) == 0){
		      strcat(curtag,"_");
		      strcat(curtag,new);
		      curtagcpy = mystrdup(curtag);
		      Darray_set(tag_corpus_array, count, curtagcpy);}
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
	for (count = 0; count <= corpus_size; ++count) {
	  strcpy(tempstr, Darray_get(tag_corpus_array, count));
	  if (strcmp(tempstr, "STAART") == 0 &&
	      strcmp(Darray_get(tag_corpus_array, count + 1), "STAART") == 0 &&
	      count !=0 )
	    printf("\n");
	  else if (strcmp(tempstr, "STAART") != 0)
	    printf("%s/%s ", Darray_get(word_corpus_array, count), tempstr);
	}
	printf("\n");
      }
