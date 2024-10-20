$StreamDeckExe = "C:\Program Files\Elgato\StreamDeck\StreamDeck.exe"
# Change these variables to match your plugin
$BinDir = "C:\Users\yeste\src\streamdeck-tools\SamplePlugin\bin\Debug\com.test.sdtools.sdPlugin"
$Plugin = "com.test.sdtools"

# These commands require the StreamDeck CLI
# See:https://docs.elgato.com/streamdeck/sdk/introduction/getting-started
# TLDR: npm install -g @elgato/cli
$output = streamdeck link $BinDir | Out-String
if ($output -match "Plugin already installed"){
    Write-Output '✔️Plugin linked to StreamDeck'
}
streamdeck validate $BinDir
Stop-Process -Name "StreamDeck"
Start-Process $StreamDeckExe
# streamdeck r com.test.sdtools