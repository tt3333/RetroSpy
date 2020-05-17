#ifndef WiiSpy_h
#define WiiSpy_h

#include "ControllerSpy.h"

class WiiSpy : public ControllerSpy {
    public:
        void loop();
        void writeSerial();
        void debugSerial();
        void updateState();

    private:
        unsigned char rawData[NES_BITCOUNT * 3];
};

#endif
