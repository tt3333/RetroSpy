
RetroSpy for Raspberry Pi Pico
======

ミニスーファミ (SNES Classic) のコントローラーをRaspberry Pi Picoで使えるように、RetroSpyを改造したものです。<br>
動作確認はミニスーファミでのみ行っていますが、以下のコントローラーが使えると思います。

- Nintendo Wii
- Nintendo NES Classic
- Nintendo SNES Classic

## カスタマイズせずに使う場合

Raspberry Pi PicoのBOOTSELボタンを押しながらUSBケーブルを接続し、firmware.ino.sf2を書き込んでください。<br>
SCLを27番ピン(GP21)、SDAを26番ピン(GP20)、GNDを23番ピン(GND)に接続してください。

|  Controller   |  Raspberry Pi Pico  |
|:-------------:|:-------------------:|
|      3.3V     |    Not Connected    |
|      SCL      |      27 (GP21)      |
|      3.3V     |    Not Connected    |
|      SDA      |      26 (GP20)      |
|      GND      |      23 (GND)       |

![wii-pinout](tutorial-images/wii-pinout.jpg)

## 使用するピンを変更したい場合

Wii.cpp の PIN_SCL と PIN_SDA の値を変更し、以下の手順でファームウェアをビルドしてください。

1. Arduino IDEをインストール
2. firmware.inoを開く
3. 「File」→「Preferences...」をクリック
4. 「Additional boards manager URLs」欄に以下のURLをコピペし、「OK」をクリック<br>
`https://github.com/earlephilhower/arduino-pico/releases/download/global/package_rp2040_index.json`
5. 「Tools」→「Board」→「Board Manager...」をクリック
6. 検索欄に「pico」と入力し、「Raspberry Pi Pico/RP2040 by Earle F. Philhower, III」をインストール
7. 「Tools」→「Board」→「Raspberry Pi Pico/RP2040」→「Raspberry Pi Pico」をクリック
8. 「Tools」→「Optimize」→「Fast (-Ofast) (maybe slower)」をクリック
9. 「Sketch」→「Export Compiled Binary」をクリック
