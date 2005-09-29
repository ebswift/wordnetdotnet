#include <stddef.h>
/*#include <stdlib.h>*/
/*#include <string.h>*/
#include <stdio.h>
#include <assert.h>
#include <ctype.h>

#include "sysdep.h"
#include "memory.h"
#include "bool.h"
#include "useful.h"

#include "registryP.h"

/* Creates and returns and empty directory */

Registry Registry_create(compare_func, hash_func)
     Registry_CompareFunc compare_func;
     Registry_HashFunc hash_func;
{
  Registry_rep *temp = create();
  
  temp->hash_table = NULL;
  temp->ht_size = (unsigned int)0;
  temp->comp_fun = compare_func;
  temp->hash_fun = hash_func;
  temp->record_count = (unsigned int)0;
  Registry_size_hint(raise(temp), DEFAULT_HT_SIZE);
  return raise(temp);
}

/* Deal with the expected size value.  */

NORET Registry_size_hint(dir, size_hint_value)
     Registry dir;
     unsigned int size_hint_value;
{
  int i;
  if (lower(dir)->record_count != (unsigned int)0) return;
  if (lower(dir)->ht_size != (unsigned int)0) 
    Memory_free((VOIDP)lower(dir)->hash_table);
  lower(dir)->ht_size = size_hint_value;
  lower(dir)->hash_table = 
    (RegistryRecord **)Memory_allocate(sizeof(RegistryRecord *)
				   * size_hint_value);
  for (i=0; i < size_hint_value; ++i)
    *(lower(dir)->hash_table + i) = (RegistryRecord *)NULL;
}

/* Finds a named object in a directory.  Returns NULL if the named 
 * object is not in the directory */

VOIDP Registry_get(dir, key)
     Registry dir;
     CONSTVOIDP key;
{
  RegistryRecord *p;
  Registry_CompareFunc comp_func = lower(dir)->comp_fun;
  Registry_HashFunc hash_func = lower(dir)->hash_fun;

  assert(comp_func);
  for (p = *(lower(dir)->hash_table + (*hash_func)(key, lower(dir)->ht_size));
       p != NULL; 
       p = p->next) {
    if ((*comp_func)(key, p->name) == 0) 
	return p->obj;
  }
  return NULL;			/* not found */
}

/* Finds a named object in a directory and returnd the original key */
 /* used to index that object.  Returns NULL if the named  object is */
 /* not in the directory.  This is useful for getting the original */
 /* string used to make an entry into a registry in order to free it. */
 /* In this case, a pointer to the name should be storred, then the */
 /* entry should be removed using Registry_remove, then the key may be */
 /* freed */

VOIDP Registry_get_original_key(dir, key)
     Registry dir;
     CONSTVOIDP key;
{
  RegistryRecord *p;
  Registry_CompareFunc comp_func = lower(dir)->comp_fun;
  Registry_HashFunc hash_func = lower(dir)->hash_fun;

  for (p = *(lower(dir)->hash_table + (*hash_func)(key, lower(dir)->ht_size));
       p != NULL; 
       p = p->next) {
    if ((*comp_func)(key, p->name) == 0) 
      return p->name;
  }
  return NULL;			/* not found */
}

/* Adds a named object to a directory. Returns Bool_TRUE unless an error occurs.
 * An error will occur if Registry_get(dir, name) would succeed (return
 * non-NULL) */

Bool Registry_add(dir, name, obj)
     Registry dir;
     VOIDP name;
     VOIDP obj;
{
  RegistryRecord *p;
  RegistryRecord **table_entry;
  Registry_HashFunc hash_func = lower(dir)->hash_fun;
  Registry_CompareFunc comp_func = lower(dir)->comp_fun;

  table_entry = lower(dir)->hash_table + (*hash_func)(name, lower(dir)->ht_size);
  
  for (p = *table_entry;
       p != NULL; 
       p = p->next) {
    if ((*comp_func)(name, p->name) == 0) 
	return Bool_FALSE;
  }

  p = (RegistryRecord *)Memory_allocate(sizeof(RegistryRecord));
  p->next = *table_entry;
  p->name = name;
  p->obj = obj;
  *table_entry = p;
  ++(lower(dir)->record_count);
  return Bool_TRUE;
}

/* Removes a named object from the directory. Returns Bool_TRUE unless an 
 * error occurs (Bool_FALSE if an error does occur).  The object is 
 * not freed.  It is the responsibility of the 
 * caller to do so if necessary.
 */

Bool Registry_remove(dir, key)
     Registry dir;
     CONSTVOIDP key;
{
  RegistryRecord *p, **prev_p;
  Registry_rep *ldir = lower(dir);
  Registry_CompareFunc comp_func = ldir->comp_fun;
  Registry_HashFunc hash_func = lower(dir)->hash_fun;

  prev_p = lower(dir)->hash_table + (*hash_func)(key, lower(dir)->ht_size);
  while ((p = *prev_p) != NULL) {
    if ((*comp_func)(key, p->name) == 0) {
      *prev_p = p->next;
      Memory_free((VOIDP)p);
      --(ldir->record_count);
      return Bool_TRUE;
    }
    prev_p = &(p->next);
  }
  return Bool_FALSE;
}

/* Replaces an association in the registry.  If an association with the
 * given key already exists, the value is changed to new_value, and the 
 * old value is returned.  If no association already exists, one is added
 * and NULL is returned. */

VOIDP Registry_replace_value(dir, key, new_value)
     Registry dir;
     VOIDP key;
     VOIDP new_value;
{
  RegistryRecord *p;
  Registry_CompareFunc comp_func = lower(dir)->comp_fun;
  Registry_HashFunc hash_func = lower(dir)->hash_fun;

  VOIDP temp_obj;
  for (p = *(lower(dir)->hash_table + (*hash_func)(key, lower(dir)->ht_size));
       p != NULL; 
       p = p->next) {
    if ((*comp_func)(key, p->name) == 0)  {
      temp_obj = p->obj;
      p->obj = new_value;
      return temp_obj;
    }
  }
  Registry_add(dir, key, new_value);
  return NULL;			/* not found */
}

NORET Registry_traverse(dir, action, priv_ptr)
     Registry dir;
     Registry_ActionProc action;
     VOIDP priv_ptr;
{
  RegistryRecord *p;
  int i;

  for (i = 0; i < lower(dir)->ht_size; ++i)
    for (p = *(lower(dir)->hash_table + i);
	 p != NULL; 
	 p = p->next) {
      (*action)(p->name, p->obj, priv_ptr);
    }
  return;
}

unsigned int Registry_entry_count(dir)
     Registry dir;
{
  return lower(dir)->record_count;
}

static NORET add_to_darrays(key, value, priv_ptr)
     VOIDP key;
     VOIDP value;
     VOIDP priv_ptr;
{
  struct darray_pair *dapp = (struct darray_pair *) priv_ptr;
  
  if (dapp->key_darray != NULL)
    Darray_addh(dapp->key_darray, (VOIDP)key);	/* Specs forbid mods to key */
  if (dapp->value_darray != NULL)
    Darray_addh(dapp->value_darray, value);
}

NORET Registry_fetch_contents(dir, key_darray, value_darray)
     Registry dir;
     Darray key_darray, value_darray;
{
  struct darray_pair dap;

  assert (key_darray == NULL || Darray_len(key_darray) == 0);
  assert (value_darray == NULL || Darray_len(value_darray) == 0);

  dap.key_darray = key_darray;
  dap.value_darray = value_darray;

  Registry_traverse(dir, add_to_darrays, (VOIDP)&dap);
}

NORET Registry_destroy(dir)
     Registry dir;
{
  RegistryRecord *p, *next;
  int i;

  for (i = 0; i < lower(dir)->ht_size; ++i)
    for (p = *(lower(dir)->hash_table + i);
	 p != NULL; 
	 p = next) {
      next = p->next;
      Memory_free((VOIDP)p);
    }
  Memory_free((VOIDP)lower(dir)->hash_table);
  destroy(lower(dir));
}

unsigned int Registry_ptrhash(ptr, htsize)
     CONSTVOIDP ptr;
     unsigned int htsize;
{
  unsigned int uns_int_ptr = (unsigned int)ptr;

  if ((int)uns_int_ptr > 0)
    return htsize ? (int)uns_int_ptr % (int)htsize : 0;
  else
    return htsize ? uns_int_ptr % htsize : 0;
}

unsigned int Registry_strhash(strp, htsize)
     CONSTVOIDP strp;
     unsigned int htsize;
{
  char *cp = (char *)strp;
  int hash_temp = 0;

  while (*cp != '\0') {
    if (hash_temp < 0)
      hash_temp = (hash_temp << 1) +1;
    else
      hash_temp = hash_temp << 1;
    hash_temp ^= *cp;
    ++cp;
  }
  return htsize ? ((unsigned int)hash_temp) % htsize : 0;
}

unsigned int Registry_strcasehash(strp, htsize)
     CONSTVOIDP strp;
     unsigned int htsize;
{
  char *cp = (char *)strp;
  int hash_temp = 0;

  while (*cp != '\0') {
    if (hash_temp < 0)
      hash_temp = (hash_temp << 1) +1;
    else
      hash_temp = hash_temp << 1;
    if (isalpha(*cp) && isupper(*cp))
      hash_temp ^= tolower(*cp);
    else
      hash_temp ^= *cp;
    ++cp;
  }
  return htsize ? ((unsigned int)hash_temp) % htsize : 0;
}

int Registry_strcmp(str1, str2)
     CONSTVOIDP str1;
     CONSTVOIDP str2;
{
  return strcmp((char *)str1, (char *)str2);
}

int Registry_strcasecmp(str1, str2)
     CONSTVOIDP str1;
     CONSTVOIDP str2;
{
  return strcasecmp((char *)str1, (char *)str2);
}

int Registry_ptrcmp(ptr1, ptr2)
     CONSTVOIDP ptr1;
     CONSTVOIDP ptr2;
{
  if (ptr1==ptr2)
    return 0;
  else
    return 1;
}    
     
