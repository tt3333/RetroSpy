#include "AmigaCd32.h"

#if defined(__arm__) && defined(CORE_TEENSY)
void AmigaCd32Spy::loop() {


}

void AmigaCd32Spy::writeSerial() {

}

void AmigaCd32Spy::debugSerial() {

}

void AmigaCd32Spy::updateState() {

}
#else
void AmigaCd32Spy::loop() {
#ifdef MODE_CD32
#error "MODE_CD32 not supported on Arduino"
#endif
}

void AmigaCd32Spy::writeSerial() {

}

void AmigaCd32Spy::debugSerial() {

}

void AmigaCd32Spy::updateState() {

}
#endif
