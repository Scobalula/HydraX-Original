# ![](https://i.imgur.com/1dyZzhy.png)

The virulent Black Ops III Stock File Printer 🖨

![](https://img.shields.io/github/downloads-pre/Scobalula/HydraX/total.svg) ![](https://img.shields.io/github/license/Scobalula/HydraX.svg)

HydraX can export several files from Black Ops III Fast Files listed below. It also allows you to decompress Fast Files for examining in a hex editor.

## Credits

Harry Bo21 - AI Information / Alpha Testing

[DidUknowiPwn](https://github.com/DidUknowiPwn) - Bug Fixes

[Hydra Logo](https://www.kisspng.com/png-hercules-and-the-lernaean-hydra-hydra-bay-the-pira-1770604/)

[ZlibNet](https://github.com/gdalsnes/zlibnet)


# Supported Assets

| Compatible Assets |
| ----------------- |
| .Script           |
| .LUA*             |
| .GSC*             |
| .CSC*             |
| .GSH*             |
| .Vision           |
| .CFG              |
| .Graph            |
| .TXT              |
| .CSV              |
| .ATR              |
| .MAP (Entities)** |
| .AI_AM            |
| .AI_AST           |
| .AI_ASM           |
| .AI_BT            |

\* These files are compiled. They can be included using [L3akMod](http://phabricator.aviacreations.com/w/black_ops_3/lua_%28lui%29/). LUA Files can be "disassembled" using [Zorteok](http://phabricator.aviacreations.com/w/black_ops_3/lua_%28lui%29/zorteok/).

\** These may require you to copy the Header and SSI of another map to open in Radiant, in some cases errors may occur without further edits, a future update will process them for use in Radiant.

# License/Disclaimers

HydraX is licensed under GPL 3.0. It is provided in the hope it is useful to you but comes WITH NO WARRANTY. The user assumes full responsibility for any damages caused.

HydraX was developed for the users of the Black Ops III Mod Tools to provide some files/information Treyarch couldn't/didn't include with the Mod Tools. All work was done on legally obtained copies of Black Ops III and the Black Ops III Mod Tools. Most of the files it exports, are only useful to those using Black Ops III's Mod Tools. HydraX does not and will never rebuild FFs or provide methods to modify game content. I don't support hacking!

# FAQs

- Q: What is the string cache?
- A: The string cache is a huge list of strings processed from multiple files included included files with the mod tools etc. it is currently only used by the string tables in the event a string within it is from another part of the FF. It is loaded and written to with any new strings every time the app runs.

- Q: Some files don't export correctly?
- A: Yikes! All files are currently under active investigation, but I think most should export perfectly fine. If you find a file that exports incorrectly (particularly the AI files and possibly incorrect entries, missing data, etc) let me know with as much information as possible so it the bug report can be shipped to Sir. Bugg0rt.


- Q: I'm pretty sure x file contains z asset that's supported, but it's not exporting?
- A: Due to the way the engine loads Fast Files, in order to properly export assets, you would need to process every assets. With this in mind, some **methods** have to be used to "scan" for assets and verify them before exporting. In some cases this doesn't always work. If you know the name off the asset, hit a brother up with the name of it and the 


- Q: x Asset completely failed, what to doooooo?
- A: The best thing to do is to send me a lovely PM with the name of the asset, its location, and any other information you can give.

- Q: Pfffft, xyz can already export some of these.
- A: Correct. 

- Q: Your app is crap!!!!!
- A: Aww damn, sorry to disappoint you. I've been doing this for less than a year and it's a constant learning process, if you have suggestions, tips, see anywhere I butchered completely, do let me know. :c

## Support Me

If you use HydraX in any of your projects, it would be appreciated if you credit me, a lot of time and work went into developing it and a simple credit isn't too much to ask for.

If you'd like to support me even more, considering buying me a coffee (I drink a lot :x):

[![Donate](https://img.shields.io/badge/Donate-Buy%20Me%20a%20Coffee-yellow.svg)](https://www.buymeacoffee.com/Scobalula)
