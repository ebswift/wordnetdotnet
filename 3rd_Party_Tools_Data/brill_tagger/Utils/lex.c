#include <stdio.h>
#include "useful.h"
#include "lex.h"

char *append_with_space(w1,w2)
     char *w1,*w2;
{
  char *result;
  
  result = 
    (char *)malloc((strlen(w1) + strlen(w2)+2) * sizeof(char)); 
  strcpy(result,w1);
  strcat(result," ");
  strcat(result,w2);
  return(result); }




char *append_with_char(w1,w2,w3)
     char *w1,*w2,w3;
{
  char *result;
  
  result = 
    (char *)malloc((strlen(w1) + strlen(w2)+2) * sizeof(char)); 
  sprintf(result,"%s%c%s",w1,w3,w2);
  return(result);
}
  


char **perl_split(buf)
     char *buf;
{
  char **return_buf;
  int cntr = 0;
  char *temp,*temp2;

  temp2 = (char *)malloc(sizeof(char)*(1+strlen(buf)));
  while(*buf == ' ' || *buf == '\t') ++buf;
  strcpy(temp2,buf);
  return_buf = (char **) malloc(sizeof(char *) * ((numspaces(temp2)+1) + 2));
  return_buf[cntr++] = (char *)strtok(temp2," \t");
  while (temp = (char *)strtok(NULL," \t")) 
	if (temp != NULL) {
		return_buf[cntr] = temp;
		++cntr;}
  return_buf[cntr] = NULL;
  return(return_buf); }



char **perl_split_independent(buf)
     char *buf;
{
  char **return_buf;
  int cntr = 0;
  char *temp;

  while(*buf == ' ' || *buf == '\t') ++buf;
  return_buf = (char **) malloc(sizeof(char *) * (numspaces(buf)+3));
  return_buf[cntr++] = (char *)mystrdup((char *)strtok(buf," \t"));
  while ((temp = (char *)strtok(NULL,"\t ")) != NULL) {
    return_buf[cntr] =(char *)mystrdup(temp);
    ++cntr;
  }
  return_buf[cntr] = NULL;
  return(return_buf); }




char **perl_split_on_char(buf,achar)
     char *buf;
     char achar;
{
  char **return_buf;
  int cntr = 0;
  char *temp,temp2[2],*temp3;

  temp3 = (char *)malloc(sizeof(char)*(1+strlen(buf)));
  temp2[0] = achar; temp2[1] = '\0';
  return_buf = (char **) malloc(sizeof(char *) * ((numchars(temp3,achar)+1) + 2));
  return_buf[cntr++] = (char *)strtok(temp3,temp2);
  while (temp = (char *)strtok(NULL,temp2)) 
	if (temp != NULL) {
		return_buf[cntr] = temp;
		++cntr;}
  return_buf[cntr] = NULL;
  return(return_buf); }




char **perl_split_on_nothing(buf)
     char *buf;
{
  char **return_buf;
  int cntr;
  char *temp2;  

  temp2 = (char *)malloc(sizeof(char)*(1+strlen(buf)));
  strcpy(temp2,buf);
  
  return_buf = (char **) malloc(sizeof(char *) * (strlen(buf)+1));
  for (cntr = 0; cntr < strlen(buf); ++cntr) {
    return_buf[cntr] = (char *)malloc(sizeof(char)*2);
    return_buf[cntr][0] = temp2[cntr];
    return_buf[cntr][1] = '\0'; }
    return_buf[cntr] = NULL;
    return(return_buf); 
}

char *perl_explode(buf)
     char *buf;
{
  char *return_buf;
  int cntr;
 

  
  return_buf = (char *) malloc(sizeof(char) * ((strlen(buf)*2)+1));
  for (cntr = 0; (cntr/2) < strlen(buf); cntr+=2) {
    return_buf[cntr] = buf[cntr/2];
    return_buf[cntr+1] = ' '; }
    return_buf[cntr-1] = '\0';
    return(return_buf); 
}

char **perl_free(ptr)
     char **ptr;
    
{

/*  while (ptr[count] != NULL) {
    free(ptr[count]);
    count++;
  }}*/

  free(ptr[0]); }



int numspaces(buf)
     char *buf;
{
  int tot,count;
  tot = 0;
  for (count = 0; count < strlen(buf); ++count) 
    if (buf[count]==' ')
      ++tot;
  return(tot); }
    
int numchars(buf,achar)
     char *buf,achar;
{
  int tot,count;
  tot = 0;
  for (count = 0; count < strlen(buf); ++count) 
    if (buf[count]== achar)
      ++tot;
  return(tot); }
    


char *return_tag(theword)
     char *theword;
{
  char *tempword;
  tempword = (char *)strchr(theword,'/');
  if (tempword != NULL)  return (tempword+1); 
  else return(NULL); }


char *before_tag(theword)
     char *theword;
{
  int count = 0;
  
  while (theword[count] != '\0' &&
	 theword[count] != '/')
    count++;
  if (theword[count] == '/')
    theword[count] = '\0';
  return(theword); }



