#include "Wii.h"


#if defined(__arm__) && defined(CORE_TEENSY)
void WiiSpy::loop() {

}

void WiiSpy::writeSerial() {

}

void WiiSpy::debugSerial() {

}

void WiiSpy::updateState() {

}
#else
void WiiSpy::loop() {
#ifdef MODE_WII
#error "MODE_WII not supported on Arduino"
#endif
}

void WiiSpy::writeSerial() {

}

void WiiSpy::debugSerial() {

}

void WiiSpy::updateState() {

}
#endif
