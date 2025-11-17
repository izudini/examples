#include "Heartbeat.h"
#include <iostream>
#include <sstream>
#include <cstring>

#ifdef _WIN32
    #define WIN32_LEAN_AND_MEAN
    #include <windows.h>
    #include <winsock2.h>
    #include <ws2tcpip.h>
    #include <iphlpapi.h>
    #pragma comment(lib, "ws2_32.lib")
    #pragma comment(lib, "iphlpapi.lib")
#else
    #include <sys/socket.h>
    #include <netinet/in.h>
    #include <arpa/inet.h>
    #include <unistd.h>
    #include <ifaddrs.h>
    #include <net/if.h>
    #include <sys/sysinfo.h>
#endif

Heartbeat::Heartbeat(const std::string& multicastAddr, int port, int intervalMs)
    : multicastAddress_(multicastAddr)
    , port_(port)
    , intervalMs_(intervalMs)
    , running_(false)
    , startTime_(std::chrono::steady_clock::now())
    , socketHandle_(0)
{
#ifdef _WIN32
    WSADATA wsaData;
    if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0) {
        std::cerr << "WSAStartup failed" << std::endl;
    }
#endif
}

Heartbeat::~Heartbeat() {
    stop();
#ifdef _WIN32
    WSACleanup();
#endif
}

void Heartbeat::start() {
    if (running_) {
        std::cout << "Heartbeat already running" << std::endl;
        return;
    }

    running_ = true;
    heartbeatThread_ = std::thread(&Heartbeat::heartbeatLoop, this);
    std::cout << "Heartbeat started" << std::endl;
}

void Heartbeat::stop() {
    if (!running_) {
        return;
    }

    running_ = false;
    if (heartbeatThread_.joinable()) {
        heartbeatThread_.join();
    }

#ifdef _WIN32
    if (socketHandle_ != 0) {
        closesocket(static_cast<SOCKET>(socketHandle_));
        socketHandle_ = 0;
    }
#else
    if (socketHandle_ > 0) {
        close(socketHandle_);
        socketHandle_ = 0;
    }
#endif

    std::cout << "Heartbeat stopped" << std::endl;
}

bool Heartbeat::isRunning() const {
    return running_;
}

void Heartbeat::heartbeatLoop() {
    // Create UDP socket
#ifdef _WIN32
    SOCKET sock = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
    if (sock == INVALID_SOCKET) {
        std::cerr << "Failed to create socket: " << WSAGetLastError() << std::endl;
        running_ = false;
        return;
    }
    socketHandle_ = static_cast<unsigned long long>(sock);
#else
    int sock = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
    if (sock < 0) {
        std::cerr << "Failed to create socket" << std::endl;
        running_ = false;
        return;
    }
    socketHandle_ = sock;
#endif

    // Set socket options for multicast
    int ttl = 1; // Time to live (1 = local network only)
#ifdef _WIN32
    if (setsockopt(sock, IPPROTO_IP, IP_MULTICAST_TTL, (const char*)&ttl, sizeof(ttl)) < 0) {
#else
    if (setsockopt(sock, IPPROTO_IP, IP_MULTICAST_TTL, &ttl, sizeof(ttl)) < 0) {
#endif
        std::cerr << "Failed to set TTL option" << std::endl;
    }

    // Setup multicast address
    struct sockaddr_in multicastAddr;
    memset(&multicastAddr, 0, sizeof(multicastAddr));
    multicastAddr.sin_family = AF_INET;
    multicastAddr.sin_port = htons(port_);
    multicastAddr.sin_addr.s_addr = inet_addr(multicastAddress_.c_str());

    while (running_) {
        // Get local IP and uptime
        std::string localIP = getLocalIPAddress();
        long long uptime = getUptimeSeconds();

        // Create message: "IP UPTIME"
        std::ostringstream oss;
        oss << localIP << " " << uptime;
        std::string message = oss.str();

        // Send multicast message
#ifdef _WIN32
        int bytesSent = sendto(sock, message.c_str(), static_cast<int>(message.length()), 0,
                               (struct sockaddr*)&multicastAddr, sizeof(multicastAddr));
#else
        ssize_t bytesSent = sendto(sock, message.c_str(), message.length(), 0,
                                   (struct sockaddr*)&multicastAddr, sizeof(multicastAddr));
#endif

        if (bytesSent < 0) {
#ifdef _WIN32
            std::cerr << "Failed to send multicast message: " << WSAGetLastError() << std::endl;
#else
            std::cerr << "Failed to send multicast message" << std::endl;
#endif
        } else {
            std::cout << "Sent: " << message << std::endl;
        }

        // Sleep for the specified interval
        std::this_thread::sleep_for(std::chrono::milliseconds(intervalMs_));
    }
}

std::string Heartbeat::getLocalIPAddress() {
#ifdef _WIN32
    // Windows implementation
    char hostName[256];
    if (gethostname(hostName, sizeof(hostName)) == SOCKET_ERROR) {
        return "0.0.0.0";
    }

    struct addrinfo hints, *result = nullptr;
    memset(&hints, 0, sizeof(hints));
    hints.ai_family = AF_INET;
    hints.ai_socktype = SOCK_DGRAM;

    if (getaddrinfo(hostName, nullptr, &hints, &result) != 0) {
        return "0.0.0.0";
    }

    std::string ipAddress = "0.0.0.0";
    for (struct addrinfo* ptr = result; ptr != nullptr; ptr = ptr->ai_next) {
        struct sockaddr_in* sockaddr_ipv4 = (struct sockaddr_in*)ptr->ai_addr;
        char ipStr[INET_ADDRSTRLEN];
        inet_ntop(AF_INET, &(sockaddr_ipv4->sin_addr), ipStr, INET_ADDRSTRLEN);
        std::string ip(ipStr);

        // Skip loopback
        if (ip != "127.0.0.1") {
            ipAddress = ip;
            break;
        }
    }

    freeaddrinfo(result);
    return ipAddress;
#else
    // Unix/Linux implementation
    struct ifaddrs* ifAddrStruct = nullptr;
    struct ifaddrs* ifa = nullptr;
    void* tmpAddrPtr = nullptr;
    std::string ipAddress = "0.0.0.0";

    getifaddrs(&ifAddrStruct);

    for (ifa = ifAddrStruct; ifa != nullptr; ifa = ifa->ifa_next) {
        if (!ifa->ifa_addr) {
            continue;
        }

        if (ifa->ifa_addr->sa_family == AF_INET) {
            tmpAddrPtr = &((struct sockaddr_in*)ifa->ifa_addr)->sin_addr;
            char addressBuffer[INET_ADDRSTRLEN];
            inet_ntop(AF_INET, tmpAddrPtr, addressBuffer, INET_ADDRSTRLEN);
            std::string ip(addressBuffer);

            // Skip loopback, get first non-loopback interface
            if (ip != "127.0.0.1" && !(ifa->ifa_flags & IFF_LOOPBACK)) {
                ipAddress = ip;
                break;
            }
        }
    }

    if (ifAddrStruct != nullptr) {
        freeifaddrs(ifAddrStruct);
    }

    return ipAddress;
#endif
}

long long Heartbeat::getUptimeSeconds() {
#ifdef _WIN32
    // Windows: Use GetTickCount64 for milliseconds since boot
    return static_cast<long long>(GetTickCount64() / 1000);
#else
    // Linux: Use sysinfo
    struct sysinfo info;
    if (sysinfo(&info) == 0) {
        return static_cast<long long>(info.uptime);
    }
    return 0;
#endif
}
