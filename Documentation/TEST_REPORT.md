# Informe de comprobaciones

## H00 · 21 de noviembre de 2026

Versión de entrega: `0.0.1-H00`, sobre commit inicial `208d5bb`, con cambios locales todavía sin commit. Unity 6000.3.11f1, URP 17.3.0, calidad PC, Windows x86_64, Mono. Se conservan los cambios gráficos previos en PC_RPAsset y Mobile_RPAsset.

## Procedimiento y evidencia

| ID | Tipo | Procedimiento | Resultado / evidencia |
| --- | --- | --- | --- |
| H00-V01 | Inspección | Versión, pipeline activo, plataforma y soporte de build por Unity MCP | 6000.3.11f1; PC_RPAsset, calidad PC; StandaloneWindows64 soportado; 0 controles físicos |
| H00-V02 | Inspección | Git, .meta, serialización y paquetes | ForceText y Visible Meta Files; .gitignore Unity existente; manifiesto y lock presentes |
| H00-V03 | Automática | Compilar scripts y revisar consola | Aprobada: sin errores de compilación; consola final sin errores ni warnings |
| H00-V04 | Integración PlayMode | Menú, selección, confirmar, cancelar, pausa y carga/retorno con teclado virtual | Aprobada: KeyboardNavigatesConfirmsCancelsPausesAndReturns |
| H00-V05 | Integración PlayMode | Cruceta, stick, botones inferior/derecho y menú con Gamepad virtual | Aprobada: VirtualGamepadUsesDpadStickFaceButtonsAndMenu; no sustituye prueba física |
| H00-V06 | Integración PlayMode | Mantener Confirm al cargar Combat | Aprobada: HeldConfirmDoesNotConfirmTheNextScene; no confirma la pantalla nueva |
| H00-V07 | Build | Compilar MainMenu + Combat para Windows64 y Mono | Aprobada: build-661594265f, 162,87 MB, 80,12 s, **0 errores y 0 warnings** en la build final |
| H00-V08 | Ejecutable automatizado | Ejecutar la build real fuera del editor con -foundation-smoke: arranque, navegación, controles, Combat, pausa, retorno y selección de Salir | Aprobada en ambas ejecuciones: initialScene MainMenu, passed true, exitedThroughMenu true y código de proceso 0; teclado y control virtuales |
| H00-V09 | Inspección visual | Revisar cuatro capturas por resolución: menú, controles, Combat y pausa | Aprobada: 8 capturas inspeccionadas, texto completo y selección visible a 1280 × 720 y 1920 × 1080; método y límite de monitor detallados abajo |

No registrar una prueba como aprobada hasta observar su resultado. Las capturas, informes y resultados de esta entrega se guardan en `Documentation/Evidence/H00/` cuando corresponde.

## Alcance exacto de la evidencia

- PlayMode: **3/3 pruebas aprobadas**, 0 fallidas, 7,4515 s. [Resultados](Evidence/H00/playmode-results.json). El primer filtro por nombre de fixture seleccionó 0 pruebas y no se contó como validación; luego se ejecutó la asamblea completa.
- [Build final](Evidence/H00/build-summary.json), versión de producto 0.0.1, identificada como entrega 0.0.1-H00. Las recompilaciones intermedias ajustaron únicamente el arnés Development de validación.
- [Player 720p](Evidence/H00/result-1280x720.json): ventana real y captura de 1280 × 720.
- [Player 1080p](Evidence/H00/result-1920x1080.json): ventana real limitada por Windows a 1366 × 768; captura y escalado de la UI a **1920 × 1080 mediante RenderTexture**. No se afirma una presentación nativa en un monitor 1080p.
- El proceso de prueba estuvo oculto, con Input System virtual. Las capturas usan Camera.Render y el mismo Canvas, temporalmente en ScreenSpaceCamera; la escena normal conserva ScreenSpaceOverlay. Las primeras capturas del backbuffer oculto fueron negras y se descartaron como evidencia visual.
- Computer Use rechazó abrir la aplicación nueva por falta de autorización para esa herramienta. No se automatizó su UI de Windows. La alternativa fue ejecutar la prueba interna del player por proceso y verificar sus archivos y salida.
- Al apagar el player oculto con capturas explícitas, Unity/URP informa GraphicsBuffer y allocaciones nativas pendientes; el entorno restringido genera Curl error 6. No hubo excepciones de lógica ni fallos de las aserciones. Estos avisos se conservan como limitaciones H00-L05, sin atribuirles una causa de juego no demostrada.
- [Huellas SHA-256](Evidence/H00/fingerprints.json) del ejecutable, DLL de juego, escenas e input asocian la evidencia a los archivos entregados. Los logs originales permanecen en Builds/H00/Validation; no se copian datos de red/equipo innecesarios a la evidencia versionable.

## Revisión práctica del usuario

Abrir `Builds/H00/UltimaGuardia.exe` o MainMenu en Unity. Confirmar Jugar para ver Combat vacía, pausar y continuar, regresar al menú, consultar Controles, cancelar y elegir Salir. El foco debe ser visible y conservarse al regresar de Controles. Esta entrega no contiene todavía la pelea de H01.

## Pendientes de otros hitos

Control físico, desconexión/reconexión y partida completa por dispositivo se verifican con el producto completo en H11/H13. H00 comprueba únicamente el menú mínimo. No se midió rendimiento ni balance del combate porque todavía no existe.
