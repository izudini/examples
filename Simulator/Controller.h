#ifndef CONTROLLER_H
#define CONTROLLER_H

#include <thread>
#include <atomic>

class Controller {
public:
    Controller();
    ~Controller();

    // Start the controller thread
    void start();

    // Stop the controller thread
    void stop();

    // Check if controller is running
    bool isRunning() const;

private:
    // Thread function that runs the controller logic
    void threadFunction();

    std::atomic<bool> running_;
    std::thread controllerThread_;
};

#endif // CONTROLLER_H
