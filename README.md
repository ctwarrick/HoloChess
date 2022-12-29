# HoloChess
A chess game for Microsoft HoloLens 2, currently a work in progress.

## Requirements
* Microsoft HoloLens 2 or emulator
* Unity 2021.3.16f1 or greater
* Azure Spatial Anchors SDK Core 2.13.3 or greater
* Azure Spatial Anchors SDK for Windows 2.13.3 or greater
* MRTK Extensions 2.8.3 or greater
* MRTK Foundation 2.8.3 or greater
* MRTK Standard Assets 2.8.3 or greater
* MRTK Tools 2.8.3 or greater
* Mixed Reality OpenXR plugin 1.7.0 or greater
* Microsoft Spatializer 2.0.37 or greater
* Visual Studio 2019

## Installation and Deployment
Open the project in Unity with required dependencies, and build under Universal Windows Platform with the following settings:
* Architecture:  ARM64
* Build type:  D3D Project
* Target SDK:  Latest Installed
* Minimum Platform Version:  10.0.10240.0
* Visual Studio version: Latest Installed
* Build configuration:  Release

Deploy the resulting HoloChess.sln remotely to HoloLens device using Visual Studio 2019.

App package to follow for sideloading in later updates.

## Getting Around
On launching the application, you will see a transparent gray chessboard attached to your hand ray.  Air tap to place the chessboard in a convenient spot, and it will spawn the board along with a debug console.  

You can move pieces by either grabbing them directly or using far manipulation via one of your hand rays.  Hovering over a piece will display its legal moves as blue squares.  Illegal moves will error out, and cause the piece to return to its previous spot.

## What Isn't Implemented Yet
* Castling
* Capturing
* Check
* Checkmate
* Hiding the debug console

## Attribution
Mixed Reality Toolkit is copyright Microsoft Corporation, licensed under the MIT License.

"Chess set" (https://skfb.ly/ooxAN) by Brendan Wood is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/), modified by me for HoloLens 2 in Blender.  

Chessboard textures are modified using Metal007 and Wood027 from ambientCG.com, licensed under the Creative Commons CC0 1.0 Universal License.