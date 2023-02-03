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

response=$(curl -k -sH "Authorization: token ${1}" https://api.github.com/repos/retrospy/RetroSpy-private/releases/tags/nightly)

eval $(echo "$response" | grep -C3 "name.:.\+kernel.tar.gz" | grep -w id | tr : = | tr -cd '[[:alnum:]]=')
[ "$id" ] || { echo "Error: Failed to get asset id, response: $response" | awk 'length($0)<100' >&2; exit 1; }

GH_ASSET="https://${1}:@api.github.com/repos/retrospy/RetroSpy-private/releases/assets/${id}"

echo "Downloading installer script..."
wget -q -t 3 --no-check-certificate --output-file=/tmp/wget-log --show-progress --auth-no-challenge --header "Accept:application/octet-stream" ${GH_ASSET} -O /tmp/kernel.tar.gz

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
