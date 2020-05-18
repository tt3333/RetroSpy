/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RetroSpy Firmware for Arduino
// v4.0
// RetroSpy written by zoggins
// NintendoSpy originally written by jaburns

// ---------- Uncomment one of these options to select operation mode --------------
// 
//#define MODE_GC
//#define MODE_N64
//#define MODE_SNES
//#define MODE_NES
//#define MODE_GENESIS
//#define MODE_SMS_ON_GENESIS // For using a genesis retrospy cable and the genesis reader in the exe while playing SMS games.
//#define MODE_GENESIS_MOUSE

//-- Arduino Only
//#define MODE_SMS
//#define MODE_BOOSTER_GRIP
//#define MODE_PLAYSTATION
//#define MODE_TG16
//#define MODE_SATURN
//#define MODE_SATURN3D
//#define MODE_NEOGEO
//#define MODE_ThreeDO
//#define MODE_INTELLIVISION
//#define MODE_JAGUAR
//#define MODE_FMTOWNS
//#define MODE_PCFX
//#define MODE_COLECOVISION

//--- Teensy Only
//#define MODE_DREAMCAST
//#define MODE_WII
//#define MODE_CD32

//Bridge one of the analog GND to the right analog IN to enable your selected mode
//#define MODE_DETECT

// ---------------------------------------------------------------------------------
// The only reason you'd want to use 2-wire mode is if you built a NintendoSpy
// before the 3-wire firmware was implemented.  This mode is for backwards
// compatibility only.
//#define MODE_2WIRE_SNES
//#define MODE_2WIRE_NES
// ---------------------------------------------------------------------------------

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

#if defined(MODE_NES)
NESSpy NESSpy;
#elif defined(MODE_SNES)
SNESSpy SNESSpy;
#elif defined(MODE_N64)
N64Spy N64Spy;
#elif defined(MODE_GC)
GCSpy GCSpy;
#elif defined(MODE_BOOSTER_GRIP)
BoosterGripSpy BoosterGripSpy;
#elif defined(MODE_GENESIS)
GenesisSpy GenesisSpy;
#elif defined(MODE_GENESIS_MOUSE)
GenesisMouseSpy GenesisMouseSpy;
#elif defined(MODE_SMS)
SMSSpy SMSSpy;
#elif defined(MODE_SMS_ON_GENESIS)
SMSSpy SMSOnGenesisSpy;
#elif defined(MODE_SATURN)
SaturnSpy SaturnSpy;
#elif defined(MODE_SATURN3D)
Saturn3DSpy Saturn3DSpy;
#elif defined(MODE_COLECOVISION)
ColecoVisionSpy ColecoVisionSpy;
#elif defined(MODE_FMTOWNS)
FMTownsSpy FMTownsSpy;
#elif defined(MODE_INTELLIVISION)
IntellivisionSpy IntelliVisionSpy;
#elif defined(MODE_JAGUAR)
JaguarSpy JaguarSpy;
#elif defined(MODE_NEOGEO)
NeoGeoSpy NeoGeoSpy;
#elif defined(MODE_PCFX)
PCFXSpy PCFXSpy;
#elif defined(MODE_PLAYSTATION)
PlayStationSpy PlayStationSpy;
#elif defined(MODE_TG16)
TG16Spy TG16Spy;
#elif defined(MODE_ThreeDO)
ThreeDOSpy ThreeDOSpy;
#elif defined(MODE_DREAMCAST)
DreamcastSpy DCSpy;
#elif defined(MODE_WII)
WiiSpy WiiSpy;
#elif defined(MODE_CD32)
AmigaCd32Spy Cd32Spy;
#elif defined(MODE_DETECT)
NESSpy NESSpy;
SNESSpy SNESSpy;
N64Spy N64Spy;
GCSpy GCSpy;
DreamcastSpy DCSpy;
WiiSpy WiiSpy;
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

#if defined(MODE_NES)
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
#elif defined(MODE_ThreeDO)
    ThreeDOSpy.setup();
#elif defined(MODE_Dreamcast)
    DCSpy.setup();
#elif defined(MODE_Wii)
    WiiSpy.setup();
#elif defined(MODE_CD32)
    Cd32Spy.setup();    
#elif defined(MODE_DETECT)
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
#endif

  T_DELAY(5000);

  Serial.begin( 115200 );
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Arduino sketch main loop definition.
void loop()
{
#if defined(MODE_GC)
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
#elif defined(MODE_ThreeDO)
    ThreeDOSpy.loop();
#elif defined(MODE_DETECT)
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
#endif
}
