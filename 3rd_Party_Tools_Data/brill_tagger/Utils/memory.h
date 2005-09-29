#ifndef _Memory_h_
#define _Memory_h_

#include <stddef.h>

#include "sysdep.h"

#ifdef __STDC__
extern VOIDP Memory_allocate(size_t);
extern VOIDP Memory_reallocate(VOIDP, size_t);
extern NORET Memory_free(VOIDP);
extern long Memory_unfreed_bytes(NOARGS);
#else
extern VOIDP Memory_allocate();
extern VOIDP Memory_reallocate();
extern NORET Memory_free();
extern long Memory_unfreed_bytes(NOARGS);
#endif /* __STDC__ */

#endif
