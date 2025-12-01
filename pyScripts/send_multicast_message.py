#!/usr/bin/env python3
"""
UDP Multicast sender for ConfigureBitResults protocol buffer message.
Sends a ConfigureBitResults message to 224.0.0.0:40000
"""

import socket
import struct
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
    from ConfigureBitResults_pb2 import ConfigureBitResults, BIT_ResponseType, ComponentStatus
except ImportError as e:
    print(f"Error: Could not import ConfigureBitResults_pb2")
    print(f"Details: {e}")
    print(f"\nMake sure to run compile_proto_python.bat first to generate the proto files")
    print(f"\nAlso ensure the pyProto folder exists at: {proto_path}")
    sys.exit(1)


def create_configure_bit_results():
    """Create and configure a ConfigureBitResults message"""
    message = ConfigureBitResults()

    # Set BIT Response
    message.BitResponse = BIT_ResponseType.BIT_RESPONSE_PASS

    # Set duration in milliseconds
    message.DurationOfResponse_msec = 1500

    # Set sensor statuses
    message.Sensor_1_Status = ComponentStatus.ComponentOperational
    message.Sensor_2_Status = ComponentStatus.ComponentOperational
    message.Sensor_3_Status = ComponentStatus.ComponentOperational
    message.Sensor_4_Status = ComponentStatus.ComponentOperational
    message.Sensor_5_Status = ComponentStatus.ComponentOperational
    message.Sensor_6_Status = ComponentStatus.ComponentOperational

    # Set Heimdall status
    message.Heimdall_Status = ComponentStatus.ComponentOperational

    return message


def send_multicast_message(message, multicast_group='224.0.0.0', port=40000):
    """Send a message via UDP multicast"""

    # Serialize the message to bytes
    message_bytes = message.SerializeToString()

    print(f"Message Details:")
    print(f"  BitResponse: {message.BitResponse}")
    print(f"  DurationOfResponse_msec: {message.DurationOfResponse_msec}")
    print(f"  Sensor_1_Status: {message.Sensor_1_Status}")
    print(f"  Sensor_2_Status: {message.Sensor_2_Status}")
    print(f"  Sensor_3_Status: {message.Sensor_3_Status}")
    print(f"  Sensor_4_Status: {message.Sensor_4_Status}")
    print(f"  Sensor_5_Status: {message.Sensor_5_Status}")
    print(f"  Sensor_6_Status: {message.Sensor_6_Status}")
    print(f"  Heimdall_Status: {message.Heimdall_Status}")
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
    print("UDP Multicast ConfigureBitResults Sender")
    print("=" * 50)
    print()

    # Create the message
    print("Creating ConfigureBitResults message...")
    message = create_configure_bit_results()
    print()

    # Send the message
    send_multicast_message(message)
    print()
    print("=" * 50)


if __name__ == "__main__":
    main()
