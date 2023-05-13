import os
import random
import tkinter as tk
from tkinter import filedialog
from tkinter import ttk
from pydub import AudioSegment
import simpleaudio as sa
from PIL import Image, ImageTk

# Specify the directory where your music files are stored
MUSIC_DIRECTORY = ""
BG_IMAGE_DIRECTORY = ""
play_obj = None

# Specify the scaling factor (e.g., 1.5 for 150% scaling)
scaling_factor = 1.5

def load_music_files(directory):
    music_files = []
    for file in os.listdir(directory):
        if file.endswith((".wav", ".mp3", ".ogg")):
            music_files.append(os.path.join(directory, file))
    return music_files

def convert_to_wav(file_path):
    audio = AudioSegment.from_file(file_path)
    temp_file = os.path.join(temp_dir, "temp.wav")
    audio.export(temp_file, format="wav")
    return temp_file

def play_selected_song():
    global play_obj
    selected_index = music_listbox.curselection()
    if selected_index:
        selected_file = music_listbox.get(selected_index)
        wav_file = convert_to_wav(selected_file)
        wave_obj = sa.WaveObject.from_wave_file(wav_file)
        if play_obj:
            play_obj.stop()  # Stop the current playback
        play_obj = wave_obj.play()
        status_label.config(text="Playing: " + os.path.basename(selected_file))

def pause():
    # Pause the currently playing audio
    global play_obj
    if play_obj and play_obj.is_playing():
        play_obj.stop()
        status_label.config(text="Music Paused")

def unpause():
    # Unpause the paused audio
    global play_obj
    if play_obj and not play_obj.is_playing():
        play_obj.play()
        status_label.config(text="Playing")

def stop():
    # Stop playing the audio
    global play_obj
    if play_obj:
        play_obj.stop()
        status_label.config(text="Music Stopped")
    else:
        status_label.config(text="No music playing")

def choose_directory():
    global MUSIC_DIRECTORY
    directory = filedialog.askdirectory()
    if directory:
        MUSIC_DIRECTORY = directory
        music_files = load_music_files(MUSIC_DIRECTORY)
        if not music_files:
            status_label.config(text="No music files found in the directory.")
            return
        music_listbox.delete(0, tk.END)
        for file in music_files:
            music_listbox.insert(tk.END, file)

# Create a temporary directory for storing temporary audio files
temp_dir = os.path.join(os.getcwd(), "temp")
os.makedirs(temp_dir, exist_ok=True)

root = tk.Tk()
root.title("Music Player")
root.configure(bg="#121212")
root.option_add("*Font", "Helvetica 12")
root.option_add("*Background", "#121212")
root.option_add("*Foreground", "#ffffff")

# Set the scaling factor for ttk elements
ttk.Style().configure(".", font=("Helvetica", int(12 * scaling_factor)))

# Set the window size and position
window_width = int(800 * scaling_factor)
window_height = int(600 * scaling_factor)
position_x = int((root.winfo_screenwidth() - window_width) / 2)
position_y = int((root.winfo_screenheight() - window_height) / 2)
root.geometry(f"{window_width}x{window_height}+{position_x}+{position_y}")

# Load and display the background image
bg_image_path = os.path.join(r'C:\Users\Ginld\Videos\Music\bg.png')  # Replace with the actual path to your image file
bg_image = Image.open(bg_image_path)
bg_image = bg_image.resize((window_width, window_height), Image.LANCZOS)
background_image = ImageTk.PhotoImage(bg_image)
background_label = tk.Label(root, image=background_image)
background_label.place(relx=0, rely=0, relwidth=1, relheight=1)

# Create the main content frame
frame = ttk.Frame(root, padding=20)
frame.place(relx=0.5, rely=0.5, relwidth=0.8, relheight=0.8, anchor="center")

info_frame = ttk.Frame(frame, padding=20)
info_frame.grid(row=0, column=0, sticky="w")

control_frame = ttk.Frame(frame, padding=20)
control_frame.grid(row=1, column=0, pady=10)

status_frame = ttk.Frame(frame, padding=20)
status_frame.grid(row=2, column=0, pady=20)

choose_directory_button = ttk.Button(info_frame, text="Choose Directory", command=choose_directory)
choose_directory_button.grid(row=0, column=0, sticky="w")

music_listbox = tk.Listbox(info_frame, width=50, height=15, font=("Helvetica", 12), bg="#000000", fg="#ffffff")
music_listbox.grid(row=1, column=0, pady=10, sticky="nsew")

music_listbox_scrollbar = ttk.Scrollbar(info_frame, orient="vertical", command=music_listbox.yview)
music_listbox.configure(yscrollcommand=music_listbox_scrollbar.set)
music_listbox_scrollbar.grid(row=1, column=1, sticky="ns")

play_button = ttk.Button(control_frame, text="Play", command=play_selected_song)
play_button.grid(row=0, column=0, padx=(0, 10))

stop_button = ttk.Button(control_frame, text="Stop", command=stop)
stop_button.grid(row=0, column=1, padx=(0, 10))

status_label = ttk.Label(status_frame, text="No music directory selected.", font=("Helvetica", 12), background="#121212", foreground="#ffffff")
status_label.pack()

root.mainloop()
