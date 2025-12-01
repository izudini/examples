#!/usr/bin/env python3
"""
UDP Multicast sender for Configure_STAMP_Status protocol buffer message.
Sends a Configure_STAMP_Status message to 224.0.0.0:40000
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
    from Configure_STAMP_Status_pb2 import Configure_STAMP_Status, SystemStatus
except ImportError as e:
    print(f"Error: Could not import Configure_STAMP_Status_pb2")
    print(f"Details: {e}")
    print(f"\nMake sure to run compile_proto_python.bat first to generate the proto files")
    print(f"\nAlso ensure the pyProto folder exists at: {proto_path}")
    sys.exit(1)


def create_configure_stamp_status(status):
    """Create a Configure_STAMP_Status message with the specified status"""
    message = Configure_STAMP_Status()
    message.status = status
    return message


def send_multicast_message(message, multicast_group='224.0.0.0', port=40000):
    """Send a message via UDP multicast"""

    # Serialize the message to bytes
    message_bytes = message.SerializeToString()

    # Map status enum to readable name
    status_names = {
        SystemStatus.Status_Initializing: "Initializing",
        SystemStatus.Status_Normal: "Normal",
        SystemStatus.Status_Degraded: "Degraded",
        SystemStatus.Status_Inoprable: "Inoperable"
    }
    status_name = status_names.get(message.status, "Unknown")

    print(f"Message Details:")
    print(f"  SystemStatus: {status_name} ({message.status})")
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
    print("UDP Multicast Configure_STAMP_Status Sender")
    print("=" * 50)
    print()

    # Check if a status was provided as argument
    if len(sys.argv) > 1:
        status_arg = sys.argv[1].lower()
        if status_arg == 'initializing':
            status = SystemStatus.Status_Initializing
            print("Sending INITIALIZING status...")
        elif status_arg == 'normal':
            status = SystemStatus.Status_Normal
            print("Sending NORMAL status...")
        elif status_arg == 'degraded':
            status = SystemStatus.Status_Degraded
            print("Sending DEGRADED status...")
        elif status_arg == 'inoperable':
            status = SystemStatus.Status_Inoprable
            print("Sending INOPERABLE status...")
        else:
            print(f"Unknown status: {sys.argv[1]}")
            print("Usage: python send_configure_stamp_status.py [initializing|normal|degraded|inoperable]")
            print()
            print("Defaulting to NORMAL status...")
            status = SystemStatus.Status_Normal
    else:
        print("No status specified, defaulting to NORMAL")
        print("Usage: python send_configure_stamp_status.py [initializing|normal|degraded|inoperable]")
        print()
        status = SystemStatus.Status_Normal

    print()

    # Create the message
    print("Creating Configure_STAMP_Status message...")
    message = create_configure_stamp_status(status)
    print()

    # Send the message
    send_multicast_message(message)
    print()
    print("=" * 50)


if __name__ == "__main__":
    main()
