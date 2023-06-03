#include <iostream>
#include <string>
#include <vector>
#include <fstream>

int main(int argc, char **argv)
{
    std::string fileName = "/proc/stat";
    std::cout << "file is: " << fileName << std::endl;

    std::string lineData;
    std::ifstream ifs(fileName);
    if (ifs.is_open()) 
    {
        if(ifs.good())
        {
            std::getline(ifs, lineData); //get the first line of the file that contains overall CPU stat info
            ifs.close();
        }
    }

    std::cout << lineData << std::endl;

    return 0;
}