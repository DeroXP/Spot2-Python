# Spot2 Music Player
Spot2 is a Python-based media player that allows you to play music files similar to Spotify. It provides a graphical user interface (GUI) built using the tkinter library, where you can choose a music directory, browse and select music files, and control playback.

# Features
Supports music files with the following formats: .ogg, .wav, and .mp3.
Converts music files to .wav format using the pydub library before playing them.
Play, pause, stop, and resume functionality for audio playback.
Displays the currently playing song and status information.
Background image display for an enhanced user interface.
Getting Started
To use the Spot2 Music Player, follow these steps:

Clone the repository to your local machine using Git:


`git clone <repository-url>`

Install the required dependencies using pip:

shell
Copy code
pip install pygame pydub simpleaudio pillow
Run the spot2.py script:

shell
Copy code
python spot2.py
Select a music directory by clicking the "Choose Directory" button and browse to the desired location.

The music files in the selected directory will be displayed in the list. Click on a file to play it.

Control the audio playback using the provided buttons (Play, Pause, Stop).

Enjoy your music with the Spot2 Music Player!

Requirements
Python 3.6 or higher
pygame
pydub
simpleaudio
pillow
License
This project is licensed under the GNU License.
