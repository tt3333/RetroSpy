#ifndef IntellivisionSpy_h
#define IntellivisionSpy_h

#include "ControllerSpy.h"

// The raw output of an Intellivision controller when
// multiple buttons is pressed is very strange and
// not likely what you want to see, but set this to false
// if you do want the strange behavior
#define INT_SANE_BEHAVIOR true

class IntellivisionSpy : public ControllerSpy {
    public:
        void loop();
        void writeSerial();
        void debugSerial();
        void updateState();

    private:
        byte intRawData;
        unsigned char rawData[32];

	  static const unsigned char buttonMasks[32];
};

#endif
