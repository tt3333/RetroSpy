//
// KeyboardController.h
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

// Uncomment this to provide a pretty print of the non-debug serial stream
//#define PRETTY_PRINT

#ifndef KeyboardController_h
#define KeyboardController_h

#include "ControllerSpy.h"

class KeyboardControllerSpy : public ControllerSpy {
public:
	void setup(uint8_t controllerMode);
	void loop();
	void writeSerial();
	void debugSerial();
	void updateState();

	enum controllerMode {
		MODE_NORMAL = 0,
		MODE_STAR_RAIDERS = 1,
		MODE_BIG_BIRD = 2,
	};

private:

	uint8_t currentControllerMode;

};

#endif
