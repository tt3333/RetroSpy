#ifndef AmigaCd32_h
#define AmigaCd32_h

#include "ControllerSpy.h"

class AmigaCd32Spy : public ControllerSpy {
public:
	void setup();
	void loop();
	void writeSerial();
	void debugSerial();
	void updateState();

private:
	byte      rawData[16000];
};

#endif
