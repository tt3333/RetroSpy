#!/bin/bash
 
#Github URL
RETROSPY_URL=https://github.com/retrospy/RetroSpy/releases/latest/download/retrospy

if grep -q "stretch" "/etc/apt/sources.list"; then
  echo "Device needs full image installed!"
  exit 0
fi

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

echo ""
echo "Downloading Kernel..."
echo ""
rm -rf /tmp/kernel.tar.gz
rm -rf /tmp/kernel
wget -q -t 3 --no-check-certificate --output-file=/tmp/wget-log --show-progress -O /tmp/kernel.tar.gz https://github.com/retrospy/RetroSpy/releases/latest/download/kernel.tar.gz

echo ""
echo "Installing Kernel..."
echo ""

mkdir /tmp/kernel
tar zxvf /tmp/kernel.tar.gz -C /tmp/kernel
sudo mv /tmp/kernel/zImage /boot/vmlinuz-5.10.145-ti-r55
sudo rm -rf /lib/modules/5.10.145+/
sudo mv /tmp/kernel/rootfs/lib/modules/5.10.145+/ /lib/modules/
sudo depmod

rm -rf /tmp/kernel
rm -rf /tmp/kernel.tar.gz
