# WinRFID
UWP App that get SUP's ID from Bluetooth, and calculate price(departure/arrival). Also use client database.

# Target:
  * UWP compatible device

# Why?
WindSUP, a Swiss enterprise who rent SUP on Neuchâtel lake. A lot of clients can be there at a given time,
so the time spent is not easy to track for the renter.

Thus, a solution has to be found.

Website: https://windsup.ch/

# How?
A small RFID reader is connected to a RN4020 (Microchip BLE module). Using UART, the app will be able to receive the SUP's ID,
thus defining which type it is and link it to a price by time table.

In the long run, Clients infos should be embedded in the device, to follow Switzerland law of naval security.


# Used Features:
  * UWP app
  * Bluetooth
  * MySQL
  * Use Contacts from system
  
  
  
  
# Basic features for v1.0:
  * Get ID from Bluetooth
  * Save departure time
  // Client get back after some time
  * Scan ID again => get time spent
  * calculate price from Time vs SUP table
