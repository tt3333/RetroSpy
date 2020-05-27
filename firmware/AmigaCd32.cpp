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