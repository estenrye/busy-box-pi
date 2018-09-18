# busy-box-pi

## Description

The busy-box-pi project is a personal side project and school class project of mine to turn my Raspberry Pi Model 3 B into a Windows IOT Core powered educational entertainment station for my 1 year old daughter.  This device will be integrated into a wooden desk enclosure.

The initial version of this project will focus on reading an RFID chip embedded in a stuffed toy.  Once the system has read the RFID tag, the system will then display graphical feedback of what color or shape was presented to the device.

## Microcontroller

- [Raspberry Pi 3 B](https://www.adafruit.com/product/3055)

## Inputs

- [Adafruit PN532 Breakout Board](https://www.adafruit.com/product/364)
- [MiFare 13.56MHz RFID/NFC Clear Tag - 1KB](https://www.adafruit.com/product/361)
- Power Switch

## Outputs

- Graphical Display

## Functions

The system will recognize 7 different RFID tags.  

- Primary Colors
  - Red
  - Yellow
  - Blue
- Secondary Colors
  - Green
  - Purple
  - Orange
- Rainbow

Each of the primary and secondary colors when scanned will display a graphic reflecting the color and/or shape of the stuffed toy the RFID tag is embedded in.

The Rainbow Tag will display a rainbow graphic.

## Power

This device must be able to run on both battery and plugged in.

## Physical Size and Weight

This device will be embedded in a wooden desk with a flip top.