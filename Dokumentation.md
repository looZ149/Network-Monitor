# Dokumentation

# Day 1 - Build GUI base - Scan network for Devices 
## Resources:

### MAUI - GUI 
- James Montemagno - Youtube

### How to scan Network for devices?

- Google + ChatGPT
- https://de.wikipedia.org/wiki/ARP-Cache 
- ARP Cache seems likely to be the worse decision to check which devices are connected to my network
- So we send pings to the whole network 
		-> Maybe write function to get local ip? 
			-> Default should be 192.168.0.1 and 255.255.255.0 Subnetmask
			-> ipconfig Shows 
					-> PC IP : 192.168.178.39
					-> Default Gateway: 192.168.178.1

			-> So we iterate trough the whole network (192.168.178.1 -> 178.254?) and send out pings

### Write function to get the local ip
- https://stackoverflow.com/questions/6803073/get-local-ip-address 

# Day 2 - What is async, Task and await
## Resources

- https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/async
- https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/await
- https://stackoverflow.com/questions/25009437/running-multiple-async-tasks-and-waiting-for-them-all-to-complete
- https://learn.microsoft.com/de-de/dotnet/api/system.threading.lock?view=net-9.0
- can run functions in the Background, while still navigating the UI 

### Write the function to iterate trough the local network and send pings to check for devices
## Resources
- https://learn.microsoft.com/en-us/dotnet/api/system.net.networkinformation.ping?view=net-9.0
- https://learn.microsoft.com/de-de/dotnet/csharp/language-reference/statements/exception-handling-statements
	-> Need try-catch because SendPingAsync() throws exceptions on errors

-> Save every device in a ObserveableObject so we can use INotifyPropertyChange for the Online Status
	
### Day 2 - UI - Bugfixing the Lists cause they do not get the data properly

- Noticed while working with the UI that the Lists doesnt work properly 
- Need to rewrite the function for scanning the network
	-> Need to call it in MainViewModel so i get the data there
		-> Need to return a list of strings of type NetworkDevices

### Day 3 - Work on the UI
## Resources
- https://www.youtube.com/@dotnet

### Day 4 - Work on the UI + Code

### Day 5 - Fixing code
Encountered a weird bug: When debugging with a breakpoint, code works as intendet.
Without a breakpoint, it doesnt work at all. (Probably need to add some sort of timeout/sleep into the function?)
Got another weird bug with Converting.
## Resources
- https://learn.microsoft.com/en-us/answers/questions/1309181/how-to-fix-a-cs0029-error-in-net-maui-app
- https://learn.microsoft.com/en-us/dotnet/api/system.net.networkinformation.ping?view=net-9.0
- Couldnt figure it out so i asked Copilot. 

###Day 6 - AddDevice/Ping DeviceID/Functions coding
##Resources
- https://learn.microsoft.com/de-de/dotnet/api/microsoft.maui.controls.entry?view=net-maui-9.0