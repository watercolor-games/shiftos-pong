# ShiftOS Pong

This is an Android port of the Pong minigame from ShiftOS. In this version of Pong, the game is split into 
60-second levels. The higher the level, the harder the game gets and the more Codepoints you earn for surviving 
the level or beating the computer player.

In ShiftOS, the mouse was used to control your paddle by moving the mouse up and down the screen to move the 
paddle. In this version, since there is no mouse, you use your finger to swipe up and down the screen to move 
your paddle.

This version of the game also does not allow you to cash out your Codepoints into your Shiftorium, because this 
is not in any way attached to a ShiftOS backend. It is simply a way to see how far you can get in ShiftOS Pong 
without having to play ShiftOS.

It can actually get quite addictive. The current "world" record (the record set by the developer of the game) is 
Level 4 with 43 Codepoints. Can you beat that?

## Compiling

You will need Xamarin for Visual Studio 2017, as well as the MonoGame 3.6 SDK for Visual Studio. That'll get you 
at least a working development environment on your computer.

The game is tested on Android 7.0 Nougat on an LG G5, however it should theoretically work on Marshmallow as 
well.

## Getting the game on your device

Since this game isn't on Google Play, you'll have to side-load it. You can do this from within Visual Studio if 
you have a proper Xamarin Android dev environment set up. However, there are a few things you need to do on your 
device to get it to let you side-load.

For modern Android versions, you'll want to head into your Settings and find your Android build number. Tap that 
at least 5 times in a row, and your phone should go into Developer Mode. Developer Mode unlocks some advanced 
settings for Android developers, under the "Developer Options" category. In there, you will find an option for 
USB Debugging.

Turn that on. If it is greyed out, make sure your device is **not** plugged in via USB! Once it's on, plug your 
device into your development environment through USB and make sure the device is set to charge off your PC. If 
the device asks you if you want to allow USB debugging for this system, say yes. Now you can side-load the game 
onto your device from within Visual Studio by compiling the game an using your device as the target.
