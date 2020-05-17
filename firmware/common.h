#include "Arduino.h"

#if defined(__arm__) && defined(CORE_TEENSY)
#include "config_teensy.h"
#else
#include "config_arduino.h"
#endif

#define N64_BITCOUNT		32
#define SNES_BITCOUNT       16
#define SNES_BITCOUNT_EXT   32
#define NES_BITCOUNT         8
#define GC_BITCOUNT			64
#define GC_PREFIX           25
#define ThreeDO_BITCOUNT	32
#define PCFX_BITCOUNT		16
#define CD32_BITCOUNT		 7

#define PIN_READ PIND_READ

#define WAIT_FALLING_EDGE( pin ) while( !PIN_READ(pin) ); while( PIN_READ(pin) );
#define WAIT_LEADING_EDGE( pin ) while( PIN_READ(pin) ); while( !PIN_READ(pin) );

#define WAIT_FALLING_EDGEB( pin ) while( !PINB_READ(pin) ); while( PINB_READ(pin) );
#define WAIT_LEADING_EDGEB( pin ) while( PINB_READ(pin) ); while( !PINB_READ(pin) );

#define ZERO  ((uint8_t)0)  // Use a byte value of 0x00 to represent a bit with value 0.
#define ONE   '1'  // Use an ASCII one to represent a bit with value 1.  This makes Arduino debugging easier.
#define SPLIT '\n'  // Use a new-line character to split up the controller state packets.

void common_pin_setup();
void read_shiftRegister_2wire(unsigned char rawData[], unsigned char latch, unsigned char data, unsigned char longWait, unsigned char bits);
void sendRawData(unsigned char rawControllerData[], unsigned char first, unsigned char count);
void sendRawDataDebug(unsigned char rawControllerData[], unsigned char first, unsigned char count);
