# The CodeToSurvive Project

## App

A simple WebApp with giraffe and Htmx for an interactive UI.

## Lib

Contains the core game implementation:

* Tick-Management
* Player-Script-Running
* Plugin-Management
* State-Management

The world itself, with all its items, chunks and jobs can be found in the Base-plugin.

## Resource

The resource project contains the base (and currently debug) plugin. This project implements the tasks expected of the game and provides the minimal Environment for starting a world.

Other Plugins can and should use the default plugin as a dependency for providing a starting point in the world.

## Runner

A bare bones console runner for the game. Used for development.
