/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RetroSpy CD-i Controller Firmware for Arduino
// v1.0
// RetroSpy written by zoggins

// IR Requirements
// Requires this package: https://github.com/cyborg5/IRLib2
// Requires an IR receiver attached to Arduino digital pin #2
// Here is what I used: https://www.adafruit.com/product/157 (TSOP38238)

// Wired requires the cable to be spliced.
// NOTE: The board that I bought to "splice" the wired controller cable has PIN 2 marked as PIN 8. 
// The only thing that needs to connected is Controller Pin 2 to Arduino Digital Pin 9.
// Obviously this took me while as my board was marked wrong.

// You do not need to have both to work.  If you only want wired and not wireless you do not need the IR receiver and vice versa.

#include <SoftwareSerial.h>
SoftwareSerial vSerial(9, 10, true);
#include "IRLibAll.h"
IRdecode myDecoder; 
//Create a receiver object to listen on pin 2
IRrecv myReceiver(2);

// Uncomment 1 of these for different levels of debugging output
// I would not recomend using more than 1 at a time!
//#define WIRED_PRETTY_PRINT
//#define WIRELESS_PRETTY_PRINT
//#define WIRED_DEBUG
//#define WIRELESS_DEBUG

// These values may need adjusting based on the controller used.
#define WIRED_TIMEOUT 50

#define WIRELESS_TIMEOUT 100

unsigned long wireless_timeout;
unsigned long wireless_remote_timeout;
byte wireless_rawData[15];
byte wireless_yaxis;
byte wireless_xaxis;

byte max_right;
byte max_left;
byte max_up;
byte max_down;

byte wired_rawData[6];
byte wired_yAxis;
byte wired_xAxis;
unsigned long wired_timeout;

void setup() {
  vSerial.begin(1200); 
  wired_rawData[4] = wired_rawData[5] = 0;

  max_right = max_left = max_up = max_down = 0;

  Serial.begin( 115200 );
  delay(2000); while (!Serial); //delay for Leonardo
  
  myReceiver.enableIRIn(); // Start the receiver
#ifdef WIRELESS_DEBUG
  Serial.println(F("Ready to receive IR signals"));
#endif
  wireless_timeout = wired_timeout = wireless_remote_timeout = millis();
}

void printRawData()
{
#ifdef WIRED_PRETTY_PRINT
  for(int i = 0; i < 6; ++i)
  {
    Serial.print(wired_rawData[i]);
    Serial.print("|"); 
  }

  Serial.print(wired_yAxis > 128 ? 256-wired_yAxis+128 : wired_yAxis);
  Serial.print("|"); 
  Serial.print(wired_xAxis > 128 ? 256-wired_xAxis+128 : wired_xAxis);
  Serial.print("\n"); 
#elif defined WIRELESS_PRETTY_PRINT
  for(int i = 0; i < 15; ++i)
  {
    Serial.print(wireless_rawData[i]);
    Serial.print("|"); 
  }
  Serial.print(wireless_yaxis);
  Serial.print("|"); 
  Serial.print(wireless_xaxis);
  Serial.print("\n");
#else
  int j = 0;
  for(int i = 0; i < 6; ++i)
  {
    Serial.write(wired_rawData[i] & 0xF0);
    Serial.write((wired_rawData[i] & 0x0F) << 4);
  }
  for(int i = 0; i < 15; ++i)
  {
    Serial.write(wireless_rawData[i] & 0xF0);
    Serial.write((wireless_rawData[i] & 0x0F) << 4);
  }
  Serial.write("\n");
#endif
}

static int ScaleInteger(float oldValue, float oldMin, float oldMax, float newMin, float newMax)
{
  float newValue = ((oldValue - oldMin) * (newMax - newMin)) / (oldMax - oldMin) + newMin;
  if (newValue > newMax)
    return newMax;
  if (newValue < newMin)
    return newMin;

  return newValue;
}

void HandleSerial()
{

    if (vSerial.available() >= 3) {

      char c = vSerial.read();
      if ((c & 0b11000000) == 0b11000000)
      {
   
#ifndef WIRED_DEBUG     
  
        wired_rawData[4] = (c & 0b00100000) != 0 ? 1 : 0;  // Button 1
        wired_rawData[5] = (c & 0b00010000) != 0 ? 1 : 0;  // Button 2
  
         wired_yAxis = ((c & 0b00001100) << 4);
         wired_xAxis = ((c & 0b00000011) << 6);
  
        c = vSerial.read();
        wired_xAxis = wired_xAxis + (byte)(c & 0b00111111);
  
        c = vSerial.read();
        wired_yAxis = wired_yAxis + (byte)(c & 0b00111111);
  
        if (wired_yAxis == 0 || wired_yAxis == 128)
        {
          wired_rawData[0] = 0;
          wired_rawData[1] = 0;
        }
        else if (wired_yAxis > 128)
        {
          if (wired_yAxis > max_up)
            max_up = wired_yAxis;
          
          wired_rawData[0] =  ScaleInteger(256-wired_yAxis+128, 128,  max_up, 0, 255);
          wired_rawData[1] = 0;
        }
        else
        {
          if (wired_yAxis > max_down)
            max_down = wired_yAxis;
          
          wired_rawData[1] =  ScaleInteger(wired_yAxis, 0,  max_down, 0, 255);
          wired_rawData[0] = 0;
        }
  
        if (wired_xAxis == 0 || wired_xAxis == 128)
        {
          wired_rawData[2] = 0;
          wired_rawData[3] = 0;
        }
        else if (wired_xAxis > 128)
        {
           if (wired_xAxis > max_left)
             max_left = wired_xAxis;
             
          wired_rawData[2] = ScaleInteger(256-wired_xAxis+128, 128, max_left, 0, 255);
          wired_rawData[3] = 0;
        }
        else
        {
          if (wired_xAxis > max_right)
             max_right = wired_xAxis;
             
          wired_rawData[3] = ScaleInteger(wired_xAxis, 0,  max_right, 0, 255);
          wired_rawData[2] = 0;
        }
  
        wired_timeout = millis();
#else     
       for(int i = 7; i >= 0; --i)
          Serial.print((c & (1 << i)) ? "1" : "0");
       Serial.print("|");
       
       for(int i = 0; i < 2; ++i)
       {
        char c = vSerial.read();
        for(int i = 7; i >= 0; --i)
          Serial.print((c & (1 << i)) ? "1" : "0");
        if (i == 1)
          Serial.print("\n");
        else
          Serial.print("|");
       }
#endif
     }
  }
  else if (((millis() - wired_timeout) >= WIRED_TIMEOUT))
  {
    for(int i = 0; i < 4; ++i)
      wired_rawData[i] = 0;
  wired_xAxis = wired_yAxis = 0;

  }
}

void HandleIR()
{
    if (myReceiver.getResults()) {
      myDecoder.decode();   //Decode it

    if (myDecoder.bits == 29)
    {
      myDecoder.value = myDecoder.value >> 1;
    }
    
    if (myDecoder.protocolNum == 3
        && myDecoder.address == 0 
        && myDecoder.bits == 29
        && (myDecoder.value & 0b11111111111100000000000000000000) == 0b00001001001000000000000000000000
       )                     
    {
#ifdef WIRELESS_DEBUG
        for(int32_t i = 31; i >=0;--i)
        {  Serial.print((myDecoder.value & ((int32_t)1 << i)) != 0 ? "1" : "0");
          if (i % 8 == 0)
            Serial.print("|");
        }
        Serial.print("\n");
    }
#else
        char button_pushed = 0;
        for (int i = 0; i < 8;++i)
        {
            button_pushed |= (myDecoder.value & (1 << i));
        }

        switch(button_pushed)
        {
          case 0x03: // Pause
            wireless_rawData[6] = 1;
            break;
          case 0x02: // Stop
            wireless_rawData[8] = 1;
            break;
          case 0x014: // Previous Track
            wireless_rawData[9] = 1;
            break;
          case 0x04: // Play
            wireless_rawData[10] = 1;
            break;
          case 0x10: // Next Track
            wireless_rawData[11] = 1;
            break;
          case 0x17: // TV/CDI
            wireless_rawData[12] = 1;
            break;
          case 0x18: // Volume Down
            wireless_rawData[13] = 1;
            break;
          case 0x19: // Volume Up
            wireless_rawData[14] = 1;
            break;
        }
        wireless_remote_timeout = millis();
    }

#endif          
      if ((myDecoder.protocolNum == 3 || myDecoder.protocolNum == 4) 
          && myDecoder.address == 0 
          && (myDecoder.bits == 32 || (myDecoder.bits == 29 && (myDecoder.value & 0b11111111111100000000000000000000) != 0b00001001001000000000000000000000))
         )                       
      {
#ifdef WIRELESS_DEBUG

        for(int32_t i = 31; i >=0;--i)
        {  Serial.print((myDecoder.value & ((int32_t)1 << i)) != 0 ? "1" : "0");
          if (i % 8 == 0)
            Serial.print("|");
        }
        Serial.print("\n");
        }
    }
#else
        wireless_yaxis = 0;
        for (int i = 0; i < 8;++i)
        {
            wireless_yaxis |= (myDecoder.value & (1 << i));
        }
          
        if (wireless_yaxis == 0 || wireless_yaxis == 128)
        {
          wireless_rawData[0] = 0;
          wireless_rawData[1] = 0;
        }
        else if (wireless_yaxis > 128)
        {
          if (myDecoder.bits == 29)
            wireless_yaxis = 128 + 256 - wireless_yaxis;

          if (wireless_yaxis > max_up)
            max_up = wireless_yaxis;
          
          wireless_rawData[0] =  ScaleInteger(wireless_yaxis, 128, max_up, 0, 255);
          wireless_rawData[1] = 0;
        }
        else
        {
          if (wireless_yaxis > max_down)
            max_down = wireless_yaxis;
          
          wireless_rawData[1] =  ScaleInteger(wireless_yaxis, 0, max_down, 0, 255);
          wireless_rawData[0] = 0;
        }
        
        wireless_xaxis = 0;
        for (int i = 8; i < 16;++i)
        {
            wireless_xaxis |= ((myDecoder.value & (1 << i)) >> 8);
        } 
        
        if (wireless_xaxis == 0 || wireless_xaxis == 128)
        {
          wireless_rawData[2] = 0;
          wireless_rawData[3] = 0;
        }
        else if (wireless_xaxis > 128)
        {
          if (myDecoder.bits == 29)
            wireless_xaxis = 128 + 256 - wireless_xaxis;


          if (wireless_xaxis > max_left)
            max_left = wireless_xaxis;
            
          wireless_rawData[2] = ScaleInteger(wireless_xaxis, 128, max_left, 0, 255);
          wireless_rawData[3] = 0;
        }
        else
        {
          if (wireless_xaxis > max_right)
            max_right = wireless_xaxis;
            
          wireless_rawData[3] = ScaleInteger(wireless_xaxis, 0, max_right, 0, 255);
          wireless_rawData[2] = 0;
        }

        wireless_rawData[5] = (myDecoder.value & 0b00000000000000100000000000000000) != 0 ? 1 : 0;
        wireless_rawData[4] = (myDecoder.value & 0b00000000000000010000000000000000) != 0 ? 1 : 0;
                      
        wireless_timeout = millis();
      }

    }

#endif
    
    if (((millis() - wireless_timeout) >= WIRELESS_TIMEOUT))
    {
      for(int i = 0; i < 6; ++i)
        wireless_rawData[i] = 0;
      wireless_xaxis = wireless_yaxis = 0;
    }
  
    if (((millis() - wireless_remote_timeout) >= WIRELESS_TIMEOUT))
    {
      for(int i = 6; i < 15; ++i)
        wireless_rawData[i] = 0;
    }
    
    myReceiver.enableIRIn();      //Restart receiver
}

void loop() {
  
    HandleSerial();
    HandleIR();
#ifndef WIRED_DEBUG
#ifndef WIRELESS_DEBUG
    printRawData();
#endif
#endif
} 
