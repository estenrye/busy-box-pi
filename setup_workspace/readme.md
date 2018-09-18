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