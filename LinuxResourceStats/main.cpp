#include <iostream>
#include <string>
#include <vector>
#include <fstream>

std::vector<std::string> parseCpuStats(std::string& lineData)
{
    size_t currPos = 0, prevPos = 0;
    std::vector<std::string> lineDataVec;
    while ((currPos = lineData.find(" ", prevPos)) != std::string::npos) {
        if (currPos > prevPos)
            lineDataVec.push_back(lineData.substr(prevPos, currPos - prevPos));
        prevPos = ++currPos;
    }
    lineDataVec.push_back(lineData.substr(prevPos, prevPos - 1));
    return lineDataVec;
}

int main(int argc, char **argv)
{
    std::string fileName = argv[1];
    std::cout << "file is: " << fileName << std::endl;

    std::string lineData;
    std::ifstream ifs(fileName);

    //if the file is open and no errors while reading, get the first line from the file stream and close it
    if (ifs.is_open())
    {
        if (ifs.good())
        {
            std::getline(ifs, lineData); //get the first line of the file that contains overall CPU stat info
            ifs.close();
        }
    }

    std::cout << lineData << std::endl;

    std::vector<std::string> lineDataVec = parseCpuStats(lineData);

    for (std::vector<std::string>::const_iterator it = lineDataVec.cbegin(); it != lineDataVec.cend(); it++)
    {
        std::cout << *it << std::endl;
    }

    return 0;
}