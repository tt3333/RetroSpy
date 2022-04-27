//
// ColecoVision.cpp
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

#include "ColecoVision.h"

#if defined(TP_PINCHANGEINTERRUPT) && !(defined(__arm__) && defined(CORE_TEENSY))
#include <PinChangeInterrupt.h>
#include <PinChangeInterruptBoards.h>
#include <PinChangeInterruptPins.h>
#include <PinChangeInterruptSettings.h>

static volatile byte rawData[2];
static volatile byte currentState;
static volatile bool quadbit[2];

static void pin5bithigh_isr()
{
	asm volatile(MICROSECOND_NOPS MICROSECOND_NOPS MICROSECOND_NOPS MICROSECOND_NOPS MICROSECOND_NOPS MICROSECOND_NOPS MICROSECOND_NOPS MICROSECOND_NOPS);
	rawData[0] = (PIND & 0b01111100);
}

static void pin8bithigh_isr()
{
	asm volatile(MICROSECOND_NOPS MICROSECOND_NOPS MICROSECOND_NOPS MICROSECOND_NOPS MICROSECOND_NOPS MICROSECOND_NOPS MICROSECOND_NOPS MICROSECOND_NOPS);
	if (PINB_READ(0) != 0)
		rawData[1] = (PIND & 0b01111100);
	
}

static void bitchange_isr1()
{
	byte currentEncoderValue = (quadbit[0] == false ? 0x00 : 0x01) | (quadbit[1] == false ? 0x00 : 0x02);
	byte encoderValue = (quadbit[0] == false ? 0x00 : 0x01) | (!quadbit[1] == false ? 0x00 : 0x02);
	
	asm volatile(MICROSECOND_NOPS MICROSECOND_NOPS MICROSECOND_NOPS MICROSECOND_NOPS MICROSECOND_NOPS MICROSECOND_NOPS  MICROSECOND_NOPS MICROSECOND_NOPS);
	
	if ((currentEncoderValue == 0x2 && encoderValue == 0x0)
		|| (currentEncoderValue == 0x1 && encoderValue == 0x3))
	{
		if (currentState == 63)
			currentState = 0;
		else
			currentState = (currentState + 1) % 64;
	}
	else if ((currentEncoderValue == 0x3 && encoderValue == 0x1)
		|| (currentEncoderValue == 0x0 && encoderValue == 0x2))
	{
		if (currentState == 0)
			currentState = 63;
		else
			currentState = (currentState - 1) % 64;
	}
	quadbit[1] = !quadbit[1];

}

static void bitchange_isr2()
{
	noInterrupts();
	byte currentEncoderValue = (quadbit[0] == false ? 0x00 : 0x01) | (quadbit[1] == false ? 0x00 : 0x02);
	byte encoderValue = (!quadbit[0] == false ? 0x00 : 0x01) | (quadbit[1] == false ? 0x00 : 0x02);
	
	asm volatile(MICROSECOND_NOPS MICROSECOND_NOPS MICROSECOND_NOPS MICROSECOND_NOPS MICROSECOND_NOPS MICROSECOND_NOPS MICROSECOND_NOPS MICROSECOND_NOPS);

	if ((currentEncoderValue == 0x3 && encoderValue == 0x2)
		|| (currentEncoderValue == 0x0 && encoderValue == 0x1))
	{	
		if (currentState == 63)
			currentState = 0;
		else
			currentState = (currentState + 1) % 64;
	}
	else if ((currentEncoderValue == 0x2 && encoderValue == 0x3)
		|| (currentEncoderValue == 0x1 && encoderValue == 0x0))
	{
		
		if (currentState == 0)
			currentState = 63;
		else
			currentState = (currentState - 1) % 64;
	}
	
	quadbit[0] = !quadbit[0];

}

void ColecoVisionSpy::setup()
{
	for (int i = 2; i <= 10; ++i)
		pinMode(i, INPUT_PULLUP);
	
	noInterrupts();
	quadbit[0] = (PINB & 0b00000001) != 0;
	quadbit[1] = (PIND & 0b10000000) != 0;
	interrupts();
	
	currentState = 0;

	attachPinChangeInterrupt(digitalPinToPinChangeInterrupt(7), bitchange_isr1, CHANGE);
	attachPinChangeInterrupt(digitalPinToPinChangeInterrupt(8), bitchange_isr2, CHANGE);
	attachPinChangeInterrupt(digitalPinToPinChangeInterrupt(9), pin5bithigh_isr, RISING);
	attachPinChangeInterrupt(digitalPinToPinChangeInterrupt(10), pin8bithigh_isr, RISING);
}

void ColecoVisionSpy::loop() {

#ifndef DEBUG
	writeSerial();
#else 
	debugSerial();
#endif

	//delay(5);
}

void ColecoVisionSpy::updateState() {
}

void ColecoVisionSpy::writeSerial() {
	for (unsigned char i = 0; i < 2; ++i)
	{
		for (unsigned char j = 2; j < 7; ++j)
		{
			Serial.write((rawData[i] & (1 << j)) != 0 ? ZERO : ONE);
		}
	}
	Serial.write(currentState + 11);
	Serial.write(SPLIT);
}

void ColecoVisionSpy::debugSerial() {
	Serial.print((rawData[0] & 0b00000100) != 0 ? "0" : "U");
	Serial.print((rawData[0] & 0b00001000) != 0 ? "0" : "D");
	Serial.print((rawData[0] & 0b00010000) != 0 ? "0" : "L");
	Serial.print((rawData[0] & 0b00100000) != 0 ? "0" : "R");
	Serial.print((rawData[0] & 0b01000000) != 0 ? "0" : "1");
	Serial.print((rawData[1] & 0b00000100) != 0 ? "0" : "A");
	Serial.print((rawData[1] & 0b00001000) != 0 ? "0" : "B");
	Serial.print((rawData[1] & 0b00010000) != 0 ? "0" : "C");
	Serial.print((rawData[1] & 0b00100000) != 0 ? "0" : "D");
	Serial.print((rawData[1] & 0b01000000) != 0 ? "0" : "2");
	Serial.print("|");
	Serial.print(currentState);
	Serial.print("|");
	Serial.print(quadbit[0]);
	Serial.print("|");
	Serial.print(quadbit[1]);
	Serial.print("\n");
}

#else
void ColecoVisionSpy::loop() {}
void ColecoVisionSpy::setup() {}
void ColecoVisionSpy::writeSerial() {}
void ColecoVisionSpy::debugSerial() {}
void ColecoVisionSpy::updateState() {}
#endif