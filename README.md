TimeParabox
===

![Run duration: 00:17:15.367](https://img.shields.io/badge/run%20duration-00%3A17%3A15.367-success) [![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/Aldaviva/TimeParabox/dotnet.yml?branch=master&logo=github)](https://github.com/Aldaviva/TimeParabox/actions/workflows/dotnet.yml)

Program that automates speedruns of [Patrick's Parabox](https://www.patricksparabox.com).

Solves 156 puzzles, a minimal set to reach the credits. No game defects are exploited. (A glitchless any% TAS, in speedrunning jargon.)

Run duration is measured from the first input on the title screen to the last input before the credits.

## ▶ Watch

### Run in 17m 15.367s
[![Watch on YouTube](https://i.ytimg.com/vi_webp/wZ--hjWwByQ/maxresdefault.webp)](https://www.youtube.com/watch?v=wZ--hjWwByQ)

## Game settings
|Setting|Value|
|-|-:|
|Enter speed|2×|
|Allow rapid inputs|true|

## Run
1. Install [.NET 7 x64 Desktop Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/7.0) or later
1. Download the [latest TimeParabox release](https://github.com/Aldaviva/TimeParabox/releases/latest/download/TimeParabox.exe)
1. Launch Patrick's Parabox
1. Choose a save file
1. Set the [game settings](#game-settings)
1. Launch `TimeParabox.exe`
    - On first run, allow the installation of ScpVBus by Scarlet.Crush Productions when prompted, which is a virtual gamepad driver
    - To begin the run from the title screen, you don't need to pass any extra arguments
    - To begin the run from a specific hub, move to the first square when entering the hub, and pass the hub's name as the first argument:
        ```bat
        TimeParabox.exe Possess
        ```
    - To begin the run from a specific puzzle, start the puzzle manually, and pass the hub name and puzzle number as the first and second arguments:
        ```bat
        TimeParabox.exe "Infinite Exit" 2
        ```
1. Focus the game window
1. This program will run the game
1. To stop this program, focus its console window and press `Ctrl`+`C`
