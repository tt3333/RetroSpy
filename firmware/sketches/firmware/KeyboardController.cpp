//
// KeyboardController.cpp
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

#include "KeyboardController.h"

#if defined(TP_PINCHANGEINTERRUPT) && !(defined(__arm__) && defined(CORE_TEENSY))

#include <PinChangeInterrupt.h>
#include <PinChangeInterruptBoards.h>
#include <PinChangeInterruptPins.h>
#include <PinChangeInterruptSettings.h>

// The below values are not scientific, but they seem to work.  These may need to be tuned for different systems.
#define LINE_WAIT 200
#define DIGITAL_HIGH_THRESHOLD 50

static volatile byte currentState = 0;
static byte lastState = 0xFF;
static byte lastRawData = 0;

static byte rawData;

void row1_isr()
{
	delayMicroseconds(LINE_WAIT);
	byte cachedCurrentState = currentState;
	if (currentState > 3)
		return;
	else if (PIN_READ(7) == 0)
		currentState = 3;
	else if (PINB_READ(0) == 0)
		currentState = 2;
	else if (PIN_READ(6) == 0)
		currentState = 1;
	else if (cachedCurrentState >= 1 && cachedCurrentState <= 3)
		currentState = 0;
}

void row2_isr()
{
	delayMicroseconds(LINE_WAIT);
	byte cachedCurrentState = currentState;
	if (currentState > 6)
		return;
	else if (PIN_READ(7) == 0)
		currentState = 6;
	else if (PINB_READ(0) == 0)
		currentState = 5;
	else if (PIN_READ(6) == 0)
		currentState = 4;
	else if (cachedCurrentState >= 4 && cachedCurrentState <= 6)
		currentState = 0;
}

void row3_isr()
{
	delayMicroseconds(LINE_WAIT);
	byte cachedCurrentState = currentState;
	if (currentState > 9)
		return;
	else if (PIN_READ(7) == 0)
		currentState = 9;
	else if (PINB_READ(0) == 0)
		currentState = 8;
	else if (PIN_READ(6) == 0)
		currentState = 7;
	else if (cachedCurrentState >= 7 && cachedCurrentState <= 9)
		currentState = 0;
}

void row4_isr()
{
	delayMicroseconds(LINE_WAIT);
	byte cachedCurrentState = currentState;
	if (PIN_READ(7) == 0)
		currentState = 12;
	else if (PINB_READ(0) == 0)
		currentState = 11;
	else if (PIN_READ(6) == 0)
		currentState = 10;
	else if (cachedCurrentState >= 10)
		currentState = 0;
}

void sr_row1sr_isr()
{
	delayMicroseconds(LINE_WAIT);
	byte cachedCurrentState = currentState;
	if (currentState > 3)
		return;
	else if (PIN_READ(7) == 0)
		currentState = 3;
	else if (analogRead(1) < DIGITAL_HIGH_THRESHOLD)
		currentState = 2;
	else if (analogRead(0) < DIGITAL_HIGH_THRESHOLD)
		currentState = 1;
	else if (cachedCurrentState >= 1 && cachedCurrentState <= 3)
		currentState = 0;
}

void sr_row2sr_isr()
{
	delayMicroseconds(LINE_WAIT);
	byte cachedCurrentState = currentState;
	if (currentState > 6)
		return;
	else if (PIN_READ(7) == 0)
		currentState = 6;
	else if (analogRead(1) < 50)
		currentState = 5;
	else if (analogRead(0) < 50)
		currentState = 4;
	else if (cachedCurrentState >= 4 && cachedCurrentState <= 6)
		currentState = 0;
}

void KeyboardControllerSpy::setup(byte controllerMode)
{
	this->currentControllerMode = controllerMode;

	currentState = 0;
	lastState = 0xFF;
	for (int i = 2; i <= 8; ++i)
		pinMode(i, INPUT_PULLUP);

#ifndef DEBUG
	if (currentControllerMode == MODE_NORMAL)
	{
		attachPinChangeInterrupt(digitalPinToPinChangeInterrupt(2), row1_isr, FALLING);
		attachPinChangeInterrupt(digitalPinToPinChangeInterrupt(3), row2_isr, FALLING);
		attachPinChangeInterrupt(digitalPinToPinChangeInterrupt(4), row3_isr, FALLING);
		attachPinChangeInterrupt(digitalPinToPinChangeInterrupt(5), row4_isr, FALLING);
	}
	else if (currentControllerMode == MODE_STAR_RAIDERS)
	{
		attachPinChangeInterrupt(digitalPinToPinChangeInterrupt(2), sr_row1sr_isr, FALLING);
		attachPinChangeInterrupt(digitalPinToPinChangeInterrupt(3), sr_row2sr_isr, FALLING);
	}
#endif
}

void KeyboardControllerSpy::loop()
{
#ifdef DEBUG
	noInterrupts();
	rawData = 0;
	rawData |= (PIND >> 2) | (PINB << 6);
	int analog0 = analogRead(0);
	int analog1 = analogRead(1);
	int analog2 = analogRead(2);
	int analog3 = analogRead(3);
	interrupts();
#else
	if (currentControllerMode == MODE_BIG_BIRD)
	{
		noInterrupts();
		byte pin6 = PIN_READ(7);
		int pin9 = analogRead(1);
		interrupts();
		if ((pin6 & 0b10000000) == 0)
			currentState = 6;
		else if (pin9 < DIGITAL_HIGH_THRESHOLD)
			currentState = 5;
		else
			currentState = 0;
	}
#endif

#ifdef DEBUG
	//if (rawData != lastRawData)
	//{
	Serial.print(currentState);
	Serial.print("|");
	Serial.print((rawData & 0b0000000000000001) != 0 ? "-" : "1");
	Serial.print((rawData & 0b0000000000000010) != 0 ? "-" : "2");
	Serial.print((rawData & 0b0000000000000100) != 0 ? "-" : "3");
	Serial.print((rawData & 0b0000000000001000) != 0 ? "-" : "4");
	Serial.print((rawData & 0b0000000000010000) != 0 ? "-" : "5");
	Serial.print((rawData & 0b0000000000100000) != 0 ? "-" : "6");
	Serial.print((rawData & 0b0000000001000000) != 0 ? "-" : "7");
	Serial.print("|");
	Serial.print(analog0);
	Serial.print("|");
	Serial.print(analog1);
	Serial.print("|");
	Serial.print(analog2);
	Serial.print("|");
	Serial.print(analog3);
	Serial.print("\n");
	lastRawData = rawData;
	//}
#else
	if (currentState != lastState)
	{
#ifdef PRETTY_PRINT
		Serial.print(currentState);
		Serial.print("\n");
#else
		Serial.write(currentState + (byte)65);
		Serial.write("\n");
#endif
		lastState = currentState;
	}
#endif
}

void KeyboardControllerSpy::writeSerial() {}
void KeyboardControllerSpy::debugSerial() {}
void KeyboardControllerSpy::updateState() {}

#else

void KeyboardControllerSpy::setup(byte controllerMode) {}
void KeyboardControllerSpy::loop() {}
void KeyboardControllerSpy::writeSerial() {}
void KeyboardControllerSpy::debugSerial() {}
void KeyboardControllerSpy::updateState() {}

#endif
