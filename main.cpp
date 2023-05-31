#include <iostream>
#include <string>
#include <fstream>
#include <regex>

int main(int argc, char **argv)
{
    std::string fileName = "/proc/stat";
    std::cout << "file is: " << fileName << std::endl;

    std::regex re("(cpu)(.*)");

    //open file
    std::ifstream ifs(fileName);
    if (ifs.is_open()) 
    {
        //read each line of file while no errors and print the current line if it matches a defined regex pattern
        std::string lineData;
        while (ifs.good())
        {
            std::getline(ifs, lineData);
            if (std::regex_match(lineData, re)) {
                std::cout << lineData << std::endl;
            }
        }
    }
    ifs.close();

    return 0;
}