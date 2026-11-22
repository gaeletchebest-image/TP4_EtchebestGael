# ÚLTIMA GUARDIA

> Documento de trabajo para la IA desarrolladora. Contiene el GDD completo y un plan de producción con tareas verificables.
>
> Fuente: [GDD de Google Docs](https://docs.google.com/document/d/1LPU6YM92nCy1HZX1Ax_baBCnAB7gYP07UGAtHKMVVHo).
> Conversión y ampliación del plan: 21 de noviembre de 2026.
> Estado: H00 a H09 aprobados por el usuario. H10 a H13 pendientes.

## Cómo usar este archivo

1. Leer primero las decisiones de la parte A y las reglas de las secciones 1 a 16.
2. Seguir la forma de trabajo entre chats: realizar un solo hito por conversación, en orden numérico, comprobarlo y entregarlo para revisión del usuario. La sección 17 detalla tareas y dependencias, y la sección 18 conserva la relación con la consigna.
3. Mantener los valores de las tablas como configuración inicial. Cambiar valores únicamente como ajuste de balance documentado, sin agregar mecánicas.
4. Usar los identificadores de tarea para informar avances. Marcar `[x]` solamente cuando el entregable exista, esté integrado si corresponde y supere su comprobación.
5. Mantener dentro de este Markdown las casillas de tareas, el estado de hitos y las revisiones. Registrar recursos, licencias, decisiones técnicas y pruebas en los archivos indicados. Si hay un bloqueo, continuar solamente con tareas independientes del mismo hito.
6. Los placeholders permiten avanzar en los primeros hitos. Los modelos 3D, animaciones, sonidos y elementos de interfaz definitivos tienen tareas propias y forman parte del cierre.
7. No ampliar el alcance: una arena, dos aliados, dos enemigos, las acciones y objetos definidos aquí. No agregar tienda, campaña, inventario fuera de combate, guardado, resurrección ni sistemas que no aparezcan en el GDD.
8. Las tareas describen trabajo para realizar. Los escenarios de prueba describen resultados esperados, no resultados ya obtenidos.

## Forma de trabajo entre chats

### Un hito por chat

El desarrollo se realiza de a un hito por conversación, en el orden H00, H01, H02 y así sucesivamente hasta H13. La IA implementa el hito que corresponda, lo verifica, actualiza este Markdown y entrega el resultado para que el usuario haga su revisión final. Después se detiene. El siguiente hito se solicita en otro chat.

Las dependencias técnicas de la sección 17 siguen siendo válidas, pero no autorizan adelantar hitos. Por ejemplo, aunque H07 solo dependa de H01, se realiza después de H06. Si el hito actual está bloqueado, se informa el bloqueo y no se salta a otro hito por cuenta propia.

Dentro del hito actual, la IA puede completar tareas independientes mientras resuelve un problema. También puede corregir una dependencia anterior si es necesario para que el hito funcione, siempre que registre qué corrigió y compruebe que no rompió lo ya entregado. Esto no autoriza agregar funciones de un hito posterior.

### Documento vigente y continuidad

Este Markdown es la fuente principal del estado del desarrollo: conserva el GDD, las 245 tareas, el estado de los 14 hitos, las revisiones y los pendientes. En el proyecto, mantener la copia vigente en `Documentation/Ultima_Guardia_GDD_y_Plan_de_Desarrollo.md`.

Al abrir un chat nuevo, la IA debe leer esa copia y revisar el proyecto real. No debe depender de la memoria del chat anterior ni volver a empezar porque la conversación esté vacía. Si el proyecto no está disponible, debe identificar qué le falta para continuar, en lugar de afirmar que verificó archivos que no pudo abrir.

Si el usuario trabaja compartiendo archivos entre chats, debe proporcionar el Markdown actualizado y el proyecto o repositorio correspondiente. Una copia antigua del Markdown no sirve como evidencia de que el último hito quedó integrado.

`TASKS.md`, si se conserva, funciona como resumen o índice hacia este documento. No mantiene un segundo conjunto independiente de casillas. Los detalles extensos de pruebas, recursos y balance pueden estar en `TEST_REPORT.md`, `ASSET_REGISTER.md` y `BALANCE_LOG.md`, enlazados desde el registro del hito.

### Estados y significado de las casillas

| Estado del hito | Qué significa | Casilla del hito |
| --- | --- | --- |
| Pendiente | Todavía no se empezó. | `[ ]` |
| En curso | Hay trabajo realizado y tareas pendientes. | `[ ]` |
| Bloqueado | No puede completarse por un impedimento registrado. | `[ ]` |
| Requiere correcciones | La revisión detectó un problema que impide cerrar el hito. | `[ ]` |
| Implementado y verificado | La IA completó las tareas y comprobó los criterios de cierre. Falta la revisión final del usuario. | `[x]` |
| Aprobado por el usuario | El usuario revisó el resultado y autorizó continuar. | `[x]` |

La casilla de una tarea se marca `[x]` cuando su entregable existe y supera su comprobación. La de un hito indica que su implementación está completa y verificada; su aprobación por el usuario se registra por separado. Así se puede distinguir una entrega terminada de una revisión todavía pendiente.

Una prueba no realizada se registra como pendiente y nunca como aprobada. Si esa comprobación es necesaria para cerrar el hito, este sigue abierto. Si una corrección invalida una tarea que estaba marcada, se desmarca esa tarea y el hito hasta resolverla.

El estado inicial del plan tenía todos los hitos y tareas pendientes. El estado vigente se conserva en el registro global y en los registros de entrega; actualizar texto por sí solo no acredita una implementación.

### Inicio de cada conversación

1. Leer este Markdown completo en el primer chat. En los siguientes, revisar el estado global, el último registro de entrega, las correcciones abiertas y las reglas y tareas relacionadas con el hito actual.
2. Inspeccionar la versión del proyecto, sus archivos y los resultados existentes. Si el estado del Markdown contradice al proyecto, registrar la diferencia y resolverla antes de dar por completado el trabajo.
3. Revisar primero las correcciones pendientes. Si existen, trabajar sobre el mismo hito hasta resolverlas.
4. Si no hay correcciones, seleccionar el primer hito que todavía no esté implementado y verificado, respetando el orden numérico.
5. Confirmar que el hito anterior tiene revisión aprobada. Un pedido del usuario de realizar el siguiente hito cuenta como aprobación de la última entrega pendiente de revisión, siempre que no incluya correcciones todavía abiertas. Registrar ese pedido como evidencia de aprobación; no inventar una revisión detallada que no ocurrió.
6. Si la entrega anterior solo está pendiente de revisión y el usuario no pidió avanzar, informar ese estado. El silencio o el tiempo transcurrido no equivalen a aprobación.
7. Anunciar el hito seleccionado, leer sus tareas y dependencias, y trabajar únicamente dentro de ese alcance.

### Implementación, comprobación y cierre del chat

1. Implementar las tareas del hito y marcar cada una solamente después de comprobarla. Mantener los IDs existentes para que el estado pueda seguirse entre conversaciones.
2. Integrar los recursos y cambios en el proyecto, además de producir los archivos. Un modelo sin importar, un sonido sin conectar o un script que no compila no completan una tarea que pide integración.
3. Comprobar todos los criterios de cierre del hito y realizar verificaciones de regresión sobre las funciones anteriores que se hayan tocado.
4. Registrar las pruebas reales: qué se hizo, con qué condiciones y cuál fue el resultado. Distinguir comprobación automática, inspección visual y prueba manual. No afirmar que se probó un control, una build o una animación si no se tuvo acceso a esa comprobación.
5. Actualizar las casillas de tareas y el registro global. Si todo corresponde, marcar el hito como `Implementado y verificado`, con revisión del usuario pendiente. Si falta trabajo necesario, dejarlo en curso o bloqueado.
6. Añadir o actualizar el registro de entrega de ese hito con cambios, rutas relevantes, evidencia de pruebas, incidencias y cómo revisarlo.
7. Entregar el proyecto actualizado o identificar su versión, junto con este Markdown actualizado. Si se usa un repositorio, registrar el commit real cuando exista; no inventar identificadores ni crear un commit vacío solo para llenar el campo.
8. Cerrar el chat con un resumen breve de lo implementado, lo comprobado, cualquier pendiente y los pasos concretos para la revisión del usuario. Detenerse sin empezar el hito siguiente.

Cuando las herramientas o archivos no permitan completar una tarea, registrar el impedimento y el estado real de lo producido. El alcance de un chat sigue siendo un hito, aunque ese hito necesite otra conversación para terminarse.

### Revisión del usuario y correcciones

La última revisión de cada hito la hace el usuario. La entrega debe incluir una forma práctica de inspeccionarla: qué escena abrir o ejecutable lanzar, qué acción probar y qué resultado debería verse.

Si el usuario señala un error, registrarlo con un ID como `H02-C01`, su descripción, la tarea afectada y la comprobación que permitirá cerrarlo. Poner el hito en `Requiere correcciones` y desmarcar las tareas que ya no cumplen su criterio.

La IA corrige el problema, repite la comprobación afectada y las regresiones necesarias, actualiza el Markdown y vuelve a entregar el mismo hito para revisión. No utiliza una corrección como motivo para comenzar el siguiente.

Si el usuario aprueba expresamente o pide avanzar al siguiente hito sin correcciones abiertas, registrar la aprobación y continuar únicamente con el nuevo hito solicitado. No exigir una fórmula de aprobación específica.

Si el usuario comenta una preferencia que cambia el diseño, distinguirla de un error de implementación, actualizar las reglas afectadas con su autorización y conservar una nota del cambio. No cambiar estadísticas o mecánicas para ocultar un fallo.

### Agente revisor opcional

El flujo base usa una IA implementadora y la revisión final del usuario. Un agente revisor independiente es opcional y se recomienda especialmente después de H02, H04 y H10, y durante el cierre de H12/H13.

El revisor debe inspeccionar el proyecto y el Markdown actuales. Su trabajo es buscar errores de reglas, casos límite, tareas marcadas sin evidencia, problemas de integración y regresiones. Debe informar hallazgos con gravedad, archivo o escena, pasos para reproducirlos y resultado esperado.

El revisor no implementa cambios, no rediseña el juego, no marca tareas ni hitos como aprobados y no sustituye la revisión del usuario. La IA implementadora resuelve los hallazgos y registra las comprobaciones. Si no se usa un revisor, se mantiene el mismo procedimiento y los mismos criterios de cierre.

No es necesario iniciar otro agente automáticamente ni bloquear todos los hitos esperando una revisión adicional. El usuario decide en qué entregas quiere esa revisión.

### Mensajes de ejemplo para abrir los chats

**Primer hito**

> Leé el GDD y el proyecto. Realizá únicamente H00, comprobá sus criterios de cierre y actualizá las tareas y el estado del hito dentro del Markdown. Entregame el resultado para mi revisión y no empieces H01.

**Siguiente hito**

> Revisé el hito anterior y está bien. Leé el Markdown actualizado y realizá únicamente el siguiente hito pendiente en orden. Comprobalo, actualizá el documento y entregámelo para mi revisión.

**Corrección del mismo hito**

> En H02 encontré estos problemas: [describirlos]. Corregí ese hito, repetí las comprobaciones afectadas y actualizá el Markdown. No avances al siguiente.

**Revisión independiente opcional**

> Revisá el hito H02 en el proyecto y el Markdown actuales. No modifiques archivos ni cambies el estado de las tareas. Informá errores reproducibles, incumplimientos del GDD y tareas marcadas sin evidencia, con su gravedad y ubicación.

### Registro global de hitos

Actualizar esta lista al terminar o reabrir un hito. Su casilla indica implementación y comprobación completas; la revisión del usuario queda escrita en la misma fila.

- [x] **H00. Preparación del proyecto y control del alcance.** Estado: Aprobado por el usuario. Revisión del usuario: Aprobada el 21 de noviembre de 2026.
- [x] **H01. Arena provisional y combate básico terminable.** Estado: Aprobado por el usuario. Revisión del usuario: Aprobada el 21 de noviembre de 2026.
- [x] **H02. Datos configurables, ATB y recuperación.** Estado: Aprobado por el usuario. Revisión del usuario: Aprobada el 21 de noviembre de 2026.
- [x] **H03. Acciones completas, inventario e inteligencia enemiga.** Estado: Aprobado por el usuario. Revisión del usuario: Aprobada el 21 de noviembre de 2026.
- [x] **H04. Veneno, rapidez y corrosión.** Estado: Aprobado por el usuario. Revisión del usuario: Aprobada el 21 de noviembre de 2026.
- [x] **H05. Modelado y preparación de los cuatro personajes 3D.** Estado: Aprobado por el usuario. Revisión del usuario: Aprobada el 21 de noviembre de 2026.
- [x] **H06. Rigging final, animaciones y sincronización de impactos.** Estado: Aprobado por el usuario. Revisión del usuario: Aprobada el 21 de noviembre de 2026.
- [x] **H07. Modelado modular y construcción del escenario final.** Estado: Aprobado por el usuario. Revisión del usuario: Aprobada el 21 de noviembre de 2026.
- [x] **H08. Diseño de interfaz, iconos y menús finales.** Estado: Aprobado por el usuario. Revisión del usuario: aprobada el 21 de noviembre de 2026, incluida corrección H08-C06.
- [x] **H09. Búsqueda, edición e integración de audio.** Estado: Aprobado por el usuario. Revisión del usuario: Aprobada el 21 de noviembre de 2026.
- [ ] **H10. Efectos visuales y presentación de Tormenta arcana.** Estado: Pendiente. Revisión del usuario: Pendiente.
- [ ] **H11. Soporte completo de teclado y control y robustez del flujo.** Estado: Pendiente. Revisión del usuario: Pendiente.
- [ ] **H12. Pruebas funcionales, balance y rendimiento.** Estado: Pendiente. Revisión del usuario: Pendiente.
- [ ] **H13. Compilación final, documentación y entrega.** Estado: Pendiente. Revisión del usuario: Pendiente.

### Registro de entregas, revisiones y correcciones

#### Registro H00

- Estado: **Aprobado por el usuario** el 21 de noviembre de 2026. Se conservan debajo los métodos y límites de la entrega H00.
- Versión: **0.0.1-H00** (PlayerSettings 0.0.1). Cambios locales sobre commit inicial real `208d5bb`; no se creó un commit de entrega. Huellas en [fingerprints.json](Evidence/H00/fingerprints.json).
- Última actualización: 21 de noviembre de 2026.
- Tareas completadas: **H00-T01 a H00-T13**. Sin tareas de H01 o posteriores iniciadas.
- Entrega: Unity 6000.3.11f1, URP 17.3.0/calidad PC, Windows x86_64/Mono, estructura de Assets y Documentation, escenas MainMenu y Combat vacía, menú y pausa mínimos, acciones de input para teclado/control, documentación e inventario de recursos.
- Archivos principales: `Assets/Scenes/MainMenu.unity`, `Assets/Scenes/Combat.unity`, `Assets/Data/Input/GuardiaInput.inputactions`, `Assets/Scripts/Core/FoundationMenu.cs`, `Assets/Scripts/Editor/FoundationSetup.cs`, `Assets/Tests/PlayMode/FoundationInputTests.cs`, README y documentos de seguimiento.
- Build: `Builds/H00/UltimaGuardia.exe` y toda su carpeta de datos. Build final `build-661594265f`, 162,87 MB, **0 errores y 0 advertencias**. [Resumen](Evidence/H00/build-summary.json).
- Comprobaciones automáticas: compilación y consola sin errores; **3/3 pruebas PlayMode** aprobadas; dos ejecuciones reales del player con input virtual que cargan MainMenu, navegan, cancelan, abren Combat, pausan, regresan y salen mediante Salir, ambas con código 0. [Informe completo](TEST_REPORT.md).
- Comprobación visual: inspección de MainMenu, controles, Combat y pausa a 1280 × 720 y 1920 × 1080; capturas en [Evidence/H00](Evidence/H00/). La UI permanece completa y legible.
- Método y límites de H00-T07/T13: ventana real de 1280 × 720; la solicitud nativa de 1080p se limita al monitor 1366 × 768, por lo que el escalado a **1920 × 1080 se comprobó por RenderTexture**. El control se comprobó mediante un Gamepad virtual, no un dispositivo físico. No se presenta ninguna de esas comprobaciones como una prueba de hardware no realizada.
- Incidencias: Computer Use no autorizó operar el nuevo ejecutable; se sustituyó esa vía por validación interna del player, sin automatizar su UI de Windows. Hay avisos de red y apagado de URP en la prueba oculta. Métodos, límites y seguimiento H00-L01 a H00-L05 en [KNOWN_ISSUES.md](KNOWN_ISSUES.md). No se detectaron bloqueos del flujo de H00.
- Límites de comprobación de H00: prueba con control físico y presentación nativa en monitor 1080p no realizadas; su revisión fue aprobada por el usuario el 21 de noviembre de 2026. Se mantienen identificadas como límites de la evidencia; H11 exige la verificación completa de dispositivos reales.
- Cómo revisarlo: ejecutar `Builds/H00/UltimaGuardia.exe` o abrir MainMenu y pulsar Play en Unity. Usar flechas y Enter/Espacio para elegir Jugar, comprobar Combat vacía, Escape para pausa/continuar, volver al menú, abrir Controles, cancelar y elegir Salir. El foco debe ser visible. Se puede repetir con cruceta/stick, botón inferior, botón derecho y botón de menú del control.
- Revisión del usuario: **aprobada**. Evidencia: mensaje del 21 de noviembre de 2026: «Ha si, el h00 esta aprobado, vamos por el 01 entonces.» Autoriza comenzar H01; no se atribuye una revisión detallada adicional.
- Correcciones abiertas del usuario: ninguna. El procedimiento de captura se ajustó después de detectar backbuffer negro en procesos ocultos; la evidencia final se regeneró y volvió a inspeccionar.
- Conservación del proyecto: se mantuvieron versión, renderer, paquetes y cambios previos en los assets gráficos; Unity actualizó serialización y filtros de shaders durante la compilación. No se adquirieron recursos pagos ni se añadieron sistemas de combate.
- Siguiente hito autorizado: **H01**, iniciado en este chat tras la aprobación del usuario.

#### Registro H01

- Estado: **Aprobado por el usuario** el 21 de noviembre de 2026.
- Versión: **0.0.2-H01**, cambios locales sobre commit real `c843243` (entrega H00). No se creó un commit de H01. Huellas de archivos en [fingerprints.json](Evidence/H01/fingerprints.json).
- Última actualización: 21 de noviembre de 2026.
- Autorización y dependencia: el usuario aprobó H00 y pidió «vamos por el 01 entonces» en este chat. No hay correcciones abiertas del usuario.
- Tareas completadas: **H01-T01 a H01-T17**. Ninguna tarea de H02 o posterior iniciada.
- Entrega: arena provisional ProBuilder con suelo, muro, portón y marco; cuatro prefabs distinguibles, anclajes y sockets; cámara fija; identidades, equipos, vida/maná y eventos; daño/curación; turnos secuenciales provisionales; básicos, Golpe pesado, tres Pociones compartidas; enemigos básicos, muerte, victoria/derrota, pausa y reinicio limpio.
- Decisión provisional: orden Guerrero → Maga → Ogro → Chamán, omitiendo muertos. Impacto a 0,35 s y cierre a 0,85 s de presentación; pausa congela ambos. H02 reemplaza el orden por ATB y añade recuperación/datos configurables. Se conservan estadísticas y fórmulas del GDD, sin ajustes de balance.
- Archivos principales: `Assets/Scenes/Combat.unity`, `Assets/Scripts/Combat/CombatSession.cs`, `CombatPrototype.cs`, `CombatantView.cs`, `Assets/Prefabs/H01/*`, `Assets/Materials/H01/*`, `Assets/Scripts/Editor/CombatPrototypeSetup.cs`, `Assets/Tests/PlayMode/CombatRulesTests.cs` y `CombatFlowTests.cs`. La utilidad de H01 es explícita y reemplaza solo Combat; no ejecutarla después de editar escenas de hitos posteriores.
- Comprobaciones automáticas: **17/17 PlayMode**, 0 fallidas, 65,31 s. Verificados daño 22/44, límites, objetivos, gasto e impacto únicos, cancelación, pausa antes/después de impacto, supervivientes, resultados y reinicios. Incluye regresión del flujo de H00. [Resultados](Evidence/H01/playmode-results.json).
- Peleas: victoria desde encuentro normal con teclado virtual (29 acciones) y Gamepad virtual (27 acciones); derrota preparada con ambos aliados a 1 de vida; reinicio después de ambos resultados. Estas muestras verifican flujo, no el balance final ni hardware físico.
- Comprobación visual: arena a 1280 × 720 y 1920 × 1080 offscreen, selección de objetivo, pausa, victoria y derrota; cuatro participantes completos y texto sin recortes. Captura del player a 720p inspeccionada. [Evidencia](Evidence/H01/).
- Build de revisión: `Builds/H01/UltimaGuardia.exe`, Windows x86_64/Mono, Development, **157,01 MB**, **0 errores y 5 advertencias** (shader de Unity Core y conexión de MCP). [Resumen](Evidence/H01/build-summary.json).
- Ejecutable real: regresión `-foundation-smoke` aprobada, MainMenu → Combat → pausa → MainMenu → Salir, con teclado/control virtual, ventana 1280 × 720 y código 0. El recorrido del player no incluye una pelea completa. [Resultado](Evidence/H01/player-result-1280x720.json).
- Corrección interna resuelta: H01-C01, rearme de confirmación después de una acción. Primera pasada 15/17, corregida y sustituida por pasada final 17/17; no hay correcciones abiertas. Métodos y límites en [TEST_REPORT.md](TEST_REPORT.md) y [KNOWN_ISSUES.md](KNOWN_ISSUES.md).
- Pendientes de comprobación posterior: control físico, monitor nativo 1080p y estabilidad prolongada de los avisos de render/red al cerrar la prueba oculta. No se presentan como pruebas realizadas. Recursos finales y sistemas posteriores siguen pendientes en sus propios hitos.
- Cómo revisarlo: ejecutar la build de H01 o MainMenu en Unity; jugar con flechas/Enter o cruceta/stick/botón inferior; cancelar un objetivo sin gasto; Golpe pesado contra el ogro (44 por 8 de maná), Poción sobre herido (hasta 50), pausa y reinicio, victoria y reinicio desde resultado. README incluye controles y pasos.
- Revisión del usuario: **aprobada**. Evidencia: mensaje del 21 de noviembre de 2026: «Bien. Te doy como el h01 aprobado». No se solicitaron correcciones ni se inició H02.
- Próximo hito en orden: **H02**, identificación solamente; **no iniciado**.
#### Registro H02

- Estado: **Aprobado por el usuario** el 21 de noviembre de 2026.
- Versión: **0.0.3-H02** (PlayerSettings 0.0.3), cambios locales sobre commit real `0333b21` (H01). Sin commit nuevo de entrega. [Huellas SHA-256](Evidence/H02/fingerprints.json).
- Última actualización: 21 de noviembre de 2026.
- Autorización: H00/H01 estaban aprobados, sin correcciones abiertas. Tras leer y explicar H02 en este chat, el usuario pidió «Bien. Entonces hacelo».
- Tareas completadas: **H02-T01 a H02-T14**. Ninguna tarea H03 o posterior iniciada.
- Entrega: 18 ScriptableObjects de configuración; validación de datos; snapshots y estado mutable independientes por intento; ATB individual, reloj activo, recuperación con sobrante, cola por cruce/velocidad/desempate, eliminación de muertos, barras y mensajes provisionales integrados en Combat.
- Datos: `Assets/Data/H02/Resources/Encounters/`. Cuatro combatientes, cinco acciones existentes de H01, cinco objetos, tres efectos y un encuentro. Básicos, Golpe pesado y Poción conservan su ejecución; los demás objetos y efectos solo tienen datos preparados para H03/H04. No se agregan habilidades ni IA completas antes de sus hitos.
- Archivos principales: `Assets/Scripts/Combat/{CombatantData,ActionData,ItemData,EffectData,EncounterData,CombatSession,CombatPrototype}.cs`, `Assets/Scripts/Editor/AtbSetup.cs`, `Assets/Scenes/Combat.unity` y pruebas PlayMode. MainMenu, geometría y prefabs de H01 conservados. La utilidad explícita H02 integra sobre Combat sin regenerar la arena y conserva datos ya creados.
- Reglas temporales verificadas: llenado en 5 s a velocidad 20 y 100/28 s a velocidad 28; recuperación 0,6/2,2/0,8 s para básicos/pesado/Poción; congelación durante decisión, ejecución, pausa y resultado. La cola se vacía antes de reanudar el reloj y omite muertos. No se ajustaron estadísticas ni balance.
- Comprobaciones automáticas: **27/27 PlayMode**, 0 fallidas, 76,7128748 s; incluye regresiones H00/H01, datos configurables, validación, gasto/impacto únicos, cola, tiempos, pausa, victoria/derrota y reinicios. Tras corregir la superposición de barras con pausa, **4/4 pruebas de flujo** adicionales, 68,7754229 s. [Informe y métodos](TEST_REPORT.md), [suite](Evidence/H02/playmode-results.json), [regresión visual/flujo](Evidence/H02/flow-after-ui-fix.json).
- Peleas: teclado virtual desde encuentro base, victoria en 26 acciones; control virtual con una herida preparada de 1 de vida para probar cancelación de Poción, victoria en 23 acciones. Derrota con ambos aliados preparados a 1 de vida. Tiempo de juego acelerado a ×4 solo en las peleas automatizadas. Estas muestras verifican flujo, no balance ni hardware real.
- Comprobación visual: arena y recuperación a 1280 × 720 y 1920 × 1080 mediante RenderTexture; avance, selección, pausa, victoria y derrota. Inspección adicional de Combat y pausa del player a 720p. [Capturas](Evidence/H02/). No acredita monitor físico 1080p.
- Build final: `Builds/H02/UltimaGuardia.exe`, Windows x86_64/Mono, Development, **157,04 MB**, 22,5173845 s, **0 errores / 1 advertencia** del shader DebugOccluder de Unity Core. Job `build-76cf7166d7`. [Resumen](Evidence/H02/build-summary.json). Una build anterior se sustituyó tras ajustar el orden visual de las barras.
- Ejecutable real: `-foundation-smoke` aprobado; MainMenu → Combat → pausa → MainMenu → Salir, ventana 1280 × 720, dispositivos virtuales y **código de salida 0**. [Resultado](Evidence/H02/player-result-1280x720.json). No se atribuyen peleas completas a esta prueba del player.
- Incidencias: H02-C01 resuelto, barras correctamente detrás de pausa/resultado. Se conservan avisos de red y liberación de GraphicsBuffer/ComputeBuffer al cerrar el player oculto con capturas, sin excepción de lógica; seguimiento en [KNOWN_ISSUES.md](KNOWN_ISSUES.md). Configuraciones de entrada a Play y Unity Connect recibidas restauradas después de pruebas/build.
- Cómo revisarlo: ejecutar la build H02 o MainMenu en Unity. Observar que la maga queda lista primero; esperar en su menú sin que avancen barras; usar básico y después Golpe pesado del guerrero; observar recuperación de 2,2 s, cancelar sin gasto, pausar y reiniciar. Ganar o perder y reiniciar. Inspeccionar los assets y modificar un valor fuera de Play para comprobarlo en el nuevo intento; restaurar el valor al terminar. Detalle en README.
- Correcciones abiertas del usuario: ninguna. Revisión del usuario: **aprobada** el 21 de noviembre de 2026. Evidencia: «Bien. Lo doy por aprobado y te pido que me des un nombre en ingles para el commit». No se solicitaron correcciones ni comenzar H03.
- Próximo hito en orden: **H03**, identificación solamente; **no iniciado en este chat**.

#### Registro H03

- Estado: **Aprobado por el usuario** el 21 de noviembre de 2026. No se inició H04.
- Versión: **0.0.4-H03** (PlayerSettings 0.0.4), cambios locales sobre commit real `52b3306` (H02). No se creó un commit de entrega.
- Fecha: 21 de noviembre de 2026. Autorización: «Bien. Entonces implementalo», después de explicar el alcance de H03. H02 aprobado, sin correcciones abiertas.
- Tareas: **H03-T01 a H03-T18**, con las dependencias de estados diferidas expresamente a H04 según T09/T15 y los criterios de cierre.
- Entrega: diez acciones configuradas, Curación, Tormenta arcana a todos los enemigos vivos, inventario compartido con Poción/Éter ejecutables, menús Atacar/Habilidades/Objetos, selección individual o grupal, gasto único, validación de disponibilidad, alternancia del ogro y prioridad de curación del chamán.
- Alcance de **H03-T09/T15/T16**: Tónico, Frasco, Antídoto y Veneno tienen datos, costos, recuperación y equipo objetivo preparados; están bloqueados con motivo explícito y no gastan recursos. La prioridad del chamán consulta Curación oscura → Veneno ejecutable → Descarga. Como Veneno todavía no tiene estados runtime, en H03 omite esa opción y usa Descarga; **no se acredita aplicación de Veneno ni selección por ausencia del estado**. H04-T04/T15 completan y verifican esos comportamientos. No se simula un Veneno sin efecto ni se desperdicia un turno.
- Configuración: se mantienen rutas y GUID de H02 en `Assets/Data/H02/Resources/Encounters/`. Se agregan `heal`, `arcane_storm`, `cleave`, `poison_spell` y `dark_heal`. No se cambian estadísticas, inventario inicial ni balance. ItemData incorpora un comando explícito validado; la sesión conserva snapshots independientes.
- Archivos principales: `CombatSession.cs`, `CombatPrototype.cs`, `ItemData.cs`, `EncounterData.cs`, `Assets/Scripts/Editor/ActionsSetup.cs`, assets anteriores y `Combat.unity`. La utilidad explícita integra sin regenerar arena, prefabs o MainMenu y preserva datos existentes. H02/AtbSetup se adapta al nuevo campo de comando de objetos.
- Reglas: Curación 36/10/1,2 s; Tormenta potencia 1,6/18/2,8 s, **32 al ogro y 34 al chamán**; Machetazo potencia 1,5/8/2,2 s; Curación oscura 40/10/1,5 s, exclusivamente sobre el ogro vivo. Poción 50 y Éter 20, con 0,8 s y cero maná. Básicos y Golpe pesado mantienen sus valores originales.
- Pruebas: **49/49 PlayMode**, 0 fallidas/omitidas, 88,2797253 s; job `00c3fe8fd4fb4ff2a9a10eaa1902fa4b`. Regresiones H00–H02 incluidas. [Resultado](Evidence/H03/playmode-results.json) y [métodos/cobertura](TEST_REPORT.md).
- Flujo: victoria normal con teclado virtual en 19 acciones, 2 Pociones restantes; control virtual en 22 acciones, 0 Pociones. Derrota con ambos aliados preparados a 1 de vida desde el fixture; reinicios tras ambos resultados. Las peleas se aceleran a ×4 para verificar flujo; no miden balance o duración objetivo.
- Visual: selección grupal y lista de cinco objetos capturadas a 720p y 1080p; curación propia, Éter, pausa de área, recuperación, victoria y derrota. Capturas por RenderTexture, no monitor físico 1080p. [Evidencia](Evidence/H03/).
- Build: `Builds/H03/UltimaGuardia.exe`, Windows x86_64/Mono, Development, **163,17 MB**, 57,2910924 s, **0 errores / 1 advertencia** del shader DebugOccluder de Unity Core. Job `build-aaabbd3673`. [Resumen](Evidence/H03/build-summary.json).
- Player real: `-foundation-smoke` aprobado, MainMenu → Combat → pausa → MainMenu → Salir, ventana 1280 × 720, input virtual, **código de salida 0**. [Resultado](Evidence/H03/player-result-1280x720.json). Combat del ejecutable inspeccionada sin materiales faltantes. Las peleas completas corresponden a PlayMode. Se conservan avisos de red y liberación de buffers/asignaciones al apagar el proceso oculto, como en H00–H02, sin excepción de lógica. Configuraciones de entrada a Play, entrada precargada y Unity Connect conservadas/restauradas. [Huellas SHA-256](Evidence/H03/fingerprints.json).
- Limitaciones: estados activos en H04, arte/animaciones/UI/audio/VFX finales en H05–H10, dispositivos físicos en H11 y balance/rendimiento en H12. [Seguimiento](KNOWN_ISSUES.md). Tormenta utiliza impacto a 0,35 s y cierre a 0,85 s provisionales; la secuencia audiovisual final de H10 no se adelanta.
- Cómo revisar: ejecutar la build H03 o abrir MainMenu en Unity; consultar Habilidades de la maga, cancelar y confirmar Tormenta, curar tras una herida, usar Éter tras gastar maná, revisar los tres objetos pendientes, observar alternancia del ogro y curación del chamán bajo 120 de vida. Pausar, reiniciar, ganar/perder y reiniciar. Pasos y valores esperados en [README](../README.md).
- Revisión del usuario: **aprobada** el 21 de noviembre de 2026. Evidencia: «Bien. Queda aprobado. Dame nombre en ingles para el commit». Sin correcciones solicitadas. H04 permanece pendiente para otro chat.

#### Registro H04

- Estado: **Aprobado por el usuario** el 21 de noviembre de 2026. H05 no iniciado.
- Versión: **0.0.5-H04** (PlayerSettings 0.0.5), cambios locales sobre commit real `8ea908e` (H03). No se creó un commit de entrega.
- Fecha: 21 de noviembre de 2026. Autorización: «Dale! Entonces hacelo», después de leer y explicar H04. El usuario pidió continuar tras recuperar tokens; se conservó el mismo hito. H00–H03 aprobados, sin correcciones abiertas.
- Tareas completadas: **H04-T01 a H04-T15**. Ninguna tarea H05 o posterior iniciada.
- Entrega: instancias de efectos con snapshots e identidad de aplicación, Veneno al inicio del turno, Rapidez y Corrosión por acciones, Antídoto inmediato al confirmar, Tónico y Frasco funcionales, reaplicación sin acumular potencia, coexistencia, estadísticas efectivas y limpieza al morir/reiniciar. IA del chamán completa: curación → aliado vivo sin veneno → Descarga.
- Duraciones: Veneno aplica 8 tres veces, antes de decisión/IA; puede matar, limpiar cola y producir resultado sin gasto. Rapidez ×1,5 y Corrosión ×0,5 conservan decimales y duran las próximas dos acciones del afectado. ATB acumulado y recuperación fija no cambian. Las aplicaciones presentes al inicio se descuentan al finalizar; usar Tónico propio o renovar durante esa acción termina con dos cargas. Antídoto exige Veneno, lo elimina al confirmar y no devuelve el daño recibido.
- Datos y conservación: se preservaron GUID/rutas de H02, estadísticas, costos, cantidades, arena, prefabs y MainMenu. Las descripciones ya no indican pendientes de H04. No hubo ajustes de balance ni recursos externos. Las referencias finales de arte/audio/animación permanecen para sus hitos.
- Archivos principales: `CombatSession.cs`, `CombatPrototype.cs`, `EncounterData.cs`, `StatusState.cs`, `StatusHud.cs`, `StatusIcon.cs`, `Assets/Scripts/Editor/StatusSetup.cs`, `Assets/Tests/PlayMode/StatusRulesTests.cs` y ampliaciones de pruebas. Utilidad explícita H04 ya ejecutada sobre Combat existente; no regenera escenas ni restaura números de configuración.
- UI: nombres, duración y formas provisionales (gota verde, flecha azul, escudo roto naranja) en cada participante. Fondos independientes por fila y compactación evitan nombres 3D detrás de contadores sin reducir el encuadre. Arte final de H08 pendiente.
- Pruebas: **68/68 PlayMode**, 0 fallidas/omitidas, **119,2678023 s**, job `09ee960906414054887eda50084c9e17`; regresiones H00–H03 incluidas. Después del ajuste visual final, **4/4 pruebas de flujo**, **41,299563 s**, job `842711d79d0a4c7ba9f98faebe245697`. [Suite](Evidence/H04/playmode-results.json), [regresión visual/flujo](Evidence/H04/flow-after-ui-fix.json), [informe y métodos](TEST_REPORT.md).
- Peleas: victoria normal con teclado virtual en **22 acciones, 0 Pociones**, y con control virtual en **22 acciones, 0 Pociones**. Derrota con aliados a 1 y derrota por Veneno con último aliado a 8 en fixtures. Reloj ×4 solo para automatización; no son mediciones de balance, duración objetivo ni hardware.
- Visual: coexistencia de tres estados en 720p/1080p, uso/cancelación de Tónico con control virtual, Frasco con pausa antes/después de impacto, Antídoto eliminado al confirmar, derrota por Veneno y reinicios. RenderTexture con Canvas/cámara reales; no acredita monitor físico 1080p. [Capturas](Evidence/H04/).
- Build: `Builds/H04/UltimaGuardia.exe`, Windows x86_64/Mono, Development, **163,18 MB**, **77,8586972 s**, **0 errores / 1 advertencia** del shader DebugOccluder de Unity Core/d3d11, confirmada por BuildReport. Job `build-91496c0737`. [Resumen](Evidence/H04/build-summary.json), [detalle](Evidence/H04/build-report-details.json).
- Player real: `-foundation-smoke` aprobado, versión `0.0.5-flow`, MainMenu → Combat → pausa → MainMenu → Salir, ventana/captura 1280 × 720, input virtual y **código 0**. [Resultado](Evidence/H04/player-result-1280x720.json). Combat inspeccionada sin materiales faltantes. Las peleas completas son PlayMode. Se conservan avisos de red/liberación de buffers/asignaciones al apagar el proceso oculto; evaluación interactiva/prolongada en H12. Configuración de entrada a Play y Unity Connect recibida conservada/restaurada. [Huellas](Evidence/H04/fingerprints.json).
- Correcciones de verificación: primera pasada interrumpida por enrutamiento de input sin foco; resuelto solo en fixtures con restauración. Pasada dirigida 2/17 por duplicación de IDs de Veneno al clonar fixtures; corregida preservando referencias compartidas. Fondo de contadores ajustado y regresiones repetidas. Historial y límites en [TEST_REPORT.md](TEST_REPORT.md) y [KNOWN_ISSUES.md](KNOWN_ISSUES.md).
- Cómo revisar: ejecutar build H04 o MainMenu en Unity; usar Tónico propio y contar 2 → 1 → 0; Frasco al ogro y comprobar básico del guerrero 25 frente a 22 tras vencer; observar Veneno sin daño directo y 8 al comienzo de turnos; usar Antídoto y comprobar unidad/limpieza sin curación; pausar/reiniciar y verificar recursos y estados vacíos. Detalle en [README](../README.md).
- Limitaciones: control físico y monitor nativo 1080p no comprobados; modelos, escenario, UI, audio, animación y VFX finales posteriores. Sin ajustes de balance.
- Revisión del usuario: **aprobada** el 21 de noviembre de 2026. Evidencia: «Bien! Queda aprobado entonces.». Sin correcciones solicitadas. H05 permanece pendiente para otro chat.
- Próximo hito en orden: **H05**, identificación solamente; **no iniciado**.

#### Registro H05

- Estado: **Aprobado por el usuario** el 21 de noviembre de 2026. H06 no iniciado.
- Versión: **0.0.6-H05** (PlayerSettings 0.0.6), cambios locales sobre commit real `3e1bec1` de H04. No se creó commit de entrega.
- Fecha: 21 de noviembre de 2026. Autorización: «Bien. Hacelo entonces.», después de leer y explicar el alcance H05. H00–H04 aprobados y sin correcciones abiertas.
- Tareas completadas: **H05-T01 a H05-T25**. No se inició H06 ni otro hito posterior.
- Entrega: cuatro modelos propios de estilo facetado con materiales de color plano; guerrero con armadura/espada, maga con túnica/sombrero/bastón de cristal, ogro voluminoso con colmillos/hombrera/machete y chamán con máscara/cuernos/bastón ritual. 20 materiales URP, 7.598 triángulos en total y 16 articulaciones por personaje. Armas en agarres de mano y sockets de conjuro, objeto e impacto.
- Representación animable: alternativa de piezas rígidas admitida por H05-T22. Articulaciones de cadera, torso, cabeza, hombros, codos, manos, muslos, rodillas, pies y agarre; túnicas segmentadas. Sin piel continua, rig Humanoid ni simulación de tela. Seis poses estáticas por modelo comprueban reposo, ataque, conjuro, reacción, objeto y caída; no completan clips ni controladores H06.
- Fuentes: `Assets/Scripts/Editor/CharacterModeling.cs`, `CharacterArtValidation.cs`, `Assets/Scripts/Combat/CharacterRig.cs`; mallas guardadas en `Assets/Models/H05/Meshes`, exportaciones OBJ/MTL/jerarquía JSON en `Assets/Models/H05/Exports`, materiales en `Assets/Materials/H05` y prefabs en `Assets/Prefabs/H05`. [Dirección visual y reconstrucción](H05_CHARACTER_ART.md), [autoría y recursos](ASSET_REGISTER.md).
- Integración: prefabs sustituidos en los anclajes existentes de Combat; cuatro referencias de prefab actualizadas en los datos H02. Se conservan estadísticas, costos, inventario, IA, estados, arena y MainMenu. Cámara fija ajustada a FOV 50 y mirada a (0; 0,1; 2) para reservar espacio a tres filas de estados. Sin ajustes de balance.
- Comprobación de recursos: [auditoría aprobada](Evidence/H05/model-audit.json), modelos importados, mallas/normales/índices válidos, materiales URP, referencias y sockets coherentes, altura y contacto con suelo. Los cuatro OBJ importados conservan materiales remapeados. Vistas [frontal](Evidence/H05/character-lineup-front.png) y [trasera](Evidence/H05/character-lineup-back.png) y cuatro hojas de poses inspeccionadas.
- Regresión: **68/68 PlayMode**, 0 fallidas/omitidas, **117,155875 s**, job `2f8f396d49b84754b8f538c24cb99b18`. Después del ajuste de cámara, **4/4 regresiones de flujo**, job `ab768f9dfe5044d1afc8d9a566c77dc0`. [Suite](Evidence/H05/playmode-results.json), [estado final de regresión](Evidence/H05/flow-after-framing-fix.json), [métodos y límites](TEST_REPORT.md). El resultado detallado del segundo job no se conservó tras recarga de dominio; se registra su estado real sin inventar una duración.
- Peleas de regresión: victoria con teclado virtual en 21 acciones, 1 Poción restante; control virtual en 22 acciones, 0 Pociones. Derrotas preparadas y reinicios comprobados. Reloj ×4 y dispositivos virtuales; no acredita balance, duración objetivo o hardware.
- Visual en juego: capturas 720p/1080p con cámara/Canvas reales mediante RenderTexture, incluyendo tres estados sobre la maga, objetivos, listas, pausa y reinicio. No acredita monitor físico 1080p. [Evidencia](Evidence/H05/).
- Build: `Builds/H05/UltimaGuardia.exe`, Windows x86_64/Mono, Development, **164,21 MB**, **153,4675862 s**, **0 errores / 1 advertencia** heredada de DebugOccluder de Unity Core/d3d11. Job `build-56b0a79878`. [Resumen](Evidence/H05/build-summary.json), [BuildReport](Evidence/H05/build-report-details.json).
- Player real: smoke aprobado, versión `0.0.6-flow`, MainMenu → Combat → pausa → MainMenu → Salir, ventana/captura 1280 × 720, input virtual y **salida 0**. [Resultado](Evidence/H05/player-result-1280x720.json). Captura del ejecutable inspeccionada sin materiales faltantes. Las peleas completas corresponden a PlayMode. Se conservan avisos de red y liberación de buffers/asignaciones al apagar el proceso oculto, como en H00–H04; seguimiento H12. Configuración de entrada a Play y Unity Connect conservada/restaurada. [Huellas de entrega](Evidence/H05/fingerprints.json).
- Correcciones internas cerradas: normales explícitas de OBJ, encuadre/aspecto de hojas de poses y cámara de combate para estados sobre modelos. [Seguimiento H05-C01 a C03](KNOWN_ISSUES.md). Sin correcciones abiertas del usuario.
- Cómo revisar: ejecutar la build H05 o MainMenu en Unity; observar las cuatro siluetas, materiales y armas, seleccionar objetivos, atacar, usar objetos, pausar y reiniciar. Para revisar articulación, consultar las hojas de poses o inspeccionar `CharacterRig` en los prefabs H05. Guía en README. La presentación de acciones continúa provisional hasta H06.
- Revisión del usuario: **aprobada** el 21 de noviembre de 2026. Evidencia: «Bien. Lo doy por aprobado.». Sin correcciones solicitadas. H06 permanece pendiente para otro chat.
- Próximo hito en orden: **H06**, identificación solamente; **no iniciado**.

#### Registro H06

- Estado: **Aprobado por el usuario** el 21 de noviembre de 2026. H07 no iniciado.
- Revisión del usuario: **aprobada**. Evidencia: «Bien. Esta aprobado. Dame ademas un nombre en ingles para el commit.». Sin correcciones solicitadas; no se pidió comenzar H07.
- Versión: **0.0.7-H06** (PlayerSettings 0.0.7), cambios locales sobre commit real `34592c6` de H05. No se creó commit de entrega.
- Fecha: 21 de noviembre de 2026. Autorización: «Bien, hacelo entonces.», después de explicar el alcance H06. H00–H05 aprobados, sin correcciones abiertas del usuario.
- Tareas completadas: **H06-T01 a H06-T19**. Ningún hito posterior iniciado.
- Entrega: 24 clips propios (6 guerrero, 7 maga, 5 ogro, 6 chamán), cuatro controladores Animator y cuatro CharacterAnimationSet sobre los prefabs H05 existentes. Reposos cíclicos, básicos/pesados, conjuros, gesto de los cinco objetos en ambos aliados, reacciones y muerte terminal. Armas, ropa segmentada y sockets conservados.
- Ejecución: CombatPresentation es el único dueño del tiempo de cada acción y de la señal de impacto, con protección adicional de CombatSession. Proyectiles desde CastOrigin hacia HitAnchor del objetivo confirmado, sin volver a elegirlo. Pausa congela clips, trayectoria e impacto; reinicio y salida cancelan y limpian la presentación. Sin eventos de gameplay en los clips.
- Tiempos visuales: físicos 0,85 s, pesados 1 s, magia ordinaria 0,95 s, objetos 0,85 s; impacto ordinario a 0,35 s y lanzamiento de básicos mágicos a 0,15 s. La pose de Tormenta dura 2,5 s e impacta a 1,2 s; cámara, ocultamiento de HUD y efectos/sonido finales siguen en H10. Recuperaciones y demás reglas de H04 conservadas, sin cambios de balance.
- Archivos: `Assets/Animations/H06`, los cuatro prefabs H05, diez referencias ActionData.animationClip, `CharacterAnimationAuthoring.cs`, `CharacterAnimationValidation.cs`, `CharacterAnimationSet.cs`, `CharacterAnimationPlayer.cs`, `CombatPresentation.cs`, integración en CombatPrototype/CombatantView y pruebas. [Funcionamiento y reconstrucción](H06_ANIMATIONS.md), [autoría](ASSET_REGISTER.md).
- Auditoría: [aprobada](Evidence/H06/animation-audit.json), 24 clips con 112 curvas cada uno, controladores conectados, muerte sin transición de salida, sin root motion ni eventos de gameplay. Retorno de posiciones/rotaciones comprobado en cada clip no terminal; caída sobre el suelo con mínimo Y = 0 en los cuatro personajes. Cuatro hojas de secuencias inspeccionadas.
- Pruebas: **71/71 PlayMode**, 0 fallidas/omitidas, **148,2203066 s**, job `a5e373a2386d4d4f95924c960b2e80b9`. Incluye regresiones H00–H05 y tres nuevas pruebas de presentación: [matriz de 20 casos](Evidence/H06/action-matrix.txt), pausa antes/después de impacto, gasto único, recuperación, trayectoria, muerte estable, restauración y salida sin callbacks anteriores. [Resultados completos](Evidence/H06/playmode-results.json), [métodos](TEST_REPORT.md).
- Peleas de regresión: victorias con teclado y control virtual, ambas de 22 acciones y 0 Pociones restantes, derrotas preparadas y reinicios. Tiempo ×4 en los fixtures de pelea; no son mediciones de balance, duración objetivo o hardware físico. Capturas de juego en 720p/1080p con cámara/Canvas reales mediante RenderTexture; no acredita monitor físico 1080p.
- Build: `Builds/H06/UltimaGuardia.exe`, Windows x86_64/Mono, Development, **164,68 MB**, **93,3874899 s**, **0 errores / 1 advertencia** heredada de DebugOccluder de Unity Core/d3d11. Job `build-bc35a877d5`. [Resumen](Evidence/H06/build-summary.json), [BuildReport](Evidence/H06/build-report-details.json).
- Player real: smoke ampliado aprobado, versión `0.0.7-flow`, MainMenu → Combat → básico animado de maga con pausa/reanudación e impacto → pausa → MainMenu → Salir, captura/ventana 1280 × 720 y **salida 0**. [Resultado](Evidence/H06/player-result-1280x720.json). Capturas del player inspeccionadas. Las peleas completas corresponden a PlayMode. Avisos heredados de red y liberación de buffers/asignaciones al apagar el player oculto conservados, seguimiento en H12; no se atribuye una causa no demostrada.
- Correcciones internas H06-C01 a C03 cerradas: nulidad de componentes Unity en autoría, expectativa de Éter en el fixture y espera de frame para capturas de poses. Primera pasada dirigida 2/3, segunda 3/3 y suite final 71/71. [Incidencias y límites](KNOWN_ISSUES.md). Sin correcciones abiertas del usuario.
- Conservación: versión Unity/renderer/paquetes, estadísticas, inventario, estados, IA, arena y menús conservados. Opciones de entrada/tiempo de fixtures restauradas y opciones de entrada a Play/Unity Connect restituidas tras pruebas/build. No se compraron recursos ni se añadieron servicios pagos. [Huellas](Evidence/H06/fingerprints.json).
- Cómo revisar: ejecutar la build H06 y elegir Jugar; observar básicos, Golpe pesado, Machetazo, Curación propia, objetos y conjuros. Pausar durante preparación o vuelo y después del impacto; reanudar, reiniciar y volver al menú. Los derrotados conservan la caída y reiniciar devuelve articulaciones y recursos al estado inicial. Guía en README y H06_ANIMATIONS.
- Próximo hito en orden: **H07**, identificación solamente; **no iniciado**.

#### Registro H07

- Estado: **Aprobado por el usuario** el 21 de noviembre de 2026. H08 no iniciado.
- Versión: **0.0.8-H07** (PlayerSettings 0.0.8), cambios locales sobre commit real `4a11d47` de H06. No se creó commit de entrega.
- Fecha: 21 de noviembre de 2026. Autorización: «Bien. Entonces hacelo.», después de explicar el alcance H07. H00–H06 aprobados, sin correcciones abiertas del usuario.
- Tareas completadas: **H07-T01 a H07-T15**. Ninguna tarea posterior iniciada.
- Entrega: fortaleza final con siete módulos propios: pavimento, muro de hiladas alternadas, almenas, portón de madera con arco/herrajes, contrafuerte, estandarte y brasero. 56 instancias/renderers, 25.650 triángulos, ocho materiales URP compartidos, fuente editable y siete exportaciones OBJ/MTL con materiales remapeados. Sustituido el blockout visible; no se descargaron ni compraron recursos.
- Integración: `Arena_Fortress_H07` en Combat, suelo con superficie Y = 0 y collider, luz principal cálida con sombras, relleno frío sin sombras y ambiente. Conservados los cuatro anclajes, modelos H05, animadores/datos, Canvas, MainMenu, estadísticas, inventario, IA, estados y reglas. Sin cambios de balance, motor, renderer ni paquetes.
- Archivos: `Assets/Prefabs/H07`, `Assets/Models/H07/{Meshes,Exports}`, `Assets/Materials/H07`, `FortressAuthoring.cs`, `FortressValidation.cs` y Combat. [Dirección, módulos y reconstrucción](H07_FORTRESS.md), [autoría](ASSET_REGISTER.md).
- Auditoría final: [aprobada](Evidence/H07/environment-audit.json), siete mallas/OBJ importados y materiales URP, normales/UV y triángulos válidos; 24 clips en 216 poses, 648 líneas de visión sin oclusión del entorno, contacto con tolerancia de interpolación de 4 mm y anclajes exactos. 21 muestras de cámara especial reservada, maga y bastón completos en su encuadre y corredores verticales libres para rayos. La secuencia runtime/VFX/audio final sigue en H10.
- Inspección visual: vistas base 720p/1080p, conjunto, cámara especial reservada, HUD, selección, estados, conjuros/proyectiles, caídas, pausa y capturas del ejecutable. [Evidencia H07](Evidence/H07/). Capturas RenderTexture; no acredita monitor físico 1080p.
- Regresión: **71/71 PlayMode**, 0 fallidas/omitidas, **151,6974113 s**, job `6ee56d0f19fe4a818bc77c78b1587ff4`. [Resultados](Evidence/H07/playmode-results.json), [matriz de 20 acciones/objetos](Evidence/H07/action-matrix.txt), [métodos y límites](TEST_REPORT.md). Victorias con teclado y control virtual, ambas de 22 acciones y 0 Pociones, derrotas preparadas y reinicios. Tiempo ×4; no mide balance, duración objetivo ni hardware físico.
- Build: `Builds/H07/UltimaGuardia.exe`, Windows x86_64/Mono, Development, **166,62 MB**, **73,7210562 s**, **0 errores / 1 advertencia** heredada de DebugOccluder de Unity Core/d3d11. Job `build-2f205ce4bb`. [Resumen](Evidence/H07/build-summary.json), [mensaje de compilación](Evidence/H07/build-messages.txt).
- Player real: smoke aprobado, versión `0.0.8-flow`, MainMenu → Combat con fortaleza → básico animado de maga con pausa/reanudación e impacto → pausa → MainMenu → Salir; ventana/captura 1280 × 720, input virtual y **salida 0**. [Resultado](Evidence/H07/player-result-1280x720.json). No es una pelea completa del ejecutable. Avisos heredados de red y buffers/asignaciones al cerrar el proceso oculto conservados para H12. Unity Connect restituido tras la build.
- Correcciones internas cerradas: **H07-C01**, contacto del pie en cuatro ataques H06, compensación de tobillo sin cambiar tiempos/anclajes; **H07-C02**, recorte del bastón en primera propuesta especial; **H07-C03**, remapeo URP de OBJ importados. Auditoría final y regresiones correspondientes aprobadas. [Incidencias y límites](KNOWN_ISSUES.md). Sin correcciones abiertas del usuario.
- Cómo revisar: ejecutar la build H07 y elegir Jugar, o MainMenu en Unity. Observar portón, piedra, decoración, sombras y apoyos; usar ataques físicos, proyectiles, objetos y conjuros; seleccionar ambos enemigos, observar muertes, pausar/reanudar y reiniciar. Inspeccionar `Arena_Fortress_H07` y prefabs de H07 para módulos editables. La cámara especial mostrada en evidencia es una reserva, no una secuencia implementada.
- Revisión del usuario: **aprobada** el 21 de noviembre de 2026. Evidencia: «Bien. Queda aprobado. Dame nombre en ingles para el commit tambien.». Sin correcciones solicitadas; no se pidió comenzar H08.
- Próximo hito en orden: **H08**, identificación solamente; **no iniciado**.

#### Registro H08

- Estado: **Aprobado por el usuario** el 21 de noviembre de 2026, incluida corrección H08-C06. H09 no iniciado.
- Versión: **0.0.9-H08**, cambios locales sobre commit real `209626c` (H07). No se creó un commit de entrega.
- Fecha: 21 de noviembre de 2026. Autorización: «Bien. Hacelo entonces.» después de explicar H08; continuidad tras interrupción: «Me quede sin tokens y cortaste. Ahora podes continuar.». H00–H07 aprobados, sin correcciones abiertas del usuario.
- Tareas completadas: **H08-T01 a H08-T23**. Ninguna tarea de H09 o posterior iniciada.
- Entrega: diseño oscuro/dorado con tipografía incorporada de Unity; 26 sprites propios e importación 2D; identificadores de cuatro combatientes, equipos, vida/maná actuales y máximos, barras, ATB, recuperación y estados con cargas; listas y restricciones consultables; descripción antes de confirmar; rombo de actor y aros de objetivos individuales/grupales; números efectivos de daño/vida/maná y registro de cuatro eventos.
- Menús: Jugar, Cómo jugar, Volumen y Salir; ayuda de cuatro páginas desde principal/pausa; volumen 0–100 de a 10 conservado durante la sesión; pausa con cinco opciones; reinicio/abandono con Cancelar seleccionado por defecto; resultados sin confirmación adicional. Foco visible, retorno a selección anterior, navegación circular, ayudas por dispositivo y pulsaciones nuevas para confirmar.
- Integración: Canvas finales de MainMenu/Combat; datos de combatientes/acciones/ítems/efectos con referencias visuales; `FinalCombatHud`, `InterfaceTheme`, `FoundationMenu`, `InterfaceAuthoring` y `InterfaceBuild`. Modelos, clips, fortaleza, anclajes, cámara, reglas, inventario y balance conservados. [Fuentes, estilos y reconstrucción](H08_INTERFACE.md), [licencias y autoría](ASSET_REGISTER.md).
- Auditorías: [26 sprites y 23 assets de encuentro](Evidence/H08/resource-audit.json), sin cambios de valores jugables; [preservación de escena](Evidence/H08/scene-preservation.json), 150 bloques anteriores intactos y cambios limitados a referencias de UI/marcadores/raíces.
- Pruebas: **76/76 PlayMode**, sin fallidas/omitidas, **177,3846759 s**, job `28ccbde3a92b4885b3ef5eacf4f9bab9`. Después del ajuste final de etiquetas/números se repitieron las siete pruebas afectadas: **7/7**, **48,6678073 s**, job `75f17e3b77ab4eb59ffa3a605be9dd24`. [Suite completa](Evidence/H08/playmode-results.json), [regresión visual final](Evidence/H08/final-ui-results.json), [métodos y límites](TEST_REPORT.md). Teclado y Gamepad virtuales; no acredita hardware físico.
- Inspección: menú, ayuda, volumen, pausa, confirmaciones, recursos/estados, objetos, selección grupal, números y resultados a 720p/1080p. [Capturas canónicas finales](Evidence/H08/Run-20261121-200944-516-0dcacd/). RenderTexture; no acredita monitor físico 1080p. Las imágenes sueltas preliminares no sustituyen las capturas finales enlazadas.
- Build: `Builds/H08/UltimaGuardia.exe`, Windows x86_64/Mono, Development, **168,33 MB**, **52,4549261 s**, **0 errores / 1 advertencia** heredada de DebugOccluder de Unity Core/d3d11. Creada mediante `InterfaceBuild.BuildReview`; [resumen y mensajes BuildReport](Evidence/H08/build-summary.json).
- Player: MainMenu → ayuda → Combat → básico animado con pausa/reanudación e impacto → pausa → retorno confirmado → MainMenu → Salir. Versión `0.0.9-flow`, **passed = true**, **animatedActionVerified = true**, salida desde Salir y **código 0**. [Resultado](Evidence/H08/C06/player-result-1280x720.json), [captura del player](Evidence/H08/C06/animated-action-complete-1280x720.png). Ventana/captura 1280 × 720, proceso oculto/input virtual; no es una pelea completa. Avisos de buffers/asignaciones al apagar conservados para H12. Unity Connect y opciones de entrada a Play restituidos.
- Correcciones internas cerradas: orientación de marcadores H08-C01; expectativas provisionales del fixture H08-C02; capturas abiertas en Windows H08-C03; superposición de etiquetas/números H08-C04; expectativa de texto blanco del smoke H08-C05. Resultados fallidos conservados y comprobaciones afectadas repetidas. [Incidencias](KNOWN_ISSUES.md). Sin correcciones abiertas del usuario.
- Cómo revisar: ejecutar la build H08 o MainMenu en Unity. Recorrer ayuda y volumen, jugar, consultar todas las categorías y restricciones, seleccionar ambos bandos y Tormenta, observar números/registro, pausar y volver desde ayuda/volumen, cancelar y aceptar un reinicio/abandono, terminar y reiniciar desde resultado. Comprobar que el volumen se conserva y que Salir cierra la build. [Pasos completos](../README.md).
- Revisión del usuario: **aprobada**, incluida H08-C06. Evidencia: «Ya esta, ya quedo bien. Te lo doy por aprobado. Dame el nombre del commit en base a todo lo que hiciste.» Sin correcciones abiertas; no se solicitó iniciar H09.
- Próximo hito en orden: **H09**, identificación solamente; **no iniciado**.

- Corrección H08-C06 (solicitada por el usuario): textos de menús pixelados en comparación con el combate. Migradas 13 etiquetas de MainMenu y 3 de overlays de Combat a glifos de doble resolución con Text nativo, rectángulos dobles y escala 0,5; sin cambiar tamaño visual, controles ni HUD. Fuente `InterfaceTheme.MenuText` y autoría actualizadas. Cinco pruebas de interfaz repetidas desde Test Runner: 5/5, 37,1183806 s. [XML real](Evidence/H08/C06/playmode-results.xml), [auditoría de 16 etiquetas](Evidence/H08/C06/menu-text-audit.json), [capturas de esta corrección](Evidence/H08/Run-20261121-232425-443-3746e2/). Build corregida recompilada: 52,4549261 s, 176.509.903 bytes, 0 errores y 1 advertencia heredada. Smoke de la build corregida aprobado, acción animada verificada y salida desde Salir/código 0. [Player C06](Evidence/H08/C06/player-result-1280x720.json). Revisión final aprobada por el usuario el 21 de noviembre de 2026.

#### Registro H09

- Estado: **Aprobado por el usuario** el 21 de noviembre de 2026. Versión **0.0.10-H09**; no se creó commit de entrega.
- Autorización: «Bien. Hacelo entonces.» el 21 de noviembre de 2026; H08 y H08-C06 ya aprobados.
- Producción: 28 clips WAV originales generados con `Tools/H09/generate_audio.py`; sin recursos de terceros. `Assets/Resources/Audio/H09`, `GameAudio`, `AudioImportH09`, conexiones de menú y combate. [Autoría, mapa y revisión](H09_AUDIO.md), [registro de recursos](ASSET_REGISTER.md).
- Pruebas: **76/76 PlayMode** de regresión antes de la corrección final de inicio de música, más **1/1** prueba específica después de esa corrección y **1/1** tras ampliarla a volumen 0/100. [Auditoría de señales](Evidence/H09/audio-audit.json): 28 clips a 24 kHz mono, pico máximo 0,820, salto del borde musical 0,006653 frente a un salto interno máximo de 0,01233. Se corrigieron transiciones de notas bruscas y una aplicación de Veneno que habría repetido el mismo cue de aplicación en preparación e impacto. Se comprobó recompilación sin errores.
- Build final: `Builds/H09/UltimaGuardia.exe`, Windows x86_64/Mono, Development, **169,3 MB**, **64,7044 s**, **0 errores / 1 advertencia** heredada de DebugOccluder; job `build-ffc23726ec`. [Resumen](Evidence/H09/build-summary.json). Un intento anterior conservó por error 0.0.9 en el player; se ajustó PlayerSettings en el editor y se recompiló.
- Player final: smoke **passed = true, animatedActionVerified = true, audioVerified = true**, versión `0.0.10-flow`, MainMenu → Combat → básico animado con música, pausa/reanudación sonora, retorno confirmado y Salir; **código 0**. [Resultado](Evidence/H09/player-result-1280x720.json). Input virtual y proceso oculto; no demuestra escucha humana ni una pelea completa del ejecutable. La escucha final con parlantes o auriculares corresponde a la revisión del usuario.
- Revisión del usuario: **aprobada**. Evidencia: «Bien. Lo doy por aprobado y dame nombre en ingles para el commit.» Sin correcciones solicitadas ni commit creado por la IA.
- Próximo hito en orden: **H10**, identificación solamente; **no iniciado**.
- H10 se solicita en otro chat, según la forma de trabajo entre hitos.

La siguiente plantilla se conserva para las próximas entregas. Mantener un registro por hito y actualizarlo en cada corrección, conservando un historial breve.

```markdown
#### Registro HXX

- Estado: Pendiente / En curso / Bloqueado / Requiere correcciones / Implementado y verificado / Aprobado por el usuario.
- Versión del proyecto o commit real, si existe:
- Última actualización:
- Tareas completadas: IDs.
- Tareas pendientes o bloqueadas: IDs y motivo.
- Archivos, escenas o recursos principales:
- Comprobaciones realizadas: procedimiento, condiciones y resultado.
- Comprobaciones pendientes:
- Cómo revisarlo: pasos concretos y resultado esperado.
- Revisión del usuario: pendiente / aprobada / con correcciones.
- Evidencia de aprobación: pedido o mensaje real del usuario, si existe.
- Correcciones abiertas: ID, descripción, tarea afectada y criterio de cierre.
- Correcciones resueltas y regresiones comprobadas:
- Próximo hito en orden: identificación solamente; no iniciarlo en este chat.
```



Game Design Document | Prototipo de combate por turnos con ATB

Versión 1.1 | 21 de noviembre de 2026 | Se incorpora la forma de trabajo entre chats y el seguimiento de revisiones.

Plataforma: PC. Motor: Unity. Modalidad: un jugador. Idioma: español.

Documento de diseño para el trabajo práctico TSGD-TP4. Define un combate completo y acotado que cubre los requisitos de las notas 4, 7 y 10. Los valores numéricos son una propuesta inicial para implementar y ajustar mediante pruebas. Cubrir los requisitos permite apuntar a la nota máxima, pero la evaluación también depende del funcionamiento y la presentación del prototipo.

## A. Preguntas resueltas antes del diseño

Estas decisiones cierran los puntos que habían quedado abiertos en la idea inicial. Forman parte del diseño de esta versión.

### ¿Cuál es el alcance exacto?

Una pelea fija de dos aliados contra dos enemigos, en una arena 3D con cámara fija. Hay menú principal, ayuda, pausa y pantalla de resultado. La partida se puede reiniciar. No hay exploración, experiencia, equipamiento, campaña ni guardado de progreso.

### ¿Por qué controlar solamente dos personajes?

Permite generar una relación clara entre daño y apoyo, mostrar selección de aliados y mantener viable el trabajo de animación, interfaz y balance. Ambos personajes pueden usar el inventario compartido.

### ¿El combate es en tiempo real o por turnos?

Las barras avanzan con el tiempo, pero las acciones se ejecutan de a una. Cuando alguien queda listo, se detiene el reloj del combate. Si es un aliado, se espera su decisión. Si es un enemigo, se ejecuta su decisión automática. Este modo de espera permite pensar sin perder turnos al navegar los menús.

### ¿Qué pasa si dos barras se completan al mismo tiempo?

Se crea una cola de participantes listos. Se ordena por el momento en que alcanzaron el máximo. Si hay empate, actúa primero el de mayor velocidad efectiva. Si persiste, se usa este orden fijo: maga, guerrero, chamán y ogro. Se vacía la cola antes de reanudar el reloj.

### ¿Qué diferencia hay entre velocidad y recuperación?

La velocidad determina cuánto tarda en llenarse la barra. La recuperación es una espera adicional después de la acción, antes de que vuelva a llenarse. Las habilidades potentes tienen una recuperación mayor. Los dos tiempos usan el mismo reloj del combate.

### ¿Se puede cancelar una selección o perder un turno por equivocarse?

Se puede volver desde objetivos a acciones sin gastar recursos ni tiempo. El gasto ocurre únicamente al confirmar una acción válida. No existe un límite de tiempo para decidir. Cancelar en el menú principal de acciones no salta el turno.

### ¿Qué ocurre cuando no queda maná?

Cada participante conserva un ataque básico gratuito. Los aliados también pueden usar objetos. Las habilidades sin recursos suficientes aparecen deshabilitadas con su costo visible. Los enemigos usan su ataque básico si no pueden pagar otra acción.

### ¿Cómo funciona la muerte?

Al llegar a cero de vida, el personaje queda fuera del combate, pierde sus estados y sale de la cola. No puede actuar ni ser seleccionado por curaciones u objetos. No hay resurrección. Un solo aliado superviviente puede completar la pelea.

### ¿El veneno puede matar antes de elegir una acción?

Sí. El daño se aplica al comenzar el turno del afectado, antes de mostrar el menú o ejecutar la inteligencia enemiga. Si muere, se cancela ese turno y se comprueba si terminó el combate.

### ¿Cuándo vencen los efectos temporales?

El veneno pierde una carga cada vez que aplica su daño al inicio de un turno. Rapidez y corrosión pierden una carga después de que el afectado completa una acción. Un efecto aplicado durante la propia acción no pierde duración en esa misma acción. Las animaciones y los menús no reducen duraciones.

### ¿Pueden acumularse los estados?

Los estados diferentes pueden coexistir. Un mismo estado no acumula potencia. Volver a aplicarlo renueva su duración. La muerte elimina todos los efectos y la muerte del aplicador no elimina los efectos que ya causó.

### ¿La habilidad especial necesita una barra propia?

No. Tormenta arcana usa maná y tiene una recuperación larga. Su tratamiento especial es audiovisual: oculta la interfaz de combate y utiliza una breve secuencia de cámara, animación, partículas y sonido. No requiere otro recurso ni un sistema de carga.

### ¿Qué implica el soporte completo para control?

Es posible abrir el juego, consultar la ayuda, ajustar el volumen, combatir, elegir objetivos, pausar, reiniciar y volver al menú usando solamente teclado o solamente control. La navegación siempre tiene un elemento seleccionado y admite volver al menú anterior.

### ¿Qué estrategia debería tener sentido?

Concentrar ataques en el chamán reduce curaciones y veneno. Atacar primero al ogro reduce la principal fuente de daño. La maga debe repartir su maná entre curación y daño de área. Los objetos permiten corregir una situación peligrosa, pero consumen una acción y son limitados.

### ¿Cómo se evita agrandar el proyecto?

Se mantiene una única composición de enemigos, cinco acciones aliadas, cinco objetos y tres efectos temporales contando veneno, rapidez y corrosión. El contenido adicional queda fuera de esta versión. Los ajustes se concentran en números, legibilidad y errores de funcionamiento.

## B. GDD

## 1. Concepto y experiencia

Última guardia es un RPG de combate por turnos con barras ATB. Un guerrero y una maga defienden la entrada de una fortaleza frente a un ogro y un chamán. El jugador administra vida, maná, tiempos de recuperación y consumibles para derrotar a los dos enemigos antes de perder a sus personajes.

La experiencia busca que cada elección sea fácil de comprender: pegar ahora, gastar más tiempo para pegar fuerte, curar a un compañero o usar un objeto. La presión viene del daño y de los recursos que se agotan, mientras que el modo de espera permite evaluar las opciones con calma.

La duración objetivo es de tres a cinco minutos por intento, incluyendo selección de acciones y animaciones. Es una meta de diseño pendiente de playtesting. El prototipo se dirige a jugadores que conocen los RPG por turnos y también a quienes necesitan una introducción breve al sistema.

### Criterios que guían las decisiones

Roles reconocibles. El guerrero resiste y golpea fuerte, la maga combina apoyo y daño de área, el ogro amenaza con daño y el chamán sostiene al equipo enemigo.

Consecuencias visibles. El costo de maná, el objetivo y la recuperación se informan antes de confirmar. Los cambios de vida, estados y turnos tienen una respuesta visual y sonora.

Alcance cerrado. Una sola pelea debe permitir probar todas las funciones pedidas sin necesitar contenido adicional.

## 2. Objetivos, estructura y alcance

Objetivo del jugador: derrotar al ogro y al chamán. Objetivo inmediato: elegir una acción útil cada vez que un aliado queda listo. Objetivo de gestión: mantener al menos un aliado con vida y conservar recursos suficientes para cerrar la pelea.

La partida sigue esta estructura: menú principal, comienzo del combate, avance de barras, resolución de turnos, victoria o derrota y elección de reiniciar o volver al menú. La pausa puede interrumpir una partida sin alterar su estado.

El producto mínimo completo incluye una arena, cuatro combatientes 3D, cinco acciones aliadas, cinco acciones enemigas contando sus ataques básicos, cinco objetos, veneno, dos modificadores temporales, una habilidad con presentación especial y todos los menús navegables con teclado y control.

No se incluyen movimiento libre, cobertura, posiciones tácticas, cambios de formación, resistencias elementales, críticos, fallos de ataque, árbol de habilidades, niveles, economía, guardado ni resurrección. Las distancias entre modelos son visuales y no cambian la efectividad de las acciones.

## 3. Escenario y presentación de la pelea

La arena representa el acceso de piedra a una fortaleza. El portón queda al fondo, los aliados a la izquierda y los enemigos a la derecha. El guerrero se ubica delante de la maga, aunque esto no impide atacar a ninguno. El ogro ocupa más espacio que el chamán para comunicar su amenaza.

Una cámara fija en perspectiva permite ver los cuatro personajes y deja espacio inferior para los paneles de aliados y acciones. La cámara de Tormenta arcana vuelve siempre a esta posición al terminar.

Al iniciar, se muestra una frase breve: «Defendé la entrada. Derrotá al ogro y al chamán». No hay una introducción que deba verse en cada intento. Las instrucciones se consultan desde el menú principal y desde la pausa.

## 4. Estadísticas y reglas numéricas

Cada combatiente tiene valores máximos de vida y maná, ataque, defensa y velocidad. La vida y el maná actuales nunca pueden ser negativos ni superar sus máximos. La velocidad efectiva incluye los modificadores temporales. La defensa efectiva puede reducirse mediante corrosión.

| Estadística | Guerrero | Maga | Ogro | Chamán |
| --- | --- | --- | --- | --- |
| Vida máxima | 160 | 110 | 240 | 130 |
| Maná máximo | 30 | 45 | 24 | 36 |
| Ataque | 28 | 24 | 28 | 18 |
| Defensa | 8 | 4 | 6 | 4 |
| Velocidad | 22 | 28 | 16 | 20 |

Todos comienzan con vida y maná completos, sin efectos temporales, con la barra ATB en cero y sin recuperación pendiente. No existe regeneración pasiva de vida o maná. Los valores de esta tabla son una base inicial, no un balance validado.

### Daño y curación

Daño directo = máximo entre 1 y el resultado de redondear (ataque del usuario × potencia de la acción − defensa efectiva del objetivo). Se redondea al entero más cercano, con los valores terminados en 0,5 hacia arriba. Todos los ataques válidos impactan.

El ataque básico tiene potencia 1. Contra el ogro, el guerrero inflige 22 de daño y la maga 18. Golpe pesado tiene potencia 1,8 y causa 44 al mismo objetivo. Estos ejemplos permiten comprobar rápidamente que la fórmula está implementada de forma consistente.

Curaciones y objetos recuperan cantidades fijas. La cantidad efectiva se limita al recurso faltante. La curación no depende de la defensa. No se puede confirmar una recuperación sobre un objetivo que ya tiene ese recurso completo.

## 5. Sistema ATB y resolución de turnos

### Llenado y recuperación

La barra ATB va de 0 a 100. Mientras el reloj está activo y la recuperación del personaje es cero, aumenta a razón de velocidad efectiva × segundos transcurridos. Con velocidad 20, tarda cinco segundos de reloj activo en completarse. Con velocidad 28, tarda aproximadamente 3,57 segundos.

Después de ejecutar una acción, la barra vuelve a cero y comienza la recuperación indicada para esa acción. Mientras se recupera, la barra se mantiene vacía. Al llegar a cero el tiempo pendiente, vuelve a llenarse. Si una actualización supera el fin de la recuperación, el tiempo sobrante se utiliza para llenar ATB.

El reloj se detiene al existir una cola de participantes listos, durante los menús de decisión, las animaciones, la presentación especial, la pausa y la pantalla de resultado. Las recuperaciones de todos también se detienen. Por eso, esperar en un menú o reproducir una animación más larga no cambia la ventaja temporal de una acción.

La rapidez modifica la velocidad de llenado, no la recuperación fija. Tampoco altera retroactivamente el porcentaje ya acumulado. Una barra al 70 % sigue al 70 % cuando se recibe el tónico, pero el 30 % restante se llena más rápido.

### Orden del turno

Cuando una barra alcanza 100, el participante se incorpora a la cola y el reloj se detiene. Si varios alcanzan el máximo en la misma actualización, se calcula el instante de cruce dentro de esa actualización para ordenarlos. Los empates se resuelven según las reglas de la sección A.

Al retirar a alguien de la cola, se verifica que siga vivo. Se registra qué efectos tenía al comenzar ese turno y se procesa el veneno. Si sobrevive, el aliado recibe el menú de acciones o el enemigo elige automáticamente. Después se ejecuta una sola acción, se descuentan las duraciones que correspondan y se establece su recuperación.

Al terminar, se comprueba la victoria o derrota. Si el combate sigue y quedan participantes en la cola, se procesa el siguiente. Si la cola está vacía, se reanuda el reloj. Un personaje derrotado se elimina de cualquier posición de la cola y no conserva una acción pendiente.

### Confirmación y gasto de recursos

Elegir un comando abre su lista o la selección de objetivos. El panel informa efecto, maná, recuperación y restricciones. Cancelar retrocede sin gastar recursos. Confirmar valida otra vez que el actor esté vivo, tenga recursos y que el objetivo sea válido. Recién entonces se descuenta maná o cantidad del objeto y se bloquea la navegación de combate durante la ejecución.

Una confirmación no puede disparar dos acciones. Las pulsaciones mantenidas no confirman pantallas nuevas automáticamente. Un recurso se cobra una sola vez y una acción inválida vuelve a selección con un mensaje breve, sin consumir el turno.

## 6. Aliados y acciones

El guerrero funciona como atacante resistente. Puede golpear de forma frecuente o gastar maná y asumir una espera mayor para usar Golpe pesado. No atrae ataques mediante una regla de tanque: cualquiera de los dos aliados puede ser objetivo del ogro.

La maga actúa más seguido y tiene menos vida. Puede sostener al grupo con Curación o acelerar el final con Tormenta arcana. Su ataque básico permite seguir aportando daño cuando se agota el maná.

| Acción | Usuario | Maná | Efecto y objetivo | Recuperación |
| --- | --- | --- | --- | --- |
| Ataque básico | Guerrero | 0 | Potencia 1. Un enemigo vivo. | 0,6 s |
| Golpe pesado | Guerrero | 8 | Potencia 1,8. Un enemigo vivo. | 2,2 s |
| Proyectil mágico | Maga | 0 | Potencia 1. Un enemigo vivo. | 0,6 s |
| Curación | Maga | 10 | Recupera 36 de vida. Un aliado vivo, incluida la maga. | 1,2 s |
| Tormenta arcana | Maga | 18 | Potencia 1,6. Todos los enemigos vivos. | 2,8 s |

Los menús principales son Atacar, Habilidades y Objetos. Atacar usa el básico propio. Habilidades muestra las del personaje activo y Objetos muestra el inventario común. No existen Defender, Huir ni Pasar en esta versión.

Tormenta arcana cuesta 18 de maná una sola vez, aunque afecte a dos enemigos. Con un único enemigo vivo mantiene el mismo costo y lo afecta solamente a él. La interfaz destaca al grupo enemigo y requiere confirmación antes de iniciar la secuencia.

## 7. Enemigos e inteligencia de combate

| Acción | Usuario | Maná | Efecto y objetivo | Recuperación |
| --- | --- | --- | --- | --- |
| Golpe | Ogro | 0 | Potencia 1. Un aliado vivo. | 0,6 s |
| Machetazo | Ogro | 8 | Potencia 1,5. Un aliado vivo. | 2,2 s |
| Descarga | Chamán | 0 | Potencia 1. Un aliado vivo. | 0,6 s |
| Veneno | Chamán | 6 | Aplica veneno sin daño directo. Un aliado vivo. | 1,2 s |
| Curación oscura | Chamán | 10 | Recupera 40 de vida del ogro vivo. | 1,5 s |

### Reglas del ogro

En su primera acción usa Golpe. En la siguiente intenta Machetazo y repite esa alternancia. Si no tiene maná para Machetazo, usa Golpe y continúa alternando los intentos. El contador avanza solamente al ejecutar una acción.

El objetivo se elige al azar con la misma probabilidad entre los aliados vivos. Si queda uno solo, siempre lo ataca. El objetivo queda visible antes del impacto. No hay selección de personajes muertos ni una decisión nueva por cada fotograma.

### Reglas del chamán

Primera prioridad: si el ogro está vivo, tiene menos del 50 % de su vida máxima y hay al menos 10 de maná, usa Curación oscura sobre él. No puede curarse a sí mismo.

Segunda prioridad: si tiene al menos 6 de maná y existe un aliado del jugador vivo sin veneno, aplica Veneno. Si los dos cumplen la condición, elige uno al azar con probabilidades iguales.

Tercera prioridad: usa Descarga sobre un aliado vivo elegido al azar. También llega a esta opción si ambos ya están envenenados o no puede pagar las otras habilidades.

Las decisiones se calculan con el estado actual al comenzar la acción, después de procesar el veneno del propio chamán si llegara a tenerlo en una prueba. Al caer el ogro, el chamán sigue atacando y envenenando. No se queda intentando curar un objetivo inexistente.

## 8. Inventario y consumibles

Ambos aliados acceden al mismo inventario. Cada intento comienza con las cantidades indicadas. No hay obtención de objetos durante la pelea. Todos los consumibles tienen recuperación de 0,8 segundos y gastan una unidad al confirmar una acción válida.

| Objeto | Cantidad | Objetivo | Efecto |
| --- | --- | --- | --- |
| Poción | 3 | Un aliado vivo | Recupera 50 de vida. |
| Éter | 2 | Un aliado vivo | Recupera 20 de maná. |
| Tónico de rapidez | 1 | Un aliado vivo | Velocidad × 1,5 durante sus próximas dos acciones. |
| Frasco corrosivo | 1 | Un enemigo vivo | Defensa × 0,5 durante sus próximas dos acciones. |
| Antídoto | 2 | Un aliado vivo con veneno | Elimina el veneno inmediatamente. |

Se puede usar un objeto sobre el propio usuario si cumple sus condiciones. Los objetos no consumen maná. Una entrada sin unidades queda deshabilitada. Si no hay ningún objetivo válido, se informa el motivo y no se puede confirmar.

La Poción no se habilita sobre alguien con vida completa y el Éter tampoco sobre alguien con maná completo. El Antídoto requiere veneno. Un tónico o frasco puede renovar su efecto si este ya está activo, aunque los recursos iniciales solo permiten usar cada uno una vez por intento.

## 9. Estados y modificadores temporales

| Efecto | Cambio | Duración | Lectura visual |
| --- | --- | --- | --- |
| Veneno | Pierde 8 de vida al inicio de su turno. Ignora defensa. | 3 activaciones de daño. | Gota verde y cargas restantes. |
| Rapidez | Velocidad efectiva = velocidad base × 1,5. | Próximas 2 acciones del afectado. | Flecha azul y acciones restantes. |
| Corrosión | Defensa efectiva = defensa base × 0,5. | Próximas 2 acciones del afectado. | Escudo roto naranja y acciones restantes. |

Se conservan decimales en velocidad y defensa efectivas. El redondeo se aplica al resultado final del daño. Veneno y corrosión son efectos distintos: reducir la defensa no aumenta el daño fijo del veneno.

### Ejemplos de duración

Si el guerrero usa Tónico de rapidez sobre sí mismo, termina esa acción con dos cargas. La siguiente carga de ATB ya recibe el aumento. Después de su próxima acción queda una carga y después de la siguiente el efecto desaparece.

Si la maga usa Frasco corrosivo sobre el ogro, su defensa se reduce inmediatamente. Permanece reducida hasta que el ogro haya completado dos acciones. Los ataques que otros personajes realicen entre esas acciones aprovechan la reducción.

Si el chamán envenena a la maga, esta pierde 8 de vida en sus próximos tres inicios de turno. Si usa un antídoto durante el primero, el daño de ese inicio ya ocurrió, pero se evitan los dos siguientes. El veneno puede matar. No se espera a la animación de ataque para comprobarlo.

Al reaplicar un mismo efecto se conserva su potencia y se restablece su contador máximo. Los efectos que estaban presentes al inicio del turno son los únicos que pueden perder una carga al finalizar esa acción. Un efecto renovado durante la acción se trata como una aplicación nueva.

## 10. Tormenta arcana y secuencia especial

Tormenta arcana tiene que distinguirse por su presentación y demostrar el requisito de ocultar temporalmente la interfaz. La secuencia dura aproximadamente 2,5 segundos reales de animación, independientes de los 2,8 segundos de recuperación medidos por el reloj del combate.

Antes de comenzar, el jugador ve el costo y los enemigos afectados. Al confirmar se descuentan 18 de maná y se guardan la posición de cámara y el estado del menú. El reloj queda detenido durante toda la ejecución.

| Momento | Acción visual y sonora |
| --- | --- |
| 0,0 a 0,4 s | Se ocultan HUD, menús, marcadores y números. La cámara se acerca a la maga y suena una carga. |
| 0,4 a 1,2 s | La maga levanta el bastón. Aparece un círculo mágico y crecen las partículas azules. |
| En 1,2 s | Caen rayos sobre todos los enemigos vivos. Se aplica el daño una sola vez por objetivo y suena el impacto. |
| 1,2 a 2,0 s | Se reproducen reacciones o muertes. La cámara vuelve a su posición de combate. |
| 2,0 a 2,5 s | Se restaura el HUD y se muestran brevemente los daños pendientes. Finaliza la secuencia. |

No se aceptan comandos de combate durante la secuencia. La pausa sigue disponible y detiene también cámara, partículas, animaciones y audio de la escena. El menú de pausa es una capa independiente del HUD oculto.

Si la habilidad elimina a los últimos enemigos, se completa la secuencia y se presenta la victoria, sin abrir otro menú de acciones. Si el combate continúa, se pasa al próximo participante listo o vuelve a avanzar el reloj. La restauración de cámara e interfaz se ejecuta también ante una interrupción por reinicio o salida.

La habilidad no necesita saltarse, porque dura pocos segundos. No utiliza pulsaciones rápidas, apuntado ni decisiones adicionales.

## 11. Interfaz y navegación

### Información visible durante el combate

Cada aliado tiene un panel con nombre, retrato o icono, vida actual y máxima, maná actual y máximo, barra ATB y efectos con duración restante. Los enemigos muestran los mismos recursos en paneles más pequeños. Los cuatro participantes permanecen identificables aunque un menú esté abierto.

La barra se representa como Llenando, Recuperando o Listo, mediante texto y cambios visuales. Durante la recuperación se informa el tiempo pendiente. Al detenerse el reloj para una decisión, se muestra «Elegí una acción». No se presenta una barra congelada sin explicación.

El personaje activo recibe un marcador. El objetivo seleccionado tiene un contorno o aro, su nombre y la indicación de aliado o enemigo. Una acción grupal destaca a todos sus objetivos. La selección no depende solamente del color.

El panel de descripción informa nombre de la acción, efecto, costo de maná o unidades disponibles, recuperación y duración de estados si corresponde. Las acciones deshabilitadas siguen mostrando información, con motivos como «Maná insuficiente», «Sin unidades» o «No hay objetivos válidos».

Un registro breve conserva los últimos cuatro eventos relevantes: acción, objetivo, daño o curación y aplicación o fin de estados. No sustituye los números sobre los personajes.

### Menús y foco

Menú principal: Jugar, Cómo jugar, Volumen y Salir. Pausa: Continuar, Cómo jugar, Volumen, Reiniciar y Volver al menú. Resultado: Reiniciar y Volver al menú. Salir cierra la aplicación compilada.

Volumen ofrece un ajuste general de 0 a 100 en pasos de 10 y un comando Volver. La ayuda se divide en páginas cortas para objetivo, ATB, recursos y controles. Se puede pasar de página y regresar sin mouse. En pausa, volver desde ayuda o volumen conserva la pausa.

Cada menú abre con un elemento seleccionado. Volver a una pantalla recupera la selección anterior si sigue siendo válida. Si una opción deja de estar disponible, el foco pasa a la primera opción válida. Al cerrar la habilidad especial no puede quedar un selector invisible ni perderse la navegación.

La navegación vertical recorre comandos y listas. La horizontal recorre objetivos válidos en orden de posición en pantalla. En ambos casos el recorrido es circular. Los personajes muertos se omiten. Seleccionar un objeto no cambia automáticamente al objetivo equivocado de un menú anterior.

## 12. Controles

| Acción | Teclado | Control |
| --- | --- | --- |
| Mover selección de menú | Flechas arriba y abajo | Cruceta o stick izquierdo arriba y abajo |
| Cambiar objetivo o página de ayuda | Flechas izquierda y derecha | Cruceta o stick izquierdo izquierda y derecha |
| Confirmar | Enter o Espacio | Botón inferior: A en Xbox, cruz en PlayStation |
| Volver o cancelar selección | Escape | Botón derecho: B en Xbox, círculo en PlayStation |
| Abrir pausa desde combate | Escape desde el menú raíz o durante ejecución | Botón de menú: Start u Options |
| Cerrar pausa | Escape o Continuar | Botón de menú, botón derecho o Continuar |
| Ajustar volumen | Flechas izquierda y derecha | Cruceta o stick izquierdo izquierda y derecha |

En teclado, Escape vuelve un nivel si se está eligiendo habilidad, objeto u objetivo. Desde el menú raíz de acciones abre la pausa. Durante animaciones o avance de barras también abre la pausa. En control, el botón de menú abre o cierra la pausa desde cualquier fase del combate.

La primera pulsación de navegación mueve una opción. Mantener una dirección puede repetir después de una espera breve. Confirmar y cancelar responden solamente a una pulsación nueva. Se aplica una zona muerta al stick para evitar movimientos accidentales.

Se permite cambiar de dispositivo durante la partida. Las ayudas visuales se actualizan al último dispositivo utilizado y se mantiene el foco. Si un control se desconecta, aparece un aviso y la partida se pausa. Puede continuarse con teclado o con el control reconectado.

El mouse no es necesario para ninguna acción. Las dos rutas de verificación obligatorias son jugar una partida completa solo con teclado y otra solo con control.

## 13. Feedback, arte y sonido

El estilo visual usa modelos 3D sencillos con una misma dirección general de proporciones, materiales e iluminación. Se puede trabajar con recursos existentes, respetando sus licencias y registrando sus créditos. El guerrero se reconoce por su espada y armadura, la maga por su bastón, el ogro por su tamaño y el chamán por su silueta y efectos verdes.

Cada combatiente necesita reposo, ataque, reacción al daño y muerte. Las habilidades pueden reutilizar animaciones acompañadas de efectos distintos. Curación utiliza una pose de conjuro y partículas suaves. El ataque del guerrero y el ogro puede simular alcance mediante un paso corto o una inclinación, retornando siempre a su anclaje.

Los ataques normales duran aproximadamente entre 0,6 y 1 segundo. Curación y las habilidades enemigas duran entre 0,8 y 1,2 segundos. Son tiempos de presentación y no sustituyen los costos de recuperación.

| Evento | Respuesta visual | Respuesta sonora |
| --- | --- | --- |
| Selección y confirmación | Resaltado y cambio visible del foco. | Sonidos de navegación y confirmación. |
| Daño directo | Reacción del objetivo y número de daño. | Impacto físico o mágico según la acción. |
| Curación y recuperación de maná | Número con signo más, icono y efecto breve. | Sonido de recuperación. |
| Veneno | Icono, cargas y pulso verde al aplicar daño. | Sonido breve de estado. |
| Rapidez y corrosión | Icono propio, contador y efecto de aplicación. | Sonido de buff o debuff. |
| Muerte | Animación, panel marcado como derrotado y barra detenida. | Sonido de caída. |
| Victoria o derrota | Panel de resultado y detención del combate. | Jingle correspondiente. |

Los daños se distinguen de las recuperaciones mediante signo, texto y contexto, además del color. Se evita que los efectos tapen las barras. La información importante debe leerse en 1280 × 720 y en 1920 × 1080 mediante una interfaz que se adapte a ambas resoluciones.

El audio incluye música de combate, efectos y sonidos de interfaz. El control de volumen general afecta a todos. La pausa conserva el punto de reproducción y la salida al menú termina o cambia la música sin superponer pistas.

## 14. Balance inicial y decisiones esperadas

Los valores iniciales buscan que los ataques básicos permitan ganar con decisiones razonables, mientras que habilidades y objetos ayuden a controlar el riesgo o terminar antes. No se exige memorizar una secuencia única.

Golpe pesado debe causar bastante más daño por acción que el básico, a cambio de maná y recuperación. Tormenta arcana es más eficiente cuando quedan dos enemigos. Curación tiene valor cuando evita la muerte de un aliado, pero usarla sin necesidad retrasa el daño al rival.

La vida del chamán es menor que la del ogro para favorecer la opción de eliminarlo primero. Su reserva de maná limita las curaciones y el veneno. El ogro mantiene presión con sus golpes fuertes, pero su menor velocidad deja tiempo para reaccionar.

Se probarán al menos dos prioridades: derrotar primero al chamán y derrotar primero al ogro. Ambas deberían ser viables con gestión de recursos, aunque una pueda resultar más segura. También se probará la pelea sin habilidades y sin consumibles para detectar si el sistema permite ignorar casi todas sus opciones sin consecuencias.

Si la pelea termina demasiado rápido, se revisa primero la vida enemiga y el rendimiento de Tormenta arcana. Si se alarga, se revisan la frecuencia de curación del chamán y la vida del ogro. Si las derrotas ocurren sin margen para responder, se ajusta el daño enemigo antes de agregar mecánicas.

La duración, tasa de victoria y frecuencia de uso de objetos quedan pendientes de pruebas con jugadores. No se presentan como resultados medidos. Los ajustes de balance deben mantener las reglas y actualizar las tablas de este documento.

## 15. Organización de sistemas y datos en Unity

Esta sección propone una organización para implementar el diseño. No obliga a usar una arquitectura específica, pero separa los datos de configuración del estado de cada pelea.

| Sistema | Responsabilidad |
| --- | --- |
| Controlador de combate | Estados de partida, reloj, cola, resolución de acciones y resultado. |
| Instancia de combatiente | Vida y maná actuales, ATB, recuperación, efectos y referencia a sus datos. |
| Resolución de acciones | Validación, gasto único, objetivos, aplicación del efecto y eventos de presentación. |
| Inteligencia enemiga | Aplicar prioridades del chamán y alternancia del ogro al recibir su turno. |
| Inventario de combate | Cantidades compartidas y restauración de valores al reiniciar. |
| Interfaz e input | Menús, foco, objetivos, ayudas de dispositivo y pausa. |
| Presentación | Animaciones, cámara, partículas, audio y restauración de la secuencia especial. |

### ScriptableObjects

Datos de combatiente: identificador, nombre, equipo, vida máxima, maná máximo, ataque, defensa, velocidad, prefab o referencias visuales, acciones disponibles y tipo de inteligencia cuando corresponda.

Datos de acción: identificador, nombre, descripción, costo de maná, potencia o cantidad fija, modo de objetivo, recuperación, efecto aplicado y referencias de animación, sonido y partículas. La presentación especial se identifica en estos datos.

Datos de ítem: identificador, nombre, descripción, icono, modo de objetivo, tipo de efecto, cantidad, duración si corresponde y recuperación. Las cantidades disponibles pertenecen al inventario de la partida, no al asset.

Datos de efecto: identificador, tipo, duración inicial, momento de activación, daño fijo o multiplicador e icono. Configuración del encuentro: cuatro participantes, ubicaciones y cantidades iniciales de ítems.

Los ScriptableObjects guardan configuración. La vida actual, el maná actual, las cargas de efectos, los contadores de inteligencia y las unidades restantes se crean como estado independiente al comenzar cada intento. Reiniciar no debe reutilizar valores modificados del intento anterior.

### Estados y orden de procesamiento

Estados mínimos: menú, inicialización, avance ATB, inicio de turno, selección aliada, decisión enemiga, ejecución, resultado y pausa. La presentación especial es una variante de ejecución. La pausa conserva el estado previo y luego lo restaura.

El controlador aplica cada efecto de juego una sola vez en el momento de impacto definido por la acción. Las animaciones acompañan ese evento, pero no deben duplicar el daño por tener más de una señal. Al abandonar una pelea se cancelan acciones pendientes, efectos visuales y callbacks.

La comprobación de resultado se realiza después del daño o de una muerte y antes de habilitar otra acción. Si ambos enemigos están derrotados, hay victoria. Si ambos aliados están derrotados, hay derrota. Como no hay daño simultáneo a ambos bandos en esta versión, no se necesita un empate.

## 16. Casos límite y reglas de cierre

Con un único aliado vivo, este puede atacar a los enemigos, curarse o usar objetos sobre sí mismo según la acción. Los ataques ofensivos no pueden dirigirse al propio usuario ni a otro aliado. No se ofrece al personaje muerto como destino. Con un único enemigo vivo, las habilidades de área se ejecutan sobre ese enemigo.

Si el veneno mata a quien iba a actuar, no se muestra su menú ni se gasta maná. Se retira de la cola, se resuelve su muerte y se pasa al resultado o al siguiente participante.

Si una acción derrotó al objetivo de otro participante que estaba listo, el siguiente participante elige sus objetivos con el estado actualizado. No conserva una selección vieja. Si no quedan enemigos, el resultado se presenta antes de solicitar otra decisión.

Si un efecto expira, las estadísticas vuelven a calcularse desde el valor base. No se suman o restan modificadores sobre un valor ya modificado, evitando que se acumulen errores después de aplicar y retirar estados.

Reiniciar desde la pausa requiere una confirmación navegable con ambos dispositivos. Volver al menú durante una partida también la requiere. Estas confirmaciones evitan perder un intento por una pulsación accidental. En la pantalla de resultado no son necesarias.

Reiniciar restaura vida, maná, ATB, inventario, estados, contadores del ogro, modelos, cámara, HUD y selección de menús. El volumen elegido se conserva durante la sesión. No se promete persistencia entre ejecuciones.

La pausa detiene reloj, animaciones, partículas, secuencia de cámara y audio de combate. La navegación y los sonidos propios de la pausa siguen operativos. Reanudar no vuelve a cobrar recursos ni repite el impacto de una acción.

## 17. Plan de desarrollo detallado por hitos

Este plan reemplaza las seis etapas resumidas del Google Doc. Descompone el mismo alcance en trabajo de programación, diseño, modelado, animación, escenario, UI, VFX, audio y validación. No introduce más personajes, niveles, acciones ni sistemas jugables.

### Reglas de ejecución y seguimiento

- Las casillas de tareas y del registro global reflejan el estado real. Los identificadores son estables. La implementación y verificación de un hito se distinguen de su aprobación por el usuario, según la forma de trabajo entre chats.
- Completar técnicamente un hito requiere todas sus tareas y sus criterios de cierre. Después se entrega para la revisión final del usuario. Un resultado provisional no cierra una tarea que pide un recurso final.
- Realizar un solo hito por chat, siguiendo H00 a H13 sin saltos. Resolver sus dependencias antes de comenzarlo. Las dependencias no autorizan adelantar H05, H07, H08 o H09 ni desarrollar varios hitos en paralelo. Los primeros hitos pueden usar placeholders hasta la etapa de producción correspondiente.
- La implementación de input comienza en H00 y continúa en todos los menús. H11 audita la cobertura completa.
- Usar recursos existentes está permitido por el GDD. Para cada modelo, animación o sonido, producirlo o adquirirlo con una licencia válida y completar su preparación e integración. Adaptar un recurso puede sustituir su modelado desde cero; no sustituye el resultado exigido ni su verificación.
- Si una herramienta no permite producir o inspeccionar un recurso, dejar la tarea pendiente, registrar la limitación y completar solo las tareas independientes del mismo hito. No sustituir un personaje final por una cápsula y marcar modelado como terminado.
- Registrar aquí tareas, hitos, entregas, revisiones y correcciones. TASKS.md puede enlazar este documento o resumirlo, sin mantener otro estado independiente. No fijar fechas ni estimaciones sin conocer disponibilidad, herramientas y experiencia del equipo.

### Mapa de hitos y dependencias

| Hito | Resultado | Depende de |
| --- | --- | --- |
| H00: Preparación del proyecto y control del alcance | Proyecto Unity que abre sin errores, estructura de carpetas, documentación inicial y primera compilación. | Ninguno |
| H01: Arena provisional y combate básico terminable | Escena de combate con cuatro participantes, recursos, comandos básicos, victoria, derrota y reinicio. | H00 |
| H02: Datos configurables, ATB y recuperación | Combate con ATB para los cuatro participantes y datos editables desde el Inspector. | H01 |
| H03: Acciones completas, inventario e inteligencia enemiga | Diez acciones configuradas, cinco consumibles y las dos inteligencias enemigas. | H02 |
| H04: Veneno, rapidez y corrosión | Estados funcionales, consumibles completos y contadores visibles. | H03 |
| H05: Modelado y preparación de los cuatro personajes 3D | Cuatro modelos 3D coherentes con materiales, rig o jerarquía animable, archivos fuente y prefabs. | H00, H01 |
| H06: Rigging final, animaciones y sincronización de impactos | Combatientes animados e impactos sincronizados con una sola aplicación de efectos. | H05, H03, H04 |
| H07: Modelado modular y construcción del escenario final | Arena final con módulos 3D, materiales, iluminación y cámaras validadas. | H01 |
| H08: Diseño de interfaz, iconos y menús finales | HUD y menús finales con iconos, foco, ayuda, volumen y escalado. | H02, H03, H04 |
| H09: Búsqueda, edición e integración de audio | Música, efectos e interfaz sonora con volumen, pausa y créditos correctos. | H00, H03 |
| H10: Efectos visuales y presentación de Tormenta arcana | Efectos integrados y secuencia especial completa, pausables y sin pérdida de foco. | H04, H06, H07, H08, H09 |
| H11: Soporte completo de teclado y control y robustez del flujo | Recorrido completo sin mouse con teclado y con control, incluyendo casos de desconexión. | H08, H10 |
| H12: Pruebas funcionales, balance y rendimiento | Informe de pruebas, valores revisados y ausencia de errores bloqueantes. | H11 |
| H13: Compilación final, documentación y entrega | Build final, proyecto fuente, instrucciones, créditos e informe de validación. | H12 |

### H00. Preparación del proyecto y control del alcance

**Objetivo:** Dejar un proyecto reproducible y una especificación verificable antes de producir contenido.

**Dependencias:** ninguna.

**Entregable:** Proyecto Unity que abre sin errores, estructura de carpetas, documentación inicial y primera compilación.

#### Proyecto y configuración

- [x] **H00-T01. Inspeccionar el proyecto o crear uno.** Si existe, conservar su versión de Unity, renderer y estructura útil. Si no existe, crear un proyecto 3D sencillo y registrar versión exacta y renderer en README.md.
- [x] **H00-T02. Fijar plataforma de entrega.** Preparar Windows de 64 bits como entrega inicial para PC. Registrar cualquier limitación del equipo de desarrollo y los módulos de compilación disponibles.
- [x] **H00-T03. Configurar carpetas.** Separar Scenes, Scripts, Data, Prefabs, Models, Animations, Materials, Textures, UI, VFX, Audio, Tests y Documentation; no dejar referencias a rutas personales.
- [x] **H00-T04. Configurar control de versiones.** Añadir .gitignore de Unity, incluir .meta y configuración del proyecto, usar serialización de texto y excluir archivos temporales y compilaciones del repositorio.
- [x] **H00-T05. Registrar paquetes y dependencias.** Conservar manifiesto y bloqueo de paquetes, registrar los usados y elegir un solo sistema de input y de UI para todo el juego.
- [x] **H00-T06. Crear escenas iniciales.** Crear MainMenu y Combat, un punto de entrada consistente y el orden de escenas necesario para la compilación.
- [x] **H00-T07. Preparar pantalla y calidad base.** Definir modo de ventana, escalado de interfaz y calidad inicial; comprobar apertura en 1280 × 720 y 1920 × 1080.
- [x] **H00-T08. Realizar una compilación vacía.** Abrir el ejecutable fuera del editor y comprobar que carga la escena inicial y puede cerrarse.

#### Documentación y producción

- [x] **H00-T09. Guardar la especificación en el proyecto.** Mantener este Markdown vigente en Documentation/Ultima_Guardia_GDD_y_Plan_de_Desarrollo.md, conservar el enlace de origen e inicializar el registro global sin marcar trabajo no realizado.
- [x] **H00-T10. Crear documentos de seguimiento.** Crear README.md, ASSET_REGISTER.md, TEST_REPORT.md, BALANCE_LOG.md y KNOWN_ISSUES.md, con propósito y campos mínimos. Si se crea TASKS.md, usarlo como índice o resumen hacia este Markdown, que conserva las casillas y estados autorizados.
- [x] **H00-T11. Definir criterios de recursos.** Preferir recursos gratuitos o propios, coherentes con el estilo simple. No comprar assets ni introducir servicios pagos para completar el prototipo.
- [x] **H00-T12. Crear inventario de recursos.** Anotar los cuatro personajes, armas, módulos de fortaleza, animaciones, iconos, partículas, música y sonidos requeridos; asignar cada elemento a un hito.
- [x] **H00-T13. Preparar input mínimo desde el comienzo.** Crear acciones de navegar, confirmar, cancelar y pausa para teclado y control. Validar un menú de prueba con ambos dispositivos.

#### Criterios para cerrar el hito

- El proyecto abre y compila sin errores bloqueantes.
- El README registra versión, plataforma y forma de ejecutar.
- Existe un inventario con todos los recursos pendientes y las tareas están sin completar salvo lo realmente verificado.

### H01. Arena provisional y combate básico terminable

**Objetivo:** Conseguir una pelea completa usando representaciones provisionales, antes de invertir en acabado.

**Dependencias:** H00.

**Entregable:** Escena de combate con cuatro participantes, recursos, comandos básicos, victoria, derrota y reinicio.

#### Escena provisional

- [x] **H01-T01. Construir el blockout.** Crear suelo, muro y portón con geometría simple. Reservar espacio para personajes, UI y el acercamiento de la habilidad especial.
- [x] **H01-T02. Definir anclajes de combate.** Colocar posiciones del guerrero, maga, ogro y chamán, orientación hacia el rival y puntos de salida de proyectiles y recepción de efectos.
- [x] **H01-T03. Configurar cámara base.** Encuadrar los cuatro participantes con margen para HUD; guardar su posición y rotación de referencia.
- [x] **H01-T04. Crear prefabs provisionales.** Usar cuatro volúmenes 3D distinguibles, nombres visibles y referencias a sus anclajes; no usar estos placeholders como arte final.

#### Programación del núcleo

- [x] **H01-T05. Implementar equipos e identidad.** Crear identificadores estables de combatiente y equipo; separar aliado y enemigo de su posición visual.
- [x] **H01-T06. Implementar estado de combatiente.** Guardar vida, maná, vivo o muerto y referencias de datos. Aplicar límites de recursos y exponer eventos de cambio.
- [x] **H01-T07. Implementar daño y curación.** Usar la fórmula y el redondeo del GDD. Verificar 22 y 44 de daño del guerrero contra el ogro con básico y golpe pesado.
- [x] **H01-T08. Implementar objetivos válidos.** Resolver un enemigo vivo, un aliado vivo y todos los enemigos vivos; excluir muertos y aliados de ataques ofensivos.
- [x] **H01-T09. Crear acciones secuenciales provisionales.** Permitir una sola acción en ejecución y un orden temporal simple que luego será sustituido por ATB.
- [x] **H01-T10. Implementar ataque básico y una habilidad.** Permitir confirmar un ataque y Golpe pesado, validar maná y aplicar sus efectos una sola vez.
- [x] **H01-T11. Implementar consumible mínimo.** Añadir una Poción compartida que recupere vida, reduzca existencias y no se confirme sobre vida completa.
- [x] **H01-T12. Implementar enemigo básico.** Permitir que cada enemigo elija un aliado vivo y lo ataque, sin bloquear la pelea por falta de objetivos.
- [x] **H01-T13. Implementar muerte y resultado.** Retirar a los muertos del flujo, detectar victoria y derrota y evitar acciones posteriores al resultado.
- [x] **H01-T14. Implementar reinicio limpio.** Restaurar participantes, recursos e inventario sin recargar valores alterados de la pelea anterior.

#### UI provisional y comprobación

- [x] **H01-T15. Mostrar estado y comandos.** Presentar nombres, vida, maná y acciones con texto funcional, además de un indicador de turno y objetivo.
- [x] **H01-T16. Añadir navegación y cancelación.** Seleccionar acciones y objetivos con teclado y control. Cancelar no gasta recursos ni termina el turno.
- [x] **H01-T17. Completar una pelea en ambos sentidos.** Registrar una victoria y una derrota, abrir el resultado y reiniciar. Confirmar que solo hay una acción a la vez.

#### Criterios para cerrar el hito

- Puede terminarse y reiniciarse una pelea sin depender del mouse.
- Los cuatro participantes tienen vida y maná y pueden atacar.
- Daño, curación, gasto y muerte cumplen las reglas base. Este hito todavía no cubre toda la nota 4 porque el arte animado es provisional.

### H02. Datos configurables, ATB y recuperación

**Objetivo:** Sustituir el orden provisional por el sistema temporal definitivo y llevar la configuración a assets.

**Dependencias:** H01.

**Entregable:** Combate con ATB para los cuatro participantes y datos editables desde el Inspector.

#### Datos y estado de ejecución

- [x] **H02-T01. Crear datos de combatiente.** Definir ScriptableObjects con estadísticas, equipo, prefab, acciones y tipo de IA, usando las tablas del GDD.
- [x] **H02-T02. Crear datos de acciones.** Definir costo, potencia o valor fijo, objetivo, recuperación y referencias de presentación. Incluir identificadores únicos.
- [x] **H02-T03. Crear datos de ítems y efectos.** Definir valores, duración, iconos y reglas de objetivo sin guardar existencias ni cargas actuales dentro del asset.
- [x] **H02-T04. Crear configuración de encuentro.** Referenciar exactamente dos aliados y dos enemigos, sus posiciones e inventario inicial.
- [x] **H02-T05. Separar configuración de estado.** Crear copias o instancias de runtime para recursos, barras, recuperaciones, contadores de IA e inventario.
- [x] **H02-T06. Validar datos incompletos.** Detectar referencias faltantes, costos negativos, velocidades inválidas e identificadores duplicados con mensajes que permitan corregirlos.

#### ATB y orden

- [x] **H02-T07. Implementar el reloj activo.** Actualizar ATB y recuperación únicamente durante avance de combate. Menús, ejecución, pausa y resultado no consumen este tiempo.
- [x] **H02-T08. Implementar llenado individual.** Usar velocidad efectiva por segundo, rango 0 a 100, y estado Listo al completar la barra.
- [x] **H02-T09. Implementar recuperación por acción.** Vaciar ATB después de actuar, consumir recuperación antes del llenado y aprovechar correctamente el sobrante de una actualización.
- [x] **H02-T10. Implementar cola de participantes listos.** Calcular el cruce de umbral, ordenar por instante, velocidad efectiva y desempate fijo del GDD. No duplicar entradas.
- [x] **H02-T11. Integrar inicio y cierre de turno.** Procesar estado inicial, decisión, confirmación, impacto y cierre en ese orden; vaciar la cola antes de reanudar el reloj.
- [x] **H02-T12. Eliminar muertos de la cola.** Invalidar turnos pendientes sin gastar recursos y comprobar resultado antes de procesar otra decisión.
- [x] **H02-T13. Mostrar ATB y recuperación.** Añadir barra y estados Llenando, Recuperando y Listo, con recuperación pendiente visible.
- [x] **H02-T14. Comprobar tiempos y empates.** Verificar cinco segundos de llenado a velocidad 20 y aproximadamente 3,57 a velocidad 28, sin recuperación. Forzar empates y pausas de decisión.

#### Criterios para cerrar el hito

- Aliados y enemigos llenan barras según su velocidad.
- Ataque básico y Golpe pesado tienen recuperaciones diferentes.
- Modificar un asset cambia el valor correspondiente sin editar scripts y reiniciar no modifica el asset original.

### H03. Acciones completas, inventario e inteligencia enemiga

**Objetivo:** Completar todas las opciones de combate y las reglas de decisión definidas en el GDD.

**Dependencias:** H02.

**Entregable:** Diez acciones configuradas, cinco consumibles y las dos inteligencias enemigas.

#### Acciones aliadas

- [x] **H03-T01. Configurar Ataque básico y Proyectil mágico.** Crear sus datos definitivos con costo cero, potencia 1 y recuperación de 0,6 segundos.
- [x] **H03-T02. Configurar Golpe pesado.** Aplicar 8 de maná, potencia 1,8 y recuperación de 2,2 segundos; bloquear confirmación sin maná.
- [x] **H03-T03. Implementar Curación.** Recuperar 36 de vida por 10 de maná y 1,2 segundos de recuperación, permitiendo curar a la maga y excluyendo vida completa.
- [x] **H03-T04. Implementar efecto de Tormenta arcana.** Aplicar potencia 1,6 a los enemigos vivos por un único costo de 18 y recuperación de 2,8 segundos. La secuencia final se integra en H10.
- [x] **H03-T05. Construir menús de acciones.** Atacar, Habilidades y Objetos muestran opciones del actor actual, descripción, costo, recuperación y disponibilidad.
- [x] **H03-T06. Validar confirmación atómica.** Revisar actor, objetivo y recursos antes del gasto. Bloquear confirmaciones repetidas y acciones simultáneas.

#### Inventario completo

- [x] **H03-T07. Crear inventario compartido.** Una sola colección para ambos aliados, inicializada con 3 Pociones, 2 Éteres, 1 Tónico, 1 Frasco y 2 Antídotos.
- [x] **H03-T08. Configurar Poción y Éter.** Recuperar 50 de vida o 20 de maná, limitar al máximo y mostrar unidades restantes.
- [x] **H03-T09. Preparar Tónico, Frasco y Antídoto.** Crear datos y selección correcta de equipo. Mantenerlos identificados como pendientes hasta integrar los estados de H04.
- [x] **H03-T10. Implementar reglas de consumibles.** Todos usan recuperación de 0,8 segundos, no gastan maná y descuentan una unidad solo al confirmar.
- [x] **H03-T11. Mostrar indisponibilidad.** Permitir consultar la descripción de opciones agotadas o sin objetivos, informar el motivo y bloquear su ejecución.

#### IA y acciones enemigas

- [x] **H03-T12. Configurar Golpe y Machetazo.** Aplicar las potencias, costos y recuperaciones de la tabla enemiga.
- [x] **H03-T13. Implementar alternancia del ogro.** Comenzar con Golpe, intentar Machetazo en la siguiente acción y alternar. Si falta maná, usar Golpe sin romper el contador.
- [x] **H03-T14. Configurar Descarga y Curación oscura.** Descarga es gratuita. La curación recupera 40 al ogro por 10 de maná y recuperación de 1,5 segundos.
- [x] **H03-T15. Preparar acción Veneno del chamán.** Configurar costo 6, recuperación 1,2 y efecto sin daño directo; completar su funcionamiento con H04.
- [x] **H03-T16. Implementar prioridades del chamán.** Curar al ogro por debajo del 50 %, luego envenenar un aliado sin veneno si puede pagarlo y por último usar Descarga.
- [x] **H03-T17. Implementar selección aleatoria válida.** Elegir uniformemente entre aliados vivos al comenzar la acción. Con un superviviente, seleccionarlo siempre.
- [x] **H03-T18. Comprobar agotamiento y supervivientes.** Probar maná cero, muerte del ogro, último enemigo, último aliado y habilidades de área con un único objetivo.

#### Criterios para cerrar el hito

- Las cinco acciones aliadas y las cinco enemigas tienen datos y ejecución de recursos correctos.
- Todos los consumibles existen y las opciones dependientes de estados pasan a H04 con seguimiento explícito.
- La IA respeta prioridades y siempre conserva un ataque gratuito.

### H04. Veneno, rapidez y corrosión

**Objetivo:** Cerrar todas las reglas de efectos temporales y completar las acciones que dependen de ellas.

**Dependencias:** H03.

**Entregable:** Estados funcionales, consumibles completos y contadores visibles.

#### Programación y datos

- [x] **H04-T01. Crear instancias de efectos.** Guardar tipo, potencia, duración restante e identidad de aplicación. Permitir coexistencia de efectos diferentes.
- [x] **H04-T02. Implementar Veneno.** Aplicar 8 de daño que ignora defensa al inicio de los próximos tres turnos, antes de la selección o decisión automática.
- [x] **H04-T03. Resolver muerte por Veneno.** Si el daño mata, no abrir menú ni ejecutar IA; limpiar cola y recursos visuales del muerto y comprobar resultado.
- [x] **H04-T04. Completar habilidad enemiga Veneno.** Aplicar el estado sin daño directo y permitir que la IA identifique aliados vivos todavía no envenenados.
- [x] **H04-T05. Completar Antídoto.** Eliminar el veneno al confirmar, exigir un aliado vivo afectado y no devolver el daño que ya ocurrió al inicio del turno.
- [x] **H04-T06. Implementar Rapidez.** Multiplicar velocidad base por 1,5 sin cambiar ATB acumulado ni recuperación fija. Conservar decimales.
- [x] **H04-T07. Implementar Corrosión.** Multiplicar defensa base por 0,5 y calcular daño con defensa efectiva, sin modificar el daño de Veneno.
- [x] **H04-T08. Completar Tónico y Frasco.** Aplicar los efectos al equipo correcto, con dos próximas acciones del afectado de duración.
- [x] **H04-T09. Implementar vencimiento por acciones.** Registrar efectos al inicio del turno y descontar al finalizar solo los que correspondan. Un tónico usado sobre uno mismo conserva dos cargas al terminar esa acción.
- [x] **H04-T10. Implementar reaplicación.** Renovar duración sin sumar potencia ni descontar inmediatamente una aplicación nueva o renovada.
- [x] **H04-T11. Limpiar y recalcular estadísticas.** Eliminar efectos al morir o reiniciar y reconstruir estadísticas desde los valores base, sin acumulación de errores.

#### Lectura y verificación

- [x] **H04-T12. Mostrar efectos y contadores provisionales.** Indicar Veneno, Rapidez y Corrosión con nombre, icono provisional y duración restante.
- [x] **H04-T13. Probar duración exacta.** Registrar tres activaciones de Veneno, rapidez sobre el usuario y corrosión hasta la segunda acción del objetivo.
- [x] **H04-T14. Probar coexistencia y reaplicación.** Aplicar dos estados, renovar uno y confirmar que no duplica potencia ni altera el otro.
- [x] **H04-T15. Verificar la IA completa.** Confirmar prioridades de curación, veneno y básico con las condiciones y costos reales.

#### Criterios para cerrar el hito

- Antídoto, Tónico y Frasco funcionan y gastan correctamente sus unidades.
- Las duraciones dependen de los turnos o acciones definidos, no de segundos reales.
- Veneno puede matar antes de actuar y la pelea continúa o finaliza correctamente.

### H05. Modelado y preparación de los cuatro personajes 3D

**Objetivo:** Producir los cuatro modelos definitivos y sus armas, listos para animar e integrar.

**Dependencias:** H00, H01.

**Entregable:** Cuatro modelos 3D coherentes con materiales, rig o jerarquía animable, archivos fuente y prefabs.

#### Dirección visual y preparación

- [x] **H05-T01. Fijar estilo y proporciones.** Preparar una hoja breve de referencias: personajes simples, materiales legibles, guerrero y maga diferenciados y ogro más grande.
- [x] **H05-T02. Elegir producción por recurso.** Decidir para cada personaje si se modela desde cero o se adapta un recurso permitido. Registrar la opción, procedencia y trabajo pendiente.
- [x] **H05-T03. Definir escala y presupuesto.** Comparar siluetas en la cámara real y fijar complejidad suficiente para el prototipo, sin perseguir detalle que no se verá.
- [x] **H05-T04. Preparar convenciones de exportación.** Definir unidades, orientación, pivote, nombres de huesos y puntos de sujeción. Guardar archivos fuente y exportaciones con nombres estables.

#### Guerrero

- [x] **H05-T05. Modelar cuerpo y armadura.** Construir silueta humana robusta, cabeza, torso y extremidades con articulaciones que permitan reposo, ataque y muerte.
- [x] **H05-T06. Modelar espada.** Crear hoja, guarda y empuñadura y un punto de agarre correcto, con un material distinguible de la armadura.
- [x] **H05-T07. Preparar UV y materiales.** Crear UV si las texturas las requieren o materiales de color plano, revisar superficies, normales y lectura a tamaño de juego.
- [x] **H05-T08. Preparar rig y pesos.** Crear o adaptar esqueleto y asignar pesos. Probar deformaciones de hombros, codos, cadera y manos antes de exportar.

#### Maga

- [x] **H05-T09. Modelar cuerpo y vestimenta.** Crear una silueta distinta, vestimenta simple y brazos con libertad para elevar el bastón, sin requerir simulación de tela.
- [x] **H05-T10. Modelar bastón y foco mágico.** Preparar agarre, extremo emisor y forma visible durante la cámara especial.
- [x] **H05-T11. Preparar UV y materiales.** Resolver colores, superficies y detalles del bastón que permitan distinguir magia aliada.
- [x] **H05-T12. Preparar rig y pesos.** Verificar hombros, brazos y manos durante la pose de conjuro y evitar deformaciones graves de la vestimenta.

#### Ogro

- [x] **H05-T13. Modelar cuerpo y rasgos.** Crear mayor volumen que los aliados, torso pesado y una cabeza reconocible desde la cámara.
- [x] **H05-T14. Modelar arma del ogro.** Crear un machete o arma pesada coherente con Machetazo y dejar el agarre alineado.
- [x] **H05-T15. Preparar UV y materiales.** Diferenciar piel, ropa y arma con materiales simples y coherentes con los demás modelos.
- [x] **H05-T16. Preparar rig y pesos.** Comprobar brazos, torso y piernas en golpe, reacción y caída, con proporciones propias del ogro.

#### Chamán

- [x] **H05-T17. Modelar cuerpo y vestimenta.** Crear una silueta menor que el ogro, con elementos que comuniquen su función de conjurador.
- [x] **H05-T18. Modelar foco de conjuro.** Crear un bastón, tótem de mano o pieza equivalente con punto de emisión de magia.
- [x] **H05-T19. Preparar UV y materiales.** Reservar detalles verdes para sus efectos sin confundirlos con la identificación del equipo.
- [x] **H05-T20. Preparar rig y pesos.** Comprobar poses de conjuro, reacción y muerte con la vestimenta y el accesorio.

#### Exportación e integración

- [x] **H05-T21. Exportar e importar los cuatro modelos.** Conservar escala, orientación, materiales, rig y archivos fuente en formatos utilizables por el proyecto.
- [x] **H05-T22. Validar representación final.** Si se usa rig de huesos o jerarquía de piezas rígidas, demostrar que permite todas las animaciones del siguiente hito.
- [x] **H05-T23. Crear prefabs finales.** Añadir referencias de animación, arma, anclajes VFX, nombre y datos de personaje, sin duplicar el estado de combate.
- [x] **H05-T24. Sustituir placeholders.** Colocar los prefabs en los anclajes existentes, ajustar encuadre y verificar lectura de las cuatro siluetas.
- [x] **H05-T25. Registrar licencias y créditos.** Registrar fuente, autor, licencia, modificaciones y ruta de cada recurso externo. Para los propios, registrar autoría y archivo fuente.

#### Criterios para cerrar el hito

- Los cuatro participantes son modelos 3D finales distinguibles y con materiales completos.
- Hay fuentes editables y una forma comprobada de animarlos.
- Las armas están sujetas correctamente y los prefabs reemplazan los volúmenes provisionales.

### H06. Rigging final, animaciones y sincronización de impactos

**Objetivo:** Dar una representación animada a todas las acciones sin alterar las reglas temporales.

**Dependencias:** H05, H03, H04.

**Entregable:** Combatientes animados e impactos sincronizados con una sola aplicación de efectos.

#### Producción de clips

- [x] **H06-T01. Completar reposo del guerrero.** Crear o adaptar un ciclo estable con espada en mano, sin desplazamiento acumulado.
- [x] **H06-T02. Completar ataques del guerrero.** Crear básico y Golpe pesado, permitiendo reutilización con variación clara de pose, velocidad o recorrido.
- [x] **H06-T03. Completar reacción y muerte del guerrero.** Mostrar impacto y caída legibles, evitando volver automáticamente al reposo al estar muerto.
- [x] **H06-T04. Completar reposo y básico de la maga.** Crear reposo y lanzamiento del Proyectil mágico con punto de emisión sincronizado.
- [x] **H06-T05. Completar conjuro de Curación.** Crear pose dirigida al objetivo y regreso al reposo, compatible con curarse a sí misma.
- [x] **H06-T06. Completar pose especial de la maga.** Crear elevación de bastón y carga para Tormenta arcana dentro de los tiempos del GDD.
- [x] **H06-T07. Completar reacción y muerte de la maga.** Evitar intersecciones evidentes con suelo, vestimenta y bastón.
- [x] **H06-T08. Completar reposo y ataques del ogro.** Crear Golpe y Machetazo diferenciables, conservando su posición inicial al terminar.
- [x] **H06-T09. Completar reacción y muerte del ogro.** Dar peso a las respuestas sin ocultar permanentemente a otros participantes.
- [x] **H06-T10. Completar reposo y conjuros del chamán.** Preparar Descarga, Veneno y Curación oscura, con clips reutilizados si los efectos distinguen la acción.
- [x] **H06-T11. Completar reacción y muerte del chamán.** Conservar el modelo derrotado en un estado estable y sin volver a actuar.
- [x] **H06-T12. Preparar uso de objetos.** Dar a ambos aliados un gesto corto reutilizable y un punto de presentación del consumible.
- [x] **H06-T13. Resolver movimiento de ataque.** Añadir un paso corto o inclinación de ataque cuando corresponda y regresar exactamente al anclaje, sin locomoción libre.

#### Programación de presentación

- [x] **H06-T14. Configurar controladores de animación.** Definir estados, transiciones y parámetros en los cuatro prefabs. Evitar entradas que cancelen una muerte.
- [x] **H06-T15. Conectar momento de impacto.** Usar un único evento o controlador por acción para daño, curación o estado. Proteger contra señales duplicadas.
- [x] **H06-T16. Integrar proyectiles y objetivos.** Mover el proyectil visual hacia el objetivo ya confirmado sin recalcular el destinatario durante el vuelo.
- [x] **H06-T17. Soportar pausa y reanudación.** Congelar clips y movimiento visual al pausar; reanudar desde el mismo punto sin repetir eventos.
- [x] **H06-T18. Soportar reinicio y salida.** Cancelar secuencias y devolver animadores y transformaciones a su estado inicial.
- [x] **H06-T19. Comprobar todas las acciones animadas.** Reproducir las diez acciones y objetos con modelos finales y registrar que efecto y animación coinciden.

#### Criterios para cerrar el hito

- Cada combatiente tiene reposo, ataque, daño y muerte, además de los gestos especiales necesarios.
- Las acciones muestran movimiento y aplican el efecto una sola vez.
- Pausar, matar, reiniciar y salir no dejan animadores ni transformaciones en estados incorrectos.

### H07. Modelado modular y construcción del escenario final

**Objetivo:** Reemplazar el blockout por una fortaleza sencilla que no interfiera con la lectura del combate.

**Dependencias:** H01.

**Entregable:** Arena final con módulos 3D, materiales, iluminación y cámaras validadas.

#### Producción de geometría

- [x] **H07-T01. Definir kit modular.** Dibujar o listar suelo, muro, portón, columnas o contrafuertes y remates. Mantener suficiente variedad sin construir una fortaleza completa.
- [x] **H07-T02. Modelar suelo.** Crear superficie con tamaño suficiente, altura estable y detalles que no confundan los anclajes de personajes.
- [x] **H07-T03. Modelar muros y remates.** Preparar módulos encajables con pivotes consistentes y sin huecos visibles desde la cámara.
- [x] **H07-T04. Modelar portón.** Crear puerta o reja central y marco de piedra reconocibles como acceso defendido.
- [x] **H07-T05. Modelar soportes laterales.** Crear columnas o contrafuertes que permitan cerrar el encuadre y ocultar bordes del escenario.
- [x] **H07-T06. Crear decoración mínima.** Preparar uno o dos recursos reutilizables, como estandarte y antorcha, y distribuirlos sin tapar participantes.
- [x] **H07-T07. Preparar UV y materiales del entorno.** Resolver piedra, metal, madera y tela según los módulos elegidos, usando colores y detalle coherentes con personajes.
- [x] **H07-T08. Exportar módulos y crear prefabs.** Comprobar unidades, pivotes, normales y reutilización; conservar fuentes y procedencia.

#### Construcción e iluminación

- [x] **H07-T09. Montar arena con módulos finales.** Sustituir blockout y mantener los cuatro anclajes, dejando margen para efectos y HUD.
- [x] **H07-T10. Configurar suelo y contactos.** Resolver sombras y apoyo de personajes sin flotación, hundimientos ni desplazamientos durante ataques.
- [x] **H07-T11. Construir iluminación base.** Iluminar caras y siluetas de los cuatro combatientes; usar sombras y ambiente sin ocultar acciones.
- [x] **H07-T12. Ajustar cámara de combate.** Validar encuadre con ogro, armas, paneles y objetivo marcado en ambas resoluciones.
- [x] **H07-T13. Reservar cámara especial.** Comprobar que el acercamiento a la maga y los rayos no atraviesan muros ni salen del escenario.
- [x] **H07-T14. Revisar materiales y rendimiento.** Corregir materiales incompatibles o faltantes, geometría fuera de cuadro innecesaria y sombras demasiado costosas para el prototipo.
- [x] **H07-T15. Probar pelea en escenario final.** Comprobar ataques, muertes, proyectiles y partículas sin oclusiones que impidan entender el combate.

#### Criterios para cerrar el hito

- No quedan módulos provisionales visibles en la arena final.
- Los cuatro combatientes y sus objetivos son legibles con el HUD.
- Cámara base y especial tienen espacio y materiales sin errores.

### H08. Diseño de interfaz, iconos y menús finales

**Objetivo:** Completar la información visual y la navegación del producto, conservando teclado y control.

**Dependencias:** H02, H03, H04.

**Entregable:** HUD y menús finales con iconos, foco, ayuda, volumen y escalado.

#### Diseño y recursos 2D

- [x] **H08-T01. Definir estilo de interfaz.** Elegir tipografía permitida, tamaños, paleta, paneles y estados de selección, manteniendo legibilidad en 720p.
- [x] **H08-T02. Crear retratos o identificadores.** Preparar cuatro retratos o iconos de participante y una marca inequívoca de aliado o enemigo.
- [x] **H08-T03. Crear iconos de recursos.** Preparar vida, maná, ATB y recuperación con formas que no dependan solo del color.
- [x] **H08-T04. Crear iconos de acciones.** Preparar representaciones para básicos, golpe fuerte, curación, tormenta, descarga y veneno. Reutilizar iconos solo cuando su significado sea claro.
- [x] **H08-T05. Crear iconos de consumibles.** Preparar Poción, Éter, Tónico de rapidez, Frasco corrosivo y Antídoto.
- [x] **H08-T06. Crear iconos de estados.** Preparar gota de Veneno, flecha de Rapidez y escudo roto de Corrosión, con lugar para su contador.
- [x] **H08-T07. Crear indicadores de control.** Preparar ayudas de teclado y control para navegar, confirmar, volver y pausar, según el dispositivo activo.
- [x] **H08-T08. Preparar importación 2D.** Configurar sprites, transparencia, filtros, tamaños y licencias de tipografías e iconos.

#### HUD

- [x] **H08-T09. Construir paneles aliados y enemigos.** Mostrar nombres, vida, maná, barras ATB y estados de los cuatro, con formato actual/máximo.
- [x] **H08-T10. Mostrar estado de barra.** Distinguir llenado, recuperación y listo, con tiempo pendiente y mensaje de selección.
- [x] **H08-T11. Construir panel de acciones.** Listas, costos, cantidades, descripción y recuperación reflejan los datos configurados.
- [x] **H08-T12. Construir marcador de actor y objetivo.** Crear aro o contorno y nombre del objetivo; destacar el grupo completo en Tormenta arcana.
- [x] **H08-T13. Construir números flotantes.** Distinguir daño, vida y maná recuperados con signo e identificación además del color; evitar superposición ilegible.
- [x] **H08-T14. Construir registro de combate.** Conservar los últimos cuatro eventos de acción, daño, curación y cambios de efectos.
- [x] **H08-T15. Mostrar restricciones y errores.** Informar falta de maná, unidades o objetivos, sin ocultar la descripción de la opción.

#### Menús y navegación

- [x] **H08-T16. Construir menú principal.** Implementar Jugar, Cómo jugar, Volumen y Salir, todos accesibles sin mouse.
- [x] **H08-T17. Construir ayuda por páginas.** Escribir objetivo, ATB, recursos y controles. Permitir cambiar página y volver al contexto de origen.
- [x] **H08-T18. Construir volumen general.** Ajustar 0 a 100 en pasos de 10 con izquierda y derecha y conservar el valor durante la sesión.
- [x] **H08-T19. Construir pausa y confirmaciones.** Continuar, ayuda, volumen, reiniciar y volver al menú. Confirmar acciones que abandonan el intento.
- [x] **H08-T20. Construir pantallas de resultado.** Distinguir victoria y derrota y permitir reiniciar o volver al menú sin confirmación adicional.
- [x] **H08-T21. Gestionar foco en todas las pantallas.** Abrir con selección válida, recordar la anterior al volver y evitar foco sobre un elemento oculto o destruido.
- [x] **H08-T22. Implementar navegación circular.** Arriba/abajo en listas e izquierda/derecha en objetivos y páginas, omitiendo personajes muertos.
- [x] **H08-T23. Comprobar escalado y texto.** Probar 1280 × 720 y 1920 × 1080, texto español y tildes, paneles largos y mensajes sin recortes.

#### Criterios para cerrar el hito

- El jugador puede anticipar costo, efecto y destinatario antes de confirmar.
- Todos los menús funcionan y el foco siempre es visible.
- Los cuatro participantes y efectos se leen en ambas resoluciones.

### H09. Búsqueda, edición e integración de audio

**Objetivo:** Producir un conjunto sonoro completo y conectarlo a los eventos del juego.

**Dependencias:** H00, H03.

**Entregable:** Música, efectos e interfaz sonora con volumen, pausa y créditos correctos.

#### Búsqueda y selección

- [x] **H09-T01. Buscar música de combate.** Seleccionar una pista permitida, sin voces que dificulten la lectura y que pueda reproducirse en bucle.
- [x] **H09-T02. Buscar sonidos de interfaz.** Conseguir navegación, confirmación, cancelación y opción inválida, distinguibles y de duración breve.
- [x] **H09-T03. Buscar ataques físicos.** Conseguir corte o movimiento de espada, golpe normal del ogro e impacto pesado.
- [x] **H09-T04. Buscar magia ofensiva.** Conseguir lanzamiento de proyectil aliado, Descarga del chamán y sus impactos.
- [x] **H09-T05. Buscar curación y maná.** Conseguir recuperación de vida, recuperación de maná y sonido de uso de consumible si hace falta.
- [x] **H09-T06. Buscar estados.** Conseguir aplicación y activación de veneno, buff de rapidez, debuff de corrosión y limpieza de estado.
- [x] **H09-T07. Buscar sonidos de Tormenta arcana.** Conseguir carga, crecimiento mágico y rayos o impacto principal, con duración adaptable a 2,5 segundos.
- [x] **H09-T08. Buscar muertes y resultados.** Conseguir caídas o muerte y dos señales diferenciadas de victoria y derrota.
- [x] **H09-T09. Registrar permisos de audio.** Guardar URL, autor, licencia, atribución, modificaciones y archivo por cada recurso. Si no se encuentra uno válido, producir una alternativa simple.

#### Edición y preparación

- [x] **H09-T10. Limpiar archivos elegidos.** Recortar silencios y sonidos ajenos, añadir fades cortos y eliminar clics evidentes.
- [x] **H09-T11. Equilibrar niveles.** Ajustar volúmenes para que música, interfaz e impactos se escuchen sin saturación; conservar margen en la mezcla.
- [x] **H09-T12. Preparar bucle musical.** Comprobar que el corte de repetición no tiene salto audible y no introduce silencio inesperado.
- [x] **H09-T13. Configurar importación.** Elegir formatos, canales y carga adecuados al uso de música larga o efectos cortos, sin fijar ajustes que rompan el renderer de audio existente.

#### Programación e integración

- [x] **H09-T14. Crear reproducción centralizada.** Separar rutas de música, combate e interfaz bajo un volumen general, con referencias configurables.
- [x] **H09-T15. Conectar comandos y eventos.** Asignar cada clip a navegación, confirmación, impacto, recuperación, estado, muerte o resultado según corresponda.
- [x] **H09-T16. Sincronizar impactos.** Reproducir golpe y efecto en el evento de impacto real, sin duplicación por animaciones o listeners.
- [x] **H09-T17. Implementar pausa sonora.** Pausar y reanudar música y sonidos de combate conservando el punto; mantener operativos los sonidos del menú de pausa.
- [x] **H09-T18. Resolver transiciones.** No superponer música al reiniciar ni conservar efectos de una pelea abandonada. Diferenciar victoria y derrota.
- [x] **H09-T19. Probar silencio y mezcla final.** Recorrer todas las acciones, volumen 0 y 100, pausa y reinicio. No deben aparecer clips faltantes ni picos desproporcionados.

#### Criterios para cerrar el hito

- Las acciones y eventos de la tabla de feedback tienen respuesta sonora.
- El volumen general y la pausa afectan a todo el audio previsto.
- Cada archivo externo tiene licencia registrada y las pistas no se duplican al reiniciar.

### H10. Efectos visuales y presentación de Tormenta arcana

**Objetivo:** Completar el feedback y la habilidad especial usando el escenario, modelos y UI finales.

**Dependencias:** H04, H06, H07, H08, H09.

**Entregable:** Efectos integrados y secuencia especial completa, pausables y sin pérdida de foco.

#### Efectos ordinarios

- [ ] **H10-T01. Crear impacto físico.** Preparar un efecto breve para espada y golpe del ogro que no tape a otros combatientes.
- [ ] **H10-T02. Crear proyectil y Descarga.** Preparar variantes visuales distinguibles y puntos de emisión y recepción coherentes.
- [ ] **H10-T03. Crear curación y recuperación de maná.** Preparar efectos para la maga, Poción y Éter que indiquen correctamente el recurso recuperado.
- [ ] **H10-T04. Crear efectos de estados.** Preparar aplicación y activación del Veneno, aplicación de Rapidez y Corrosión y limpieza con Antídoto.
- [ ] **H10-T05. Integrar números y registro.** Disparar números y mensajes desde el resultado real de la acción, sin confundir daño calculado con vida efectivamente descontada.
- [ ] **H10-T06. Limpiar efectos transitorios.** Destruir o reutilizar instancias al acabar y retirarlas al reiniciar o volver al menú.

#### Producción de la especial

- [ ] **H10-T07. Crear círculo mágico y carga.** Preparar partículas y materiales azules que crezcan alrededor de la maga sin salir del encuadre.
- [ ] **H10-T08. Crear rayos por objetivo.** Instanciar rayos solo para enemigos vivos seleccionados y sincronizar el impacto común.
- [ ] **H10-T09. Preparar trayectoria de cámara.** Acercarse a la maga y regresar a la transformación original con interpolación controlada.
- [ ] **H10-T10. Construir secuencia de 2,5 segundos.** Organizar ocultamiento, pose, carga, impacto en 1,2 segundos, reacciones, vuelta de cámara y restauración de UI según la tabla del GDD.
- [ ] **H10-T11. Ocultar interfaz de combate completa.** Retirar HUD, comandos, marcadores y números durante el tramo indicado, conservando una capa independiente para pausa.
- [ ] **H10-T12. Aplicar daño de área una sola vez.** Cobrar 18 de maná una vez al confirmar y aplicar la potencia a cada objetivo vivo una sola vez en el impacto.
- [ ] **H10-T13. Restaurar HUD y mostrar resultados.** En el tramo final, mostrar daños pendientes y regresar al flujo correcto sin abrir un menú del personaje que ya actuó.
- [ ] **H10-T14. Resolver victoria con especial.** Si elimina a los últimos enemigos, terminar la presentación y abrir victoria sin reanudar ATB.
- [ ] **H10-T15. Pausar toda la secuencia.** Detener cámara, partículas, animación y audio y reanudar desde el mismo instante sin impacto duplicado.
- [ ] **H10-T16. Cancelar con limpieza.** Al reiniciar o volver al menú desde la pausa, restaurar cámara, visibilidad, input y efectos aunque se interrumpa antes del impacto.
- [ ] **H10-T17. Probar especial con uno y dos enemigos.** Verificar costo único, objetivos correctos, números, muertes y recuperación de 2,8 segundos posteriores a la ejecución.

#### Criterios para cerrar el hito

- La especial tiene animación, VFX y sonido y oculta de forma comprobable la interfaz.
- La pausa sigue disponible y toda la presentación se detiene con ella.
- No quedan cámaras, HUD ni selección alterados después de terminar, ganar, reiniciar o salir.

### H11. Soporte completo de teclado y control y robustez del flujo

**Objetivo:** Cerrar la cobertura de todos los dispositivos y las transiciones del juego terminado.

**Dependencias:** H08, H10.

**Entregable:** Recorrido completo sin mouse con teclado y con control, incluyendo casos de desconexión.

#### Entrada y navegación

- [ ] **H11-T01. Auditar todas las pantallas.** Enumerar menú principal, ayuda, volumen, comandos, habilidades, objetos, objetivos, pausa, confirmaciones y resultado; revisar ambos dispositivos.
- [ ] **H11-T02. Resolver prioridad de Escape.** Volver un nivel en selección y abrir pausa desde raíz, avance de barras o ejecución, según el GDD.
- [ ] **H11-T03. Resolver botones de control.** Mapear confirmar, cancelar y menú con las posiciones definidas; no depender del nombre de un fabricante.
- [ ] **H11-T04. Configurar repetición de navegación.** Usar espera inicial, repetición y zona muerta de stick para navegar sin saltos accidentales.
- [ ] **H11-T05. Evitar confirmaciones heredadas.** Abrir una pantalla nueva no vuelve a confirmar por una pulsación mantenida ni ejecuta la primera opción automáticamente.
- [ ] **H11-T06. Permitir cambio de dispositivo.** Actualizar ayudas y conservar foco al pasar de teclado a control y viceversa, sin duplicar acciones.
- [ ] **H11-T07. Implementar desconexión del control.** Si se desconecta el control de la sesión, pausar y mostrar aviso navegable; permitir continuar con teclado o tras reconectar.
- [ ] **H11-T08. Mantener uso exclusivo de teclado.** No mostrar errores por iniciar el juego sin control conectado. No exigir conectarlo para continuar.

#### Flujo y limpieza

- [ ] **H11-T09. Revisar pausa por contexto.** Conservar el estado anterior desde menús, avance, impacto y especial. La UI de pausa usa un reloj independiente.
- [ ] **H11-T10. Comprobar confirmaciones de abandono.** Reiniciar o salir al menú desde la pausa exige confirmar, y cancelar retorna a la misma pausa.
- [ ] **H11-T11. Limpiar listeners y tareas asíncronas.** Salir o reiniciar no deja callbacks que dañen personajes nuevos, dupliquen sonidos o reactiven un HUD viejo.
- [ ] **H11-T12. Probar foco después de muertes.** Saltar objetivos muertos y escoger el primer objetivo válido si desaparece el seleccionado.
- [ ] **H11-T13. Completar recorrido solo con teclado.** Iniciar desde ejecutable, consultar ayuda, cambiar volumen, usar todas las categorías de acciones, pausar, terminar y reiniciar.
- [ ] **H11-T14. Completar recorrido solo con control.** Repetir el flujo completo sin usar teclado ni mouse y registrar el resultado con el dispositivo utilizado.
- [ ] **H11-T15. Probar desconexión y reconexión.** Desconectar en selección y durante especial, continuar con teclado o control y verificar que no se repitan efectos.

#### Criterios para cerrar el hito

- Todas las acciones del juego son accesibles con cualquiera de los dos dispositivos.
- Foco, pause y reconexión no bloquean el flujo.
- Existe evidencia registrada de una partida completa con cada dispositivo.

### H12. Pruebas funcionales, balance y rendimiento

**Objetivo:** Verificar requisitos y corregir problemas con evidencia antes de preparar la entrega.

**Dependencias:** H11.

**Entregable:** Informe de pruebas, valores revisados y ausencia de errores bloqueantes.

#### Pruebas de reglas

- [ ] **H12-T01. Verificar fórmulas y límites.** Probar básicos, potencias, corrosión, redondeo y recursos en cero o máximo con valores esperados explícitos.
- [ ] **H12-T02. Verificar reloj y cola.** Probar distintas velocidades, recuperación, sobrante de tiempo, empates y eliminación de un participante listo.
- [ ] **H12-T03. Verificar gasto y cancelación.** Comprobar costos de las diez acciones, cantidades de cinco ítems, falta de recursos, cancelar y mantener confirmar.
- [ ] **H12-T04. Verificar IA del ogro.** Registrar alternancia con y sin maná y selección entre dos aliados y un único superviviente.
- [ ] **H12-T05. Verificar IA del chamán.** Probar ogro en el umbral exacto del 50 % y por debajo, ogro muerto, ambos aliados envenenados y maná insuficiente.
- [ ] **H12-T06. Verificar efectos temporales.** Probar duración exacta, reaplicación, coexistencia, Antídoto, daño letal y retorno a estadísticas base.
- [ ] **H12-T07. Verificar resultados y supervivientes.** Probar muerte del último aliado, último enemigo, victoria con especial y continuidad con un único aliado.
- [ ] **H12-T08. Verificar todos los reinicios.** Reiniciar en selección, pausa de impacto, pausa de especial, victoria y derrota; comparar con el estado inicial.
- [ ] **H12-T09. Verificar pausas prolongadas.** Esperar en decisión y pausa y comprobar que no cambian ATB, recuperación, cargas, animaciones ni recursos.
- [ ] **H12-T10. Automatizar reglas críticas cuando sea útil.** Crear comprobaciones independientes para fórmula, orden ATB y duración de estados si permiten detectar regresiones. No reemplazan las pruebas visuales y de control.

#### Pruebas de experiencia y balance

- [ ] **H12-T11. Jugar priorizando al chamán.** Registrar duración, supervivientes, vida, maná, objetos y acciones que determinaron el resultado.
- [ ] **H12-T12. Jugar priorizando al ogro.** Registrar las mismas variables y evaluar si sigue siendo una estrategia viable.
- [ ] **H12-T13. Jugar con uso limitado de recursos.** Probar básicos, sin habilidades y sin consumibles para detectar si se ignoran las decisiones o si la pelea exige una secuencia única.
- [ ] **H12-T14. Observar un jugador externo.** Comprobar comprensión de ATB, recuperación, objetivos, costos y estados sin explicar cada elección. Registrar la muestra real utilizada.
- [ ] **H12-T15. Ajustar números con evidencia.** Cambiar datos, registrar antes y después y volver a probar las estrategias afectadas, manteniendo alcance y reglas.
- [ ] **H12-T16. Revisar duración objetivo.** Comparar tiempos reales con la meta de tres a cinco minutos y distinguir tiempo de decisión del reloj activo.
- [ ] **H12-T17. Comprobar legibilidad y audio.** Revisar ambas resoluciones, tipografía, números, efectos, identificación de equipos, volúmenes y ausencia de saturación.

#### Estabilidad y rendimiento

- [ ] **H12-T18. Revisar consola y referencias.** Corregir excepciones, referencias nulas, materiales faltantes, clips ausentes y mensajes que crezcan durante la partida.
- [ ] **H12-T19. Medir en el equipo disponible.** Registrar hardware, resolución, calidad y comportamiento de rendimiento durante pelea y especial, sin prometer resultados no medidos.
- [ ] **H12-T20. Reducir problemas observados.** Corregir emisiones de partículas, sombras, audio o instancias innecesarias si las mediciones muestran problemas.
- [ ] **H12-T21. Repetir intentos consecutivos.** Completar varias partidas y reinicios, observando fugas de instancias, audio duplicado o estados acumulados.
- [ ] **H12-T22. Auditar los 18 requisitos.** Marcar cada fila de la sección 18 con escena de demostración y evidencia, anotando cualquier incumplimiento.
- [ ] **H12-T23. Cerrar o documentar incidencias.** Corregir bloqueos, errores de reglas y falta de recursos obligatorios; listar defectos menores pendientes con su impacto real.

#### Criterios para cerrar el hito

- Los 18 requisitos tienen una comprobación registrada.
- El informe distingue lo probado, lo no probado y los ajustes realizados.
- No quedan bloqueos de flujo, gasto duplicado, efectos permanentes por error ni recursos obligatorios faltantes.

### H13. Compilación final, documentación y entrega

**Objetivo:** Entregar un prototipo ejecutable y un proyecto que otra persona pueda abrir y revisar.

**Dependencias:** H12.

**Entregable:** Build final, proyecto fuente, instrucciones, créditos e informe de validación.

#### Preparación final

- [ ] **H13-T01. Revisar escenas de compilación.** Comenzar en MainMenu, cargar Combat correctamente y excluir escenas de prueba que no formen parte del recorrido.
- [ ] **H13-T02. Retirar ayudas de depuración.** Deshabilitar comandos de vida infinita, controles de prueba y overlays internos sin retirar información necesaria para jugar.
- [ ] **H13-T03. Revisar assets y licencias.** Confirmar que modelos, texturas, tipografía, animaciones, sonidos y música usados están incluidos y atribuidos cuando corresponde.
- [ ] **H13-T04. Actualizar README.** Indicar versión exacta de Unity, cómo abrir, cómo jugar, controles, plataforma, compilación y limitaciones conocidas.
- [ ] **H13-T05. Actualizar documentación de diseño.** Sincronizar tablas con los valores entregados y dejar registro de cambios respecto de la configuración inicial.
- [ ] **H13-T06. Completar estado de tareas e hitos.** Actualizar este Markdown con tareas verificadas, hitos completados, evidencias, correcciones y revisiones del usuario. Dejar H13 pendiente de revisión final hasta recibirla y señalar claramente cualquier bloqueo o pendiente.
- [ ] **H13-T07. Crear compilación final de PC.** Generar ejecutable y carpeta de datos necesarios con nombre y versión identificables.
- [ ] **H13-T08. Probar el paquete entregable.** Copiar o descomprimir en una ubicación distinta y ejecutar sin depender del editor ni de rutas del desarrollador.
- [ ] **H13-T09. Repetir verificación final con teclado y control.** Validar inicio, victoria o derrota, pausa, especial, reinicio, audio y cierre desde Salir en la build.
- [ ] **H13-T10. Preparar entrega del proyecto fuente.** Incluir Assets con .meta, Packages, ProjectSettings, documentación y fuentes de producción necesarias; excluir cachés y temporales regenerables.
- [ ] **H13-T11. Adjuntar informe de entrega.** Resumir funciones, requisitos demostrables, pruebas realizadas, hardware usado y defectos conocidos.
- [ ] **H13-T12. Conservar versión final reproducible.** Crear etiqueta o commit final y asociar la build y documentación a esa misma versión.

#### Criterios para cerrar el hito

- El ejecutable funciona fuera del editor y el proyecto abre con la versión documentada.
- El paquete incluye todos los recursos que utiliza y sus créditos.
- La versión de código, datos, build y documentación coincide.

### Resumen del trabajo

El plan contiene **14 hitos y 245 tareas**. La primera entrega jugable se obtiene en H01, el sistema completo de reglas se cierra en H04, los recursos definitivos y su integración se cierran entre H05 y H10, y la cobertura de dispositivos y la entrega se verifican entre H11 y H13.

## 18. Criterios de aceptación y relación con la consigna

La referencia de evaluación es «TSGD-TP4.pdf», páginas 1 a 3, adjunta a la conversación. Las siguientes comprobaciones traducen sus requisitos a comportamientos observables del prototipo. No agregan requisitos oficiales.

| Nota | Requisito de la consigna | Cómo se comprueba |
| --- | --- | --- |
| 4 | Turnos secuenciales para ambos bandos. | Nunca se ejecutan dos acciones a la vez. Aliados y enemigos completan turnos. |
| 4 | Personajes controlables 3D y animados. | Guerrero y maga tienen modelo 3D y animaciones visibles de movimiento y ataque. |
| 4 | Enemigos con decisiones y objetivos. | El ogro alterna acciones. El chamán cura, envenena o ataca según el estado. |
| 4 | Ataques y al menos una habilidad con maná. | Se ejecuta un básico gratuito y una habilidad que descuenta el costo correcto. |
| 4 | Vida y maná en personajes y enemigos. | Los cuatro participantes muestran recursos que cambian según sus acciones. |
| 4 | Consumibles de recuperación. | Poción y Éter recuperan recursos y reducen la cantidad disponible. |
| 4 | UI de recursos, participantes y acciones. | Se identifican los cuatro combatientes, vida, maná y comandos utilizables. |
| 4 | Escena de combate funcional. | La composición de dos aliados contra dos enemigos funciona desde el inicio. |
| 4 | Victoria y derrota. | Cada condición abre el resultado correcto y permite reiniciar. |
| 7 | ATB según velocidad en ambos bandos. | Con recuperación equivalente, una velocidad mayor completa antes la barra. |
| 7 | Costos de recuperación. | Golpe pesado tarda más en volver a llenar ATB que el ataque básico. |
| 7 | Habilidades con costos y características distintas. | Daño individual, curación y área tienen efectos y costos diferenciados. |
| 7 | Objetos con buffs o debuffs temporales. | Tónico y Frasco modifican estadísticas y vencen tras las acciones definidas. |
| 7 | Configuración con ScriptableObjects. | Se cambia un valor de personaje, enemigo, habilidad e ítem sin editar código. |
| 7 | Feedback visual y sonoro. | Se distinguen ataques, impactos, curaciones y aplicación de efectos. |
| 10 | Estados alterados temporales. | Veneno aplica tres daños o termina antes al utilizar Antídoto. |
| 10 | Especial con UI oculta, animación, VFX y audio. | Tormenta arcana oculta el HUD y lo restaura después de la secuencia. |
| 10 | Teclado y control completos. | Se juega de menú a resultado con cada dispositivo sin usar mouse. |

### Pruebas necesarias antes de entregar

Partida completa con teclado y otra con control, incluyendo ayuda, volumen, habilidades, objetos, pausa, selección de ambos equipos y reinicio. Verificar también cambio de dispositivo y desconexión del control.

Comprobar gasto único de maná e ítems, bloqueo de opciones sin recursos, cancelación sin gasto, curación limitada al máximo y objetivos muertos omitidos.

Forzar un empate de barras, una muerte por veneno antes de actuar, una victoria con Tormenta arcana y la derrota del último aliado. Ninguno de estos casos debe bloquear el flujo.

Aplicar rapidez sobre el propio usuario y corrosión sobre un enemigo, contar exactamente las acciones de duración y confirmar la restauración de estadísticas. Pausar durante una acción y durante la especial para comprobar que no se duplica daño.

Reiniciar en medio del combate y después de ambos resultados. Confirmar que no quedan barras, inventario, estados, cámara ni recursos del intento anterior. Probar la interfaz en las dos resoluciones previstas y comprobar que el juego compilado también cierra desde Salir.

La versión está lista cuando completa estas comprobaciones, permite entender cada acción antes de confirmarla y conserva un alcance de una sola pelea. El trabajo posterior se centra en ajustar los valores con pruebas, manteniendo las reglas documentadas.
