#include <iostream>
#include <string>
#include <fstream>

int main(int argc, char **argv)
{
    std::string fileName= argv[1];
    std::cout << "file is: " << fileName << std::endl;

    //open file
    std::ifstream ifs(fileName);
    if (ifs.is_open()) 
    {
        //read each line of file while no errors and print the current line
        std::string lineData;
        while (ifs.good())
        {
            std::getline(ifs, lineData);
            std::cout << lineData << std::endl;
        }
    }

    return 0;
}