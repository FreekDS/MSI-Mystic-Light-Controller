# MSI Mystic Light Controller
Library written in C# used to easily change the lights of your MSI PC programmatically. 
This library is built on top of the Mystic Light [SDK](https://nl.msi.com/Landing/mystic-light-rgb-gaming-pc/download) by MSI

## How to use
First download the MSI Mystic Light SDK from the MSI [website](https://nl.msi.com/Landing/mystic-light-rgb-gaming-pc/download) and put it 
somewhere on your PC.

Create a LightController object and pass in the location to the Mystic Light SDK dll.
```c#
LightController controller = new LightController("C:\\Path\\To\\SDK.dll");
```
*Note: the path to pass in is optional. If none is given, the default windows procedure of locating a DLL is used.*


Use the interface of the controller class to change the state of your lights. This includes:
- changing the color
- changing the style (for example: rainbow)
- changing speed
- changing brightness

Most of the functions of the LightController class require you to pass in a device. The available devices are stored in the 
`Devices` property.

## Minimal working example
```c#
LightController controller = new LightController("C:\\MSI\\SDK_FOLDER\\");
string device = controller.Devices[0];
Color color = new Color(150,217,40);
LED led = controller.GetDeviceLED(device, 0);
Console.WriteLine(led.ToString());
led.CurrentStyle = "rainbow";   // Update the style of a single LED
controller.SetAllLedColors(device, color);   // Update the colors of all LED's of a certain device
```
