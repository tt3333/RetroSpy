//
// TG16.cpp
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

#include "TG16.h"

void TG16Spy::loop() {
	noInterrupts();
	updateState();
	interrupts();
#if !defined(DEBUG)
	writeSerial();
#else
	debugSerial();
#endif
	delay(1);
}

void TG16Spy::updateState() {
	word temp = 0;
	currentState = 0;

	while ((PIND & 0b01000000) == 0) {}
	asm volatile("nop\nnop\n");
	temp = ((PIND & 0b00111100) >> 2);
	if ((temp & 0b00001111) == 0b00000000) {
		currentState |= lastDirections;
		highButtons = false;
		seenHighButtons = true;
	}
	else {
		currentState |= temp;
		lastDirections = temp;
		highButtons = true;
	}

	while ((PIND & 0b01000000) != 0) {}
	asm volatile(MICROSECOND_NOPS);
	temp = ((PIND & 0b00111100) << 2);
	if (highButtons == true && seenHighButtons == true) {
		currentState |= (temp << 4);
		lastHighButtons = temp;
		currentState |= lastButtons;
	}
	else {
		currentState |= temp;
		lastButtons = temp;
		if (seenHighButtons) {
			currentState |= (lastHighButtons << 4);
		}
		else {
			currentState |= 0b0000111100000000;
		}
	}

	currentState = ~currentState;
}

void TG16Spy::writeSerial() {
	for (unsigned char i = 0; i < 12; ++i) {
		Serial.write(currentState & (1 << i) ? ONE : ZERO);
	}
	Serial.write(SPLIT);
}

void TG16Spy::debugSerial() {
	Serial.print((currentState & 0b0000000000000001) ? "U" : "0");
	Serial.print((currentState & 0b0000000000000010) ? "R" : "0");
	Serial.print((currentState & 0b0000000000000100) ? "D" : "0");
	Serial.print((currentState & 0b0000000000001000) ? "L" : "0");
	Serial.print((currentState & 0b0000000000010000) ? "A" : "0");
	Serial.print((currentState & 0b0000000000100000) ? "B" : "0");
	Serial.print((currentState & 0b0000000001000000) ? "S" : "0");
	Serial.print((currentState & 0b0000000010000000) ? "R" : "0");
	Serial.print((currentState & 0b0000000100000000) ? "3" : "0");
	Serial.print((currentState & 0b0000001000000000) ? "4" : "0");
	Serial.print((currentState & 0b0000010000000000) ? "5" : "0");
	Serial.print((currentState & 0b0000100000000000) ? "6" : "0");
	Serial.print("\n");
}
