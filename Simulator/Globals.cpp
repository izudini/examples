#include "Globals.h"

// Initialize static members
std::queue<std::string> Globals::messageQueue_;
std::mutex Globals::queueMutex_;

void Globals::pushMessage(const std::string& message) {
    std::lock_guard<std::mutex> lock(queueMutex_);
    messageQueue_.push(message);
}

bool Globals::popMessage(std::string& message) {
    std::lock_guard<std::mutex> lock(queueMutex_);
    if (messageQueue_.empty()) {
        return false;
    }
    message = messageQueue_.front();
    messageQueue_.pop();
    return true;
}

bool Globals::hasMessages() {
    std::lock_guard<std::mutex> lock(queueMutex_);
    return !messageQueue_.empty();
}

size_t Globals::getMessageCount() {
    std::lock_guard<std::mutex> lock(queueMutex_);
    return messageQueue_.size();
}

void Globals::clearMessages() {
    std::lock_guard<std::mutex> lock(queueMutex_);
    // Clear the queue by swapping with an empty queue
    std::queue<std::string> emptyQueue;
    std::swap(messageQueue_, emptyQueue);
}
