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

## H03 · 21 de noviembre de 2026

Versión **0.0.4-H03**, cambios locales sobre `52b3306` (H02). Autorización: «Bien. Entonces implementalo». H02 aprobado y sin correcciones abiertas. Unity 6000.3.11f1, URP 17.3.0, Windows x86_64/Mono; se conservan motor, pipeline, paquetes y arena recibidos.

### Resultado y cobertura

**49/49 pruebas PlayMode aprobadas**, 0 fallidas, 0 omitidas, **88,2797253 s**. Job `00c3fe8fd4fb4ff2a9a10eaa1902fa4b`. [Resultados completos](Evidence/H03/playmode-results.json). Consola sin errores ni advertencias antes de compilar la build.

| Tareas | Método y resultado observado |
| --- | --- |
| H03-T01/T02/T12/T14/T15 | Encuentro real con diez ActionData distintos; costos, potencias, cantidades y recuperaciones del GDD. Veneno preparado con efecto sin daño directo, bloqueado hasta H04 |
| H03-T03 | Curación a guerrero y propia, límite al máximo, rechazo de enemigo/vida completa, gasto único de 10 y recuperación 1,2 s. Recorrido de UI con herida preparada de 50: maga 60 → 96 y maná 45 → 35 |
| H03-T04/T06 | Tormenta a uno/dos enemigos: costo único 18, daños 32 al ogro y 34 al chamán; repetición de confirmación/impacto rechazada; pausa congela impacto; recuperación 2,8 s; victoria cierra presentación sin reanudar ATB |
| H03-T05/T11 | Input virtual real recorre raíz, Habilidades, Objetos y objetivos. Mantener confirmar no ejecuta el objetivo automáticamente; cancelar conserva recursos y selección. Cinco objetos consultables; bloqueo por recurso completo o dependencia H04. Capturas de grupo y lista en 720p/1080p |
| H03-T07/T08/T10 | Cantidades compartidas 3/2/1/1/2, Poción de 50, Éter de 20, cero maná y recuperación 0,8 s. Éter del guerrero restaura a la maga con límite de 18 faltantes; rechazo de recurso completo y cero unidades. Cancelar Éter en UI conserva unidades |
| H03-T09 | Datos/equipo y bloqueo explícito de Tónico, Frasco y Antídoto. No consumen unidades, maná ni turno. No se acredita aplicación de estados |
| H03-T13 | Diez acciones consecutivas del ogro verifican Golpe/Machetazo, tres gastos de 8, daño y recuperación; agotado el maná continúa alternando intentos con Golpe. Segunda confirmación no avanza contador |
| H03-T14/T16 | Ogro a 120: no cura; a 119 y maná 10 o 36: cura 40, cobra 10 y recupera 1,5 s; maná 9/0 u ogro muerto: Descarga. Curación oscura rechaza al chamán y se limita al máximo del ogro. Veneno sigue indisponible: su prioridad está condicionada a ejecución y H04 completa la selección sin veneno |
| H03-T17/T18 | Semilla 1, 100 decisiones por enemigo: ambos aliados reciben objetivos, dentro de 30–70 por aliado; selección única conservada al rechazar una confirmación repetida. Un único aliado superviviente es el único objetivo. No se interpreta esta muestra como prueba estadística general |
| Reinicio/regresión | Nueva sesión restaura recursos, inventario, ATB, recuperación y contadores, con snapshots independientes de cambios de assets. Reinicio en pausa de Tormenta limpia pendientes y marcadores. Regresiones de fórmula, reloj, cola, muerte, menú, pausa, confirmación mantenida, teclado y control virtual |

Las dos peleas normales usan la configuración de producto y priorizan al chamán con básicos, Golpe pesado, Curación, Tormenta o Poción según recursos y heridas. Teclado virtual: **19 acciones, 2 Pociones**; control virtual: **22 acciones, 0 Pociones**. Se aceleran a `Time.timeScale = 4`, restaurado después. La derrota parte de ambos aliados a 1 de vida exclusivamente en el fixture. Estas comprobaciones acreditan flujo y recursos, no balance, duración objetivo ni hardware físico.

### Integración y evidencia visual

La primera petición de integración por código no encontró el nuevo tipo de editor porque el refresh de solo scripts no había importado el archivo nuevo. Se forzó refresh de assets, se comprobó compilación sin errores y se integró antes de iniciar pruebas. La única ejecución de la suite fue **49/49**; no se cuentan resultados de una configuración sin integrar.

Se inspeccionaron selección de Curación propia y Éter, lista de objetos a 720p, grupo de Tormenta a 1080p, pausa de Tormenta y victoria. Los nombres, costos, recuperaciones, cantidades, equipos y marcadores son legibles y completos. La captura usa cámara y Canvas reales sobre RenderTexture, cambiando temporalmente Overlay a ScreenSpaceCamera y restaurándolo después; no acredita monitor físico 1080p. [Capturas](Evidence/H03/). La UI/modelos continúan siendo provisionales.

### Build

Build `build-aaabbd3673`: `Builds/H03/UltimaGuardia.exe`, Windows x86_64/Mono, Development, **163,17 MB**, **57,2910924 s**, **0 errores / 1 advertencia**. [Resumen](Evidence/H03/build-summary.json). La advertencia corresponde al truncamiento implícito del shader `Hidden/Core/DebugOccluder` de Unity Core (d3d11); no es un error de lógica de combate.

Player real ejecutado como proceso oculto con `-foundation-smoke`, ventana real y captura 1280 × 720: [JSON](Evidence/H03/player-result-1280x720.json), `version = 0.0.4-flow`, `initialScene = MainMenu`, `passed = true`, `exitedThroughMenu = true`, [código de salida 0](Evidence/H03/player-exit-code-1280x720.txt). Se verifica navegación, ayuda de controles, carga de Combat, pausa con teclado/control virtual, retorno y cierre desde Salir. Se inspeccionó la [captura de Combat del player](Evidence/H03/player-combat-1280x720.png), con materiales y textos presentes; también se conserva [pausa](Evidence/H03/player-pause-1280x720.png). Esta comprobación externa no juega una pelea completa: las peleas anteriores son PlayMode.

El log `Builds/H03/Validation/player-1280x720.log` conserva Curl error 6 y avisos de liberación de GraphicsBuffer/ComputeBuffer y asignaciones persistentes/JobTempAlloc al cerrar el proceso oculto con capturas explícitas, como en H00–H02. Sin excepción de lógica observada, smoke aprobado y salida 0; no se atribuye una causa del juego no demostrada. Evaluación interactiva/prolongada pendiente de H12. Entrada a Play y asset de input precargado conservados; Unity Connect restaurado al valor recibido tras la build. [Huellas de archivos entregados](Evidence/H03/fingerprints.json).

H03 fue **aprobado por el usuario el 21 de noviembre de 2026**, mediante «Bien. Queda aprobado. Dame nombre en ingles para el commit». Sin correcciones solicitadas; H04 permanece pendiente. Estados completos y selección de Veneno/Antídoto según estado real se verifican en H04; secuencia final de Tormenta en H10; control físico/desconexión en H11; balance y rendimiento en H12.


## H04 · Veneno, rapidez y corrosión — 21 de noviembre de 2026

### Suite e integración

Unity 6000.3.11f1, URP 17.3.0, Input System 1.19.0. Escena Combat existente y encuentro real de `Assets/Data/H02/Resources/Encounters/`. Integración explícita con `StatusSetup.Install`, sin regenerar arena ni reiniciar configuración. Compilación de scripts sin errores.

Suite PlayMode `09ee960906414054887eda50084c9e17`: **68/68 aprobadas**, 0 fallidas/omitidas, **119,2678023 s**. [Resultado completo](Evidence/H04/playmode-results.json). Incluye regresiones de H00–H03 y `StatusRulesTests`.

Después del ajuste visual final: **4/4 pruebas de flujo aprobadas**, 0 fallidas/omitidas, **41,299563 s**, job `842711d79d0a4c7ba9f98faebe245697`. [Resultado](Evidence/H04/flow-after-ui-fix.json). Se repiten cancelación/pausa/reinicio, menús y grupos, muerte por veneno antes del menú, y uso de los tres consumibles con coexistencia y reinicio.

### Cobertura observada

| Tareas | Comprobación y resultado |
| --- | --- |
| H04-T01/T10/T14 | Tres estados simultáneos, duración e identidad independientes. Renovar Rapidez durante la propia acción cambia identidad, conserva multiplicador 1,5, repone dos cargas y no reduce Veneno ni Corrosión. No hay acumulación de potencia |
| H04-T02/T13 | Maga recibe exactamente 8 antes de sus primeros tres turnos (110 → 102 → 94 → 86); cuarto inicio sin daño. Corrosión coexistente no cambia el daño fijo. Veneno se elimina al consumir la tercera activación |
| H04-T03 | Daño letal al inicio del turno aliado/enemigo produce derrota/victoria antes de menú o IA, sin gastar maná ni completar acciones. Cola vacía al finalizar; sin pendiente ni actor. Muerte de una maga en cola permite actuar al guerrero vivo; muertes consecutivas no generan reentrada |
| H04-T04/T15 | Veneno cuesta 6, recupera 1,2 s y aplica tres cargas sin daño directo. Selección excluye aliados muertos/envenenados. Con 5 de maná usa básico; con 6 envenena; con ambos envenenados y 30 de maná usa Descarga. Curación oscura conserva prioridad, umbral estricto 119/120, costo 10, objetivo ogro y fallback cuando está muerto |
| H04-T05 | Antídoto exige aliado vivo envenenado; elimina al confirmar, cobra una sola unidad, no gasta maná ni devuelve los 8 del inicio. Un segundo commit se rechaza; siguiente turno sin daño |
| H04-T06/T08/T09/T13 | Tónico propio termina con 2 cargas, después 1 y 0. Velocidad 28 → 42 en UI; fixture 100 → 150 para medir reloj. Mantiene recuperación 0,8 s. Tónico a otro aliado conserva ATB 21 y cambia velocidad 21 → 31,5; 0,1 s agrega 3,15 de ATB |
| H04-T07/T08/T13 | Frasco a enemigo vivo, no aliado. Defensa ogro 6 → 3 en escena. Fixture defensa 7 → 3,5 conserva decimal; Proyectil da 21 al redondear 20,5. La defensa se restaura después de la segunda acción del afectado, no de las acciones ajenas |
| H04-T11 | Muerte limpia todos los estados, ATB y recuperación. Muerte del aplicador conserva Veneno en la víctima viva. Nueva sesión restaura inventario y efectos vacíos; snapshot activo no cambia al editar el asset del fixture. Reinicio real desde pausa y resultado limpia iconos, modelos, marcadores y cámara |
| H04-T12 | Nombres, iconos vectoriales provisionales y contadores reales en 720p/1080p. Gota verde, flecha azul y escudo roto naranja; fondos independientes, filas compactadas y unidades de activaciones/acciones |
| Regresiones | Fórmula, objetivos, recursos, costo único, impactos duplicados, Tormenta con uno/dos enemigos, ATB, cola, recuperación, IA del ogro, cancelación, mantener confirmar, teclado y control virtual, victoria, derrota y reinicio |

### Flujo e inspección visual

`StatusObjectsWorkWithPadAndKeyboardAndShowCoexistingCounters` usa el control virtual para seleccionar/cancelar/confirmar Tónico propio; teclado para Frasco y Antídoto. Comprueba 42 de velocidad, defensa 3 del ogro, consumo compartido, pausa antes del impacto sin aplicación anticipada y limpieza inmediata de Veneno al confirmar. El fixture agrega Veneno/Corrosión a la maga para mostrar los tres iconos juntos, con nombres, contadores y fuentes del juego; no cambia assets de producto. `PoisonDeathBeforeTurnShowsResultWithoutMenuAndRestartClearsMarkers` prepara guerrero a 8 y elimina a la maga; su primer turno termina por Veneno, con 0 acciones ejecutadas, sin actor/marcadores y reinicio limpio.

Inspeccionadas [coexistencia 720p](Evidence/H04/statuses-coexist-1280x720.png), [1080p](Evidence/H04/statuses-coexist-1920x1080.png), pausa tras Frasco y [derrota por Veneno](Evidence/H04/poison-lethal-defeat.png). El primer fondo ampliado dejaba nombres 3D detrás de los contadores. Se sustituyó por fondo opaco propio por fila, compactando estados activos y conservando el encuadre original de arena. La pasada final de 4/4 corresponde a ese ajuste. Iconos y recursos se leen completos; modelos y UI siguen provisionales.

Victorias normales: teclado virtual **22 acciones, 0 Pociones**; control virtual **22 acciones, 0 Pociones**. Ambas desde estadísticas e inventario iniciales del producto; decisiones inmediatas y aceleración ×4 solo durante pruebas. Derrota adicional con aliados preparados a 1 de vida. Son validación de flujo, no balance ni duración objetivo.

Las capturas usan cámara y Canvas reales sobre RenderTexture, cambiando temporalmente Overlay a ScreenSpaceCamera y restaurándolo. No acreditan monitor físico 1080p. Keyboard/Gamepad son virtuales; no se acredita dispositivo físico.

### Correcciones de la verificación

La primera ejecución completa, `e1bf403f08a84f9794ebac6c2af6adde`, se interrumpió tras tres fallos de navegación y un recorrido sin progreso: el editor sin foco no entregaba eventos virtuales a la Game View. Se corrigió el fixture de input, guardando/restaurando `editorInputBehaviorInPlayMode` y usando temporalmente `AllDeviceInputAlwaysGoesToGameView`, además de IgnoreFocus. No se modificaron las vinculaciones del producto. El recorrido de cancelación pasó después del ajuste.

La pasada dirigida `9e85cbc6c4614c06b44c2af5d3c5456a` produjo **2/17 aprobadas y 15 errores de fixture**: al clonar datos se duplicaba el asset lógico de Veneno referenciado por acción y Antídoto, y el validador rechazó los IDs duplicados antes de crear sesión. Se corrigió el clonado compartido con un diccionario por asset; no se relajó la validación. [Resultados iniciales](Evidence/H04/initial-targeted-results.json). La suite final de 68/68 y las cuatro regresiones sustituyen esas pasadas incompletas/fallidas.

### Build y player

Build `build-91496c0737`: `Builds/H04/UltimaGuardia.exe`, Windows x86_64/Mono, Development, **163,18 MB**, **77,8586972 s**, **0 errores / 1 advertencia**. [Resumen](Evidence/H04/build-summary.json). La advertencia es truncamiento implícito de vector en `Hidden/Core/DebugOccluder`, shader de Unity Core/d3d11, confirmado en `BuildReport.GetLatestReport()` después de que el historial MCP de jobs dejara de conservar el ID. [Detalle del informe](Evidence/H04/build-report-details.json). No se modificaron shaders ni paquetes.

Player real ejecutado como proceso oculto con `-foundation-smoke`: ventana y captura **1280 × 720**, `version = 0.0.5-flow`, inicio `MainMenu`, `passed = true`, `exitedThroughMenu = true` y [código de salida 0](Evidence/H04/player-exit-code-1280x720.txt). [Resultado](Evidence/H04/player-result-1280x720.json). Comprueba menú, controles, carga de Combat, pausa con teclado/control virtual, retorno al menú y cierre desde Salir. Se inspeccionó [Combat del player](Evidence/H04/player-combat-1280x720.png), con materiales y textos presentes; también se conserva [pausa](Evidence/H04/player-pause-1280x720.png). Las peleas completas corresponden a PlayMode, no a esta prueba externa.

`Builds/H04/Validation/player-1280x720.log` conserva Curl error 6 y avisos de GraphicsBuffer/ComputeBuffer, asignaciones persistentes y JobTempAlloc al apagar el proceso oculto con capturas explícitas, como en las entregas anteriores. Sin excepción de lógica observada; flujo aprobado y salida 0. No se atribuye una causa de juego no demostrada; seguimiento interactivo/prolongado en H12.

Se conserva la configuración de entrada a Play recibida; los fixtures restauran entrada del editor, BackgroundBehavior, ejecución en segundo plano y timeScale. Unity Connect se restauró al valor recibido (`m_Enabled = 0`) después de la build. [Huellas de archivos](Evidence/H04/fingerprints.json). H04 fue **aprobado por el usuario el 21 de noviembre de 2026**, mediante «Bien! Queda aprobado entonces.». Sin correcciones solicitadas. H05 no iniciado en aquella entrega.

## H05 · Personajes finales preparados para animar

Versión **0.0.6-H05**, 21 de noviembre de 2026. Cambios locales sobre commit real `3e1bec1` de H04; no se creó commit de entrega. Unity 6000.3.11f1, URP 17.3.0, Windows x86_64/Mono. Alcance exclusivo H05: modelos, armas, materiales, representación articulada, fuentes e integración. No se implementaron clips ni controladores H06.

### Auditoría de recursos

`CharacterArtValidation.AuditAndCapture()` ejecutado desde el editor sobre Combat guardada. [Resultado](Evidence/H05/model-audit.json): cuatro instancias H05 y referencias de datos coherentes; 16 articulaciones únicas por modelo y poses locales guardadas; arma ligada a mano derecha; sockets de conjuro, objeto e impacto; mallas persistidas, vértices finitos, índices válidos, triángulos no degenerados y normales completas; materiales URP; mínimo Y = 0; exportaciones OBJ/MTL/jerarquía presentes e importadas.

| Modelo | Triángulos | Renderers | Materiales usados | Altura con accesorios |
| --- | --- | --- | --- | --- |
| Guerrero | 1.812 | 37 | 8 | 2,52 m |
| Maga | 2.132 | 41 | 9 | 2,71 m |
| Ogro | 1.734 | 32 | 8 | 3 m |
| Chamán | 1.920 | 32 | 9 | 2,37 m |

Se comprobaron además los cuatro OBJ importados: todos sus materiales remapeados a URP/Lit. Exportaciones estáticas separadas del rig de piezas rígidas conservado en prefab y JSON. [Dirección visual y fuentes](H05_CHARACTER_ART.md), [registro de recursos](ASSET_REGISTER.md).

### Inspección visual

Inspeccionadas las vistas [frontal](Evidence/H05/character-lineup-front.png) y [trasera](Evidence/H05/character-lineup-back.png), y las hojas de [guerrero](Evidence/H05/warrior-pose-sheet.png), [maga](Evidence/H05/mage-pose-sheet.png), [ogro](Evidence/H05/ogre-pose-sheet.png) y [chamán](Evidence/H05/shaman-pose-sheet.png). Cada hoja contiene reposo, ataque, conjuro, reacción, objeto y caída, en ese orden. Armas siguen el agarre; extremidades, máscara, casco, túnicas segmentadas y focos permanecen unidos. Las poses son evidencia de rango articular, no animaciones jugables ni todas las coreografías finales.

La primera hoja del ogro quedó recortada; se corrigieron encuadre y aspecto explícito de captura y se inspeccionaron de nuevo las cuatro hojas. La primera exportación OBJ carecía de normales explícitas; se reexportó con normales y materiales remapeados. El encuadre heredado ocultaba el sombrero de la maga bajo tres estados; se ajustó la cámara a FOV 50, mirando a (0; 0,1; 2), conservando posición, arena y anclajes.

Capturas finales [720p](Evidence/H05/arena-1280x720.png), [1080p](Evidence/H05/arena-1920x1080.png), tres estados [720p](Evidence/H05/statuses-coexist-1280x720.png) y [1080p](Evidence/H05/statuses-coexist-1920x1080.png), objetivos, listas, pausa y reinicio. Siluetas, armas y modelos se mantienen dentro del área de combate. Cámara, Canvas y RenderTexture reales; no acredita monitor físico 1080p.

### Regresión de combate

**68/68 pruebas PlayMode aprobadas**, 0 fallidas/omitidas, **117,155875 s**, job `2f8f396d49b84754b8f538c24cb99b18`. [Resultado completo](Evidence/H05/playmode-results.json). Incluye las regresiones H00–H04: fórmulas, ATB, recuperación, cola, costos, inventario, IA, estados, pausa, confirmación, navegación, victoria, derrota y reinicios. El único cambio del fixture existente es escribir capturas en H05 para conservar la evidencia histórica H04.

Victoria normal con teclado virtual: 21 acciones, 1 Poción restante. Con control virtual: 22 acciones, 0 Pociones. Derrota con fixture de aliados a 1 de vida y muerte por veneno con último aliado a 8. Peleas ×4 para automatizar flujo, sin acreditar hardware, balance ni duración objetivo.

Después del ajuste de cámara: **4/4 regresiones de flujo aprobadas**, job `ab768f9dfe5044d1afc8d9a566c77dc0`. Cancelación/pausa/reinicio, listas/selección grupal/objetos, coexistencia de estados y muerte por veneno/reinicio. [Estado final del job](Evidence/H05/flow-after-framing-fix.json): `succeeded`, 4 completadas de 4, sin fallos; el resultado NUnit detallado dejó de estar disponible tras la recarga de dominio, por lo que no se atribuye una duración individual ni se inventa el resumen perdido.

### Build y player real

Build `build-56b0a79878`: `Builds/H05/UltimaGuardia.exe`, Development, **164,21 MB**, **153,4675862 s**, **0 errores / 1 advertencia**. [Resumen](Evidence/H05/build-summary.json), [BuildReport](Evidence/H05/build-report-details.json). Advertencia heredada de `Hidden/Core/DebugOccluder` de Unity Core/d3d11; sin modificar paquetes ni shaders.

Player real oculto con `-foundation-smoke`: versión `0.0.6-flow`, inicio MainMenu, ventana/captura 1280 × 720, `passed = true`, `exitedThroughMenu = true`, [salida 0](Evidence/H05/player-exit-code-1280x720.txt). [Resultado](Evidence/H05/player-result-1280x720.json). Comprueba menú, ayuda, carga de Combat, pausa, retorno y salida con dispositivos virtuales. [Combat](Evidence/H05/player-combat-1280x720.png) inspeccionada sin materiales faltantes; [pausa](Evidence/H05/player-pause-1280x720.png) conservada. Las peleas completas corresponden a PlayMode, no al smoke externo.

`Builds/H05/Validation/player.log` conserva Curl error 6 y avisos de liberación de GraphicsBuffer/ComputeBuffer, asignaciones persistentes y JobTempAlloc al cerrar el proceso oculto con capturas, como en H00–H04. Sin excepción de lógica observada; seguimiento en H12. No se presenta esta ejecución como prueba de rendimiento ni de hardware físico.

Configuraciones de entrada y tiempo restauradas por fixtures; configuración de entrada a Play conservada y Unity Connect restaurado a `m_Enabled = 0` después de la build. Recursos temporales del paquete de pruebas retirados automáticamente por su postproceso. [Huellas de entrega](Evidence/H05/fingerprints.json). H05 aprobado por el usuario el 21 de noviembre de 2026, mediante «Bien. Lo doy por aprobado.». Sin correcciones solicitadas. H06 no iniciado.

## H06 · Animaciones, impactos y limpieza de presentación

Versión **0.0.7-H06**, 21 de noviembre de 2026. Cambios locales sobre commit real `34592c6` de H05; sin commit de entrega. Unity 6000.3.11f1, URP 17.3.0, Windows x86_64/Mono. H06 aprobado por el usuario el 21 de noviembre de 2026, mediante «Bien. Esta aprobado. Dame ademas un nombre en ingles para el commit.». Sin correcciones solicitadas. H07 no iniciado.

### Recursos e inspección visual

`CharacterAnimationAuthoring.BuildAndIntegrate()` produjo 24 `.anim`, cuatro controladores y cuatro conjuntos de datos, conservando los modelos H05. `CharacterAnimationValidation.AuditAndCapture()` completó la [auditoría](Evidence/H06/animation-audit.json): 16 articulaciones y 112 curvas por clip; ausencia de eventos de gameplay; controladores conectados; muerte sin salida; root motion desactivado; retorno de posiciones y rotaciones al final de clips no terminales; tiempos de lanzamiento/impacto dentro de la duración; caída sobre suelo, mínimo Y = 0 en los cuatro modelos.

Inspeccionadas las hojas de [guerrero](Evidence/H06/warrior-animation-sheet.png), [maga](Evidence/H06/mage-animation-sheet.png), [ogro](Evidence/H06/ogre-animation-sheet.png) y [chamán](Evidence/H06/shaman-animation-sheet.png). Filas en el orden del JSON, ocho muestras temporales por clip, cámara fija por fila. Se observan reposos, preparación/golpe/regreso, elevación de bastón, gesto de objeto, reacción y caída; armas y ropa segmentada siguen unidas. No se presentan estas hojas como una partida jugada.

Capturas de runtime por cada caso de la matriz y de los cuatro derrotados muestran el uso de los clips con los modelos finales en la arena real. Las capturas de CombatFlowTests comprueban HUD, selección, estados, pausa y resultados a 720p/1080p con RenderTexture. Capturas del ejecutable a 720p inspeccionadas. No demuestra monitor físico 1080p ni una nueva prueba con control físico.

### Pruebas automáticas

Suite PlayMode `UltimaGuardia.PlayModeTests`: **71/71 aprobadas**, 0 fallidas/omitidas, **148,2203066 segundos**, job `a5e373a2386d4d4f95924c960b2e80b9`. [Resultado completo](Evidence/H06/playmode-results.json).

- Las 68 pruebas existentes conservan cobertura de reglas, ATB, cola, gasto, IA, estados, foco, pausa y flujo.
- `EveryActionAndBothAlliesItemsAnimateWithSingleImpactAndFixedRecovery` recorre **20 casos**: diez acciones y los cinco objetos por cada aliado. Comprueba pose visible, ausencia de impacto anticipado, objetivos, cantidad de impactos, rechazo de señal duplicada, gasto único, maná recuperado por Éter, pausa tras impacto, recuperación original y anclajes sin deriva. [Matriz](Evidence/H06/action-matrix.txt).
- `ProjectilePauseCancelAndRestartCannotAffectNextSession` pausa antes del impacto con proyectil en vuelo, comprueba inmovilidad, reinicia y espera sin daño heredado; luego abandona a MainMenu y verifica ausencia de personajes/proyectiles antiguos.
- `DeathHoldsFinalPoseRejectsNewActionsAndRestartRestoresAllJoints` derrota a los cuatro participantes, exige rotación efectiva de cadera, conserva la pose terminal aunque termine una acción y comprueba restauración de todas las articulaciones al reiniciar.

Los fixtures de presentación usan velocidades preparadas para aislar cada actor y pasos explícitos de reloj; son comprobaciones de integración, no partidas naturales. Las regresiones de flujo usan input virtual y avanzan peleas a ×4: victoria con teclado en 22 acciones/0 Pociones y con control virtual en 22 acciones/0 Pociones; derrotas preparadas y reinicios aprobados. No acreditan balance ni duración objetivo de tres a cinco minutos.

Historial: primera ejecución dirigida `130ab3d7d92840d4bee0599711bf4944`, **2/3**, falló una expectativa del fixture que exigía no recuperar maná con Éter. Corregida la expectativa, segunda dirigida `67c0045fd47b45dd9ff67837dfc523c0`, **3/3**, 12,3632058 s. Después se unificó el tiempo de muestreo del actor con el controlador de impacto y la suite completa final pasó 71/71. Se corrigieron además la nulidad Unity en autoría y las capturas que requerían esperar un frame para actualizar matrices de render. No se conservan afirmaciones de aprobación de las primeras pasadas fallidas.

### Build y player real

Build Windows x86_64/Mono, Development, **164,68 MB**, **93,3874899 s**, **0 errores y 1 advertencia** del shader DebugOccluder de Unity Core/d3d11. Job `build-bc35a877d5`; [resumen](Evidence/H06/build-summary.json), [detalles del BuildReport](Evidence/H06/build-report-details.json). MainMenu y Combat, en ese orden.

`Builds/H06/UltimaGuardia.exe -foundation-smoke -screen-fullscreen 0 -screen-width 1280 -screen-height 720` completó MainMenu, consulta de controles, Combat, primer básico animado de la maga, pausa de ejecución con reloj/animación congelados, reanudación y daño, pausa con ambos dispositivos virtuales, retorno al menú y Salir. **passed = true, animatedActionVerified = true, exitedThroughMenu = true, salida 0**. [Resultado](Evidence/H06/player-result-1280x720.json), [pausa de la acción](Evidence/H06/player-animated-action-paused-1280x720.png), [acción terminada](Evidence/H06/player-animated-action-complete-1280x720.png).

El smoke usa un proceso oculto con captura explícita por Camera.Render; no es automatización de UI de Windows ni una pelea completa del player. Los logs del ejecutable en `Builds/H06/Validation/player.log` conservan Curl error 6 y avisos de buffers/asignaciones al apagar, ya observados en H00–H05. No se observó excepción de lógica; causa y rendimiento prolongado siguen pendientes H12.

Los fixtures restauran enrutamiento de entrada del editor, BackgroundBehavior, ejecución en segundo plano y timeScale. Opciones de entrada a Play restauradas al valor recibido (habilitadas, opciones 0); Unity Connect restaurado a `m_Enabled = 0` después de la build. Sin cambios de estadísticas, costos, inventario o balance. [Huellas de entrega](Evidence/H06/fingerprints.json), [reconstrucción y tiempos](H06_ANIMATIONS.md), [incidencias y límites](KNOWN_ISSUES.md).

## H07 · Fortaleza final, contactos y encuadre

Versión **0.0.8-H07**, 21 de noviembre de 2026, cambios locales sobre commit real `4a11d47` de H06. No se creó un commit de entrega. Unity 6000.3.11f1/URP 17.3.0, Windows x86_64/Mono. H00–H07 aprobados. H07 aprobado por el usuario el 21 de noviembre de 2026 mediante «Bien. Queda aprobado. Dame nombre en ingles para el commit tambien.». Sin correcciones solicitadas. H08 no iniciado.

### Auditoría e inspección visual

`FortressValidation.AuditAndCapture()` completó la [auditoría final](Evidence/H07/environment-audit.json): siete tipos de módulos y 56 instancias, 25.650 triángulos, ocho materiales compartidos, una luz con sombras y relleno sin sombras. Mallas guardadas, UV y normales completos, vértices finitos y triángulos no degenerados. Los siete OBJ se importaron y remapearon a los materiales URP propios. Blockout retirado; prefabs H05, configuración del encuentro y los cuatro anclajes conservados.

Se muestrearon los **24 clips en 216 poses**, incluidas caídas, armas y reposos. Se comprobaron márgenes de viewport y **648 líneas de visión** hacia cabeza, torso y punto de impacto mediante intersección real contra los triángulos del entorno. Esta auditoría geométrica verifica oclusión por escenario, no sustituye la inspección visual ni afirma visibilidad entre todos los personajes en cada píxel. Se comprobó el acercamiento especial propuesto en **21 muestras**, maga/bastón completos en el encuadre final y columnas verticales libres sobre ambos enemigos para los futuros rayos.

Inspeccionadas las vistas [base 720p](Evidence/H07/fortress-base-1280x720.png), [base 1080p](Evidence/H07/fortress-base-1920x1080.png), [conjunto](Evidence/H07/fortress-overview.png), [cámara especial reservada](Evidence/H07/fortress-special-reserved.png), [HUD 720p](Evidence/H07/arena-1280x720.png), [HUD 1080p](Evidence/H07/arena-1920x1080.png), objetivo, estados coexistentes, preparación de ataques/conjuros, proyectiles, pausa y [caídas](Evidence/H07/all-deaths-1920x1080.png). Los personajes y marcadores se distinguen del suelo y de la decoración. La fachada ocupa el fondo sin interferir con acciones. El HUD es el existente, pendiente de diseño final H08.

Correcciones internas: H07-C01 detectó hasta −1,54 cm/−2,17 cm en el pie derecho de ataques del guerrero/ogro H06. Compensación del tobillo y regeneración de cuatro clips, conservando duración, impacto y anclajes. Las 216 muestras finales respetan el límite de −4 mm; el mínimo de −3,38 mm corresponde a interpolación de caída del ogro. H07-C02 corrigió un recorte del bastón en la primera propuesta especial. H07-C03 añadió remapeo URP de exportaciones importadas. La auditoría final pasó después de esas correcciones; no se usa la primera pasada fallida como evidencia de cierre.

### Regresión automática

Suite PlayMode `UltimaGuardia.PlayModeTests`: **71/71 aprobadas**, 0 fallidas/omitidas, **151,6974113 s**, job `6ee56d0f19fe4a818bc77c78b1587ff4`. [Resultado completo](Evidence/H07/playmode-results.json). La [matriz de 20 casos](Evidence/H07/action-matrix.txt) recorre las diez acciones y los cinco objetos con cada aliado, con impacto único, trayectoria, pausa, gasto, recuperación y regreso a anclajes. Se verifican muerte estable, cancelación y ausencia de callbacks/proyectiles anteriores al salir.

Peleas de regresión: victoria con teclado virtual en 22 acciones y 0 Pociones restantes; victoria con control virtual en 22 acciones y 0 Pociones. Derrotas preparadas, estados, selección, reinicio desde pausa/resultado y retorno al menú aprobados. Estos fixtures aceleran el tiempo ×4 y no miden balance o duración objetivo. Se actualizaron solo las rutas de evidencia de las suites visuales para conservar intacta la evidencia histórica de H06.

La suite pasó con los cuatro clips corregidos y el entorno definitivo. Los cambios posteriores fueron exclusivamente de validación/editor: encuadre de la captura especial reservada y remapeo de OBJ de intercambio; se repitió la auditoría afectada, sin volver a ejecutar una suite de reglas sin cambios.

### Build y player

Build Windows x86_64/Mono, Development, **166,62 MB**, **73,7210562 s**, **0 errores / 1 advertencia**, job `build-2f205ce4bb`. MainMenu y Combat en ese orden. [Resumen](Evidence/H07/build-summary.json), [advertencia exacta](Evidence/H07/build-messages.txt): truncamiento implícito en shader DebugOccluder de Unity Core/d3d11, heredada de entregas previas.

`Builds/H07/UltimaGuardia.exe -foundation-smoke -screen-fullscreen 0 -screen-width 1280 -screen-height 720` completó MainMenu, consulta de controles, Combat con la fortaleza, básico animado de maga con pausa/reanudación y daño, pausa con dispositivos virtuales, retorno y Salir. **passed = true, animatedActionVerified = true, exitedThroughMenu = true, código de salida 0**. [Resultado](Evidence/H07/player-result-1280x720.json), [acción terminada del player](Evidence/H07/player-animated-action-complete-1280x720.png). Ventana y captura 1280 × 720. Capturas del player inspeccionadas, sin materiales faltantes.

El smoke ejecuta un proceso oculto y renderiza la escena/Canvas explícitamente: no es automatización de Windows ni una pelea completa del ejecutable. El log en `Builds/H07/Validation/player.log` conserva Curl error 6 y avisos de liberación de buffers/asignaciones al cerrar, como en H00–H06. No se observó excepción de lógica; evaluación prolongada y causa siguen en H12. Unity Connect restaurado a `m_Enabled = 0`; fixtures restauran tiempo, input y ejecución en segundo plano. Versión Unity, renderer, paquetes, estadísticas y MainMenu conservados.

### Límites

Capturas 720p/1080p mediante RenderTexture, sin acreditar monitor físico 1080p. Control virtual, sin acreditar hardware ni desconexión física. No hay partículas finales, audio ni secuencia de cámara H10: se verifican proyectiles H06 existentes y espacio reservado para el contenido posterior. Los presupuestos estáticos de geometría/materiales/luces no equivalen a una medición de FPS del producto final, prevista en H12. [Fuentes y reconstrucción](H07_FORTRESS.md), [incidencias](KNOWN_ISSUES.md).

## H08 · HUD, recursos y menús finales

Versión **0.0.9-H08**, cambios locales sobre commit real `209626c` (H07). No se creó un commit de entrega. Unity 6000.3.11f1/URP 17.3.0; Windows x86_64/Mono. H00–H07 aprobados; revisión del usuario de H08 pendiente.

### Pruebas y alcance

Suite completa PlayMode: **76/76 aprobadas**, 0 fallidas/omitidas, **177,3846759 s**, job `28ccbde3a92b4885b3ef5eacf4f9bab9`. [Resultado completo](Evidence/H08/playmode-results.json). Conserva pruebas de reglas, ATB, gastos, IA, estados, entrada, flujo, impactos y animaciones. La matriz de presentación verifica las diez acciones y los cinco objetos con ambos aliados: [20 casos](Evidence/H08/Run-20261121-200346-448-bee60d/action-matrix.txt).

Después se corrigió la superposición de etiquetas 3D en selección grupal/números y se acortó el mensaje de contexto de objetivos. Se repitieron las cinco pruebas de interfaz y las dos regresiones afectadas: **7/7 aprobadas**, **48,6678073 s**, job `75f17e3b77ab4eb59ffa3a605be9dd24`. [Resultado final de UI](Evidence/H08/final-ui-results.json). No se presentan las 76 pruebas como una ejecución posterior a ese ajuste exclusivamente visual.

Los nuevos recorridos prueban teclado y Gamepad virtual: cuatro páginas circulares de ayuda; foco de origen al volver; volumen 0, 100 y pasos de 10; conservación entre escenas/reinicio; pausa desde avance y ejecución; ayuda/volumen dentro de pausa; cancelar y aceptar reinicio/abandono; resultado sin confirmación extra; mantener confirmar; cambio de dispositivo sin pérdida de foco; opciones sin objetivos; selección grupal; restauración del intento. Los casos de recursos efectivos comprueban daño limitado a 1, curación limitada a 5, Éter limitado a 18 y Veneno letal limitado a 3. Comprobación de sprites asignados y alto de texto visible en las capturas de ambas resoluciones.

[Auditoría de recursos](Evidence/H08/resource-audit.json): 26 PNG RGBA de 128 × 128 con importación Sprite y sin mipmaps; los 23 assets de encuentro conservan todos los valores de juego, excluyendo las nuevas referencias visuales de la comparación. [Preservación de escena](Evidence/H08/scene-preservation.json): 150 bloques serializados anteriores intactos; cambios existentes limitados a overrides de marcadores, controlador/referencias de UI y lista de raíces. Escenario, cámaras y anclajes conservados.

### Inspección visual

La evidencia canónica del ajuste final está en [Run-20261121-200944-516-0dcacd](Evidence/H08/Run-20261121-200944-516-0dcacd/). Se inspeccionaron MainMenu, ayuda, volumen, pausa, confirmaciones, victoria/derrota, paneles de recursos/estados, objetos, selección de ambos enemigos, curación/maná y números efectivos. Ver [selección grupal](Evidence/H08/Run-20261121-200944-516-0dcacd/storm-group-1280x720.png), [HUD 1080p](Evidence/H08/Run-20261121-200944-516-0dcacd/hud-states-1920x1080.png), [registro y maná efectivos](Evidence/H08/Run-20261121-200944-516-0dcacd/effective-mana-and-log-1280x720.png), [ayuda con control](Evidence/H08/Run-20261121-200944-516-0dcacd/help-2-gamepad-1280x720.png).

Las imágenes sueltas en la raíz de Evidence/H08 son iteraciones preliminares; no sustituyen las capturas de los directorios Run enlazados. Las capturas usan RenderTexture, sin acreditar monitor físico 1080p. El input es virtual, sin acreditar control físico ni desconexión real. Las victorias automatizadas usan decisiones preparadas y reloj ×4; no miden balance ni duración objetivo. Hardware corresponde a H11 y balance/rendimiento a H12.

### Correcciones durante la verificación

- H08-C01: caras de rombo/aro apuntaban hacia abajo; orientación corregida y marcadores visibles inspeccionados.
- H08-C02: primera suite completa 73/75; el fixture de estados buscaba componentes/textos provisionales. Se actualizó para comprobar sprites finales y contadores. Resultado final aprobado, sin cambiar reglas.
- H08-C03: Windows rechazó sobrescribir una captura abierta/mapeada (IO 1224). Las pruebas guardan evidencia por ejecución, sin sobrescribir imágenes que un revisor tenga abiertas. La primera pasada fallida se conserva en [first-full-results.json](Evidence/H08/first-full-results.json).
- H08-C04: etiquetas OBJETIVO se superponían en selección grupal y los nombres 3D competían con números. Objetivos con nombres breves; etiquetas ocultas mientras aparecen números, conservando cuatro identidades en el HUD. Siete pruebas afectadas repetidas y capturas finales inspeccionadas.
- H08-C05: el smoke heredado exigía píxeles blancos; el menú final usa tipografía dorada. La comprobación acepta el primer plano claro de la paleta y se complementa con inspección visual. No se cambió el producto por esa expectativa del arnés.

### Build y player final

Build Windows x86_64/Mono, Development, **168,33 MB** (176.509.903 bytes), **52,4549261 s**, **0 errores y 1 advertencia**. MainMenu y Combat, en ese orden. Creada con `InterfaceBuild.BuildReview` desde el editor; [BuildReport resumido y mensajes reales](Evidence/H08/build-summary.json). Advertencia: truncamiento implícito de vector en DebugOccluder de Unity Core/d3d11, heredada. La utilidad de exportación corrigió durante su preparación el tipo de los contadores de BuildReport a `int`; la build final confirma la compilación correcta del código de editor y player.

`Builds/H08/UltimaGuardia.exe -foundation-smoke -screen-fullscreen 0 -screen-width 1280 -screen-height 720` completó MainMenu, ayuda, Combat, básico animado de la maga, pausa/reanudación e impacto, pausa con ambos dispositivos virtuales, retorno confirmado a MainMenu y Salir. **passed = true, animatedActionVerified = true, exitedThroughMenu = true, salida 0**. [Resultado](Evidence/H08/player-result-1280x720.json), [menú del player](Evidence/H08/main-menu-1280x720.png), [acción terminada](Evidence/H08/animated-action-complete-1280x720.png), [pausa](Evidence/H08/pause-1280x720.png). Capturas inspeccionadas, sin materiales o texto faltantes. Ventana/captura 1280 × 720.

El primer smoke falló exclusivamente en la expectativa de píxeles blancos, antes de guardar su primera captura. [Resultado conservado](Evidence/H08/player-first-attempt.json); los logs originales están en `Builds/H08/Validation-first-attempt`. Después de corregir el verificador se recompiló y repitió el recorrido completo con el resultado aprobado anterior.

El smoke usa un proceso oculto y renderizado explícito por Camera.Render; no es una pelea completa ni una medición prolongada. El log de `Builds/H08/Validation/player.log` conserva avisos de buffers y asignaciones JobTempAlloc al cerrar, observados también en entregas anteriores. No se observó excepción de lógica en esta ejecución; seguimiento en H12. Unity Connect restaurado a `m_Enabled = 0`; opciones de entrada a Play conservadas en habilitado/opciones 0. No se modificaron paquetes, renderer, fórmulas ni datos de balance.

[Fuentes y reconstrucción](H08_INTERFACE.md), [recursos](ASSET_REGISTER.md), [incidencias](KNOWN_ISSUES.md).

## H08-C06 · Nitidez de textos de menús

El usuario señaló que los textos de menús se veían pixelados en comparación con el HUD. Se mantuvo la fuente incorporada y el componente nativo Text: fuente y rectángulo al doble de resolución, escala local 0,5, conservando tamaño visual, posición, color y referencias. La fábrica `InterfaceTheme.MenuText` y la autoría aplican ese tratamiento al reconstruir. Cambian 13 etiquetas de MainMenu y 3 de los overlays de Combat, que cubren principal, ayuda, volumen, pausa, confirmaciones y resultados. El HUD de combate conserva su configuración.

Se repitieron las cinco pruebas `FinalInterfaceTests` desde Test Runner: **5/5 aprobadas**, sin omitidas, **37,1183806 s**. [XML real](Evidence/H08/C06/playmode-results.xml). Los recorridos verifican ambos dispositivos virtuales, foco, volumen, navegación, cancelación/confirmación y texto sin recortes en ambas resoluciones. [Auditoría de 16 etiquetas](Evidence/H08/C06/menu-text-audit.json). No se atribuye una nueva ejecución completa de las 76 pruebas a esta corrección exclusivamente visual.

Inspección de [principal 720p](Evidence/H08/Run-20261121-232425-443-3746e2/main-gamepad-1280x720.png), [ayuda 720p](Evidence/H08/Run-20261121-232425-443-3746e2/help-2-gamepad-1280x720.png), [ayuda 1080p](Evidence/H08/Run-20261121-232425-443-3746e2/help-2-keyboard-1920x1080.png), pausa, confirmaciones, volumen y resultados. Se conservan tamaño visual, legibilidad, tildes y contenido, sin duplicaciones ni recortes. Una primera variante de renderizado se descartó tras detectar artefactos en la inspección visual; una ejecución intermedia también usó una asamblea de pruebas anterior pendiente de recompilación. La evidencia de cierre es exclusivamente la nueva pasada enlazada y la build siguiente.

Build actualizada de `Builds/H08/UltimaGuardia.exe`: **52,4549261 s**, **176.509.903 bytes / 168,33 MB**, **0 errores y 1 advertencia** heredada de DebugOccluder. [BuildReport de C06](Evidence/H08/C06/build-summary.json). Smoke repetido sobre esa build: **passed = true**, **animatedActionVerified = true**, salida desde Salir y **código 0**. [Resultado](Evidence/H08/C06/player-result-1280x720.json), [principal del player](Evidence/H08/C06/main-menu-1280x720.png), [ayuda](Evidence/H08/C06/controls-1280x720.png), [pausa](Evidence/H08/C06/pause-1280x720.png). Capturas reales del player inspeccionadas. Se mantienen los límites de input virtual, RenderTexture y prueba breve; no acreditan hardware físico ni rendimiento prolongado. Opciones de entrada a Play y Unity Connect restituidos.

## H09 · Audio

La suite PlayMode existente pasó **76/76**, 182,4144 s, job `da98fbe2f5cc44ba972bcb8530da79a8`. Después del ajuste de inicio de música pasó la prueba nueva `AudioIntegrationTests.AllCuesLoadAndCombatMusicSurvivesPauseThenClears`: **1/1**, 0,3567 s, job `17fefc3d97094a6cab391b818ceebfc9`. Se amplió para comprobar volumen 0 y 100 y pasó otra vez: **1/1**, 0,5560 s, job `11ba006a2d474192b3e4e5d4e8756d56`. Verifica la importación de los 28 clips, música en bucle, voz de Tormenta, pausa/reanudación, volumen general y limpieza. La corrección posterior del cue de Veneno compiló sin errores y entró en la build final; no se repitió la suite completa porque no cambia reglas ni input.

[Auditoría de WAV](Evidence/H09/audio-audit.json): 28 clips mono PCM, 24 kHz; pico máximo 0,820; salto de borde musical 0,006653, menor que el salto interno máximo 0,01233. La primera variante de música tenía saltos entre notas y se regeneró con transiciones suaves antes de la build final. La auditoría cuantifica señal y margen, pero no equivale a una escucha humana.

[Build H09](Evidence/H09/build-summary.json): Windows x86_64/Mono, Development, 169,3 MB, 64,7044 s, 0 errores y una advertencia heredada del shader DebugOccluder. [Smoke final](Evidence/H09/player-result-1280x720.json): versión 0.0.10, MainMenu → Combat → ataque animado con música, pausa/reanudación de música, vuelta confirmada y Salir; `passed`, `animatedActionVerified` y `audioVerified` verdaderos, salida 0. Input y proceso ocultos; no acredita control físico, mezcla percibida ni partida completa del ejecutable. La revisión auditiva de música, voces, volumen 0/100 y todos los eventos queda a cargo del usuario al revisar H09.

## H10 · VFX y presentación especial

La suite PlayMode completa terminó **78/78**, sin fallos ni omitidas, en 192,8143 s (job `02a33cb5e94f4afd97ffd2c7718856fb`). Cubre las reglas, los dos objetivos de Tormenta, costo único, recuperación, pausa, reinicio, combate completo y efectos de recursos. Después se añadió la prueba de ocultamiento de HUD/cámara/pausa/reinicio y pasó **1/1**. Al corregir el material para que el shader estuviera incluido en el player, las tres pruebas de presentación afectadas pasaron **3/3** (job `8df0482efd8e41258bb35e390c6b0eb2`). La captura final de carga, rayos y regreso del HUD pasó **2/2** junto con la prueba de cámara/restauración (job `39359a62252d4e7896df40ab2add7e6a`). Los ajustes visuales posteriores a la suite completa no cambian reglas ni input.

Las capturas editoriales finales están en [Evidence/H10](Evidence/H10/): carga, impacto con ambos enemigos dentro del encuadre y HUD restaurado con daños efectivos. Se inspeccionaron a 1280 × 720. La prueba usa RenderTexture; no acredita pantalla física 1080p.

El primer smoke de la build Development falló porque `Shader.Find` devolvía nulo en player al eliminarse ese shader de la build; el informe se conservó en `Builds/H10/Validation/player.log`. Se sustituyó por el material referenciado desde cada prefab, se repitieron las pruebas afectadas y el smoke posterior de versión 0.0.11 pasó con salida 0. [Build final](Evidence/H10/build-summary.json): 169,32 MB, 0 errores y 0 advertencias. [Smoke final](Evidence/H10/player-result-1280x720.json): `passed`, `animatedActionVerified` y `audioVerified` verdaderos; salida 0. El smoke recorre un ataque básico y pausa/audio, no ejecuta Tormenta; esa secuencia está cubierta por PlayMode y las capturas anteriores.
