# Dokumentation

# Build GUI base Day 1 - Scan network for Devices 
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