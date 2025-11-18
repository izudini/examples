# GUIApp - UDP Multicast Listener

A C# WinForms application that listens to UDP multicast messages on address 224.0.0.0 port 10000 and prints the received messages to the console.

## Features

- Listens to UDP multicast group 224.0.0.0 on port 10000
- Automatically starts listening when the form loads
- Prints received messages to the console
- Properly cleans up network resources when closing

## Building the Application

### Using Visual Studio
1. Open `GUIApp.sln` in Visual Studio
2. Build the solution (Ctrl+Shift+B)
3. Run the application (F5)

### Using .NET CLI
```bash
dotnet build
dotnet run
```

## Usage

1. Run the application
2. The form will appear and automatically start listening for UDP multicast messages
3. Open a console window to see the output (messages will be printed to stdout)
4. Any messages sent to multicast group 224.0.0.0:10000 will be displayed in the console

## Testing with the Simulator

You can test this receiver with the UDP multicast simulator located in the `../Simulator` directory.

## Requirements

- .NET 6.0 or later
- Windows operating system (WinForms)

## Technical Details

- Uses `UdpClient` to join multicast group
- Asynchronous message reception to avoid blocking the UI thread
- Proper resource cleanup on form closing
- UTF-8 encoding for message decoding
