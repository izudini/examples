# Heartbeat Simulator

A C++ application that sends UDP multicast heartbeat messages containing the machine's IP address and uptime.

## Features

- Sends UDP multicast messages to `224.0.0.0:10000`
- Message format: `<IP_ADDRESS> <UPTIME_SECONDS>`
- Cross-platform support (Windows and Linux)
- Runs in a separate thread
- Configurable heartbeat interval (default: 1 second)

## Building

### Prerequisites

- CMake 3.10 or higher
- C++11 compatible compiler (GCC, Clang, MSVC)

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

After building, the executable will be located in `build/bin/`:

```bash
# Windows
.\build\bin\Release\heartbeat_simulator.exe

# Linux
./build/bin/heartbeat_simulator
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

- `Heartbeat.h` - Heartbeat class definition
- `Heartbeat.cpp` - Implementation of the Heartbeat class
- `main.cpp` - Application entry point
- `CMakeLists.txt` - CMake build configuration

## Usage in Code

```cpp
#include "Heartbeat.h"

// Create heartbeat instance (multicast address, port, interval in ms)
Heartbeat heartbeat("224.0.0.0", 10000, 1000);

// Start sending heartbeats
heartbeat.start();

// ... your application logic ...

// Stop sending heartbeats
heartbeat.stop();
```

## Stopping the Application

Press `Ctrl+C` to gracefully stop the application.
