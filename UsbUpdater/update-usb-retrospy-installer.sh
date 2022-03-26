#!/bin/bash
 
#Github URL
RETROSPY_URL=https://github.com/retrospy/RetroSpy/releases/latest/download/retrospy

#RetroSpy file downloading
echo ""
echo "Installing USBProxy..."
echo ""


cd ~retrospy/USBProxy
git pull
cd src/build
cmake ..
make
sudo make install
sudo ldconfig

echo ""
echo "Installing ds4drv..."
echo ""

cd ~retrospy/ds4drv
git pull
sudo python3 setup.py install
