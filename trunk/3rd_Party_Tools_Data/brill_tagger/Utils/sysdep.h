#ifndef _SYSDEP_H_
#define _SYSDEP_H_

#define NORET void

/* CONSTVOIDP is for pointers to non-modifyable void objects */

#ifdef __STDC__
typedef const void * CONSTVOIDP;
typedef void * VOIDP;
#define NOARGS void
#define PROTOTYPE(x) x
#else
typedef char * VOIDP;
typedef char * CONSTVOIDP;
#define NOARGS
#define PROTOTYPE(x) ()
#endif

#endif /* ifndef _SYSDEP_H_ */
