<div align="center">

   [![C#](https://img.shields.io/badge/Language-C%23-%23f34b7d.svg?style=plastic)](https://en.wikipedia.org/wiki/C_Sharp_(programming_language))
   [![LOL](https://img.shields.io/badge/Game-Dead%20by%20Daylight-445fa5.svg?style=plastic)](https://deadbydaylight.com)
   [![Windows](https://img.shields.io/badge/Platform-Windows-0078d7.svg?style=plastic)](https://en.wikipedia.org/wiki/Microsoft_Windows)
   [![x64](https://img.shields.io/badge/Arch-x64-red.svg?style=plastic)](https://en.wikipedia.org/wiki/X86-64)
   [![License](https://img.shields.io/github/license/OssieFromDK/DeadByDaylight-Unlocker.svg?style=plastic)](LICENSE)
   [![Issues](https://img.shields.io/github/issues/OssieFromDK/DeadByDaylight-Unlocker.svg?style=plastic)](https://github.com/OssieFromDK/DeadByDaylight-Unlocker/issues)
   [![Total downloads](https://img.shields.io/github/downloads/OssieFromDK/DeadByDaylight-Unlocker/total.svg?label=Downloads&logo=github&cacheSeconds=600&style=plastic)](https://github.com/OssieFromDK/DeadByDaylight-Unlocker/releases/latest)
   [![Discord](https://img.shields.io/discord/1148144263792701471.svg?color=7289da&label=Discord&logo=discord&logoColor=white&cacheSeconds=3600&style=plastic)](https://discord.gg/ySsrsYdGwx)
   [![KoFi](https://img.shields.io/badge/Ko%20Fi-Support%20me-ea4aaa.svg?logo=kofi&style=plastic)](https://ko-fi.com/ossiefromdk)

   # **Ossie's Dead By Daylight Unlocker (Fortnite Burger)**
   
   I made a discord, feel free to join [here](https://discord.gg/ySsrsYdGwx). It includes some basic FAQ and status stuff, also a ticket system if you need help or have questions directly for me or about the project. Come hang out :D 

   ## Maintaining
   This project is only maintained by me, and I will update it whenever I have the time, for market files, you can refer to section below to update manually or submit a pull request for updated market files if they are not up to date.

   <img src="https://user-images.githubusercontent.com/50819244/274572040-e1bf88ae-9634-4b3d-a788-c52c92628206.jpg">

   `Fortnite Burger` is an Dead by Daylight unlocker, that can unlock characters/DLC, skins, items & perks.
</div>

- Unlock all characters for both killer and survivor
- Unlock all skins for all characters (they are also visible to others)
- Unlock all perks for all characters for both killer and survivor
- Unlink cosmetic sets to break full cosmetic sets
- Unlock all items and add-ons for both killer and survivor
- Choose between different profiles and choose your own items and character prestige amount
- Works for `Steam` (Pak Bypass needed), `Epic Games` & `Microsoft Store`
- Integrated mod browser, to easily download and configure mods like FOV, Killer Revealer and more

# Building
   1. Clone the source with `git clone --recursive https://github.com/OssieFromDK/DeadByDaylight-Unlocker.git`
   2. Add your pakbypass to the `/Resources` folder. The filename must be specifically `PakBypass.exe`
   3. Open the solution in VS22
   4. Right click the project in the solution explorer (https://i.imgur.com/7Fmne59.png)
   5. Press the `Publish` button and a new page should open
   6. The project comes with a publish profile, so this should be done automatically, but the settings need to be like this if not: (https://i.imgur.com/Grrf1JP.png)
   7. The project will build into a single file which can be found in: `\bin\Release\net6.0-windows\publish`
   8. If you want to use the cookie grabber for Steam, you will need to copy the files from `\Assemblies` into the final destination folder
    
### Steam users & Epic Games users that wanna use mods
You need to have access to a pak bypass, the one I usually use is the one from the  <a href="https://discord.gg/illegalmodding">Illegal Modding Discord</a> <br>
The project is build around this pak bypass, so I will not guarantee that other pak bypasses will work at all.

# Usage
   1. Compile source or <a href="https://github.com/OssieFromDK/DeadByDaylight-Unlocker/releases/latest">download</a> a compiled version.
   2. Open the FortniteBurger.exe file.
      - *Administrator* is needed for the pakbypass to work.
   3. Change the platform to the one your using under settings.
   4. Choose which profile to use, and whether to spoof currencies & level.
   5. Go to home page and press launch and the game should automatically begin launching.

# Updating Market Files
Right now, the market files are downloaded from this github, I will try to update them, if not, make a PR with the updated files and I will accept it, you can also remove the files being auto downloaded like this: <br>
1. Go to file `\Classes\Settings.cs`
2. Go to line 30 and delete it, it should look like this: `DownloadSettings();`, simply just remove this line, and it will not auto download anymore.

You then, need to update the market files manually, the market files are stored at `AppData\Local\FortniteBurger\Configs\Profiles`, but the file names are changed, the changes are as following based on the market updater from Olympushub.xyz: <br>
GetAll.json -> Profile.json <br>
Market.json -> SkinsWithItems.json <br>
MarketDlcOnly.json -> DlcOnly.json <br>
MarketWithPerks.json -> SkinsPerks.json <br>
MarketNoSavefile.json -> SkinsONLY.json <br>

# Supporting
I have made a [ko-fi](https://ko-fi.com/ossiefromdk) if you wish to support me or the project :)

# Credits
   This is made solely by <a href="https://github.com/OssieFromDK">me</a>, if you enjoy the tool or like the project, starring the project would be nice :)
