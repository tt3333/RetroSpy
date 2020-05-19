#ifndef NESSpy_h
#define NESSpy_h

// ---------------------------------------------------------------------------------
// The only reason you'd want to use 2-wire mode is if you built a NintendoSpy
// before the 3-wire firmware was implemented.  This mode is for backwards
// compatibility only.
//#define MODE_2WIRE_SNES
//#define MODE_2WIRE_NES
// ---------------------------------------------------------------------------------

#include "ControllerSpy.h"

class NESSpy : public ControllerSpy {
    public:
        void loop();
        void writeSerial();
        void debugSerial();
        void updateState();

    private:
        unsigned char rawData[NES_BITCOUNT * 3];
};

#endif
