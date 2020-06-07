//
// AmigaCd32.cpp
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

#include "AmigaCd32.h"

#if defined(__arm__) && defined(CORE_TEENSY)
void AmigaCd32Spy::setup() {
	// GPIOD_PDIR & 0xFF;
	pinMode(2, INPUT_PULLUP);
	pinMode(14, INPUT_PULLUP);
	pinMode(7, INPUT_PULLUP);
	pinMode(8, INPUT_PULLUP);
	pinMode(6, INPUT_PULLUP);
	pinMode(20, INPUT);
	pinMode(21, INPUT_PULLUP);
	pinMode(5, INPUT);

	// GPIOB_PDIR & 0xF;
	pinMode(16, INPUT_PULLUP);
	pinMode(17, INPUT_PULLUP);
	pinMode(19, INPUT_PULLUP);
	pinMode(18, INPUT_PULLUP);
}

void AmigaCd32Spy::loop() {
	noInterrupts();
	updateState();
	interrupts();
#if !defined(DEBUG)
	writeSerial();
#else
	debugSerial();
#endif
	delay(5);
}

void AmigaCd32Spy::writeSerial() {
	for (unsigned char i = 0; i < 9; i++) {
		Serial.write((rawData[i] & 0b11111101));
	}
	Serial.write(SPLIT);
}

void AmigaCd32Spy::debugSerial() {
	for (unsigned char i = 1; i < 8; i++)
	{
		Serial.print((rawData[i] & 0b10000000) == 0 ? 0 : 1);
	}
	Serial.print((rawData[8] & 0b00000001) == 0 ? 0 : 1);
	Serial.print((rawData[0] & 0b00000100) == 0 ? 0 : 1);
	Serial.print((rawData[0] & 0b00001000) == 0 ? 0 : 1);
	Serial.print((rawData[0] & 0b00010000) == 0 ? 0 : 1);
	Serial.print("\n");
}

void AmigaCd32Spy::updateState() {
	WAIT_FALLING_EDGE(CD32_LATCH);
	rawData[1] = (GPIOD_PDIR & 0xFF);

	asm volatile(MICROSECOND_NOPS MICROSECOND_NOPS);

	for (int i = 2; i < 8; ++i)
	{
		WAIT_FALLING_EDGE(CD32_CLOCK);
		rawData[i] = (GPIOD_PDIR & 0xFF);
	}

	rawData[0] = (GPIOD_PDIR & 0xFF);
	rawData[8] = (GPIOB_PDIR & 0xFF);
}
#else
void AmigaCd32Spy::setup() {
}

void AmigaCd32Spy::loop() {
}

void AmigaCd32Spy::writeSerial() {
}

void AmigaCd32Spy::debugSerial() {
}

void AmigaCd32Spy::updateState() {
}
#endif
