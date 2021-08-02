//
// GCv2.cpp
//
// Author:
//       Christopher "Zoggins" Mallery <zoggins@retro-spy.com>
//
// Copyright (c) 2020 RetroSpy Technologies
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#include "GCv2.h"

static int show = 0;

#if defined(__arm__) && defined(CORE_TEENSY)

static u_int8_t dummyStickData[] = {
	ONE,
	ZERO,
	ZERO,
	ZERO,
	ZERO,
	ZERO,
	ZERO,
	ZERO,
	ONE,
	ZERO,
	ZERO,
	ZERO,
	ZERO,
	ZERO,
	ZERO,
	ZERO,
	ONE,
	ZERO,
	ZERO,
	ZERO,
	ZERO,
	ZERO,
	ZERO,
	ZERO,
	ONE,
	ZERO,
	ZERO,
	ZERO,
	ZERO,
	ZERO,
	ZERO,
	ZERO,
	ZERO,
	ZERO,
	ZERO,
	ZERO,
	ZERO,
	ZERO,
	ZERO,
	ZERO,
	ZERO,
	ZERO,
	ZERO,
	ZERO,
	ZERO,
	ZERO,
	ZERO,
	ZERO,
	SPLIT
};



void GCv2Spy::loop() 
{
	unsigned char *rawDataPtr = rawData;
	elapsedMicros betweenLowSignal = 0;
	short headerVal = 0;
	int headerBits = 8;
	
findcmdinit:
	interrupts();

	rawDataPtr = rawData;
	
	// Wait for the line to go high then low.
	WAIT_FALLING_EDGE(GC_PIN);
	if (betweenLowSignal < 25)
	{
		betweenLowSignal = 0;
		goto findcmdinit;
	}
	else
	{
		headerBits = 7;
		betweenLowSignal = 0;
		
		noInterrupts();
		// Wait ~2us between line reads
		asm volatile(MICROSECOND_NOPS MICROSECOND_NOPS);

		// Read a bit from the line and store as a byte in "rawData"
		*rawDataPtr = PIN_READ(GC_PIN);
		headerVal = (*rawDataPtr != 0 ? 0x80 : 0x00);
		++rawDataPtr;

		goto readCmd;
	}
	
	goto findcmdinit;
	
readCmd:
	
	// Wait for the line to go high then low.
	WAIT_FALLING_EDGE(GC_PIN);

	// Wait ~2us between line reads
	asm volatile(MICROSECOND_NOPS MICROSECOND_NOPS);

	// Read a bit from the line and store as a byte in "rawData"
	*rawDataPtr = PIN_READ(GC_PIN);
	
	headerVal |= *rawDataPtr != 0 ? (1 << (headerBits - 1)) : 0;

	++rawDataPtr;
	if (--headerBits == 0)
	{
		if (headerVal == 0x40)
		{
			readBits = 82;
			goto readData;
		}
		if (headerVal == 0x54)
		{
			readBits = 82;
			goto readData;
		}
		if (headerVal == 0x14)
		{
			readBits = 25;
			goto readData;
		}
		else
		{
#if defined(DEBUG)
			//Serial.println(headerVal);
#endif
			interrupts();
			betweenLowSignal = 0;
			goto findcmdinit;
		}
	}
	goto readCmd;
	
readData:
	
	// Wait for the line to go high then low.
	WAIT_FALLING_EDGE(GC_PIN);
	
	// Wait ~2us between line reads
	asm volatile(MICROSECOND_NOPS MICROSECOND_NOPS);

	// Read a bit from the line and store as a byte in "rawData"
	*rawDataPtr = PIN_READ(GC_PIN);
	
	rawDataPtr++;
	
	if (--readBits == 0)
	{
		goto printData;
	}
	
	goto readData;
	
printData:
	interrupts();
#if !defined(DEBUG)
	if (headerVal == 0x40)
		sendRawData(rawData, GC_PREFIX, GC_BITCOUNT);
	else if (headerVal == 0x14 && ++show % 2 == 0)  // Gameboy Player polls too damn many times, slows down display.
		writeSerial(); 								// This doesn't seem to negatively affect other games.
	else if(headerVal == 0x54)
		writeKeyboard();
#else
	if (headerVal == 0x54)
		debugKeyboard();
	else if (headerVal == 0x14)
		debugSerial();
	else
		sendRawDataDebug(rawData, 25, 65);
#endif
	betweenLowSignal = 0;
	goto findcmdinit;
}

void GCv2Spy::updateState() {

}

void GCv2Spy::writeSerial() {
	Serial.write(ZERO);
	Serial.write(ZERO);
	Serial.write(ZERO);
	Serial.write(rawData[21] ? ONE : ZERO);
	Serial.write(ZERO);
	Serial.write(ZERO);
	Serial.write(rawData[23] ? ONE : ZERO);
	Serial.write(rawData[24] ? ONE : ZERO);
	Serial.write(ZERO);
	Serial.write(rawData[31] ? ONE : ZERO);
	Serial.write(rawData[32] ? ONE : ZERO);
	Serial.write(rawData[22] ? ONE : ZERO);
	Serial.write(rawData[18] ? ONE : ZERO);
	Serial.write(rawData[17] ? ONE : ZERO);
	Serial.write(rawData[20] ? ONE : ZERO);
	Serial.write(rawData[19] ? ONE : ZERO);
	Serial.write(dummyStickData, 49);
}

void GCv2Spy::debugSerial() {
	Serial.print("0");
	Serial.print("0");
	Serial.print("0");
	Serial.print(rawData[21] ? "t" : "0");
	Serial.print("0");
	Serial.print("0");
	Serial.print(rawData[23] ? "b" : "0");
	Serial.print(rawData[24] ? "a" : "0");
	Serial.print("0");
	Serial.print(rawData[31] ? "L" : "0");
	Serial.print(rawData[32] ? "R" : "0");
	Serial.print(rawData[22] ? "s" : "0");
	Serial.print(rawData[18] ? "u" : "0");
	Serial.print(rawData[17] ? "d" : "0");
	Serial.print(rawData[20] ? "l" : "0");
	Serial.print(rawData[19] ? "r" : "0");
	Serial.print(128);
	Serial.print(128);
	Serial.print(128);
	Serial.print(128);
	Serial.print(0);
	Serial.print(0);
	Serial.print("\n");
}


void GCv2Spy::debugKeyboard()
{
	byte vals[3];
	for (int j = 0; j < 3; ++j)
	{
		vals[j] = 0;
		for (int i = 0; i < 8; ++i)
		{
			if (rawData[GC_PREFIX + i + 32 + (j * 8)] != 0)
			{
				vals[j] |= (byte)(1 << (7 - i));
			}
		}
	}
	Serial.print("|");
	Serial.print(vals[0], HEX);
	Serial.print("|");
	Serial.print(vals[1], HEX);
	Serial.print("|");
	Serial.println(vals[2], HEX);
}

void GCv2Spy::writeKeyboard()
{
	byte vals[3];
	for (int j = 0; j < 3; ++j)
	{
		vals[j] = 0;
		for (int i = 0; i < 8; ++i)
		{
			if (rawData[GC_PREFIX + i + 32 + (j * 8)] != 0)
			{
				vals[j] |= (byte)(1 << (7 - i));
			}
		}
		if (vals[j] == 10)
			vals[j] = 11;
	}
	
	Serial.write(vals[0]);
	Serial.write(vals[1]);
	Serial.write(vals[2]);
	Serial.write(SPLIT);
}

#else
void GCv2Spy::loop() {
}

void GCv2Spy::writeSerial() {
}

void GCv2Spy::debugSerial() {
}

void GCv2Spy::updateState() {
}

void GCv2Spy::sendRawGBAData() {
}

void GCv2Spy::debugKeyboard() {
}

void GCv2Spy::writeKeyboard() {
}

#endif