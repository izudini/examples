#ifndef COMMANDLISTENER_H
#define COMMANDLISTENER_H

#include <string>
#include <thread>
#include <atomic>
#include <queue>
#include <mutex>
#include <memory>

// Forward declaration for ZeroMQ context and socket
namespace zmq {
    class context_t;
    class socket_t;
}

class CommandListener {
public:
    // Constructor with ZeroMQ endpoint (default: tcp://localhost:5555)
    CommandListener(const std::string& endpoint = "tcp://localhost:5555");
    ~CommandListener();

    // Start the ZeroMQ listener thread
    void start();

    // Stop the ZeroMQ listener thread
    void stop();

    // Check if listener is running
    bool isRunning() const;

    // Check if there are commands in the queue
    bool hasCommands() const;

    // Get the next command from the queue (returns empty string if queue is empty)
    std::string getCommand();

    // Get the number of commands in the queue
    size_t getCommandCount() const;

    // Clear all commands from the queue
    void clearCommands();

private:
    // Thread function that listens for ZeroMQ messages
    void listenerLoop();

    std::string endpoint_;
    std::atomic<bool> running_;
    std::atomic<bool> initialized_;

    // Command queue and synchronization
    std::queue<std::string> commandQueue_;
    mutable std::mutex queueMutex_;

    // ZeroMQ context and socket (using unique_ptr for forward declaration)
    std::unique_ptr<zmq::context_t> context_;
    std::unique_ptr<zmq::socket_t> socket_;

    std::thread listenerThread_;
};

#endif // COMMANDLISTENER_H
