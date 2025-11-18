#include "CommandListener.h"
#include <zmq.hpp>
#include <iostream>

CommandListener::CommandListener(const std::string& endpoint)
    : endpoint_(endpoint)
    , running_(false)
    , initialized_(false)
    , context_(nullptr)
    , socket_(nullptr)
{
    std::cout << "[CommandListener] Constructor started for endpoint " << endpoint << std::endl;

    try {
        // Create ZeroMQ context (1 I/O thread)
        context_ = std::make_unique<zmq::context_t>(1);

        // Create PULL socket for receiving messages
        socket_ = std::make_unique<zmq::socket_t>(*context_, zmq::socket_type::pull);

        // Set socket options
        int linger = 0;
        socket_->set(zmq::sockopt::linger, linger);

        // Set receive timeout to allow checking running_ flag
        int timeout = 1000; // 1 second timeout
        socket_->set(zmq::sockopt::rcvtimeo, timeout);

        initialized_ = true;
        std::cout << "[CommandListener] Initialized successfully" << std::endl;
    } catch (const zmq::error_t& e) {
        std::cerr << "[CommandListener] ZeroMQ error during initialization: " << e.what() << std::endl;
        initialized_ = false;
    } catch (const std::exception& e) {
        std::cerr << "[CommandListener] Error during initialization: " << e.what() << std::endl;
        initialized_ = false;
    }
}

CommandListener::~CommandListener() {
    std::cout << "[CommandListener] Destructor called" << std::endl;
    initialized_ = false;
    stop();

    // Clean up socket and context
    socket_.reset();
    context_.reset();
}

void CommandListener::start() {
    if (!initialized_) {
        std::cerr << "[CommandListener] Cannot start - not initialized" << std::endl;
        return;
    }

    if (running_) {
        std::cout << "[CommandListener] Already running" << std::endl;
        return;
    }

    try {
        // Connect to the ZeroMQ endpoint
        socket_->connect(endpoint_);
        std::cout << "[CommandListener] Connected to " << endpoint_ << std::endl;

        running_ = true;
        listenerThread_ = std::thread(&CommandListener::listenerLoop, this);
        std::cout << "[CommandListener] Listener thread started" << std::endl;
    } catch (const zmq::error_t& e) {
        std::cerr << "[CommandListener] ZeroMQ error: " << e.what() << std::endl;
        running_ = false;
        throw;
    } catch (const std::exception& e) {
        std::cerr << "[CommandListener] Failed to start: " << e.what() << std::endl;
        running_ = false;
        throw;
    }
}

void CommandListener::stop() {
    if (!running_) {
        return;
    }

    std::cout << "[CommandListener] Stopping..." << std::endl;
    running_ = false;

    if (listenerThread_.joinable()) {
        listenerThread_.join();
    }

    // Disconnect socket
    try {
        if (socket_) {
            socket_->disconnect(endpoint_);
        }
    } catch (const zmq::error_t& e) {
        std::cerr << "[CommandListener] Error disconnecting: " << e.what() << std::endl;
    }

    std::cout << "[CommandListener] Stopped" << std::endl;
}

bool CommandListener::isRunning() const {
    return running_;
}

void CommandListener::listenerLoop() {
    std::cout << "[CommandListener] Listener loop started" << std::endl;

    while (running_) {
        try {
            // Receive message from ZeroMQ
            zmq::message_t message;
            zmq::recv_result_t result = socket_->recv(message, zmq::recv_flags::none);

            if (result.has_value() && result.value() > 0) {
                // Convert message to string
                std::string command(static_cast<char*>(message.data()), message.size());

                if (!command.empty() && running_ && initialized_) {
                    std::cout << "[CommandListener] Received command: " << command << std::endl;

                    // Add command to queue
                    try {
                        std::lock_guard<std::mutex> lock(queueMutex_);
                        if (running_ && initialized_) {
                            commandQueue_.push(command);
                        }
                    } catch (const std::exception& e) {
                        std::cerr << "[CommandListener] Error adding command to queue: " << e.what() << std::endl;
                    }
                }
            }
        } catch (const zmq::error_t& e) {
            // Handle timeout (EAGAIN) gracefully
            if (e.num() == EAGAIN) {
                // Timeout - this is normal, just continue
                continue;
            } else if (e.num() == ETERM) {
                // Context was terminated
                std::cout << "[CommandListener] ZeroMQ context terminated" << std::endl;
                break;
            } else if (running_) {
                std::cerr << "[CommandListener] ZeroMQ error: " << e.what() << std::endl;
            }
        } catch (const std::exception& e) {
            if (running_) {
                std::cerr << "[CommandListener] Error in listener loop: " << e.what() << std::endl;
            }
        }
    }

    std::cout << "[CommandListener] Listener loop exited" << std::endl;
}

bool CommandListener::hasCommands() const {
    if (!initialized_) return false;
    try {
        std::lock_guard<std::mutex> lock(queueMutex_);
        return !commandQueue_.empty();
    } catch (...) {
        return false;
    }
}

std::string CommandListener::getCommand() {
    if (!initialized_) return "";
    try {
        std::lock_guard<std::mutex> lock(queueMutex_);
        if (commandQueue_.empty()) {
            return "";
        }
        std::string command = commandQueue_.front();
        commandQueue_.pop();
        return command;
    } catch (...) {
        return "";
    }
}

size_t CommandListener::getCommandCount() const {
    if (!initialized_) return 0;
    try {
        std::lock_guard<std::mutex> lock(queueMutex_);
        return commandQueue_.size();
    } catch (...) {
        return 0;
    }
}

void CommandListener::clearCommands() {
    if (!initialized_) return;
    try {
        std::lock_guard<std::mutex> lock(queueMutex_);
        // Clear the queue by swapping with an empty queue
        std::queue<std::string> emptyQueue;
        std::swap(commandQueue_, emptyQueue);
    } catch (...) {
        // Ignore exceptions during cleanup
    }
}
