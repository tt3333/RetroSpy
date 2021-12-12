//
// Nuon.cpp
//
// Author:
//       Christopher "Zoggins" Mallery <zoggins@retro-spy.com>
//
// Copyright (c) 2021 RetroSpy Technologies
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

#include "Nuon.h"

#if defined(__arm__) && defined(CORE_TEENSY)

#include "elapsedMillis.h"

#define CLOCK_DELAY "nop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\n"


FASTRUN void NuonSpy::loop()
{
	updateState();

	if (((rawData[0] & 0b00000100) != 0 && (rawData[1] & 0b00000100) != 0 && (rawData[2] & 0b00000100) == 0 && (rawData[3] & 0b00000100) == 0 && (rawData[4] & 0b00000100) == 0 
			&& (rawData[5] & 0b00000100) == 0 && (rawData[6] & 0b00000100) != 0 && (rawData[7] & 0b00000100) == 0 
			&& (rawData[11] & 0b00000100) != 0 && (rawData[12] & 0b00000100) != 0 && (rawData[14] & 0b00000100) == 0))
	{
#if !defined(DEBUG)
		writeSerial();
#else
		debugSerial();
#endif
	}
}

FASTRUN void NuonSpy::writeSerial()
{
	rawData[55] = '\n';
	Serial.write(&rawData[39], 17);
}

FASTRUN void NuonSpy::debugSerial()
{
	for (int i = 0; i < 128; ++i)
	{
		if (i % 8 == 0)
			Serial.print("|"); 
        
		Serial.print((rawData[i] & 0b00000100) == 0 ? "0" : "1");
	}
	Serial.print("\n");	
}

FASTRUN void NuonSpy::updateState()
{
	elapsedMicros restPeriod;

start:
	while ((GPIOD_PDIR & 0b00001000)) ;
	do 
	{
		rawData[0] = (GPIOD_PDIR);
	} while (!(rawData[0] & 0b00001000));
	
	if ((rawData[0] & 0b00000100) == 0)
	{ 
		goto start;
	}
	else if (restPeriod < 40)
	{
		restPeriod = 0;
		goto start;
	}
	else
	{
		restPeriod = 0;
		noInterrupts();
		asm volatile(CLOCK_DELAY);
		rawData[1] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[2] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[3] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[4] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[5] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[6] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[7] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[8] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[9] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[10] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[11] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[12] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[13] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[14] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[15] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[16] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[17] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[18] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[19] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[20] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[21] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[22] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[23] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[24] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[25] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[26] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[27] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[28] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[29] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[30] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[31] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[32] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[33] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[34] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[35] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[36] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[37] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[38] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[39] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[40] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[41] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[42] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[43] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[44] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[45] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[46] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[47] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[48] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[49] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[50] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[51] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[52] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[53] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[54] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[55] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[56] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[57] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[58] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[59] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[60] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[61] = GPIOD_PDIR;
		asm volatile(CLOCK_DELAY);
		rawData[62] = GPIOD_PDIR;  
		asm volatile(CLOCK_DELAY);
		rawData[63] = GPIOD_PDIR;
		interrupts();
	}
}

#else
void NuonSpy::loop()
{
	
}

void NuonSpy::writeSerial()
{
	
}

void NuonSpy::debugSerial()
{
	
}

void NuonSpy::updateState()
{
	
}
#endif