# H09 · Audio

Los 28 clips de `Assets/Resources/Audio/H09` son originales del proyecto. Se generan de forma determinista con `Tools/H09/generate_audio.py` (biblioteca estándar de Python). No se descargaron archivos ni se incorporaron obras de terceros; autoría y licencia de uso del proyecto. El script conserva frecuencias, envolventes, ruido, duración y semilla de cada clip.

`battle_loop.wav` dura ocho segundos, es mono PCM a 24 kHz y repite en el AudioSource de música. Los otros 27 archivos son efectos breves para interfaz, preparación, impacto, recuperación, estados, muerte y resultado. `storm_charge.wav` dura 2,45 segundos; `storm_impact.wav` se dispara con el impacto real de Tormenta en 1,2 segundos. La futura presentación visual de esa habilidad sigue en H10.

`AudioImportH09` deja la música en Streaming y los efectos en DecompressOnLoad, con muestreo original y sin compresión. `GameAudio` mantiene música, interfaz y seis voces de combate. La música tiene nivel 0,32; interfaz 0,55; combate 0,65. `SessionOptions` aplica el volumen general 0–100 al AudioListener. Los efectos ordinarios tienen picos limitados a 0,82 en el generador. Pausar congela música y voces de combate y deja activa la interfaz. Reiniciar detiene las voces y reinicia la música; volver al menú detiene todo el audio de combate. El audio de resultados detiene música y efectos anteriores.

La comprobación de importación y de reproducción debe distinguirse de la escucha humana. El script comprueba la existencia de todos los archivos; las pruebas de Unity comprueban el flujo y el arnés del player comprueba el ejecutable. Una mezcla satisfactoria requiere además escucha a volumen 0/100 y durante acciones simultáneas.

Para revisar: abrir `MainMenu` o `Builds/H09/UltimaGuardia.exe`, recorrer las cuatro opciones, entrar a ayuda y volumen, jugar, probar un ataque físico, magia, curación, objetos y estados, pausar durante una acción y reanudar, ejecutar Tormenta, ganar o perder, reiniciar y volver al menú.
