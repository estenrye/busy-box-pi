# Development Environment Setup Instructions.

## Enable ClickOnce Trust Prompt
To install the Windows IOT Core Dashboard software, you first have to enable the click once trust propt.  See [how to configure the clickonce trust prompt behavior](https://docs.microsoft.com/en-us/visualstudio/deployment/how-to-configure-the-clickonce-trust-prompt-behavior?view=vs-2017) how-to guide for more information.

Using an administrative powershell, run the following script:
```powershell
EnableClickOnce_Trust_Prompt.ps1
```

## Install Windows IOT Core Dashboard

To install Windows IOT Core on your Raspberry Pi 3 B, you need the IOT Core Dashboard software.  

Download the [Windows IOT Core Dashboard](https://iottools.blob.core.windows.net/iotdashboard/setup.exe) and run the installer.

## Install Visual Studio 2017 Community Edition

Follow this guide here to install [Visual Studio 2017 CE](https://tutorials.visualstudio.com/cpp-console/install)

## Download the Windows IOT Core Project Templates

Download and install the [Windows IoT Core Project Templates for VS 2017](https://marketplace.visualstudio.com/items?itemName=MicrosoftIoT.WindowsIoTCoreProjectTemplatesforVS15)

## Install Windows IoT Core on SD Card

1. Launch the Windows IoT Core Dashboard App
2. Select `Set up a new device`.
3. Select `Broadcomm [Raspberry Pi 2 & 3]` as the Device Type.
4. Select `Windows 10 IoT Core` as the OS Build.
5. Select the Drive Letter with your SD Card for Drive.
6. Give the device a meaningful device name, I chose `busy-box-pi`.
7. Choose an administrator password.
8. Accept the license terms.
9. Click `Download and install`

## Enable Remote Debugging Tools on the Raspberry Pi

1. Launch a Universal Windows Application using remote debugging in Visual Studio CE

## Download and install the Console App Universal Templates

1. Browse to https://marketplace.visualstudio.com/items?itemName=AndrewWhitechapelMSFT.ConsoleAppUniversal
2. Download and Run the VSIX installer.