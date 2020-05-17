#ifndef AmigaCd32_h
#define AmigaCd32_h

#include "ControllerSpy.h"

class AmigaCd32Spy : public ControllerSpy {
    public:
        void loop();
        void writeSerial();
        void debugSerial();
        void updateState();

    private:
        unsigned char rawData[NES_BITCOUNT * 3];
};

#endif
