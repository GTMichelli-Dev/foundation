# Enabling the Extra USB Ports on a DeskPi Lite

[← Back to README](../README.md)

The DeskPi Lite adapter board routes the Pi 4's USB-C port out to additional
USB-A sockets. That port is an OTG controller, and Raspberry Pi OS leaves it
in peripheral mode by default — so the extra sockets are dead until the Pi is
told to drive them as a host. Nothing is wrong with the board; it just needs
one line of boot config.

This is hardware setup, independent of what the Pi is running. It applies the
same way to a Pi serving the web app, a kiosk display, a print agent, or a gate
controller.

## Steps

1. Open the boot config:

   ```bash
   sudo nano /boot/firmware/config.txt
   ```

2. At the bottom of the file, under the `[all]` section, add:

   ```
   dtoverlay=dwc2,dr_mode=host
   ```

   It should end up looking something like:

   ```
   [all]
   enable_uart=1
   disable_splash=1
   dtoverlay=dwc2,dr_mode=host
   ```

3. Save and exit: `Ctrl+O`, `Enter`, `Ctrl+X`.

4. Reboot:

   ```bash
   sudo reboot
   ```

After the reboot the extra USB ports on the adapter board work normally.

## If the overlay is already in the file

Check which section it sits under. A line under a model-specific section such
as `[cm5]` only applies to that model, so a Pi 4 in a DeskPi Lite reads right
past it and the ports stay dead. It still needs to be added under `[all]`.
Having it in both places is harmless.
