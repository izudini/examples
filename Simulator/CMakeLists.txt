cmake_minimum_required(VERSION 3.10)
project(Simulator)

# Set C++ standard
set(CMAKE_CXX_STANDARD 11)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# Add executable
add_executable(simulator
    main.cpp
    NetworkComm.cpp
    NetworkComm.h
)

# Platform-specific libraries
if(WIN32)
    # Windows requires ws2_32 and iphlpapi for networking
    target_link_libraries(simulator ws2_32 iphlpapi)
else()
    # Unix/Linux requires pthread for threading
    find_package(Threads REQUIRED)
    target_link_libraries(simulator Threads::Threads)
endif()

# Set output directory
set_target_properties(simulator PROPERTIES
    RUNTIME_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin
)

# Print build information
message(STATUS "Building Simulator")
message(STATUS "System: ${CMAKE_SYSTEM_NAME}")
message(STATUS "C++ Compiler: ${CMAKE_CXX_COMPILER_ID}")
