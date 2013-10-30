#include <stdio.h>

typedef int (*FunctionCall)(void*, char*);

int default_callback(void* opaque, char* name)
{
    printf("%s was changed! (C callback)\n\n", name);
    fflush(stdout);
    return 0;
}

int exec_callback(void* callback_ptr, void* opaque, char* name)
{
    FunctionCall callback = callback_ptr;
    return callback(opaque, name);
}
