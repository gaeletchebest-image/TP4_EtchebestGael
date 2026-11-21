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

## H02 · 21 de noviembre de 2026

Versión **0.0.3-H02**, cambios locales sobre commit real `0333b21` (H01). Autorización del usuario en este chat: «Bien. Entonces hacelo», después de identificar H02 y sus límites. H00/H01 estaban aprobados y sin correcciones abiertas.

### Reglas, integración y regresión

**27/27 pruebas PlayMode aprobadas**, 0 fallidas, 0 omitidas, **76,7128748 s**. Job `bdd40ada85c74329b4d2904ccb607f6b`. [Resultados completos](Evidence/H02/playmode-results.json).

| Tareas | Procedimiento y resultado observado |
| --- | --- |
| H02-T01 a T04 | 18 assets reales integrados: 4 combatientes, 5 acciones de H01, 5 objetos, 3 efectos y encuentro con 2 aliados/2 enemigos, posiciones e inventario inicial. Datos adicionales preparados sin implementar acciones/estados de H03/H04 |
| H02-T05 | Snapshots por intento y existencias independientes. Reinicios restauran recursos, barras, recuperación, cola, contadores y presentación; los assets conservan su configuración |
| H02-T06 | Fixture con velocidad NaN, costo negativo, ID de acción duplicado y prefab ausente: mensajes específicos y creación de sesión rechazada. Se validan además cantidades, equipos, básicos gratuitos, posiciones finitas y referencias de efectos |
| H02-T07/T08/T14 | ATB 0 a 100 por velocidad; velocidad 20 completa en 5 s y velocidad 28 en 100/28 s. Decisión, ejecución, pausa y resultado congelan el reloj. Input mantenido no confirma al quedar lista la maga |
| H02-T09 | Básico 0,6 s, Golpe pesado 2,2 s, Poción 0,8 s. Fixture: después de consumir 0,5 + 0,4 s, el básico acumula 6 ATB a velocidad 20 y el pesado conserva 1,3 s de recuperación |
| H02-T10/T11 | Actualización de 7 s calcula los cruces y encola Maga → Guerrero → Chamán → Ogro. Empate total respeta orden fijo; empate entre distintas velocidades prioriza velocidad. Avanzar durante la cola no duplica turnos ni mueve el reloj |
| H02-T12 | Muerte retira entradas listas y objetivos, conserva un único superviviente válido y detecta el resultado antes de otra decisión |
| H02-T13 | Barras y estados Llenando/Recuperando/Listo integrados en Combat. Tiempo pendiente visible y descripción de costo/recuperación obtenidos de los datos |
| Cierre de datos | En copias de los assets cargados: guerrero con vida 200, maga con velocidad 40, ogro con defensa 10, Golpe pesado con costo 5/potencia 2 y Poción de 17 con 4 unidades producen los valores esperados sin cambiar scripts ni assets originales |
| Regresión H00/H01 | Fórmula 22/44/18, redondeo, límites, objetivos, gasto/impacto únicos, cancelación, falta de maná, Poción compartida, pausa antes/después de impacto, menú, teclado/control virtual y reinicios tras victoria/derrota |

Las peleas se aceleraron a `Time.timeScale = 4`, restaurado al terminar. Teclado virtual: victoria en 26 acciones, 0 Pociones restantes. Control virtual: victoria en 23 acciones, 2 Pociones restantes; se preparó una herida de 1 de vida para habilitar una selección/cancelación de Poción al comienzo. La derrota deja ambos aliados a 1 de vida exclusivamente desde el fixture. No se utilizan estas condiciones para evaluar balance ni duración objetivo.

### Iteraciones y alcance de la evidencia

Una ejecución prematura de 26 pruebas falló porque la llamada de integración había sido rechazada por un parámetro de herramienta faltante y todavía no había encounter asignado. Se integraron los assets y la escena antes de repetir. La siguiente pasada fue 25/26: el test de Poción todavía suponía que después de avanzar el reloj actuaría un aliado; ATB daba correctamente prioridad a enemigos con recuperación más corta. Se corrigió el fixture para respetar la cola. La suite final incorpora además congelación de barras/recuperación y confirmación mantenida hasta estar listo: 27/27.

Las capturas en [Evidence/H02](Evidence/H02/) usan la cámara y Canvas reales sobre RenderTexture, cambiando temporalmente Overlay a ScreenSpaceCamera y restaurándolo después. Se inspeccionaron arena y recuperación a 720p/1080p, avance ATB, selección, pausa, victoria y derrota. Acreditan composición y escalado offscreen; no un monitor físico 1080p. La UI y modelos siguen siendo provisionales.

La inspección detectó H02-C01: las barras nuevas se dibujaban sobre la capa de pausa/resultado. Se corrigió el orden de hermanos del Canvas y se regeneraron e inspeccionaron las capturas. Regresión posterior: **4/4 pruebas de flujo**, 68,7754229 s, job `fb9f4d3c22e94a909d8290e7370530b4`, incluyendo ambas victorias, derrota y reinicios. [Resultado posterior al ajuste visual](Evidence/H02/flow-after-ui-fix.json). No cambió el código runtime ni las reglas respecto de la suite de 27 pruebas.

Control físico, desconexión, arte final, IA completa, estados activos, balance y rendimiento siguen pendientes en sus hitos. No se acreditan por estas pruebas.

### Build y ejecución fuera del editor

Build final `build-76cf7166d7`: Windows x86_64/Mono, Development, `Builds/H02/UltimaGuardia.exe`, **157,04 MB**, **22,5173845 s**, **0 errores / 1 advertencia**. Advertencia: truncamiento implícito del shader `Hidden/Core/DebugOccluder` de Unity Core en d3d11. [Resumen](Evidence/H02/build-summary.json). La primera build (`build-9bde98011b`, 51,2191293 s, mismo tamaño y advertencia) se sustituyó al corregir el orden de las barras bajo pausa/resultado.

Player final ejecutado como proceso oculto con `-foundation-smoke`: [resultado JSON](Evidence/H02/player-result-1280x720.json), `version = 0.0.3-flow`, `initialScene = MainMenu`, `passed = true`, `exitedThroughMenu = true`, [código de salida 0](Evidence/H02/player-exit-code-1280x720.txt). Ventana real 1280 × 720. Se comprobaron navegación, controles, carga de Combat, pausa, regreso al menú y cierre real desde Salir. Se inspeccionaron [Combat](Evidence/H02/player-combat-1280x720.png) y [pausa](Evidence/H02/player-pause-1280x720.png), con materiales y texto completos y barras detrás de la capa de pausa.

Esta prueba del ejecutable utiliza teclado/control virtual y captura offscreen explícita; las peleas completas corresponden a PlayMode. El log conserva Curl error 6 y avisos de liberación de GraphicsBuffer/ComputeBuffer y asignaciones al apagar el proceso oculto, como en H00/H01; no hubo excepción de lógica. No se atribuye una causa del juego no demostrada. Log completo en `Builds/H02/Validation/player-1280x720.log`. Ajustes de entrada a Play y Unity Connect alterados por el entorno de pruebas/build se restauraron al valor recibido; se conservan versión de motor, pipeline, paquetes y entrada precargada. [Huellas de la entrega](Evidence/H02/fingerprints.json).

Revisión final del usuario: **H02 aprobado el 21 de noviembre de 2026**, mediante «Bien. Lo doy por aprobado y te pido que me des un nombre en ingles para el commit». Sin correcciones solicitadas; H03 permanece pendiente.
