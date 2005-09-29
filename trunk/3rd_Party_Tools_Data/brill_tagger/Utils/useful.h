#ifndef _USEFUL_H
#define _USEFUL_H

#include "sysdep.h"

#ifndef NULL
#define NULL 0
#endif

#ifndef TRUE
#define TRUE (!NULL)
#endif

#ifndef FALSE
#define FALSE NULL
#endif

#ifndef USHORT
#define USHORT unsigned short
#endif

#ifndef ULONG
#define ULONG unsigned long
#endif

#ifndef MAX
#define MAX(x, y) (x >= y ? x : y)
#endif

#ifndef MIN
#define MIN(x, y) (x <= y ? x : y)
#endif

#ifndef ABS
#define ABS(x) (x < 0 ? x * (-1) : x)
#endif

#ifdef __STDC__
extern char *mystrdup(char *);
     /* Just a hack around the fact that strdup isn't standard */
extern int not_just_blank(char *);
    /* make sure we aren't processing a blank line */
extern int num_words(char *);
    /* returns number of words in a string */
#else
extern char *mystrdup();
extern int not_just_blank();
extern int num_words();
#endif /* __STDC__ */

#endif
