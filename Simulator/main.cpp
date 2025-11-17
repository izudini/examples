#include "NetworkComm.h"
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

int main(int argc, char* argv[]) {
    // Register signal handlers for graceful shutdown
    signal(SIGINT, signalHandler);
    signal(SIGTERM, signalHandler);

    std::cout << "=== Heartbeat Simulator ===" << std::endl;
    std::cout << "Multicast Address: 224.0.0.0" << std::endl;
    std::cout << "Port: 10000" << std::endl;
    std::cout << "Press Ctrl+C to stop" << std::endl;
    std::cout << "===========================" << std::endl << std::endl;

    // Create network communication instance
    // Parameters: multicast address, port, interval in milliseconds
    NetworkComm networkComm("224.0.0.0", 10000, 1000);

    // Start the heartbeat thread
    networkComm.start();

    // Keep the main thread alive until interrupted
    while (keepRunning && networkComm.isRunning()) {
        std::this_thread::sleep_for(std::chrono::milliseconds(100));
    }

    // Stop the heartbeat
    networkComm.stop();

    std::cout << "Application terminated" << std::endl;
    return 0;
}
