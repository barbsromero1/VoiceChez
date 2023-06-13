import socket
from time import sleep


def read_wav(filename: str):
    with open(filename, 'rb') as wav:
        return wav.read()


wav = read_wav('./coordenadas.wav')
wav_size = len(wav)


s = socket.socket()
s.connect(('localhost', 12345))

# al conectarse se manda esta palabra para verifacar la usuario
s.send(b'ajedrez')


# se indica si blanca o negra y el tamaño del archivo wav
data = f'b,{wav_size}'
# se hace un padding de espacios para que el mensaje sea de tamaño 25
data = data.ljust(25)
print(f'"{data}"')

data = data.encode()  # se debe enviar en UTF8
data += wav  # se concatena el binario del archivo


txt = f'b,{wav_size}'.ljust(25).encode()
data += txt
data += wav

s.send(data)  # se envía el mensaje

sleep(10)
s.close()
