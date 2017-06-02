add_subdirectory (cpp)

if (BOND_ENABLE_PYTHON)
  add_python_subdirectory (python)
endif()
