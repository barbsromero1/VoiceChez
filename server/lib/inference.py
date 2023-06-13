import torch
from huggingsound import SpeechRecognitionModel
from nemo.collections.asr.models import EncDecSpeakerLabelModel

WAV2VEC2 = "jonatasgrosman/wav2vec2-large-xlsr-53-spanish"
VERI_MODEL = "nvidia/speakerverification_en_titanet_large"

device = "cuda" if torch.cuda.is_available() else "cpu"
print(f"Using {device}")

model = SpeechRecognitionModel(WAV2VEC2, device=device)
speaker_model = EncDecSpeakerLabelModel.from_pretrained(VERI_MODEL)


def transcribe(file: str):
    transcription = model.transcribe([file])
    return transcription[0]["transcription"]


def verify_speaker(audio_1: str, audio_2: str):
    return speaker_model.verify_speakers(audio_1, audio_2)
