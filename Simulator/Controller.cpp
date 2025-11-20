#include "Controller.h"
#include "Globals.h"
#include <iostream>
#include <chrono>

Controller::Controller()
    : running_(false)
{
    std::cout << "[Controller] Constructor called" << std::endl;
}

Controller::~Controller() {
    std::cout << "[Controller] Destructor called" << std::endl;
    stop();
}

void Controller::start() {
    if (running_) {
        std::cout << "[Controller] Already running" << std::endl;
        return;
    }

    std::cout << "[Controller] Starting..." << std::endl;
    running_ = true;
    controllerThread_ = std::thread(&Controller::threadFunction, this);
    std::cout << "[Controller] Started successfully" << std::endl;
}

void Controller::stop() {
    if (!running_) {
        return;
    }

    std::cout << "[Controller] Stopping..." << std::endl;
    running_ = false;

    if (controllerThread_.joinable()) {
        controllerThread_.join();
    }

    std::cout << "[Controller] Stopped" << std::endl;
}

bool Controller::isRunning() const {
    return running_;
}

void Controller::threadFunction() {
    std::cout << "[Controller] Thread function started" << std::endl;

    while (running_) {
        // Attempt to dequeue a message from the Globals queue
        std::string message;
        if (Globals::popMessage(message)) {
            // Message was dequeued successfully, print it
            std::cout << "[Controller] Dequeued message: " << message << std::endl;
        } else {
            // Queue is empty, sleep for 50 milliseconds
            std::this_thread::sleep_for(std::chrono::milliseconds(50));
        }
    }

    std::cout << "[Controller] Thread function exited" << std::endl;
}
