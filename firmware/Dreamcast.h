#ifndef DreamcastSpy_h
#define DreamcastSpy_h

#include "ControllerSpy.h"

class DreamcastSpy : public ControllerSpy {
    public:
        void loop();
        void writeSerial();
        void debugSerial();
        void updateState();

    private:
        unsigned char rawData[NES_BITCOUNT * 3];
};

#endif
