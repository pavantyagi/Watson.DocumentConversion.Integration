# Running Integration Tests

## config.json settings
Integration Tests require several settings to be set in the config.json file.

These variables contain the service authentication settings.

**Username**: The Api Key Username.

**Password**: The Api Key Password.

## Preventing config.json commits
The config.json file shouldn't be committed using your Username and Password.
After checking out this repository from GitHub open a Git Shell and run the following command:

git update-index --assume-unchanged .\test\Watson.DocumentConversion.Tests.Integration\config.json

This will prevent changes to the file being commited as long as the repository exists on your machine.
