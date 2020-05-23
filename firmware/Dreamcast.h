#ifndef DreamcastSpy_h
#define DreamcastSpy_h

#include "ControllerSpy.h"

class DreamcastSpy : public ControllerSpy {
public:
	void setup();
	FASTRUN void loop();
	FASTRUN void writeSerial();
	FASTRUN void debugSerial();
	FASTRUN void updateState();

private:
	byte rawData[16000];
	byte* p;
	int byteCount;
};

#endif
