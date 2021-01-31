/*******************************************************************************
 *
 * GAMEBOY PRINTER EMULATION PROJECT
 * Copyright (C) 2020 Brian Khuu
 *
 * LICENCE:
 *   This file is part of Arduino Gameboy Printer Emulator.
 *   https://github.com/mofosyne/arduino-gameboy-printer-emulator
 *
 *   Arduino Gameboy Printer Emulator is free software:
 *   you can redistribute it and/or modify it under the terms of the
 *   GNU General Public License as published by the Free Software Foundation,
 *   either version 3 of the License, or (at your option) any later version.
 *
 *   Arduino Gameboy Printer Emulator is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with Arduino Gameboy Printer Emulator.  If not, see <https://www.gnu.org/licenses/>.
 *   
 *********************************************************************************/

#ifndef GameBoyPrinterEmulator_h
#define GameBoyPrinterEmulator_h

#include "ControllerSpy.h"

class GameBoyPrinterEmulator : public ControllerSpy {
public:
	void loop();
	void writeSerial();
	void debugSerial();
	void updateState();
	void setup();

private:

};

#endif
