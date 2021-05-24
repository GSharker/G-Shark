echo 'Deleting the existing _site.'
rmdir /s /q "./docs/_site"

echo 'Building the Elements docs.'
docfx ./docs/docfx.json --serve