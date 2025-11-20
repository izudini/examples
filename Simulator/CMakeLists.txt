cmake_minimum_required(VERSION 3.10)
project(Simulator)

# Set C++ standard
set(CMAKE_CXX_STANDARD 11)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# Find ZeroMQ
find_package(ZeroMQ QUIET)
if(NOT ZeroMQ_FOUND)
    find_package(PkgConfig QUIET)
    if(PkgConfig_FOUND)
        pkg_check_modules(ZeroMQ QUIET libzmq)
    endif()
endif()

# Find cppzmq (C++ binding for ZeroMQ)
find_package(cppzmq QUIET)

# Add executable
add_executable(simulator
    main.cpp
    NetworkComm.cpp
    NetworkComm.h
    Globals.cpp
    Globals.h
    Controller.cpp
    Controller.h
)

# Link ZeroMQ libraries
if(ZeroMQ_FOUND)
    if(TARGET libzmq OR TARGET libzmq-static)
        target_link_libraries(simulator libzmq)
    elseif(ZeroMQ_LIBRARIES)
        target_include_directories(simulator PRIVATE ${ZeroMQ_INCLUDE_DIRS})
        target_link_libraries(simulator ${ZeroMQ_LIBRARIES})
    endif()
    message(STATUS "ZeroMQ found and linked")
else()
    message(WARNING "ZeroMQ not found. Please install ZeroMQ and cppzmq to build CommandListener support.")
endif()

if(cppzmq_FOUND)
    target_link_libraries(simulator cppzmq)
    message(STATUS "cppzmq found and linked")
endif()

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
