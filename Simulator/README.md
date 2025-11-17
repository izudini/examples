# Simulator

A C++ application that sends UDP multicast heartbeat messages containing the machine's IP address and application uptime.

## Features

- Sends UDP multicast messages to `224.0.0.0:10000`
- Message format: `<IP_ADDRESS> <APPLICATION_UPTIME_SECONDS>`
- Cross-platform support (Windows and Linux)
- Runs in a separate thread
- Configurable heartbeat interval (default: 1 second)

## Building

### Prerequisites

- CMake 3.10 or higher
- C++11 compatible compiler (GCC, Clang, MSVC)
- Visual Studio 2019/2022 (for Windows Visual Studio builds)

### Build Instructions

#### Windows (Visual Studio)

```bash
mkdir build
cd build
cmake ..
cmake --build . --config Release
```

#### Linux/Unix

```bash
mkdir build
cd build
cmake ..
make
```

## Running

### CMake Build
After building with CMake, the executable will be located in `build/bin/`:

```bash
# Windows
.\build\bin\Release\simulator.exe

# Linux
./build/bin/simulator
```

### Visual Studio Build
After building in Visual Studio, the executable will be located in `bin/[Platform]/[Configuration]/`:

```bash
# Example: 64-bit Debug build
.\bin\x64\Debug\Simulator.exe

# Example: 64-bit Release build
.\bin\x64\Release\Simulator.exe
```

## Testing Multicast Reception

You can test the multicast messages using a simple listener. Here's a Python example:

```python
import socket
import struct

MULTICAST_GROUP = '224.0.0.0'
PORT = 10000

sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
sock.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
sock.bind(('', PORT))

mreq = struct.pack('4sL', socket.inet_aton(MULTICAST_GROUP), socket.INADDR_ANY)
sock.setsockopt(socket.IPPROTO_IP, socket.IP_ADD_MEMBERSHIP, mreq)

print(f"Listening for multicast messages on {MULTICAST_GROUP}:{PORT}")
while True:
    data, address = sock.recvfrom(1024)
    print(f"Received from {address}: {data.decode()}")
```

## Code Structure

- `NetworkComm.h` - NetworkComm class definition
- `NetworkComm.cpp` - Implementation of the NetworkComm class
- `main.cpp` - Application entry point
- `CMakeLists.txt` - CMake build configuration
- `Simulator.sln` - Visual Studio solution file
- `Simulator.vcxproj` - Visual Studio project file

## Usage in Code

```cpp
#include "NetworkComm.h"

// Create network communication instance (multicast address, port, interval in ms)
NetworkComm networkComm("224.0.0.0", 10000, 1000);

// Start sending heartbeats
networkComm.start();

// ... your application logic ...

// Stop sending heartbeats
networkComm.stop();
```

## Stopping the Application

Press `Ctrl+C` to gracefully stop the application.
