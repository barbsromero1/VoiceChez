import re
import threading
from typing import List, Union, Tuple

from lib.inference import transcribe


PIEZAS = {
    "alfil": "B",
    "caballo": "N",
    "torre": "R",
    "reina": "Q",
    "dama": "Q",
    "rey": "K",
    "peon": "P"
}
NUMEROS = {
    "uno": 1,
    "una": 1,
    "dos": 2,
    "tres": 3,
    "cuatro": 4,
    "cinco": 5,
    "seis": 6,
    "siete": 7,
    "ocho": 8,
    "nueve": 9,
    "diez": 10
}
ACENTOS = {
    "á": "a",
    "é": "e",
    "í": "i",
    "ó": "o",
    "ú": "u",
    "ü": "u"
}


def clean_text(text: str):
    text = text.lower()
    text = [ACENTOS.get(c, c) for c in text]
    return "".join(text).strip()


def create_regex(options: List[str]):
    exp = "|".join([f"({c})" for c in options])
    return re.compile(exp)


re_piezas = create_regex(PIEZAS.keys())
re_numeros = create_regex(NUMEROS.keys())


def get_part(text: str, pattern: re.Pattern):
    try:
        match = list(pattern.finditer(text))[0]
        span = match.span(0)
        return match[0], span[0], span[1]
    except IndexError:
        return None, 0, 0


def get_letter(text: str):
    try:
        letra = text.strip().split(" ")[-1][0]
        if letra == 'v':
            letra = 'b'
        if letra == 's' or letra == 'z':
            letra = 'c'
        if 'a' <= letra <= 'h':
            return letra
        return None
    except IndexError:
        return None


def _get_coord(text: str) -> Tuple[str, int]:
    number, start_pos, end_pos = get_part(text, re_numeros)
    if number is None:
        return '', 0
    letter = get_letter(text[:start_pos])
    if letter is None:
        return '', 0
    coord = f'{letter}{NUMEROS.get(number)}'
    return coord, end_pos


def get_move(text: str) -> Union[Tuple[str, str], None]:
    text = clean_text(text)
    piece, _, end_pos = get_part(text, re_piezas)
    piece = PIEZAS.get(piece, None)
    text = text[end_pos:]
    coords_0, end_pos = _get_coord(text)
    if not coords_0:
        return None
    if piece is not None:
        return piece, coords_0
    text = text[end_pos:]
    coords_1, _ = _get_coord(text)
    if not coords_1:
        return None
    return coords_0, coords_1


class SpeachThread(threading.Thread):

    def __init__(self, filepath: str) -> None:
        super().__init__(None)
        self.__file_path = filepath
        self.result = ''

    def run(self):
        transcription = transcribe(self.__file_path)
        move = get_move(transcription)
        if move is None:
            self.result = 'ERR'
        else:
            self.result = f'{move[0]},{move[1]}'
        print(f'{transcription}  =>  [{self.result}]')
