/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RetroSpy Firmware for Arduino Uno & Teensy 3.5
// v3.8
// RetroSpy written by zoggins of RetroSpy Technologies
// NintendoSpy originally written by jaburns

// ---------- Uncomment one of these options to select operation mode --------------
// 
//#define MODE_NES
//#define MODE_SNES
//#define MODE_N64
//#define MODE_GC
//#define MODE_SMS
//#define MODE_GENESIS
//#define MODE_SMS_ON_GENESIS // For using a genesis retrospy cable and the genesis reader in the exe while playing SMS games.
//#define MODE_GENESIS_MOUSE
//#define MODE_SATURN
//#define MODE_SATURN3D

//-- Arduino Only
//#define MODE_BOOSTER_GRIP
//#define MODE_PLAYSTATION
//#define MODE_TG16
//#define MODE_NEOGEO
//#define MODE_3DO
//#define MODE_INTELLIVISION
//#define MODE_JAGUAR
//#define MODE_FMTOWNS
//#define MODE_PCFX
//#define MODE_COLECOVISION
//#define MODE_DRIVING_CONTROLLER
//#define MODE_PIPPIN

//--- Teensy Only
//#define MODE_DREAMCAST
//#define MODE_WII
//#define MODE_CD32

//Bridge one of the analog GND to the right analog IN to enable your selected mode
//#define MODE_DETECT

// Pippin Controller Configuration
#define PIPPIN_SPY_ADDRESS 0xF

///////////////////////////////////////////////////////////////////////////////
// ---------- NOTHING BELOW THIS LINE SHOULD BE MODIFIED  -------------------//
///////////////////////////////////////////////////////////////////////////////


#include "common.h"

#include "NES.h"
#include "SNES.h"
#include "N64.h"
#include "GC.h"

#include "BoosterGrip.h"
#include "Genesis.h"
#include "GenesisMouse.h"
#include "SMS.h"
#include "Saturn.h"
#include "Saturn3D.h"

#include "ColecoVision.h"
#include "FMTowns.h"
#include "Intellivision.h"
#include "Jaguar.h"
#include "NeoGeo.h"
#include "PCFX.h"
#include "PlayStation.h"
#include "TG16.h"
#include "ThreeDO.h"

#include "Dreamcast.h"
#include "AmigaCD32.h"
#include "Wii.h"

#include "DrivingController.h"
#include "Pippin.h"

#if defined(MODE_NES)
NESSpy NESSpy;
#endif
#if defined(MODE_SNES)
SNESSpy SNESSpy;
#endif
#if defined(MODE_N64)
N64Spy N64Spy;
#endif
#if defined(MODE_GC)
GCSpy GCSpy;
#endif
#if defined(MODE_BOOSTER_GRIP)
BoosterGripSpy BoosterGripSpy;
#endif
#if defined(MODE_GENESIS)
GenesisSpy GenesisSpy;
#endif
#if defined(MODE_GENESIS_MOUSE)
GenesisMouseSpy GenesisMouseSpy;
#endif
#if defined(MODE_SMS)
SMSSpy SMSSpy;
#endif
#if defined(MODE_SMS_ON_GENESIS)
SMSSpy SMSOnGenesisSpy;
#endif
#if defined(MODE_SATURN)
SaturnSpy SaturnSpy;
#endif
#if defined(MODE_SATURN3D)
Saturn3DSpy Saturn3DSpy;
#endif
#if defined(MODE_COLECOVISION)
ColecoVisionSpy ColecoVisionSpy;
#endif
#if defined(MODE_FMTOWNS)
FMTownsSpy FMTownsSpy;
#endif
#if defined(MODE_INTELLIVISION)
IntellivisionSpy IntelliVisionSpy;
#endif
#if defined(MODE_JAGUAR)
JaguarSpy JaguarSpy;
#endif
#if defined(MODE_NEOGEO)
NeoGeoSpy NeoGeoSpy;
#endif
#if defined(MODE_PCFX)
PCFXSpy PCFXSpy;
#endif
#if  defined(MODE_PLAYSTATION)
PlayStationSpy PlayStationSpy;
#endif
#if defined(MODE_TG16)
TG16Spy TG16Spy;
#endif
#if defined(MODE_3DO)
ThreeDOSpy ThreeDOSpy;
#endif
#if defined(MODE_DREAMCAST)
DreamcastSpy DCSpy;
#endif
#if defined(MODE_WII)
WiiSpy WiiSpy;
#endif
#if defined(MODE_CD32)
AmigaCd32Spy Cd32Spy;
#endif
#if defined(MODE_DRIVING_CONTROLLER)
DrivingControllerSpy DrivingControllerSpy;
#endif
#if defined(MODE_PIPPIN)
PippinSpy PippinSpy;
#endif

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// General initialization, just sets all pins to input and starts serial communication.
void setup()
{

  // for MODE_DETECT
#if defined(__arm__) && defined(CORE_TEENSY)
  for(int i = 33; i < 40; ++i)
    pinMode(i, INPUT_PULLUP);
#else
    PORTC = 0xFF; // Set the pull-ups on the port we use to check operation mode.
    DDRC  = 0x00;
#endif

#if defined(MODE_DETECT)
    if ( !PINC_READ(MODEPIN_SNES)) {
        SNESSpy.setup();
    } else if ( !PINC_READ(MODEPIN_N64))  {
        N64Spy.setup();
    } else if ( !PINC_READ(MODEPIN_GC)) {
        GCSpy.setup();
    }
#if defined(__arm__) && defined(CORE_TEENSY)
  else if( !PINC_READ( MODEPIN_DREAMCAST ) ) {
       DCSpy.setup();
    } else if( !PINC_READ( MODEPIN_WII ) ) {
        WiiSpy.setup()();
    }
#endif 
    else {
        NESSpy.setup();
    }
#elif defined(MODE_NES)
    NESSpy.setup();
#elif defined(MODE_SNES)
    SNESSpy.setup();
#elif defined(MODE_N64)
    N64Spy.setup();
#elif defined(MODE_GC)
    GCSpy.setup();
#elif defined(MODE_BOOSTER_GRIP)
    BoosterGripSpy.setup();
#elif defined(MODE_GENESIS)
    GenesisSpy.setup();
#elif defined(MODE_GENESIS_MOUSE)
    GenesisMouseSpy.setup();
#elif defined(MODE_SMS)
    SMSSpy.setup();
#elif defined(MODE_SMS_ON_GENESIS)
    SMSOnGenesisSpy.setup(SMSSpy::OUTPUT_GENESIS);
#elif defined(MODE_SATURN)
    SaturnSpy.setup();
#elif defined(MODE_SATURN3D)
    Saturn3DSpy.setup();
#elif defined(MODE_COLECOVISION)
    ColecoVisionSpy.setup();
#elif defined(MODE_FMTOWNS)
    FMTownsSpy.setup();
#elif defined(MODE_INTELLIVISION)
    IntelliVisionSpy.setup();
#elif defined(MODE_JAGUAR)
    JaguarSpy.setup();
#elif defined(MODE_NEOGEO)
    NeoGeoSpy.setup();
#elif defined(MODE_PCFX)
    PCFXSpy.setup();
#elif defined(MODE_PLAYSTATION)
    PlayStationSpy.setup();
#elif defined(MODE_TG16)
    TG16Spy.setup();
#elif defined(MODE_3DO)
    ThreeDOSpy.setup();
#elif defined(MODE_DREAMCAST)
    DCSpy.setup();
#elif defined(MODE_WII)
    WiiSpy.setup();
#elif defined(MODE_CD32)
    Cd32Spy.setup();    
#elif defined(MODE_DRIVING_CONTROLLER)
	DrivingControllerSpy.setup();
#elif defined(MODE_PIPPIN)
	PippinSpy.setup(PIPPIN_SPY_ADDRESS);
#endif

  T_DELAY(5000);
  A_DELAY(200);

  Serial.begin( 115200 );
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Arduino sketch main loop definition.
void loop()
{
#if defined(MODE_DETECT)
    if( !PINC_READ( MODEPIN_SNES ) ) {
        SNESSpy.loop();
    } else if( !PINC_READ( MODEPIN_N64 ) ) {
        N64Spy.loop();
    } else if( !PINC_READ( MODEPIN_GC ) ) {
        GCSpy.loop();
    } 
#if defined(__arm__) && defined(CORE_TEENSY)
  else if( !PINC_READ( MODEPIN_DREAMCAST ) ) {
        DreamcastSpy.loop();
    } else if( !PINC_READ( MODEPIN_WII ) ) {
        WiiSpy.loop();
    }
#endif
  else {
        NESSpy.loop();
    }
#elif defined(MODE_GC)
    GCSpy.loop();
#elif defined(MODE_N64)
    N64Spy.loop();
#elif defined(MODE_SNES)
    SNESSpy.loop();
#elif defined(MODE_NES)
    NESSpy.loop();
#elif defined(MODE_BOOSTER_GRIP)
    BoosterGripSpy.loop();
#elif defined(MODE_GENESIS)
    GenesisSpy.loop();
#elif defined(MODE_GENESIS_MOUSE)
    GenesisMouseSpy.loop();
#elif defined(MODE_SMS)
    SMSSpy.loop();
#elif defined(MODE_SMS_ON_GENESIS)
    SMSOnGenesisSpy.loop();
#elif defined(MODE_SATURN)
    SaturnSpy.loop();
#elif defined(MODE_SATURN3D)
    Saturn3DSpy.loop();
#elif defined(MODE_COLECOVISION)
    ColecoVisionSpy.loop();
#elif defined(MODE_FMTOWNS)
    FMTownsSpy.loop();
#elif defined(MODE_INTELLIVISION)
    IntelliVisionSpy.loop();
#elif defined(MODE_JAGUAR)
    JaguarSpy.loop();
#elif defined(MODE_NEOGEO)
    NeoGeoSpy.loop();
#elif defined(MODE_PCFX)
    PCFXSpy.loop();
#elif defined(MODE_PlayStation)
    PlayStationSpy.loop();
#elif defined(MODE_TG16)
    TG16Spy.loop();
#elif defined(MODE_3DO)
    ThreeDOSpy.loop();
#elif defined(MODE_DREAMCAST)
    DCSpy.loop();
#elif defined(MODE_WII)
    WiiSpy.loop();
#elif defined(MODE_CD32)
   Cd32Spy.loop();
#elif defined(MODE_DRIVING_CONTROLLER)
   DrivingControllerSpy.loop();
#elif defined(MODE_PIPPIN)
	PippinSpy.loop();
#endif
}
