
if (BOND_ENABLE_FUZZ)
    add_subfolder (fuzz/cpp/generator "tools/fuzz/generator")
    add_subfolder (fuzz/cpp/reader "tools/fuzz/reader")
endif()
