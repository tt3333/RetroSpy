//
// ThreeDO.cpp
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

#include "ThreeDO.h"

void ThreeDOSpy::loop() {
	noInterrupts();
	updateState();
	interrupts();
#if !defined(DEBUG)
	writeSerial();
#else
	debugSerial();
#endif
}

void ThreeDOSpy::updateState() {
	unsigned char *rawDataPtr = rawData;

	unsigned char bits = 0;
	WAIT_FALLING_EDGE(ThreeDO_LATCH);

	do {
		WAIT_LEADING_EDGE(ThreeDO_CLOCK);
		*rawDataPtr = PIN_READ(ThreeDO_DATA);

		if (bits == 0 && *rawDataPtr != 0)
			bytesToReturn = bits = 32;
		else if (bits == 0)
			bytesToReturn = bits = 16;

		++rawDataPtr;
	} while (--bits > 0);
}

void ThreeDOSpy::writeSerial() {
	sendRawData(rawData, 0, bytesToReturn);
}

void ThreeDOSpy::debugSerial() {
	sendRawDataDebug(rawData, 0, bytesToReturn);
}
