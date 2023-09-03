TimeParabox
===

Program that automates speedruns of [Patrick's Parabox](https://www.patricksparabox.com).

Solves 156 puzzles, a minimal set of puzzles to reach the credits. Does not use any glitches.

**Run duration:** 17m 15.367s

## ▶ Watch
[![Watch on YouTube](https://i.ytimg.com/vi_webp/wZ--hjWwByQ/maxresdefault.webp)](https://www.youtube.com/watch?v=wZ--hjWwByQ)

## Game settings
|Setting|Value|
|-|-:|
|Enter speed|2×|
|Allow rapid inputs|true|

## Run
1. Install [.NET 7 x64 Desktop Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/7.0) or later
2. Download the [latest TimeParabox release](https://github.com/Aldaviva/TimeParabox/releases/latest/download/TimeParabox.exe)
3. Start Patrick's Parabox
4. Choose a save file
5. Set the [game settings](#game-settings)
6. Go to the title screen, where it says "Start game" and "Menu"
7. Run `TimeParabox.exe`
    - On first run, allow the installation of ScpVBus by Scarlet.Crush Productions, which is a virtual gamepad driver
    - To begin the run from a specific hub, go to the first square after entering the hub, and pass the hub's name as the first argument:
        ```bat
        TimeParabox.exe Possess
        ```
    - To begin the run from a specific puzzle, start the puzzle manually, and pass the hub name and puzzle number as the first and second arguments:
        ```bat
        TimeParabox.exe "Infinite Exit" 2
        ```
8. Focus the game window
9. To stop the speedrun program, focus its console window and press `Ctrl`+`C`
