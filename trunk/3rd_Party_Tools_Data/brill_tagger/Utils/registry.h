#ifndef _registry_h_
#define _registry_h_

#include "sysdep.h"
#include "bool.h"
#include "darray.h"

typedef struct Registry_st *Registry;

#ifdef __STDC__
typedef unsigned int (*Registry_HashFunc)(CONSTVOIDP, unsigned int);
typedef int (*Registry_CompareFunc)(CONSTVOIDP, CONSTVOIDP);
typedef NORET (*Registry_ActionProc)(VOIDP, VOIDP, VOIDP);
extern Registry Registry_create(Registry_CompareFunc, Registry_HashFunc);
extern NORET Registry_size_hint(Registry, unsigned int);
extern Bool Registry_add(Registry, VOIDP, VOIDP);
extern Bool Registry_remove(Registry, CONSTVOIDP);
extern VOIDP Registry_get(Registry, CONSTVOIDP);
extern VOIDP Registry_get_original_key(Registry, CONSTVOIDP);
extern VOIDP Registry_replace_value(Registry, VOIDP, VOIDP);
extern NORET Registry_traverse(Registry, Registry_ActionProc, VOIDP);
extern unsigned int Registry_entry_count(Registry);
extern NORET Registry_fetch_contents(Registry, Darray, Darray);
extern NORET Registry_destroy(Registry);
extern int Registry_ptrcmp(CONSTVOIDP, CONSTVOIDP);
extern unsigned int Registry_ptrhash(CONSTVOIDP, unsigned int);
extern int Registry_strcmp(CONSTVOIDP, CONSTVOIDP);
extern unsigned int Registry_strhash(CONSTVOIDP, unsigned int);
extern int Registry_strcasecmp(CONSTVOIDP, CONSTVOIDP);
extern unsigned int Registry_strcasehash(CONSTVOIDP, unsigned int);
#else
typedef unsigned int (*Registry_HashFunc)();
typedef int (*Registry_CompareFunc)();
typedef void (*Registry_ActionProc)();
extern Registry Registry_create();
extern NORET Registry_size_hint();
extern int Registry_add();
extern int Registry_remove();
extern VOIDP Registry_get();
extern VOIDP Registry_get_original_key();
extern VOIDP Registry_replace_value();
extern NORET Registry_traverse();
extern unsigned int Registry_entry_count();
extern NORET Registry_fetch_contents();
extern NORET Registry_destroy();
extern int Registry_ptrcmp();
extern unsigned int Registry_ptrhash();
extern int Registry_strcmp();
extern unsigned int Registry_strhash();
extern int Registry_strcasecmp();
extern unsigned int Registry_strcasehash();
#endif /* __STDC__ */


/* 
 * Registry_create(compare_func, hash_func)
 * Creates and returns an empty registry.  compare_func is used
 * to compare items in the registry.  It should return 0 if its 
 * arguments are to be considered equal.  hash_func should return
 * a number between 0 and its second argument, and should attempt
 * an even distribution.  If compare_func
 * would return 0 for a pair of objects, hash_fuct should return 
 * the same value for those objects.  For registries of abstract
 * objects (pointers), Registry_ptrcmp() and Registry_ptrhash() should
 * be passed as the compare_func and hash_func.  Registry_strcmp and
 * Registry_strhash() may be used for strings.  Registry_strcasecmp and
 * Registry_strcasehash() may be used for strings where case is not 
 * significant (case-insensitive).
 * 
 * Registry_size_hint(registry, size_hint_value)
 * The registry may operate more efficiently if this operator is called
 * and size_hint is close to the maximum number of elements to be in 
 * the Registry, at the possible cost of additional memory use.  Likely
 * to be effective only on an empty registry.
 * 
 * Registry_add(registry, key, value)
 * Adds the association between key and value to the registry.  Neither
 * key nor value are copied, and neither may be freed before being removed
 * from the registry.  The key should not be modified in way that would
 * change the value of the compare_func or the hash_func until this
 * association is removed from the registry.
 * Will return Bool_FALSE if an association with the 
 * same key is already in the registry (in which case the add will not be 
 * performed), Bool_TRUE otherwise (on successful completion).
 * 
 * Registry_remove(registry, key)
 * Removes the association with key from the registry.  Returns Bool_FALSE
 * if no such association exists, Bool_TRUE otherwise (on successful
 * completion) 
 *
 * Registry_get(registry, key)
 * Returns the value associated with key in the registry.  Returns NULL
 * if there is no such association.
 * 
 * Registry_get_original_key(registry, key)   (added by Rich Pito 7/91)
 * Finds a named object in a directory and returnd the original key 
 * used to index that object.  Returns NULL if the named  object is 
 * not in the directory.  This is useful for getting the original 
 * string used to make an entry into a registry in order to free it. 
 * In this case, a pointer to the name should be storred, then the 
 * entry should be removed using Registry_remove, then the key may be 
 * freed 
 *
 * Registry_traverse(registry, action_proc, private_pointer)
 * Calls action_proc once for each entry in the registry.  private_pointer
 * is a VOIDP which is passed to the action_proc, but not otherwise used.
 * action_proc should not modify the registry in any way.  action_proc takes
 * three arguments, the key, the value, and private_pointer.  
 * 
 * Registry_entry_count(registry)
 * Returns the number of associations in the registry.
 *
 * Registry_fetch_contents(registry, key_darray, value_darray)
 * Stores the contents of the registry as follows:  In no particular 
 * order, each association is processed in turn by storing (using Darray_addh)
 * the key into key_darray and the value into value_darray.  Either 
 * or both key_darray and/or value_darray may be NULL, in which case
 * the corresponding data will not be processed.  Actual Darrays passed
 * (not NULL) must be empty.  Any objects added to key_darray must be 
 * treated as read-only as long as they remain in the registry.
 * 
 * Registry_destroy(registry)
 * Deallocates all resources needed by the registry.  Should be the last
 * operation performed on the registry.  Does not deallocate the objects
 * (keys and values) contained in the registry (this should be done after 
 * the registry is destroyed).  Implicitly removes all associations
 * from the registry.
 */
#endif /* _resgistry_h_ */
