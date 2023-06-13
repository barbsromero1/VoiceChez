import os
import threading
import shutil
from lib.inference import verify_speaker


class SpeakerThread(threading.Thread):

    def __init__(self, dir_path: str, audio: str, player: str) -> None:
        super().__init__(None)
        self.__audio_path = audio
        self.__sample_audio = os.path.join(dir_path, f'{player}_SAMPLE.wav')
        self.result = 0

    def run(self):
        if not os.path.exists(self.__sample_audio):
            shutil.copy(self.__audio_path, self.__sample_audio)
            self.result = 1
            return
        same_speaker = verify_speaker(self.__sample_audio, self.__audio_path)
        self.result = 1 if same_speaker else 0
