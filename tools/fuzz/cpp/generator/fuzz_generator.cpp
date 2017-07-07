
#include "fuzz_util.h"
#include "compat_reflection.h"

#include <boost/lexical_cast.hpp>
#include <bond/core/bond.h>
#include <bond/protocol/simple_json_writer.h>
#include <iostream>
#include <fstream>

using namespace std;

using namespace unittest::compat;

static const std::string prefix = "fuzz";

std::string GetFilename(uint16_t iteration, uint16_t magic)
{

    return prefix + boost::lexical_cast<std::string>(iteration) + '.' + (char)((magic & 0x00FF) + 'a' - 'A') + (char) (((magic >> 8) & 0x00FF) + 'a' - 'A');
}

template <template <typename> class Writer, class U>
void Generate(U obj, uint16_t iteration)
{
    // bond::OutputBuffer implements the Bond output stream interface on top of
    // a series of memory buffers.
    bond::OutputBuffer output;

    // Use the Compact Binary protocol for encoding; the data will be written to
    // the OutputBuffer which will allocate memory as needed.
    Writer<bond::OutputBuffer> writer(output);

    // Serialize the object
    Serialize(obj, writer);

    std::vector<bond::blob> buffers;
    output.GetBuffers(buffers);

    std::string filename = GetFilename(iteration, Writer<bond::OutputBuffer>::Reader::magic);

    ofstream theFile(filename.c_str(), ios::binary | ios::trunc);
    for (int i = 0; i < buffers.size(); i++)
    {
        theFile.write(buffers[i].content(), buffers[i].length());
    }
}



int main()
{
    for (uint16_t i = 0; i < 10; i++)
    {
        Compat obj = InitRandom<Compat>();

        Generate<bond::CompactBinaryWriter>(obj, i);
        Generate<bond::SimpleBinaryWriter>(obj, i);
        Generate<bond::FastBinaryWriter>(obj, i);
        Generate<bond::SimpleJsonWriter>(obj, i);
    }

    return 0;
}
