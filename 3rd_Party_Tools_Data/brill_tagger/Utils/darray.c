#include <stddef.h>
/*#include <stdlib.h>*/
#include <stdio.h>
#include <assert.h>

#include "sysdep.h"
#include "memory.h"
#include "bool.h"

#include "useful.h"
#include "darrayP.h"

/* Grow in the specified direction by the specified amount (number of 
 * new array slots).  It amount is zero, grow by some default amount.
 */
static NORET grow(array_rep, direction, amount)
     Darray_rep *array_rep;
     enum grow_direction direction;
     unsigned amount;
{
  int grow_amount;
  int new_length;
  VOIDP *temp_new_space;
  VOIDP *p, *q;
 
  assert(direction == GROW_HIGH || direction == GROW_LOW);

  if (amount == 0) 
    grow_amount = (MAX_GROW_STEP < array_rep->storage_length) ? 
      MAX_GROW_STEP : array_rep->storage_length;
  else
    grow_amount = amount;
  
  new_length = array_rep->storage_length + grow_amount;

  /* sort of like realloc, but not really */
  if (direction == GROW_HIGH) {
    /* Maybe this should not use realloc, since there may be unused 
       slots at the low end of the darray (before storage_offset), and it
       is a waste to copy them */
    array_rep->storage = (VOIDP *)Memory_reallocate(array_rep->storage,
					       sizeof(VOIDP) * new_length);
    array_rep->storage_length = new_length;
  } else {			/* direction == GROW_LOW */
    temp_new_space = (VOIDP *)Memory_allocate(sizeof(VOIDP) * new_length);
    for (p=temp_new_space+grow_amount+array_rep->storage_offset, 
	   q=array_rep->storage+array_rep->storage_offset; 
	 q < array_rep->storage+array_rep->storage_offset+array_rep->length; 
	 ++p, ++q) {
      *p = *q;
    }
    Memory_free(array_rep->storage);
    array_rep->storage = temp_new_space;
    array_rep->storage_length = new_length;
    array_rep->storage_offset += grow_amount;
  }
}

Darray Darray_create(NOARGS)
{
  Darray_rep *temp = dcreate();

  temp->length = 0;
  temp->storage = (VOIDP *)Memory_allocate(sizeof(VOIDP));
  temp->storage_length = 1;
  temp->storage_offset = 0;
  return draise(temp);
}

NORET Darray_destroy(dyn_ary)
     Darray dyn_ary;
{
  Darray_rep *temp = dlower(dyn_ary);

  Memory_free((VOIDP)temp->storage);
  Memory_free((VOIDP)temp);
}

NORET Darray_hint(dyn_ary, addl_hint, addh_hint)
     Darray dyn_ary;
     unsigned addl_hint, addh_hint;
{
  Darray_rep *temp = dlower(dyn_ary);
  int grow_amount;

  grow_amount = addh_hint - (temp->storage_length - temp->storage_offset 
			     - temp->length);
  if (grow_amount > 0) 
    grow(temp, GROW_HIGH, (unsigned)grow_amount);

  grow_amount = addl_hint - temp->storage_offset;
  if (grow_amount > 0)
    grow(temp, GROW_LOW, (unsigned)grow_amount);

}

NORET Darray_addh(dyn_ary, object)
     Darray_rep *dyn_ary;
     VOIDP object;
{
  Darray_rep *temp = dlower(dyn_ary);
  int add_posn = temp->storage_offset + temp->length;

  if (add_posn >= temp->storage_length)
    grow(temp, GROW_HIGH, (unsigned)0);

  *(temp->storage + add_posn) = object;
  ++temp->length;
}

NORET Darray_addl(dyn_ary, obj)
     Darray dyn_ary;
     VOIDP obj;
{
  Darray_rep *temp = dlower(dyn_ary);

  if (temp->storage_offset == 0)
    grow(temp, GROW_LOW, (unsigned)0);	/* Modifies storage_offset */

  assert(temp->storage_offset > 0);

  /* Note side-effect */
  *(temp->storage + --temp->storage_offset) = obj;
  ++temp->length;
}

VOIDP Darray_remh(dyn_ary)
     Darray dyn_ary;
{
  Darray_rep *temp = dlower(dyn_ary);

  assert(temp->length > 0);

  return *(temp->storage + temp->storage_offset + --temp->length);
}

VOIDP Darray_reml(dyn_ary)
     Darray dyn_ary;
{
  Darray_rep *temp = dlower(dyn_ary);
  VOIDP return_value;

  assert(temp->length > 0);

  return_value =  *(temp->storage + temp->storage_offset++);
  --temp->length;
  return return_value;
}

unsigned Darray_len(dyn_ary)
     Darray dyn_ary;
{
  assert(dyn_ary != (Darray)NULL);

  return dlower(dyn_ary)->length;
}

Bool Darray_valid_index(dyn_ary, index)
     Darray dyn_ary;
     unsigned index;
{
  return Bool_create(index < Darray_len(dyn_ary));
}

NORET Darray_set(dyn_ary, index, obj)
     Darray dyn_ary;
     unsigned index;
     VOIDP obj;
{
  Darray_rep *temp = dlower(dyn_ary);

  assert(Darray_valid_index(dyn_ary, index)==Bool_TRUE);
 
  *(temp->storage + temp->storage_offset + index) = obj;
}

VOIDP Darray_get(dyn_ary, index)
     Darray dyn_ary;
     unsigned index;
{
  Darray_rep *temp = dlower(dyn_ary);

  assert(Darray_valid_index(dyn_ary, index)==Bool_TRUE);
 
  /* You should suppress the saber warning on the following line */
  return *(temp->storage + temp->storage_offset + index);
}

NORET Darray_values(dyn_ary, c_ary)
     Darray dyn_ary;
     VOIDP *c_ary;
{
  Darray_rep *temp = dlower(dyn_ary);
  VOIDP *p, *q;

  for (p=c_ary, q=temp->storage+temp->storage_offset; 
       q < temp->storage+temp->storage_offset+temp->length; 
       ++p, ++q) {
    *p = *q;
  }
}

Darray Darray_copy(dyn_ary)
     Darray dyn_ary;
{
  Darray temp = Darray_create();
  unsigned int temp_len = Darray_len(dyn_ary);
  unsigned int i;

  Darray_hint(temp, (unsigned int)0, temp_len);
  for (i = 0; i < temp_len; ++i)
    Darray_addh(temp, Darray_get(dyn_ary, i));
  return temp;
}


/*--------------------------------------------------*/
/* code added by Rich Pito */
/*--------------------------------------------------*/

/* Both of thes function has to be rewrutten to use the internal darray data */
/* structure.  This is this way for ease of writing */

Darray Darray_remove(dar, index)
     Darray dar;
     int index;
{
  int s, d, l;
  l = Darray_len(dar);
  if (index == (l - 1)) {
    Darray_remh(dar);
    return dar;
  }
  if (index == 0) {
    Darray_reml(dar);
    return dar;
  }
  s = index + 1; d = index;
  while (s < l) {
    Darray_set(dar, d, (VOIDP)Darray_get(dar, s));
    s++; d++;
  }
  Darray_remh(dar);
  return dar;
}

/*--------------------------------------------------*/
Darray Darray_insert(dar, index, data)
     Darray dar;
     int index;
     VOIDP data;
{
  int l = Darray_len(dar);
  int source;
  if (index <= 0)
    Darray_addl(dar, data);
  else if (index >= l)
    Darray_addh(dar, data);
  else {
    Darray_addh(dar, NULL);
    for (source = l - 1; source >= index; source--) 
      Darray_set(dar, source + 1, Darray_get(dar, source));
    Darray_set(dar, index, data);
  }  
  return dar;
}


/*--------------------------------------------------*/
Darray Darray_duplicate(a)
     Darray a;
{
  int i, l;
  Darray ret;

  l = Darray_len(a);
  ret = Darray_create();
  Darray_hint(ret, 0, l);
  for (i = 0; i < l; i++) {
    Darray_addh(ret, (char*)Darray_get(a, i));
  }
  return ret;
}

void Darray_clear(dyn_ary)
     Darray dyn_ary;
{
  Darray_rep *temp = dlower(dyn_ary);

  temp->storage_offset = 0;
  temp->length = 0;
}
