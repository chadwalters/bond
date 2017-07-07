#include "compat_reflection.h"

#include <boost/lexical_cast.hpp>
#include <bond/core/bond.h>
#include <bond/protocol/simple_json_writer.h>
#include <iostream>
#include <fstream>

using namespace std;

using namespace unittest::compat;

static const int32_t c_max_file_size = 0x020000000;

uint16_t GetMagic(const std::string& filename)
{
    size_t dotPos = filename.find_last_of('.');
    if (dotPos == filename.size() - 3)
    {
        return (((uint16_t)(filename[dotPos + 2] - 'a' + 'A')) << 8) + ((uint16_t)(filename[dotPos + 1] - 'a' + 'A'));
    }
    return 0;
}


template <class Reader>
int ReadObj(ifstream &theFile)
{
    ifstream::pos_type pos = theFile.tellg();

    if (pos > c_max_file_size)
    {
        std::cout << "File too large" << std::endl;
        return -1;
    }

    boost::shared_ptr<char[]> data = boost::make_shared<char[]>(pos);

    theFile.seekg(0, ios::beg);
    theFile.read(data.get(), pos);

    bond::InputBuffer buffer(data.get(), (uint32_t) pos);

    Compat obj;
    Reader reader(buffer);

    try
    {
        Deserialize(reader, obj);

        std::cout << "Success!" << std::endl;
    }
    catch (bond::Exception ex)
    {
        std::cout << "Bond exception caught: " << ex.message << std::endl;
    }
    catch (std::bad_alloc ex)
    {
        std::cout << "std::bad_alloc: " << ex.what() << std::endl;
    }

    return 0;
}


int main(int argc, const char* argv[])
{
    if (argc != 2)
    {
        std::cout << "Expected exactly one command-line argument" << std::endl;
        return -1;
    }

    std::string filename = std::string(argv[1]);

    ifstream theFile(filename.c_str(), ios::binary | ios::ate);
    if (!theFile.good())
    {
        std::cout << "Could not open file: " << filename << std::endl;
        return -1;
    }

    uint16_t magic = GetMagic(filename);

    int result = 0;

    switch (magic)
    {
    case bond::CompactBinaryReader<bond::InputBuffer>::magic:
        result = ReadObj<bond::CompactBinaryReader<bond::InputBuffer>>(theFile);
        break;
    case bond::FastBinaryReader<bond::InputBuffer>::magic:
        result = ReadObj<bond::FastBinaryReader<bond::InputBuffer>>(theFile);
        break;
    case bond::SimpleBinaryReader<bond::InputBuffer>::magic:
        result = ReadObj<bond::SimpleBinaryReader<bond::InputBuffer>>(theFile);
        break;
    case bond::SimpleJsonReader<bond::InputBuffer>::magic:
        result = ReadObj<bond::SimpleJsonReader<bond::InputBuffer>>(theFile);
        break;
    case 0:
        cout << "Could not parse extension properly -- expected exactly 2 characters: " << filename << std::endl;
        result = -1;
        break;
    default:
        cout << "Extension did not match known one: " << filename << std::endl;
        result = -1;
        break;
    }

    return result;
}
