#!/bin/bash

nuget restore
cp "$HOME/.steam/steam/steamapps/common/Audiosurf 2/Audiosurf2_Data/Plugins/x86_64/libASMedia.so" ./yashgen
cp "$HOME/.steam/steam/steamapps/common/Audiosurf 2/Audiosurf2_Data/Plugins/x86_64/libbass.so" ./yashgen
msbuild ./yashgen/yashgen_linux.csproj /p:Configuration=Release