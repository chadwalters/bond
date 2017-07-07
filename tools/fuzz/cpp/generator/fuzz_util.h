#pragma once

#include <bond/core/bond.h>
#include <bond/core/config.h>
#include <bond/stream/output_buffer.h>
#include <bond/protocol/random_protocol.h>

#include <stdint.h>

#ifdef _DEBUG
    const uint32_t c_max_string_length = 20;
    const uint32_t c_max_list_size     = 10;
    const uint32_t c_iterations        = 1;
#else
    const uint32_t c_max_string_length = 50;
    const uint32_t c_max_list_size     = 20;
    const uint32_t c_iterations        = 5;
#endif

using namespace std;


template <typename Protocols = bond::BuiltInProtocols, typename T>
void InitRandom(T& x, uint32_t max_string_length = c_max_string_length, uint32_t max_list_size = c_max_list_size)
{
    // Instead of deserializing directly from RandomProtocolReader to instance of T
    // we transcode from Random protocol to Fast protocol and then deserialize
    // the Fast protocol payload into the instance of T. This obvioulsy take longer
    // at runtime, but it compiles faster; build time is the bottleneck for unit tests.
    
    bond::RandomProtocolReader                      random_reader(max_string_length, max_list_size);
    bond::bonded<void, bond::RandomProtocolReader&> random(random_reader, bond::GetRuntimeSchema<T>());

    bond::OutputBuffer                              buffer(1024);
    bond::FastBinaryWriter<bond::OutputBuffer>     writer(buffer);

    random.template Serialize<Protocols>(writer);

    bond::FastBinaryReader<bond::InputBuffer>      reader(buffer.GetBuffer());

    bond::Deserialize<Protocols>(reader, x);
}

template <typename T, typename Protocols = bond::BuiltInProtocols>
T InitRandom(uint32_t max_string_length = c_max_string_length, uint32_t max_list_size = c_max_list_size)
{
    T x;

    InitRandom<Protocols>(x, max_string_length, max_list_size);

    return x;
}

