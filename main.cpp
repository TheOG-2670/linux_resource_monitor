#include <cstdio>
#include <dirent.h>

int main(int argc, char **argv)
{
    DIR* dir;
    struct dirent* ent;

    const char *dirName = argv[1];
    dir = opendir(dirName);

    //if the directory doesn't exist, return error
    if (!dir) {
        perror("cannot read directory");
        return -1;
    }
    
    //while the current directory entry is not NULL, read it and print its name
    while (ent = readdir(dir)) {
        printf("%s\n", ent->d_name);
    }

    return 0;
}