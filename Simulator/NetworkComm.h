#ifndef NETWORKCOMM_H
#define NETWORKCOMM_H

#include <string>
#include <thread>
#include <atomic>
#include <chrono>

class NetworkComm {
public:
    NetworkComm(const std::string& multicastAddr = "224.0.0.0", int port = 10000, int intervalMs = 1000);
    ~NetworkComm();

    // Start the heartbeat thread
    void start();

    // Stop the heartbeat thread
    void stop();

    // Check if heartbeat is running
    bool isRunning() const;

private:
    // Thread function that sends heartbeat messages
    void heartbeatLoop();

    // Get the local IP address
    std::string getLocalIPAddress();

    // Get application uptime in seconds
    long long getUptimeSeconds();

    // Send a UDP multicast message
    void sendMulticastMessage(const std::string& message);

    std::string multicastAddress_;
    int port_;
    int intervalMs_;
    std::atomic<bool> running_;
    std::thread heartbeatThread_;
    std::chrono::steady_clock::time_point startTime_;

#ifdef _WIN32
    // Windows-specific socket handle
    unsigned long long socketHandle_;
#else
    // Unix-specific socket descriptor
    int socketHandle_;
#endif
};

#endif // NETWORKCOMM_H
