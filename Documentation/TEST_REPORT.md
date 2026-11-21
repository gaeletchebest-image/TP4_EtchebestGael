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

## H01 · 21 de noviembre de 2026

Versión: **0.0.2-H01**, cambios locales sobre commit real `c843243` (base H00). El usuario aprobó H00 y solicitó H01 en este chat. No se creó un commit de entrega. H02 no se inició.

### Resultado de comprobaciones

- Unity **6000.3.11f1**, URP **17.3.0**, Windows x86_64 / Mono. Se conservan manifiesto, lock, versión del motor, MainMenu y los assets gráficos recibidos.
- **17/17 pruebas PlayMode aprobadas**, 0 fallidas, 0 omitidas, 65,3117192 s. Job `311f013b8a6f483c98ba10fed5f9e7f9`. [Resultado completo](Evidence/H01/playmode-results.json).
- Victoria desde el encuentro normal con teclado virtual: **29 acciones**, 0 Pociones restantes. Victoria con Gamepad virtual: **27 acciones**, 0 Pociones restantes. Los enemigos eligen objetivos al azar; estas muestras no constituyen un estudio de balance.
- Derrota: caso límite preparado exclusivamente desde la prueba, dejando a ambos aliados con 1 de vida; se jugó hasta la muerte de ambos con control virtual y se reinició desde el resultado. No se atribuye esa derrota al balance inicial y no hay un comando de depuración equivalente en el juego.
- Regresión de H00: navegación de MainMenu, controles, carga de Combat, pausa, retorno y confirmación mantenida al cambiar de escena. Los tres tests anteriores se adaptaron a la escena de combate nueva.
- Build Windows: `Builds/H01/UltimaGuardia.exe`, **157,01 MB**, **145,37 s**, **0 errores / 5 advertencias**, job `build-986ee3fc5b`. Una advertencia de shader de Unity Core (`Hidden/Core/DebugOccluder`, truncamiento implícito) y cuatro avisos de reconexión WebSocket de Unity MCP durante la compilación. [Resumen](Evidence/H01/build-summary.json).

### Cobertura por tareas

| Tareas | Comprobación | Resultado |
| --- | --- | --- |
| H01-T01 a T04 | Combat guardada y reabierta, 7 volúmenes ProBuilder de arena, cuatro prefabs con anclajes, sockets de proyectil/efecto, cámara y nombres | Integrados; siluetas completas y nombres visibles en capturas 720p/1080p |
| H01-T05/T06 | Identificadores estables, equipos, límites de recursos y eventos de cambio | Aprobada en CombatRulesTests |
| H01-T07 | Básico guerrero vs ogro 22; Golpe pesado 44; básico maga 18; redondeo 4,5 → 5; mínimo 1 | Aprobada, cinco casos parametrizados |
| H01-T08 | Aliado vivo, enemigo vivo y conjunto de enemigos vivos; se omiten muertos y objetivos ajenos al encuentro | Aprobada; ataques contra aliados rechazados |
| H01-T09/T10 | Acción pendiente única, gasto de 8, rechazo de segunda confirmación y segundo impacto | Aprobada; el cierre sin impacto también se rechaza |
| H01-T11 | Tres Pociones compartidas, curación limitada, sin gasto sobre vida completa, agotamiento | Aprobada en reglas y peleas completas |
| H01-T12/T13 | Enemigos atacan aliados vivos, superviviente único, muertos fuera del orden, victoria y derrota | Aprobada en reglas y PlayMode |
| H01-T14 | Reiniciar durante ejecución pausada, después de victoria y después de derrota | Recursos completos, 3 Pociones, sin acción pendiente, turno inicial y presentación restaurados |
| H01-T15/T16 | Comandos, objetivos, foco textual, navegación circular, Escape contextual, cruceta/stick/botones | Aprobada con dispositivos virtuales; cancelar no gasta ni pausa desde objetivos |
| H01-T17 | Dos victorias normales, derrota forzada, reinicios y rechazo de acciones simultáneas | Aprobada con alcance y condiciones descritos arriba |

### Inspección visual y correcciones

Se inspeccionaron seis capturas: arena a 1280 × 720 y 1920 × 1080, objetivo, pausa, victoria y derrota. El encuadre se ajustó para evitar que el panel inferior tapara los pies de los participantes. La UI usa texto y figuras provisionales, no arte final. [Capturas](Evidence/H01/).

Las capturas PlayMode renderizan la cámara y el mismo Canvas en RenderTexture, cambiando temporalmente ScreenSpaceOverlay a ScreenSpaceCamera y restaurándolo después. Acreditan composición y escalado offscreen, no un monitor nativo 1080p.

La primera pasada de pruebas dio 15/17: al cerrar una acción, el bloqueo de confirmación mantenida también descartaba una pulsación nueva inmediata. Se corrigió el rearme según el estado real del botón. La pasada final de 17/17 incluye ese recorrido; no se presenta la pasada fallida como aprobada.

Consola antes de la build: sin errores ni advertencias. Los avisos de la compilación se distinguen de errores de reglas. No se probó un control físico, desconexión ni el rendimiento final; esas comprobaciones corresponden a hitos posteriores.

### Revisión práctica

Ejecutar `Builds/H01/UltimaGuardia.exe`, elegir Jugar, probar básico/Golpe pesado, cancelar un objetivo, usar una Poción tras recibir daño, pausar y reiniciar, ganar y reiniciar desde resultado. También puede abrirse MainMenu en Unity. Instrucciones completas y controles en README. H01 fue aprobado por el usuario el 21 de noviembre de 2026 mediante el mensaje «Bien. Te doy como el h01 aprobado». Sin correcciones solicitadas. H02 permanece pendiente para otro chat.

### Comprobación del ejecutable H01

Se ejecutó el player Development oculto con `-foundation-smoke`, resolución solicitada y ventana real **1280 × 720**. [JSON](Evidence/H01/player-result-1280x720.json): `initialScene = MainMenu`, `passed = true`, `exitedThroughMenu = true`. [Código de salida](Evidence/H01/player-exit-code-1280x720.txt): **0**. Se revisó visualmente la [captura de Combat del player](Evidence/H01/player-combat-1280x720.png): geometría y materiales presentes, nombres, recursos y comandos completos.

Esta prueba externa cubre menú, controles, carga de Combat, pausa, regreso al menú y Salir, con dispositivos virtuales. Las peleas completas se verificaron en PlayMode, no se atribuyen al ejecutable. Se conservaron los avisos de red, GraphicsBuffer/ComputeBuffer y asignaciones al apagar el proceso oculto en `Builds/H01/Validation/player-1280x720.log`; no hubo excepción de lógica y la prueba salió con código 0. Al terminar se restauró la configuración de entrada precargada y de entrada a Play recibida, que el Test Runner había modificado temporalmente.
