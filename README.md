# GetWeightUSBScale
This is a C# .NET desktop application for getting weight from a USB scale.
It was tested on a Mettler-Toledo scale.
The application considerations:
1) A scale is connected through USB.
2) The weight is taken from the scale using the HidLibrary https://www.nuget.org/packages/HidLibrary/
3) A weight object (JSON) is written to the local file system.
4) That weight JSON object is pulled and uploaded through FTP to a provided server. 

