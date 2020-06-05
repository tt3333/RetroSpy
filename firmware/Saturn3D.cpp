//
// Saturn3D.cpp
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

#include "Saturn3D.h"

void Saturn3DSpy::loop() {
	noInterrupts();
	updateState();
	interrupts();
#if !defined(DEBUG)
	writeSerial();
#else
	debugSerial();
#endif

	T_DELAY(5);
}

void Saturn3DSpy::updateState() {
	byte numBits = 0;

	WAIT_FALLING_EDGE(SS_SEL);

	for (int i = 0; i < 3; ++i) {
		WAIT_FALLING_EDGE(SS_REQ);
		WAIT_FALLING_EDGEB(SS_ACK);

		rawData[numBits++] = PIN_READ(SS_DATA3);
		rawData[numBits++] = PIN_READ(SS_DATA2);
		rawData[numBits++] = PIN_READ(SS_DATA1);
		rawData[numBits++] = PIN_READ(SS_DATA0);

		WAIT_LEADING_EDGE(SS_REQ);
		WAIT_LEADING_EDGEB(SS_ACK);

		rawData[numBits++] = PIN_READ(SS_DATA3);
		rawData[numBits++] = PIN_READ(SS_DATA2);
		rawData[numBits++] = PIN_READ(SS_DATA1);
		rawData[numBits++] = PIN_READ(SS_DATA0);
	}

	if (rawData[3] != 0)
	{
		int numBytes = 4;

		if (rawData[2] != 0) {
			numBytes = 1;
		}

		for (int i = 0; i < numBytes; ++i)
		{
			WAIT_FALLING_EDGE(SS_REQ);
			WAIT_FALLING_EDGEB(SS_ACK);

			rawData[numBits++] = PIN_READ(SS_DATA3);
			rawData[numBits++] = PIN_READ(SS_DATA2);
			rawData[numBits++] = PIN_READ(SS_DATA1);
			rawData[numBits++] = PIN_READ(SS_DATA0);

			WAIT_LEADING_EDGE(SS_REQ);
			WAIT_LEADING_EDGEB(SS_ACK);

			rawData[numBits++] = PIN_READ(SS_DATA3);
			rawData[numBits++] = PIN_READ(SS_DATA2);
			rawData[numBits++] = PIN_READ(SS_DATA1);
			rawData[numBits++] = PIN_READ(SS_DATA0);
		}
	}
	else {
		rawData[numBits++] = 1;
		for (int i = 0; i < 7; ++i) {
			rawData[numBits++] = 0;
		}

		rawData[numBits++] = 1;
		for (int i = 0; i < 7; ++i) {
			rawData[numBits++] = 0;
		}

		for (int i = 0; i < 16; ++i) {
			rawData[numBits++] = 0;
		}
	}
}

void Saturn3DSpy::writeSerial() {
	for (unsigned char i = 0; i < 56; ++i)
	{
		Serial.write(rawData[i] ? ONE : ZERO);
	}
	Serial.write(SPLIT);
}

void Saturn3DSpy::debugSerial() {
	for (int i = 0; i < 56; ++i)
	{
		if (i % 8 == 0) {
			Serial.print("|");
		}
		Serial.print(rawData[i] ? "1" : "0");
	}
	Serial.print("\n");
}
