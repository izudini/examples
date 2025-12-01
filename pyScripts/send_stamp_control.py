#!/usr/bin/env python3
"""
UDP Multicast sender for STAMP_Control protocol buffer message.
Sends a STAMP_Control message to 224.0.0.0:40000
"""

import socket
import sys
from pathlib import Path

# Check if protobuf is installed
try:
    import google.protobuf
except ImportError:
    print("Error: protobuf library is not installed")
    print("\nPlease install it using:")
    print("  pip install protobuf")
    print("\nOr on Windows:")
    print("  python -m pip install protobuf")
    sys.exit(1)

# Add pyProto to path to import the compiled proto
proto_path = Path(__file__).parent.parent / "pyProto"
sys.path.insert(0, str(proto_path))

try:
    from STAMP_Control_pb2 import STAMP_Control, Command
except ImportError as e:
    print(f"Error: Could not import STAMP_Control_pb2")
    print(f"Details: {e}")
    print(f"\nMake sure to run compile_proto_python.bat first to generate the proto files")
    print(f"\nAlso ensure the pyProto folder exists at: {proto_path}")
    sys.exit(1)


def create_stamp_control(command):
    """Create a STAMP_Control message with the specified command"""
    message = STAMP_Control()
    message.STAMP_Command = command
    return message


def send_multicast_message(message, multicast_group='224.0.0.0', port=40000):
    """Send a message via UDP multicast"""

    # Serialize the message to bytes
    message_bytes = message.SerializeToString()

    # Map command enum to readable name
    command_names = {
        Command.Start: "Start",
        Command.Stop: "Stop"
    }
    command_name = command_names.get(message.STAMP_Command, "Unknown")

    print(f"Message Details:")
    print(f"  STAMP_Command: {command_name} ({message.STAMP_Command})")
    print(f"  Serialized size: {len(message_bytes)} bytes\n")

    # Create UDP socket
    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

    # Set socket options
    sock.setsockopt(socket.IPPROTO_IP, socket.IP_MULTICAST_TTL, 2)

    try:
        # Send the message to the multicast group
        print(f"Sending message to {multicast_group}:{port}...")
        sock.sendto(message_bytes, (multicast_group, port))
        print("Message sent successfully!")

    except OSError as e:
        print(f"Error sending message: {e}")
        sys.exit(1)
    finally:
        sock.close()


def main():
    print("=" * 50)
    print("UDP Multicast STAMP_Control Sender")
    print("=" * 50)
    print()

    # Check if a command was provided as argument
    if len(sys.argv) > 1:
        if sys.argv[1].lower() == 'stop':
            command = Command.Stop
            print("Sending STOP command...")
        elif sys.argv[1].lower() == 'start':
            command = Command.Start
            print("Sending START command...")
        else:
            print(f"Unknown command: {sys.argv[1]}")
            print("Usage: python send_stamp_control.py [start|stop]")
            print()
            print("Defaulting to START command...")
            command = Command.Start
    else:
        print("No command specified, defaulting to START")
        print("Usage: python send_stamp_control.py [start|stop]")
        print()
        command = Command.Start

    print()

    # Create the message
    print("Creating STAMP_Control message...")
    message = create_stamp_control(command)
    print()

    # Send the message
    send_multicast_message(message)
    print()
    print("=" * 50)


if __name__ == "__main__":
    main()
