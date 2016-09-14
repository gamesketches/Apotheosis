#!/usr/bin/env bash

     mkdir -p $(pwd)/Assets/Resources/sprites/$1Animation/p1
     mkdir -p $(pwd)/Assets/Resources/sprites/$1Animation/p2
     cp $(pwd)/Assets/Resources/sprites/SpriteSheet$1.png $(pwd)/Assets/Resources/sprites/$1Animation/p1/SpriteSheet$1.png
     cp $(pwd)/Assets/Resources/sprites/SpriteSheet$1.png $(pwd)/Assets/Resources/sprites/$1Animation/p2/SpriteSheet$1.png
     /Applications/Unity/Unity.app/Contents/MacOS/Unity -logfile -projectPath $(pwd) -executeMethod SpriteSheetGenerator.GenerateSpriteSheet $1
     rm $(pwd)/Assets/Resources/sprites/SpriteSheet$1.png

