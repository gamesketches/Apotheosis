#!/usr/bin/env bash

if ["$(uname)" == "Darwin"]; then
     mkdir $(pwd)+Assets/Resources/sprites/$1+Animation/p1
     mkdir $(pwd)+Assets/Resources/sprites/$1+Animation/p2
     /Applications/Unity/Unity.app/Contents/MacOS/Unity -executeMethod SpriteSheetGenerator.GenerateSpriteSheet $1
else
     echo this shit doesn't work on windows yet


