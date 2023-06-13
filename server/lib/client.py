import socket
import threading
import random
import os
import string
import time
from typing import Tuple

from lib.speach_thread import SpeachThread
from lib.speaker_thread import SpeakerThread

MAGIC_WORD = 'ajedrez'
TIMEOUT = 60
META_SIZE = 25


def get_random_string(length: int = 10):
    letters = string.ascii_lowercase
    return ''.join(random.choice(letters) for i in range(length))


class Client(threading.Thread):

    def __init__(self, s: socket.socket) -> None:
        super().__init__(None)
        self.__dir = get_random_string()
        self.__is_running = True
        self.__socket = s
        os.mkdir(f'./{self.__dir}')

    def kill(self):
        self.__is_running = False

    def on_kill(self):
        print("Client closed")
        self.__socket.close()

    def read_move(self, data: bytes) -> Tuple[str, int, bytes]:
        metadata = data[:META_SIZE].decode().strip()
        print(metadata)
        parts = metadata.split(',', 2)
        if parts[0] not in 'wb':
            raise ValueError("The player must be 'w' or 'b'")
        return parts[0], int(parts[1]), data[META_SIZE:]

    def verify(self):
        try:
            self.__socket.settimeout(TIMEOUT)
            data = self.__socket.recv(1024)
            word_size = len(MAGIC_WORD)
            if data is None or len(data) < word_size:
                return False
            if data[:word_size].decode() != MAGIC_WORD:
                return False
            self.__socket.settimeout(None)
            return data[word_size:]
        except socket.timeout:
            print("Connection timed out")
            return False

    def save_wav(self, player: str, data: bytes):
        ts = int(time.time())
        filename = f'{player}_{ts}.wav'
        filename = os.path.join(self.__dir, filename)
        with open(filename, 'wb') as wav:
            wav.write(data)
        return filename

    def run(self):
        try:
            data = self.verify()
            if data is False:
                print('Error verifying client')
                return self.on_kill()
            wav_size = 0
            player = ''
            while self.__is_running:
                data += self.__socket.recv(1024)
                if wav_size <= 0:
                    if len(data) < META_SIZE:
                        continue
                    player, wav_size, data = self.read_move(data)
                    print(f'Reading wav of size {wav_size} for "{player}"')
                elif len(data) >= wav_size:
                    wav = self.save_wav(player, data[:wav_size])
                    print(f"WAV saved: {wav}")
                    speach_t = SpeachThread(wav)
                    speak_t = SpeakerThread(self.__dir, wav, player)
                    speak_t.start()
                    speach_t.start()
                    speak_t.join()
                    speach_t.join()
                    result = f'{speach_t.result},{speak_t.result}\n'
                    self.__socket.send(result.encode())
                    data = data[wav_size:]
                    wav_size = 0
        except Exception as e:
            print(e)
            self.on_kill()
