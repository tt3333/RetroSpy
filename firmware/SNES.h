#ifndef SNESSpy_h
#define SNESSpy_h

// ---------------------------------------------------------------------------------
// The only reason you'd want to use 2-wire mode is if you built a NintendoSpy
// before the 3-wire firmware was implemented.  This mode is for backwards
// compatibility only.
//#define MODE_2WIRE_SNES
// ---------------------------------------------------------------------------------

#include "ControllerSpy.h"

class SNESSpy : public ControllerSpy {
public:
	void loop();
	void writeSerial();
	void debugSerial();
	void updateState();

private:
	unsigned char rawData[SNES_BITCOUNT_EXT];
	unsigned char bytesToReturn = SNES_BITCOUNT;
};

#endif
