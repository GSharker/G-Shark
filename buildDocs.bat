#!/usr/bin/env bash

echo 'Deleting the existing docs.'
rm -rf ./docs/_site

echo 'Building the Elements docs.'
docfx ./docs/docfx.json -f --serve