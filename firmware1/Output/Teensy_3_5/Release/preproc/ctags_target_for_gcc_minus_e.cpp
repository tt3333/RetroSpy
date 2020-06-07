# 1 "E:\\src\\Repos\\RetroSpy\\firmware\\sketches\\firmware.ino"
# 1 "E:\\src\\Repos\\RetroSpy\\firmware\\sketches\\firmware.ino"
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
//#define MODE_AMIGA_KEYBOARD
//#define MODE_AMIGA_MOUSE

//--- Teensy Only
//#define MODE_DREAMCAST
//#define MODE_WII
//#define MODE_CD32
//#define MODE_FMTOWNS_KEYBOARD_AND_MOUSE

//Bridge one of the analog GND to the right analog IN to enable your selected mode
//#define MODE_DETECT

//--- Require 3rd Party Libraries.  Setup is slightly more complicated
//#define MODE_CDI
//#define MODE_CDTV_WIRED
//#define MODE_CDTV_WIRELESS
//#define MODE_COLECOVISION
//#define MODE_DRIVING_CONTROLLER
//#define MODE_PIPPIN
//#define MODE_KEYBOARD_CONTROLLER
//#define MODE_KEYBOARD_CONTROLLER_STAR_RAIDERS
//#define MODE_KEYBOARD_CONTROLLER_BIG_BIRD

//--- Require 2 Arduinos.  Setup is A LOT more complicated.
//#define MODE_AMIGA_ANALOG_1
//#define MODE_AMIGA_ANALOG_2
//#define MODE_ATARI5200_1
//#define MODE_ATARI5200_2

// Pippin Controller Configuration


// Amiga Mouse Video Output Mode


///////////////////////////////////////////////////////////////////////////////
// ---------- NOTHING BELOW THIS LINE SHOULD BE MODIFIED  -------------------//
///////////////////////////////////////////////////////////////////////////////

# 70 "E:\\src\\Repos\\RetroSpy\\firmware\\sketches\\firmware.ino" 2

# 72 "E:\\src\\Repos\\RetroSpy\\firmware\\sketches\\firmware.ino" 2
# 73 "E:\\src\\Repos\\RetroSpy\\firmware\\sketches\\firmware.ino" 2
# 74 "E:\\src\\Repos\\RetroSpy\\firmware\\sketches\\firmware.ino" 2
# 75 "E:\\src\\Repos\\RetroSpy\\firmware\\sketches\\firmware.ino" 2

# 77 "E:\\src\\Repos\\RetroSpy\\firmware\\sketches\\firmware.ino" 2
# 78 "E:\\src\\Repos\\RetroSpy\\firmware\\sketches\\firmware.ino" 2
# 79 "E:\\src\\Repos\\RetroSpy\\firmware\\sketches\\firmware.ino" 2
# 80 "E:\\src\\Repos\\RetroSpy\\firmware\\sketches\\firmware.ino" 2
# 81 "E:\\src\\Repos\\RetroSpy\\firmware\\sketches\\firmware.ino" 2
# 82 "E:\\src\\Repos\\RetroSpy\\firmware\\sketches\\firmware.ino" 2

# 84 "E:\\src\\Repos\\RetroSpy\\firmware\\sketches\\firmware.ino" 2
# 85 "E:\\src\\Repos\\RetroSpy\\firmware\\sketches\\firmware.ino" 2
# 86 "E:\\src\\Repos\\RetroSpy\\firmware\\sketches\\firmware.ino" 2
# 87 "E:\\src\\Repos\\RetroSpy\\firmware\\sketches\\firmware.ino" 2
# 88 "E:\\src\\Repos\\RetroSpy\\firmware\\sketches\\firmware.ino" 2
# 89 "E:\\src\\Repos\\RetroSpy\\firmware\\sketches\\firmware.ino" 2
# 90 "E:\\src\\Repos\\RetroSpy\\firmware\\sketches\\firmware.ino" 2
# 91 "E:\\src\\Repos\\RetroSpy\\firmware\\sketches\\firmware.ino" 2
# 92 "E:\\src\\Repos\\RetroSpy\\firmware\\sketches\\firmware.ino" 2

# 94 "E:\\src\\Repos\\RetroSpy\\firmware\\sketches\\firmware.ino" 2
# 95 "E:\\src\\Repos\\RetroSpy\\firmware\\sketches\\firmware.ino" 2
# 96 "E:\\src\\Repos\\RetroSpy\\firmware\\sketches\\firmware.ino" 2
# 97 "E:\\src\\Repos\\RetroSpy\\firmware\\sketches\\firmware.ino" 2

# 99 "E:\\src\\Repos\\RetroSpy\\firmware\\sketches\\firmware.ino" 2
# 100 "E:\\src\\Repos\\RetroSpy\\firmware\\sketches\\firmware.ino" 2
# 101 "E:\\src\\Repos\\RetroSpy\\firmware\\sketches\\firmware.ino" 2
# 102 "E:\\src\\Repos\\RetroSpy\\firmware\\sketches\\firmware.ino" 2
# 103 "E:\\src\\Repos\\RetroSpy\\firmware\\sketches\\firmware.ino" 2
# 104 "E:\\src\\Repos\\RetroSpy\\firmware\\sketches\\firmware.ino" 2
# 105 "E:\\src\\Repos\\RetroSpy\\firmware\\sketches\\firmware.ino" 2
# 106 "E:\\src\\Repos\\RetroSpy\\firmware\\sketches\\firmware.ino" 2
# 107 "E:\\src\\Repos\\RetroSpy\\firmware\\sketches\\firmware.ino" 2
# 108 "E:\\src\\Repos\\RetroSpy\\firmware\\sketches\\firmware.ino" 2
# 212 "E:\\src\\Repos\\RetroSpy\\firmware\\sketches\\firmware.ino"
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// General initialization, just sets all pins to input and starts serial communication.
void setup()
{
  // for MODE_DETECT

  for(int i = 33; i < 40; ++i)
    pinMode(i, 2);
# 321 "E:\\src\\Repos\\RetroSpy\\firmware\\sketches\\firmware.ino"
  delay(5000);
  (200);

  Serial.begin( 115200 );
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Arduino sketch main loop definition.
void loop()
{
# 419 "E:\\src\\Repos\\RetroSpy\\firmware\\sketches\\firmware.ino"
}
