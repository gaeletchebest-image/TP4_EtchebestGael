"""Static asset audit; it does not substitute an audible playthrough."""
from pathlib import Path
import json, math, struct, wave

root = Path(__file__).resolve().parents[2]
folder = root / 'Assets/Resources/Audio/H09'
reports = []
for path in sorted(folder.glob('*.wav')):
    with wave.open(str(path)) as wav:
        assert (wav.getnchannels(), wav.getsampwidth(), wav.getframerate()) == (1,2,24000)
        frames = wav.getnframes()
        values = struct.unpack('<%dh' % frames, wav.readframes(frames))
    peak = max(abs(v) for v in values)/32768
    rms = math.sqrt(sum(v*v for v in values)/len(values))/32768
    reports.append(dict(id=path.stem, file=str(path.relative_to(root)).replace('\\','/'),
        author='Última Guardia project', sourceUrl=None, license='Original project asset; no third-party license',
        modifications='Synthesized, enveloped, peak-limited and exported as mono PCM by Tools/H09/generate_audio.py',
        durationSeconds=round(frames/24000,3), peak=round(peak,4), rms=round(rms,4)))
assert len(reports) == 28
assert all(r['peak'] < .85 for r in reports)
loop = next(r for r in reports if r['id']=='battle_loop')
with wave.open(str(folder/'battle_loop.wav')) as wav:
    data=wav.readframes(wav.getnframes())
v=struct.unpack('<%dh'%(len(data)//2),data)
edge=abs(v[0]-v[-1])/32768
max_step=max(abs(v[i]-v[i-1]) for i in range(1,len(v)))/32768
assert edge <= max_step
assert loop['durationSeconds']==8
result=dict(clipCount=len(reports),loopEdgeJump=edge,largestInternalStep=max_step,passed=True,clips=reports)
out=root/'Documentation/Evidence/H09/audio-audit.json'
out.parent.mkdir(parents=True,exist_ok=True)
out.write_text(json.dumps(result,indent=2),encoding='utf-8')
print(f'{len(reports)} clips, max peak {max(r["peak"] for r in reports):.3f}, loop edge {edge:.6f}')
