# CodeToSurvive

Prove of concept fantasy roleplay survival programming game.

The idea is that the player and the world run 24/7 and in addition to "normal" playing via inputs the user provides a script (current thought is lua) to control the character while the player is offline. 

In addition to normal Roleplaying stuff abillities and stats are programming related traits. More memmory, maybe new functions. The game has a tiled map, but the character can only remember the last N tiles they visited, so improving the orientation skill will provide the coder with more map informations while building the custom script. 


# Layout
## NotLoggedIn
```
HeaderPanel [Logo] [Wiki-Link] [Login]
WelcomePage
```

## LoggedIn
```
HeaderPanel [Logo] [Scoreboard] [WikiLink] [Settings] [Logout]
LoggedInPanel [Admin(Optional)] [Overview(default)] [ActiveOwnedCharacter1] ... [ActiveOwnedCharacterN]
ActiveLoggedInView
```

## SettingsView
```
HeaderPanel [Logo] [Scoreboard] [WikiLink] [Settings] [Logout]
Password Change
Kill my characters
```

## ScoreboardView
```
List [
    Charactername | Accountname | Survivaltime | CreationDate | Alive/Dead
]
```

## Overview
```
List [
    MyCharacters -> Charactername | Survivaltime | CreationDate | Alive/Dead
]
[CreateNewCharacters]
```

## CharacterOverviewView
```
CharacterPanel [Overview] [Script]
Map        | Log
MemoryTree | 
```

## CharacterScriptView
```
CharacterPanel [Overview] [Script]
ScriptInput
```

## AdminView
```
AdminPanel [Server][Accounts][ActiveCharacters]
AdminSubView
```

### ServerView
```
(Stop/Start) _Status_
Active Characters: _Count_
... Other infos
```

### Accounts
```
List [
Username | Role | LastLogIn | ActiveCharacters  | [Suspend][PasswordReset][ChangeRole][Kill][Delete]
]
Invite
[Username]
[Role]
```

### ActiveCharacters
```
List [
Name | Username
]
```