Confirmation Dialogs
	- delete loads
	- 

Error Handling
	- add ExceptionController in library and publish notification events
	- add custom dialog in WPF library to handle errors

Library
	- add cable dimensions
	- add connectors

Library Editor
	- Control Station Types
	- Cable Types

Tag Settings
	- settings page
	- basic tagging system
	- tagging logic
	- bluk updates on tag changes

Cable Sizing
	- voltage drop and length calculation

Excel Reports
	- Cable BOM
	- Component BOM
	- cleanup notification
	- include / exclude all and multi-select

Dteq 
	- Dteq as Load view
	- Supply cable sizing based on upstream breaker (what is upstream breaker)
	
Load
	- Proper PD / Starter selection when calculating to taking into account if a drive is selected
	- calculate single

Grid Context Menu
	- Change Area to Area
	- ReFeed From
	- Add / Remove Local Disconnect
	- Add / Remove Drive
	- Add / Remove LCS
	- Set load factor


CABLES
	- add cable tray

Power Cables
	- 1ph cable quantities and totals
	- Prompt to recalculate load cables when changing settings
	- component cables are not updated when changing the load cable properties

Cable List
	- Tray


Areas, Systems & SubSystems
	- heatloss total by area
		- add cable to total with note
	- proper heatload calculation for each equipment type

Component type selection
	- add typelist to components (will need separate classes for each component type)

Autocad
	- drawing list
	- drawing list manager
	- single line
	- schematics
	- wiring diagrams


ISSUES
	- Component Toggle is triggering the UpdatePowerCables for Components Cables method
	- Create Separate Components (dicsonnects, Drives, Etc) 
