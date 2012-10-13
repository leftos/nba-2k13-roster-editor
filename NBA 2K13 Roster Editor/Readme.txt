NBA 2K13 Roster Editor
	by Lefteris "Leftos" Aslanoglou

	Quick Notes
		I'll be trying to automate and program any roster hex edits we discover before REDitor comes out.

		Make note that Player ID isn't the same as CFID or any other ID. The Player ID just denotes the position of the player in the player table of the roster.

		Any hex edits that you discover and you want me to implement, post here: http://forums.nba-live.com/viewtopic.php?f=150&t=88190


	Features
		- Player Editing
			- Signature Skills, Ratings, Tendencies, Hot Spots
			- Appearance: CF, Portrait, GenericF
			- PlType
		- Team Editing
			- Basic roster editing (30 current teams and FA pool)
			- Fix roster order and PlNum automatically for all teams
			
	
	Known Issues
		- (X360) Editing Signature Skills of Jamaal Magloire on Player ID 1364 doesn't work for now


	Credits
	    - JaoSming, for helping with researching the roster format
		- HyperBaller21, for helping with making the editor compatible with 360 files
		- Solovoy, for helping with researching the roster format
		- Vlad Zola Jr., for his work on REDitor all these years and his invaluable research and tools
		- All of the NLSC and OS community, especially the roster modders, who are the ones I'm doing this for. They make the game better and have supported me since Day 1.


	Version History
		v0.5.2.1
			- Fixed pasting Audio ID

		v0.5.2
			- (Experimental) Audio ID(?) added to Players table

		v0.5.1.2
			- Fixed Hot Spots overwriting Tendencies when pasting to the Players table

		v0.5.1.1
			- Fixed various bugs that would cause the tool to crash or misinterpret data when pasting into the Players and Teams tables

		v0.5.1
			- Ratings, Tendencies and Hot Spots columns now are named
			- Copy-pasting for the Players table should work completely

		v0.5
			- Players and Options tab are now spreadsheet like as well
			- Ratings, Tendencies and Hot Spots added to Players
			- Teams' PlNum and Player Order will automatically be fixed on save
			- Now the user can change the name of the file to load Player names from, to allow for multiple files

		v0.4.1.1
			- Fixed bug that would substitute the FA pool with the Bobcats players on save

		v0.4.1
			- (Experimental) The tool will now read and write 18 roster entries instead of 15 for every team

		v0.4
			- Added roster editing for all 30 current teams and the FA pool

		v0.3.3
			- Fixed player entry reading after Player ID 1365 in X360 rosters
			- Known Issue: (X360) Editing Signature Skills of Jamaal Magloire on Player ID 1364 doesn't work for now
			- If you have a "names.txt" file in your "My Documents\NBA 2K13 Roster Editor" with tab-separated Player IDs and names, the tool will now show you the name of the player you're editing

		v0.3.2.3
			- Tool no longer crashes when Search by CF doesn't find a player with that CF ID

		v0.3.2.2
			- Fixed bug with CF reading/writing introduced in v0.3.2.1

		v0.3.2.1
			- Fixed bug with PlType not being read correctly

		v0.3.2
			- Added PlType editing (PC & X360)
		
		v0.3.1
			- Added support for GenericF editing (PC & X360)
			- Added error reports

		v0.3
			- Added support for CF ID & Portrait ID editing (PC & X360)
			- Added Search by CF feature

		v0.2.6
			- Added X360 support for the Signature Skills editing

		v0.2.5
			- Added X360 support for the Team Roster editing

		v0.2.4
			- All 12 player IDs of the 76ers can be changed

		v0.2.3
			- Added changing the Player ID of the 5th player of the 76ers to test out different Player IDs

		v0.2.2
			- The offset of the current player's first signature skill is shown
			- Buttons to switch players quickly have been added

		v0.2.1
			- Added Gatorade Prime Pack and On Court Coach signature skills

		v0.2
			- Tool can now save changes to roster files

		v0.1
			- Initial release