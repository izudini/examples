#include "NetworkComm.h"
#include "Controller.h"
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
    try {
        // Register signal handlers for graceful shutdown
        signal(SIGINT, signalHandler);
        signal(SIGTERM, signalHandler);

        std::cout << "========================================" << std::endl;
        std::cout << "===       Simulator Application      ===" << std::endl;
        std::cout << "========================================" << std::endl;
        std::cout << "UDP Multicast: 224.0.0.0:10000" << std::endl;
        std::cout << "Press Ctrl+C to stop" << std::endl;
        std::cout << "========================================" << std::endl << std::endl;

        const int MULTICAST_PORT = 10000;

        // Create network communication instance
        // Parameters: multicast address, multicast port, command port (not used), interval in milliseconds
        std::cout << "Creating NetworkComm..." << std::endl;
        NetworkComm networkComm("224.0.0.0", MULTICAST_PORT, 0, 1000);
        std::cout << "NetworkComm created successfully" << std::endl;

        // Create controller instance
        std::cout << "Creating Controller..." << std::endl;
        Controller controller;
        std::cout << "Controller created successfully" << std::endl;

        // Start network communication
        std::cout << "Starting NetworkComm..." << std::endl;
        networkComm.start();
        std::cout << "NetworkComm started successfully!" << std::endl << std::endl;

        // Start controller
        std::cout << "Starting Controller..." << std::endl;
        controller.start();
        std::cout << "Controller started successfully!" << std::endl << std::endl;

        // Keep the main thread alive until interrupted
        while (keepRunning) {
            std::this_thread::sleep_for(std::chrono::milliseconds(100));
        }

        // Stop network communication
        std::cout << "\nStopping NetworkComm..." << std::endl;
        networkComm.stop();

        // Stop controller
        std::cout << "Stopping Controller..." << std::endl;
        controller.stop();

        std::cout << "\nApplication terminated successfully" << std::endl;
        return 0;

    } catch (const std::system_error& e) {
        std::cerr << "\nFATAL SYSTEM ERROR: " << e.what() << std::endl;
        std::cerr << "Error code: " << e.code() << std::endl;
        std::cerr << "Category: " << e.code().category().name() << std::endl;
        std::cerr << "Application failed to start." << std::endl;
        return 1;
    } catch (const std::exception& e) {
        std::cerr << "\nFATAL ERROR: " << e.what() << std::endl;
        std::cerr << "Application failed to start or encountered a critical error." << std::endl;
        return 1;
    } catch (...) {
        std::cerr << "\nFATAL ERROR: Unknown exception occurred" << std::endl;
        std::cerr << "Application failed to start or encountered a critical error." << std::endl;
        return 1;
    }
}
