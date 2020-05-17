#include "Dreamcast.h"


#if defined(__arm__) && defined(CORE_TEENSY)
void DreamcastSpy::loop() {



}

void DreamcastSpy::writeSerial() {

}

void DreamcastSpy::debugSerial() {

}

void DreamcastSpy::updateState() {

}
#else
void DreamcastSpy::loop() {
#ifdef MODE_DREAMCAST
#error "MODE_DREAMCAST not supported on Arduino"
#endif
}

void DreamcastSpy::writeSerial() {

}

void DreamcastSpy::debugSerial() {

}

void DreamcastSpy::updateState() {

}
#endif
