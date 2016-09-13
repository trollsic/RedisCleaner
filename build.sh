#!/bin/bash
dotnet restore
dotnet publish --runtime ubuntu.16.04-x64 -c release -o app
