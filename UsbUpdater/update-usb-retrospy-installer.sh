#!/bin/bash

#Github URL
RETROSPY_URL=https://github.com/retrospy/RetroSpy/releases/latest/download/retrospy

#RetroSpy file downloading
echo ""
echo "Installing USBProxy..."
echo ""

cd ~retrospy/USBProxy
cd src/build
cmake ..
make
sudo make install
sudo ldconfig

echo ""
echo "Installing ds4drv..."
echo ""

cd ~retrospy/ds4drv
sudo python3 setup.py install
