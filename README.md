# DeadByDaylight Unlocker
A Dead By Daylight unlocker for Skins, Characters, Perks &amp; Items - Works for (Steam) / Epic Games Store / Microsoft Store.
The project is made in C# WPF

![all](https://github.com/OssieFromDK/DeadByDaylight-Unlocker/assets/50819244/e1bf88ae-9634-4b3d-a788-c52c92628206)

## Showcase
I made a video showcase a while back, you can watch it here: https://www.youtube.com/watch?v=Ke_M9H105ZU

## How to get started
Either, if you are a Microsoft Store or Epic Games Store user, and you don't want to use mods for Epic Games, simply go to the releases section and download the build from there, then you don't have to through these steps.

### For steam users
You need to have access to a pak bypass, the one I usually use is the one from the Illegal Modding Discord: https://discord.gg/illegalmodding <br>
The project is build around this pak bypass, so I will not guarantee that other pakbypasses will work at all.

## Steps for building
**1.** Add your pakbypass to the `/Resources` folder. The filename must be specifically `PakBypass.exe`. <br>
**2.** Open the solution in VS22 <br>
**3.** Right click the project in the solution explorer (https://i.imgur.com/7Fmne59.png) <br>
**4.** Press the `Publish` button and a new page should open. <br>
**5.** The project comes with a publish profile, so this should be done automatically, but the settings need to be like this if not: (https://i.imgur.com/Grrf1JP.png) <br>
**6.** The project will build into a single file which can be found in: `\bin\Release\net6.0-windows\publish` <br>
**7.** If you want to use the cookie grabber for steam, you will need to copy the files from `\Assemblies` into the final destination folder


## How to update Market Files
Right now, the market files are downloaded from this github, I will try to update them, if not, make a PR with the updated files and I will accept it, you can also remove the files being auto downloaded like this: <br>
1. Go to file `\Classes\Settings.cs`
2. Go to line 30 and delete it, it should look like this: `DownloadSettings();`, simply just remove this line, and it will not auto download anymore.

You then, need to update the market files manually, the market files are stored at `AppData\Local\FortniteBurger\Configs\Profiles`, but the file names are changed, the changes are as following based on the market updater from Olympushub.xyz: <br>
GetAll.json -> Profile.json <br>
Market.json -> SkinsWithItems.json <br>
MarketDlcOnly.json -> DlcOnly.json <br>
MarketWithPerks.json -> SkinsPerks.json <br>
MarketNoSavefile.json -> SkinsONLY.json <br>
