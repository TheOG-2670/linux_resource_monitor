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

    //parse the captured line holding CPU stat info by traversing and storing each number into a string vector
    size_t currPos = 0, prevPos = 0;
    std::vector<std::string> lineDataStack;
    while ((currPos = lineData.find(" ", prevPos)) != std::string::npos) {
        if (currPos > prevPos)
            lineDataStack.push_back(lineData.substr(prevPos, currPos - prevPos));
        prevPos = ++currPos;
    }

    lineDataStack.push_back(lineData.substr(prevPos, prevPos - 1)); //get the last number in the string not parsed by the loop above

    //print each CPU stat value
    for (std::vector<std::string>::const_iterator it = lineDataStack.cbegin(); it != lineDataStack.cend(); it++)
    {
        std::cout << *it << std::endl;
    }

    return 0;
}