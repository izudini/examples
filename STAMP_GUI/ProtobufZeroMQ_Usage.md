# Sending Protocol Buffer Messages over ZeroMQ

This document explains how to send and receive Protocol Buffer messages using ZeroMQ in the STAMP_GUI project.

## Overview

The `STAMP_Communicator` class provides methods to:
1. **Send** Protocol Buffer messages to the STAMP device
2. **Receive** Protocol Buffer messages from the STAMP device
3. Handle specific message types (Control commands, Status updates, BIT results)

## How It Works

### Sending Messages

```csharp
// 1. Serialize the protobuf message to a byte array
byte[] messageBytes = message.ToByteArray();

// 2. Send the byte array over ZeroMQ
zmqSocket.SendFrame(messageBytes);
```

### Receiving Messages

```csharp
// 1. Receive the byte array from ZeroMQ
byte[] messageBytes = zmqSocket.ReceiveFrameBytes();

// 2. Parse the protobuf message from the byte array
T message = parser.ParseFrom(messageBytes);
```

## Usage Examples

### Example 1: Send a Start Command

```csharp
// After connecting to STAMP device
if (stampCommunicator.IsConnected)
{
    bool success = stampCommunicator.SendControlCommand(Command.Start);

    if (success)
    {
        MessageBox.Show("Start command sent successfully!");
    }
    else
    {
        MessageBox.Show("Failed to send start command");
    }
}
```

### Example 2: Send a Stop Command

```csharp
if (stampCommunicator.IsConnected)
{
    bool success = stampCommunicator.SendControlCommand(Command.Stop);

    if (success)
    {
        MessageBox.Show("Stop command sent successfully!");
    }
}
```

### Example 3: Send a Custom Protobuf Message

```csharp
// Create a custom protobuf message
var customMessage = new STAMP_Control
{
    STAMPCommand = Command.Start
};

// Send it using the generic SendMessage method
bool success = stampCommunicator.SendMessage(customMessage);
```

### Example 4: Receive Status Updates

```csharp
// Receive a status message with 5 second timeout
Configure_STAMP_Status? status = stampCommunicator.ReceiveStatusMessage(5000);

if (status != null)
{
    // Process the status
    switch (status.Status)
    {
        case SystemStatus.Initializing:
            MessageBox.Show("STAMP is initializing...");
            break;
        case SystemStatus.Operational:
            MessageBox.Show("STAMP is operational!");
            break;
    }
}
else
{
    MessageBox.Show("Failed to receive status or timeout");
}
```

### Example 5: Receive BIT Results

```csharp
// Request and receive BIT results
ConfigureBitResults? bitResults = stampCommunicator.ReceiveBitResults(10000);

if (bitResults != null)
{
    // Check overall BIT response
    if (bitResults.BitResponse == BIT_ResponseType.BitResponsePass)
    {
        MessageBox.Show($"BIT Passed! Duration: {bitResults.DurationOfResponseMsec}ms");

        // Check individual sensor statuses
        Console.WriteLine($"Sensor 1: {bitResults.Sensor1Status}");
        Console.WriteLine($"Sensor 2: {bitResults.Sensor2Status}");
        Console.WriteLine($"Sensor 3: {bitResults.Sensor3Status}");
        Console.WriteLine($"Heimdall: {bitResults.HeimdallStatus}");
    }
    else
    {
        MessageBox.Show("BIT Failed!");
    }
}
```

### Example 6: Request-Response Pattern

```csharp
// Send a start command and wait for status response
private async void StartSTAMP()
{
    if (!stampCommunicator.IsConnected)
    {
        MessageBox.Show("Not connected to STAMP");
        return;
    }

    // Send the start command
    bool sent = stampCommunicator.SendControlCommand(Command.Start);

    if (!sent)
    {
        MessageBox.Show("Failed to send start command");
        return;
    }

    // Wait for response (run in background)
    await Task.Run(() =>
    {
        Configure_STAMP_Status? response = stampCommunicator.ReceiveStatusMessage(5000);

        // Update UI on main thread
        Invoke(new Action(() =>
        {
            if (response != null)
            {
                labelStatus.Text = $"Status: {response.Status}";
            }
            else
            {
                labelStatus.Text = "No response received";
            }
        }));
    });
}
```

### Example 7: Receive Generic Protobuf Message

```csharp
// For receiving any protobuf message type
var customMessage = stampCommunicator.ReceiveMessage(
    CustomMessageType.Parser,
    timeoutMs: 3000
);

if (customMessage != null)
{
    // Process the message
    Console.WriteLine($"Received: {customMessage}");
}
```

## Important Notes

### 1. Connection State
Always check `IsConnected` before sending/receiving:
```csharp
if (stampCommunicator.IsConnected)
{
    // Safe to send/receive
}
```

### 2. Timeouts
- Default timeout for receiving: **5000ms (5 seconds)**
- Customize timeout when calling receive methods:
```csharp
var message = stampCommunicator.ReceiveStatusMessage(10000); // 10 second timeout
```

### 3. Error Handling
- Methods return `false` or `null` on failure
- Always check return values:
```csharp
bool sent = stampCommunicator.SendControlCommand(Command.Start);
if (!sent)
{
    // Handle error
}
```

### 4. Thread Safety
- Receiving is **blocking** - run in background thread or use async:
```csharp
await Task.Run(() =>
{
    var message = stampCommunicator.ReceiveStatusMessage();
    // Process message
});
```

## Available Message Types

### 1. STAMP_Control
- **Purpose**: Send commands to STAMP device
- **Fields**: `STAMPCommand` (Start or Stop)
- **Method**: `SendControlCommand(Command command)`

### 2. Configure_STAMP_Status
- **Purpose**: Receive system status from STAMP
- **Fields**: `Status` (Initializing or Operational)
- **Method**: `ReceiveStatusMessage(int timeoutMs)`

### 3. ConfigureBitResults
- **Purpose**: Receive Built-In Test results
- **Fields**:
  - `BitResponse` (Pass or Fail)
  - `DurationOfResponseMsec`
  - Sensor statuses (1-6)
  - `HeimdallStatus`
- **Method**: `ReceiveBitResults(int timeoutMs)`

## Architecture

```
┌─────────────────────────────────────────────────┐
│          STAMP_Communicator                     │
├─────────────────────────────────────────────────┤
│                                                 │
│  SendMessage(IMessage)                          │
│    └─► message.ToByteArray()                    │
│        └─► zmqSocket.SendFrame(bytes)           │
│                                                 │
│  ReceiveMessage<T>(parser)                      │
│    └─► zmqSocket.ReceiveFrameBytes()            │
│        └─► parser.ParseFrom(bytes)              │
│                                                 │
│  Helper Methods:                                │
│    • SendControlCommand()                       │
│    • ReceiveStatusMessage()                     │
│    • ReceiveBitResults()                        │
│                                                 │
└─────────────────────────────────────────────────┘
           │                          ▲
           │ Protobuf Bytes           │ Protobuf Bytes
           ▼                          │
┌─────────────────────────────────────────────────┐
│            ZeroMQ Socket                        │
│         tcp://192.168.1.10:40000                │
└─────────────────────────────────────────────────┘
```

## Key Methods Reference

| Method | Purpose | Returns |
|--------|---------|---------|
| `Connect()` | Connect to STAMP device | `Task<bool>` |
| `SendMessage(IMessage)` | Send any protobuf message | `bool` |
| `ReceiveMessage<T>(parser, timeout)` | Receive any protobuf message | `T?` |
| `SendControlCommand(Command)` | Send Start/Stop command | `bool` |
| `ReceiveStatusMessage(timeout)` | Receive status update | `Configure_STAMP_Status?` |
| `ReceiveBitResults(timeout)` | Receive BIT results | `ConfigureBitResults?` |
| `IsConnected` | Check connection state | `bool` |
| `Dispose()` | Close connection and cleanup | `void` |
