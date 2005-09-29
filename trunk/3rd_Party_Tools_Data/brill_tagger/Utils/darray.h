#ifndef _darray_h_
#define _darray_h_

#include "sysdep.h"
#include "bool.h"

typedef struct st_Darray *Darray;

#ifdef __STDC__
extern Darray Darray_create(NOARGS);
extern NORET Darray_destroy(Darray);
extern NORET Darray_hint(Darray, unsigned int, unsigned int);
extern unsigned int Darray_len(Darray);
extern NORET Darray_addh(Darray, VOIDP);
extern NORET Darray_addl(Darray, VOIDP);
extern VOIDP Darray_remh(Darray);
extern VOIDP Darray_reml(Darray);
extern Bool Darray_valid_index(Darray, unsigned int);
extern NORET Darray_set(Darray, unsigned int, VOIDP);
extern VOIDP Darray_get(Darray, unsigned int);
extern NORET Darray_values(Darray, VOIDP *);
extern Darray Darray_copy(Darray);
extern Darray Darray_remove(Darray, int);
extern Darray Darray_duplicate(Darray);
extern Darray Darray_insert(Darray, int, VOIDP);
extern void   Darray_clear(Darray); 
#else
extern Darray Darray_create();
extern NORET Darray_destroy();
extern NORET Darray_hint();
extern unsigned int Darray_len();
extern NORET Darray_addh();
extern NORET Darray_addl();
extern VOIDP Darray_remh();
extern VOIDP Darray_reml();
extern Bool Darray_valid_index();
extern NORET Darray_set();
extern VOIDP Darray_get();
extern NORET Darray_values();
extern Darray Darray_copy();
extern Darray Darray_remove();
extern Darray Darray_duplicate();
extern Darray Darray_insert();
extern void   Darray_clear(); 
#endif /* __STDC__ */


/*
 * Dynamic arrays are zero-based (0 <= index < length-of-array)
 *
 * Darray_create()
 * Creates and returns an empty dynamic array.
 *
 * Darray_destroy(dynamic_array)
 * Frees all memory used by the dynamic_array.  Calling this routine
 * should be the last use of the dynamic_array.  The object held by
 * the array are not destroyed.
 * 
 * Darray_hint(dynamic_array, addl_hint, addh_hint)
 * The hints specify how many additional (occuring after the hint call) addl
 * and addh operations are expected.  Large values may improve performance
 * at the cost of additional memory usage.
 *
 * Darray_len(dynamic_array)
 * Returns the length of the dynamic array passed.
 *
 * Darray_addh(dynamic_array, element)
 * Adds the element to the "high end" of the dynamic array.  
 * 
 * Darray_addl(dynamic_array, element)
 * Adds the element to the "low end" of the dynamic array.  
 *
 * Darray_remh(dynamic_array)
 * Returns element at the "high end" of the dynamic array,
 * and removes that element from the array.  Darray_len(dynamic_array)
 * must be > 0.
 *
 * Darray_reml(dynamic_array)
 * Returns element at the "low end" of the dynamic array,
 * and removes that element from the array.  Darray_len(dynamic_array)
 * must be > 0.
 *
 * Darray_valid_index(dynamic_array, index)
 * Checks if index is valid for use in a call to Darray_set or Darray_get.
 * Returns Bool_TRUE if index < Darray_len(dynamic_array), 
 * Bool_FALSE otherwise.
 *
 * Darray_set(dynamic_array, index, new_value)
 * Sets the index position in the dynamic array to be
 * the new value.  Darray_valid_index(dynamic_array, index) must be Bool_TRUE.
 * 
 * Darray_get(dynamic_array, index)
 * Returns the value at the index position in the dynamic array.
 * Darray_valid_index(dynamic_array, index) must be Bool_TRUE.
 * 
 * Darray_values(dynamic_array, pointer to c_array)
 * Stores all the elements (as void pointers) of the dynamic array into 
 * the c_array.  The
 * c_array must contain at least Darray_len(dynamic_array) slots for
 * void pointers (note: not object pointers).
 *
 * Darray_copy(dynamic_array)
 * Creates and returns a new dynamic array, with the same contents as
 * the dynamic array passed.  The new dynamic array does not inherit 
 * any hints that may apply to the original dynamic array.
 * 
 * Darray_remove(dynamic_array, index)
 * removes the item indexed by index from the array.  Index is base 0
 * (i.e. first item in array is 0, last item is Darray_len()-1)
 * (added by Rich Pito: this is not efficient code at the moment)
 * 
 * Darray_insert(dynamic_array, index, data)
 * inserts the data into the dynamic_array at index.  data is inserted
 * at position index.  All items with position values greater than or
 * equal to index are assigned new position values one greater than
 * their old ones.  If index is greater than the maximum position
 * value in the dynamic_array, data is added as the topmost element to
 * the array.  If index is less than zero it is inserted as if index
 * were zero. 
 * (added by Rich Pito: this is not efficient code at the moment)


 */

#endif /* ifndef _darray_h_ */
