#ifndef GLOBALS_H
#define GLOBALS_H

#include <string>
#include <queue>
#include <mutex>

class Globals {
public:
    // Delete constructor to prevent instantiation (static class)
    Globals() = delete;
    Globals(const Globals&) = delete;
    Globals& operator=(const Globals&) = delete;

    // Thread-safe queue operations
    static void pushMessage(const std::string& message);
    static bool popMessage(std::string& message);
    static bool hasMessages();
    static size_t getMessageCount();
    static void clearMessages();

private:
    static std::queue<std::string> messageQueue_;
    static std::mutex queueMutex_;
};

#endif // GLOBALS_H
