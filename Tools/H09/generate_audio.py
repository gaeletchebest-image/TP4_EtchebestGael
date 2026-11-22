"""Deterministic, original PCM assets for Última Guardia H09. Python standard library only."""
from pathlib import Path
import math, random, struct, wave

ROOT = Path(__file__).resolve().parents[2] / 'Assets/Resources/Audio/H09'
ROOT.mkdir(parents=True, exist_ok=True)
RATE = 24000

def write(name, seconds, tones, noise=0, sweep=0, decay=4, seed=1):
    rng = random.Random(seed)
    count = round(seconds * RATE)
    data = bytearray()
    prev = 0
    for i in range(count):
        t = i / RATE
        x = i / count
        env = min(1, t / .009) * min(1, (seconds-t) / .025) * math.exp(-decay*x)
        sample = sum(math.sin(2*math.pi*(f*t + .5*sweep*t*t) + p)*a for f,a,p in tones)
        n = rng.uniform(-1,1)
        # High-passed noise makes attacks crisp without a DC offset.
        sample += noise * (n-prev)
        prev = n
        sample = max(-.82, min(.82, sample*env))
        data.extend(struct.pack('<h', round(sample*32767)))
    with wave.open(str(ROOT / (name+'.wav')), 'wb') as out:
        out.setnchannels(1); out.setsampwidth(2); out.setframerate(RATE); out.writeframes(data)

def tone(name, f, length=.3, noise=.04, sweep=0, decay=3):
    write(name,length,[(f,.32,0),(f*1.5,.12,.3),(f*2,.07,1)],noise,sweep,decay,seed=sum(map(ord,name)))

for name,f,length,noise,sweep in [
 ('ui_move',660,.09,.005,180),('ui_confirm',520,.16,.004,380),
 ('ui_cancel',390,.16,.005,-180),('ui_invalid',180,.2,.02,-70),
 ('sword_swing',430,.24,.23,-360),('ogre_swing',150,.29,.20,-90),
 ('heavy_swing',250,.36,.27,-210),('sword_hit',175,.22,.23,-80),
 ('ogre_hit',90,.32,.25,-60),('heavy_hit',82,.42,.31,-60),
 ('magic_cast',620,.32,.03,550),('shaman_cast',305,.38,.06,-130),
 ('magic_hit',470,.30,.12,-500),('heal_cast',440,.33,.02,190),
 ('heal_hit',570,.55,.01,120),('mana_hit',360,.52,.02,410),
 ('item_use',760,.19,.04,-150),('poison_apply',190,.42,.12,-60),
 ('poison_tick',155,.26,.12,-100),('haste_apply',700,.45,.03,420),
 ('corrosion_apply',145,.42,.17,-120),('cleanse',510,.46,.02,300),
 ('death',105,.66,.20,-75),('storm_impact',82,.85,.26,-45)]:
    tone(name,f,length,noise,sweep)

write('storm_charge',2.45,[(110,.15,0),(220,.13,1),(330,.10,2)],.025,45,.35,28)
tone('victory',523,.85,.01,0,1.2)
tone('defeat',196,1.0,.02,-35,1.4)

# Eight-second seamless loop: all frequencies are integer multiples of 1/8 Hz.
# Envelope is periodic at its endpoints; percussion repeats inside the cycle.
data = bytearray()
notes = [110,130.75,146.875,165]
for i in range(8*RATE):
    t = i/RATE
    body = 0
    for k,note in enumerate(notes):
        distance = abs((t-(2*k+1)+4)%8-4)
        weight = (1+math.cos(math.pi*distance/2))/2 if distance < 2 else 0
        body += weight*(.105*math.sin(2*math.pi*note*t)+.04*math.sin(2*math.pi*2*note*t))
    pulse = .055*math.sin(2*math.pi*55*t)*math.exp(-18*((t*.5)%1))
    shimmer = .025*math.sin(2*math.pi*440*t)*(1-math.cos(2*math.pi*t/8))
    data.extend(struct.pack('<h',round(max(-.8,min(.8,body+pulse+shimmer))*32767)))
with wave.open(str(ROOT/'battle_loop.wav'),'wb') as out:
    out.setnchannels(1); out.setsampwidth(2); out.setframerate(RATE); out.writeframes(data)
print(f'Wrote {len(list(ROOT.glob("*.wav")))} clips to {ROOT}')
