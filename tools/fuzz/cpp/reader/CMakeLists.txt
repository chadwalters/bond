add_bond_executable (fuzz-reader
                     ../../idl/compat.bond
                     ../../idl/compat2.bond
                     ../../idl/compat_common.bond
                     fuzz_reader.cpp
                     GBC_OPTIONS
                         --allocator=Mallocator
                         --header=\\\"mallocator.h\\\")
