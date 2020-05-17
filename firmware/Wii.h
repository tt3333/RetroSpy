#ifndef WiiSpy_h
#define WiiSpy_h

#include "ControllerSpy.h"

class WiiSpy : public ControllerSpy {
    public:
        void setup();
        void loop();
        void writeSerial();
        void debugSerial();
        void updateState();

    private:
        uint8_t   current_portb = 0;
        uint8_t   last_portb;
        int       i2c_index  = 0;
        bool      isControllerPoll = false;
        bool      isControllerID = false;
        bool      isEncrypted = false;
        byte      encryptionKeySet = 0;
        bool      isKeyThing = false;
        byte      keyThing[8];
        byte      cleanData[274];
        byte      rawData[16000];
};

#endif
