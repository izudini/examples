# Simulator

A C++ application that sends UDP multicast heartbeat messages and listens for commands via ZeroMQ.

## Features

- **NetworkComm**: Sends UDP multicast messages to `224.0.0.0:10000`
  - Message format: `<IP_ADDRESS> <PORT> <APPLICATION_UPTIME_SECONDS>`
  - Configurable heartbeat interval (default: 1 second)
- **CommandListener**: Receives string commands via ZeroMQ
  - Uses ZeroMQ PULL socket pattern
  - Default endpoint: `tcp://localhost:5555`
  - Thread-safe command queue
- Cross-platform support (Windows and Linux)
- Multi-threaded architecture

## Building

### Prerequisites

- CMake 3.10 or higher
- C++11 compatible compiler (GCC, Clang, MSVC)
- Visual Studio 2019/2022 (for Windows Visual Studio builds)
- **ZeroMQ** (libzmq) and **cppzmq** (C++ bindings)

### Installing ZeroMQ

#### Windows - Method 1: Using vcpkg (Recommended)

vcpkg is Microsoft's C++ package manager and integrates seamlessly with Visual Studio and CMake.

**Step 1: Install vcpkg**
```bash
# Clone vcpkg repository
cd C:\
git clone https://github.com/Microsoft/vcpkg.git
cd vcpkg

# Bootstrap vcpkg
.\bootstrap-vcpkg.bat
```

**Step 2: Install ZeroMQ and cppzmq**
```bash
# For 64-bit Windows
.\vcpkg install zeromq:x64-windows
.\vcpkg install cppzmq:x64-windows

# For 32-bit Windows
# .\vcpkg install zeromq:x86-windows
# .\vcpkg install cppzmq:x86-windows
```

**Step 3: Integrate with Visual Studio**
```bash
# Makes packages automatically available to all Visual Studio projects
.\vcpkg integrate install
```

After integration, both Visual Studio and CMake will automatically find ZeroMQ libraries.

#### Windows - Method 2: Using Chocolatey

If you have Chocolatey package manager installed:
```bash
choco install zeromq
```

#### Windows - Method 3: Manual Installation

1. Download pre-built binaries from:
   - ZeroMQ: https://github.com/zeromq/libzmq/releases
   - cppzmq: https://github.com/zeromq/cppzmq/releases
2. Extract to a known location (e.g., `C:\zeromq`)
3. Manually configure include/library paths in Visual Studio or CMake

#### Linux - Ubuntu/Debian

```bash
sudo apt-get update
sudo apt-get install libzmq3-dev
```

For cppzmq headers:
```bash
# Download cppzmq headers
cd /tmp
git clone https://github.com/zeromq/cppzmq.git
cd cppzmq
sudo cp *.hpp /usr/local/include/
```

Or install via package manager if available:
```bash
sudo apt-get install cppzmq-dev
```

#### Linux - Fedora/RHEL/CentOS

```bash
sudo dnf install zeromq-devel cppzmq-devel
```

#### macOS - Using Homebrew

```bash
brew install zeromq
brew install cppzmq
```

### Build Instructions

#### Windows (CMake with vcpkg)

If you installed ZeroMQ via vcpkg:

```bash
mkdir build
cd build
cmake .. -DCMAKE_TOOLCHAIN_FILE=C:/vcpkg/scripts/buildsystems/vcpkg.cmake
cmake --build . --config Release
```

#### Windows (Visual Studio with vcpkg integration)

After running `vcpkg integrate install`, simply open the solution in Visual Studio:

1. Open `Simulator.sln` in Visual Studio
2. Select **Release** and **x64** configuration
3. Build > Build Solution (or press F7)

The executable will be in `bin\x64\Release\Simulator.exe`

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
    message = data.decode()
    print(f"Received from {address}: {message}")
    # Expected format: "192.168.1.100 52000 42"
    # Format: <IP_ADDRESS> <COMMAND_PORT> <UPTIME_SECONDS>
    parts = message.split()
    if len(parts) == 3:
        ip, cmd_port, uptime = parts
        print(f"  IP: {ip}, Command Port: {cmd_port}, Uptime: {uptime}s")
```

## Code Structure

- `NetworkComm.h` - NetworkComm class definition (UDP multicast sender)
- `NetworkComm.cpp` - Implementation of the NetworkComm class
- `CommandListener.h` - CommandListener class definition (ZeroMQ receiver)
- `CommandListener.cpp` - Implementation of the CommandListener class
- `main.cpp` - Application entry point
- `CMakeLists.txt` - CMake build configuration
- `Simulator.sln` - Visual Studio solution file
- `Simulator.vcxproj` - Visual Studio project file

## Usage in Code

### NetworkComm (UDP Multicast Sender)

```cpp
#include "NetworkComm.h"

// Create network communication instance
// Parameters: multicast address, multicast port, port placeholder, interval in ms
NetworkComm networkComm("224.0.0.0", 10000, 0, 1000);

// Start sending heartbeats
// This will send messages like: "192.168.1.100 0 42"
// Format: <IP_ADDRESS> <PORT> <UPTIME_SECONDS>
networkComm.start();

// ... your application logic ...

// Stop sending heartbeats
networkComm.stop();
```

### CommandListener (ZeroMQ Command Receiver)

The CommandListener uses ZeroMQ PULL socket pattern to receive string commands.

```cpp
#include "CommandListener.h"

// Create command listener (default: tcp://localhost:5555)
CommandListener listener("tcp://localhost:5555");

// Or use a different endpoint
// CommandListener listener("tcp://*:5555");  // Bind mode
// CommandListener listener("tcp://192.168.1.100:5555");  // Connect to remote

// Start listening for commands
listener.start();

// Poll for commands in your main loop
while (running) {
    if (listener.hasCommands()) {
        std::string command = listener.getCommand();
        std::cout << "Received command: " << command << std::endl;

        // Process the command
        if (command == "STOP") {
            running = false;
        } else if (command == "STATUS") {
            std::cout << "Status: Running" << std::endl;
        }
    }

    // Check queue size
    size_t count = listener.getCommandCount();
    if (count > 0) {
        std::cout << "Commands in queue: " << count << std::endl;
    }

    std::this_thread::sleep_for(std::chrono::milliseconds(10));
}

// Stop listening
listener.stop();
```

## Complete Application Example

Integrating both NetworkComm and CommandListener:

```cpp
#include "NetworkComm.h"
#include "CommandListener.h"
#include <iostream>
#include <csignal>
#include <atomic>
#include <thread>
#include <chrono>

std::atomic<bool> keepRunning(true);

void signalHandler(int signal) {
    std::cout << "\nReceived signal " << signal << ", shutting down..." << std::endl;
    keepRunning = false;
}

int main() {
    try {
        signal(SIGINT, signalHandler);
        signal(SIGTERM, signalHandler);

        const int MULTICAST_PORT = 10000;
        const std::string ZMQ_ENDPOINT = "tcp://localhost:5555";

        std::cout << "========================================" << std::endl;
        std::cout << "===       Simulator Application      ===" << std::endl;
        std::cout << "========================================" << std::endl;
        std::cout << "UDP Multicast: 224.0.0.0:10000" << std::endl;
        std::cout << "ZeroMQ Endpoint: " << ZMQ_ENDPOINT << std::endl;
        std::cout << "Press Ctrl+C to stop" << std::endl;
        std::cout << "========================================" << std::endl;

        // Create and start network communication
        NetworkComm networkComm("224.0.0.0", MULTICAST_PORT, 0, 1000);
        networkComm.start();

        // Create and start command listener
        CommandListener listener(ZMQ_ENDPOINT);
        listener.start();

        // Main loop
        while (keepRunning) {
            // Check for incoming commands
            if (listener.hasCommands()) {
                std::string command = listener.getCommand();
                std::cout << "[Main] Received command: " << command << std::endl;

                // Process commands
                if (command == "STOP" || command == "QUIT" || command == "EXIT") {
                    std::cout << "[Main] Stop command received" << std::endl;
                    keepRunning = false;
                } else if (command == "STATUS") {
                    std::cout << "[Main] Status: Running, Queue size: "
                              << listener.getCommandCount() << std::endl;
                }
            }

            std::this_thread::sleep_for(std::chrono::milliseconds(100));
        }

        // Stop all components
        std::cout << "\nStopping components..." << std::endl;
        networkComm.stop();
        listener.stop();

        std::cout << "Application terminated successfully" << std::endl;
        return 0;

    } catch (const std::exception& e) {
        std::cerr << "FATAL ERROR: " << e.what() << std::endl;
        return 1;
    }
}
```

### What Happens When Running:

1. **NetworkComm** sends UDP multicast messages every second: `192.168.1.100 0 42`
2. **CommandListener** listens for ZeroMQ commands on `tcp://localhost:5555`
3. Application processes commands like "STOP", "STATUS", etc.
4. Application runs until you press `Ctrl+C` or send a "STOP" command
5. All components shut down gracefully

### Testing the Application

#### Testing UDP Multicast Messages

Create a Python script to receive multicast messages:

```bash
python test_multicast_receiver.py
```

#### Testing ZeroMQ Commands

You need a ZeroMQ PUSH client to send commands. Here's a Python example:

**Python sender (test_zmq_sender.py):**
```python
import zmq
import sys

# Create ZeroMQ context and PUSH socket
context = zmq.Context()
socket = context.socket(zmq.PUSH)
socket.bind("tcp://*:5555")

print("ZeroMQ sender ready. Type commands (or 'quit' to exit):")

while True:
    command = input("> ")
    if command.lower() == 'quit':
        break

    # Send command
    socket.send_string(command)
    print(f"Sent: {command}")

socket.close()
context.term()
```

**Run the sender:**
```bash
python test_zmq_sender.py
```

Then type commands like:
- `STATUS` - Get application status
- `STOP` - Stop the application
- Any custom command you want to process

#### Alternative: Using ZeroMQ command-line tools

If you have ZeroMQ installed, you can use `zmqsend`:
```bash
# Send a single command
echo "STATUS" | zmqsend -c tcp://localhost:5555
```

## Stopping the Application

You can stop the application in two ways:

1. **Press `Ctrl+C`** - Triggers signal handler for graceful shutdown
2. **Send a ZeroMQ command** - Send "STOP", "QUIT", or "EXIT" command via ZeroMQ

Both methods will gracefully shut down NetworkComm and CommandListener components.

## Important Notes

### ZeroMQ Socket Pattern

The CommandListener uses **PULL** socket pattern, which means:
- The sender must use **PUSH** socket
- PUSH-PULL pattern is great for distributing work to multiple workers
- Messages are load-balanced across connected PULL sockets
- If you need request-reply pattern, consider using REQ-REP sockets instead

### Endpoint Configuration

The default endpoint is `tcp://localhost:5555` which **connects** to a remote PUSH socket. If you want CommandListener to **bind** instead:

```cpp
// Bind mode - wait for senders to connect
CommandListener listener("tcp://*:5555");
```

Note: You'll need to modify `CommandListener.cpp` to use `bind()` instead of `connect()` for bind mode.

### Troubleshooting

**Issue: CMake can't find ZeroMQ**
- Ensure you ran `vcpkg integrate install` (Windows)
- Use `-DCMAKE_TOOLCHAIN_FILE` pointing to vcpkg.cmake
- On Linux, ensure libzmq3-dev and cppzmq-dev are installed

**Issue: Application crashes on startup**
- Verify ZeroMQ DLLs are in PATH or same directory as executable (Windows)
- Check that the ZeroMQ endpoint is not already in use
- Ensure cppzmq headers are available

**Issue: Commands not received**
- Verify the PUSH sender is using the same endpoint
- Check that sender is using PUSH and receiver is using PULL socket types
- Ensure firewall allows connections on port 5555
