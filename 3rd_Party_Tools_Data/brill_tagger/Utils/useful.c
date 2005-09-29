#include <string.h>
#include "useful.h"

char *mystrdup(thestr) 
  char *thestr;
{

  return((char *)strcpy(malloc(strlen(thestr)+1),thestr));
}

int not_just_blank(thestr)
char *thestr;
/* make sure not just processing a no-character line */
{ 
  char *thestr2;
  thestr2 = thestr;
  while(*thestr2 != '\0') {
    if (*thestr2 != ' ' && *thestr2 != '\t' && *thestr2 != '\n'){
      return(1); }
    ++thestr2;
  }
  return(0);
}

int num_words(thestr)
  char *thestr;
{
  int count,returncount;
  
  returncount=0;
  count=0;
  while (thestr[count] != '\0' && (thestr[count] == ' ' 
	 || thestr[count] == '\t')) ++count;
  while (thestr[count++] != '\0') {
    if (thestr[count-1] == ' ' || thestr[count-1] == '\t') {
      ++returncount;
      while (thestr[count] == ' ' || thestr[count] == '\t')
	++count;
      if (thestr[count] == '\0') --returncount;
    }
  }
  return(returncount);
}
