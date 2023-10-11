think("This message will only be shown to the player.")
thonk("This message will only be shown to the player. It will be muted and easily filterable.")
thonk("Keep in mind that messages are capped at TBD characters")
thonk("Keep in mind that you can only log TBD messages per tick")

say("You and others in the map can read this")
yell("You, other in the current map and the next adjacent map can read this message")
whisper("NameOfTheOtherOne", "You and the other character can read this")

return getIdleJob()