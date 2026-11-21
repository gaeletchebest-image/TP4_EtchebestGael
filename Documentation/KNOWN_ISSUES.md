# Incidencias y límites

## Registro

| ID | Hito / tarea | Descripción | Impacto | Criterio de cierre | Estado |
| --- | --- | --- | --- | --- | --- |
| H00-L01 | H00-T13 | No hay control físico conectado en el equipo inspeccionado | La prueba automatizada con control virtual no acredita hardware, conexión ni drivers reales | Revisar menú con control físico; recorrido completo y desconexión exigidos en H11 | Limitación de validación física |
| H00-L02 | H00-T07/T08 | Computer Use rechazó abrir UltimaGuardia: aplicación no autorizada para esa herramienta | No se usa automatización de Windows sobre el ejecutable | Comprobar build mediante prueba interna, capturas y resultado de proceso; revisión interactiva del usuario aparte | Completada por alternativa automatizada; revisión interactiva del usuario pendiente |
| H00-L03 | H00-T08 | Primera build: warning de truncamiento implícito en shader Hidden/Core/DebugOccluder (paquete Core) y 8 avisos de reconexión WebSocket de Unity MCP mientras el editor compilaba | Build finalizó con 0 errores; revisar resultado visual y consola del player | Conservar evidencia, comprobar ausencia de fallos de render y errores en player; no modificar paquetes por avisos de herramientas | Histórico: build final con 0 advertencias; consola final limpia |
| H00-L04 | H00-T07 | La pantalla disponible es 1366 × 768 y Windows limita la ventana solicitada a 1920 × 1080 | No acredita presentación nativa en un monitor 1080p | Comprobar UI a 1920 × 1080 en RenderTexture; conservar por separado tamaño de ventana real y captura; revisión nativa al disponer de monitor adecuado | Escalado offscreen verificado a 1080p; presentación nativa no probada |
| H00-L05 | H00-T07/T08 | La captura explícita de URP en un proceso oculto deja avisos de GraphicsBuffer/allocaciones nativas al apagar el player; el sandbox también impide consultas de red de Unity (Curl error 6) | Los procesos de validación salen con código 0 y sin excepción de lógica; no se evaluó impacto en una sesión normal prolongada | Repetir en ejecución interactiva y durante H12; mantener diferenciados los avisos del entorno/prueba y los defectos del juego | Avisos de validación, sin bloqueo del flujo H00 |

## Alcance de la versión

La versión actual 0.0.9-H08 incluye reglas completas, modelos H05, animaciones H06, arena final H07 e interfaz final H08. Audio y VFX corresponden a H09–H10; dispositivos físicos a H11 y balance/rendimiento a H12. Los registros anteriores conservan el estado histórico de cada entrega.

La utilidad Regenerar escenas base reemplaza MainMenu y Combat y solo corresponde a H00. No ejecutarla sobre escenas con trabajo posterior.

Para nuevas incidencias registrar gravedad, pasos, resultado real, resultado esperado, archivo/escena, tarea, prueba para cerrarla y regresiones. Las correcciones del usuario se reflejan además en el registro autorizado del GDD.



## Seguimiento H01

| ID | Hito / tarea | Descripción y alcance observado | Estado / comprobación |
| --- | --- | --- | --- |
| H01-L01 | H01-T16/T17 | Teclado y Gamepad virtuales permiten probar las vinculaciones y completar la pelea, pero no acreditan hardware real, drivers ni desconexión | Limitación explícita; auditoría completa con control físico pendiente H11 |
| H01-L02 | H01-T03/T15 | Capturas a 720p y 1080p renderizadas offscreen; la ejecución nativa comprobada en esta entrega fue 1280 × 720 | Composición verificada en ambas; monitor nativo 1080p no probado |
| H01-L03 | Build de revisión | 5 advertencias: un truncamiento implícito en shader DebugOccluder de Unity Core y cuatro avisos de reconexión WebSocket de MCP durante la build | Build finalizada con 0 errores, player carga Combat sin materiales faltantes; avisos conservados en resumen/informe |
| H01-L04 | Validación del player | El proceso oculto con capturas Camera.Render muestra avisos de liberación de ComputeBuffer/GraphicsBuffer, asignaciones nativas y red del entorno al salir, como en H00 | Flujo aprobado, salida por menú y código 0. No se atribuye causa de lógica no demostrada; volver a observar en ejecución interactiva y H12 |
| H01-C01 | H01-T09/T16 | Primera pasada: el rearme al finalizar una acción descartaba la primera pulsación nueva del siguiente turno | Resuelto; se comprueba si el botón sigue pulsado. Pasada final 17/17 incluye peleas por ambos dispositivos |

La derrota automatizada de H01 utiliza un fixture con aliados a 1 de vida para forzar el caso límite; no es una medición del balance de una partida normal. El producto no incorpora botones de vida ni comandos de depuración para provocar esa situación.

## Seguimiento H02

| ID | Hito / tarea | Descripción y alcance | Estado / comprobación |
| --- | --- | --- | --- |
| H02-L01 | H02 / validación | Teclado y Gamepad virtuales verifican el flujo, sin hardware físico ni desconexión real | Limitación heredada; auditoría física en H11 |
| H02-L02 | H02-T13 | Capturas 720p/1080p por RenderTexture | Escalado y composición offscreen; no acredita monitor 1080p físico |
| H02-L03 | H02 / configuración | Iconos y referencias de animación, sonido y partículas pueden estar vacíos; objetos salvo Poción y efectos tienen datos sin ejecución activa | Alcance previsto: completar acciones/IA en H03, estados en H04 y presentación final en H05-H10 |
| H02-C01 | H02-T13 | Las barras agregadas al final del Canvas se dibujaban por encima de la capa de pausa/resultado | Corregido el orden de hermanos: cada barra sigue al panel de recursos y precede a la capa de pausa. Se repiten pruebas de flujo y capturas |
| H02-L04 | Build H02 | 1 advertencia de truncamiento implícito en shader DebugOccluder de Unity Core | Build final con 0 errores; Combat del player inspeccionada sin materiales faltantes |
| H02-L05 | Player oculto de validación | Curl error 6, avisos de liberación de GraphicsBuffer/ComputeBuffer y asignaciones al apagar el proceso con capturas explícitas, como en H00/H01 | Sin excepción de lógica, smoke aprobado y salida 0. Evaluación interactiva/prolongada de estos avisos pendiente, conservada para H12 |

Los valores de los ScriptableObjects se leen al crear cada intento; un cambio durante Play se refleja al reiniciar, no altera retroactivamente el snapshot de un turno en curso. Los modelos, menús y barras conservan presentación provisional. No hay correcciones abiertas del usuario.

## Seguimiento H03

| ID | Tareas | Descripción y alcance | Estado / cierre |
| --- | --- | --- | --- |
| H03-L01 | H03-T09/T15/T16 | Tónico, Frasco, Antídoto y Veneno tienen datos y equipo preparados, pero no instancias ni contadores activos de estados. Permanecen bloqueados con motivo explícito, sin gasto. La IA prioriza curación, consulta Veneno y usa Descarga al no ser ejecutable | Histórico H03; resuelto en H04-T04/T05/T08/T15, con aplicación, contadores y selección según estado real comprobados |
| H03-L02 | H03-T04 | Tormenta tiene daño de área, confirmación grupal, costo único y recuperación; usa presentación provisional 0,35/0,85 s | Cámara, HUD oculto, animación, VFX y sonido finales en H10 |
| H03-L03 | Validación | Teclado/control virtuales y capturas RenderTexture; sin hardware físico ni monitor nativo 1080p | Limitaciones heredadas, prueba física en H11 |
| H03-L04 | Build | 1 advertencia por truncamiento implícito de DebugOccluder (Unity Core/d3d11) | Build aprobada, 0 errores; sin modificación de paquetes |
| H03-L05 | Player oculto de validación | Curl error 6 y avisos al liberar buffers/asignaciones persistentes/JobTempAlloc al apagar el proceso con capturas explícitas, como en H00–H02 | Sin excepción de lógica, smoke aprobado y salida 0. Evaluación interactiva/prolongada en H12; no se atribuye una causa no demostrada |

No hay correcciones abiertas del usuario. La implementación H03 y sus 49 pruebas no acreditan estados completos ni balance final.


## Seguimiento H04

| ID | Tareas | Descripción y alcance | Estado / cierre |
| --- | --- | --- | --- |
| H04-C01 | Verificación | El editor sin foco no entregaba eventos virtuales a la Game View; primera pasada interrumpida con tres fallos de navegación | Resuelto en fixtures, enrutamiento temporal con restauración. Regresión de teclado aprobada y suite final 68/68 |
| H04-C02 | H04-T13/T14 | El fixture nuevo clonaba Veneno de acción y Antídoto en objetos diferentes con el mismo ID; el validador rechazaba la sesión (15 errores de fixture en pasada dirigida) | Resuelto con clonado compartido por referencia. Se conserva validación estricta; suite final 68/68 |
| H04-C03 | H04-T12 | Fondo inicial del HUD dejaba nombres 3D detrás de contadores. Extenderlo opaco reducía el área de la arena | Resuelto con fondo propio por fila y compactación de estados activos; se conserva encuadre. Capturas finales 720p/1080p inspeccionadas y regresión 4/4 |
| H04-L01 | Validación | Teclado/Gamepad virtuales y capturas RenderTexture; no hay evidencia de control físico ni monitor nativo 1080p | Límites heredados, dispositivos físicos en H11 |
| H04-L02 | Presentación | Iconos vectoriales de estados y modelos/UI/arena provisionales. Tormenta conserva tiempos de presentación 0,35/0,85 s | Alcance previsto; arte/animación/UI/audio/VFX en H05–H10 |
| H04-L03 | Build | 1 advertencia de truncamiento implícito del shader DebugOccluder de Unity Core/d3d11 | Build aprobada con 0 errores; BuildReport confirmado y Combat del player inspeccionada |
| H04-L04 | Player oculto | Curl error 6 y avisos al liberar GraphicsBuffer/ComputeBuffer, asignaciones persistentes y JobTempAlloc al apagar con capturas explícitas | Flujo aprobado y salida 0, sin excepción de lógica observada. No se atribuye causa no demostrada; revisión interactiva/prolongada en H12 |

Las reglas y los tres consumibles pendientes de H03 están completos en H04. Sin correcciones abiertas del usuario ni bloqueos de flujo detectados. H04 fue aprobado por el usuario el 21 de noviembre de 2026, mediante «Bien! Queda aprobado entonces.»; H05 no iniciado en aquella entrega. Las pruebas automatizadas no acreditan balance final, duración de tres a cinco minutos ni hardware físico.

## Seguimiento H05

| ID | Tareas | Descripción y alcance | Estado / cierre |
| --- | --- | --- | --- |
| H05-C01 | H05-T21 | La primera exportación OBJ omitía normales explícitas; Unity las recalculaba al importar | Resuelto: normales por vértice, importación de normales y remapeo a materiales URP propios. Cuatro OBJ importados con todos sus materiales URP |
| H05-C02 | H05-T22 | La hoja de caída del ogro quedaba recortada por el aspecto de captura vertical | Resuelto: aspecto explícito por RenderTexture y encuadre con los vértices de cada pose. Cuatro hojas finales inspeccionadas; margen de fondo comprobado en la caída del ogro |
| H05-C03 | H05-T24 | Los tres estados sobre la maga tapaban el sombrero con el encuadre heredado | Resuelto: cámara de combate FOV 50 y mirada a (0; 0,1; 2), conservando posición y anclajes. Capturas finales 720p/1080p y 4/4 regresiones de flujo |
| H05-L01 | Representación | Rig de piezas rígidas con 16 articulaciones y ropa segmentada; sin deformación continua, Humanoid ni simulación de tela | Alternativa admitida por H05-T22, comprobada en seis poses estáticas por modelo. Clips, transiciones, impactos y muerte animada en H06. El combate conserva pulso y caída provisionales |
| H05-L02 | Presentación | Modelos y armas H05 finales para el prototipo; escenario, UI, marcadores, audio, animaciones y VFX aún provisionales o pendientes | H06–H10, según el GDD. Las poses de validación no completan esas tareas |
| H05-L03 | Validación | Teclado/Gamepad virtuales y capturas RenderTexture; sin evidencia nueva de control físico ni monitor nativo 1080p | Límites heredados; comprobaciones físicas en H11 |
| H05-L04 | Build | 1 advertencia de truncamiento implícito del shader DebugOccluder de Unity Core/d3d11 | Build aprobada con 0 errores; BuildReport y captura del player comprobados |
| H05-L05 | Player oculto | Curl error 6 y avisos de liberación de buffers/asignaciones al cerrar con capturas explícitas, como en H00–H04 | Smoke aprobado y salida 0, sin excepción de lógica observada. Investigación interactiva/prolongada en H12; no se atribuye una causa no demostrada |

Sin cambios de estadísticas o mecánicas ni correcciones abiertas del usuario. H05 aprobado por el usuario el 21 de noviembre de 2026, mediante «Bien. Lo doy por aprobado.». H06 no iniciado.

## Seguimiento H06

| ID | Tareas | Descripción y alcance | Estado / cierre |
| --- | --- | --- | --- |
| H06-C01 | H06-T14 | La utilidad de autoría usaba `??` sobre componentes Unity ausentes y recibió un MissingComponentException antes de guardar el primer prefab | Resuelto con comprobaciones del operador de nulidad de Unity y adición explícita. Regeneración posterior completa, auditoría de los cuatro controladores aprobada |
| H06-C02 | H06-T19 | Primera matriz detenida por una expectativa del fixture: Éter exigía conservar el maná anterior a recuperar | Resuelto: expectativa limitada al máximo de maná. Primera pasada 2/3; dirigida posterior 3/3; suite final 71/71 |
| H06-C03 | H06-T19 | Capturas síncronas del fixture conservaban matrices de render del frame anterior, aunque las articulaciones ya habían cambiado | Resuelto esperando un frame antes de capturar. Capturas finales muestran preparación, proyectil y caídas; no se usan las imágenes iniciales para acreditar animaciones |
| H06-L01 | Representación | Se conserva el rig de piezas rígidas y ropa segmentada de H05; sin piel continua, Humanoid o simulación de tela | Alternativa autorizada por el GDD. 24 clips reales y cuatro controladores integrados, con apoyos y caída comprobados |
| H06-L02 | Presentación | Proyectiles básicos y recurso de objeto simples. Tormenta tiene pose de 2,5 s e impacto a 1,2 s, sin la secuencia final de cámara/HUD/VFX/audio | Alcance H06. Escenario en H07, interfaz en H08, audio en H09 y VFX/secuencia especial completa en H10 |
| H06-L03 | Validación | Input de teclado/control virtual y capturas RenderTexture; no demuestra control físico ni monitor nativo 1080p | Límite explícito; validación física en H11. Las peleas aceleradas verifican flujo, sin medir balance ni duración objetivo |
| H06-L04 | Build | Una advertencia de truncamiento implícito en DebugOccluder de Unity Core/d3d11 | Build aprobada con 0 errores; resumen y BuildReport conservados |
| H06-L05 | Player oculto | Curl error 6 y avisos de liberación de GraphicsBuffer/ComputeBuffer, Persistent y JobTempAlloc al apagar, como en H00–H05 | Smoke ampliado aprobado, acción animada comprobada y salida 0. No se observó excepción de lógica. Investigación interactiva/prolongada en H12; no se atribuye una causa no demostrada |

H06 aprobado por el usuario el 21 de noviembre de 2026, mediante «Bien. Esta aprobado. Dame ademas un nombre en ingles para el commit.». Se conservan 71/71 PlayMode y build/player comprobados. Sin correcciones solicitadas. H07 no iniciado.

## Seguimiento H07

| ID | Tareas | Descripción y alcance observado | Estado / cierre |
| --- | --- | --- | --- |
| H07-C01 | H07-T10 / dependencia H06 | La punta del pie derecho penetraba 1,54 cm en guerrero y 2,17 cm en ogro durante los ataques físicos H06 | Corregido compensando −5° en el tobillo; cuatro clips físicos regenerados, 24 clips/216 poses auditados y regresión 71/71 aprobada. Anclajes y tiempos conservados |
| H07-C02 | H07-T13 | Primera propuesta de acercamiento dejaba fuera el extremo del bastón elevado | Corregida posición y mirada de la propuesta; auditoría exige maga y bastón dentro del viewport. Vista final inspeccionada y 21 muestras de ruta sin oclusión del escenario |
| H07-C03 | H07-T07/T08/T14 | Los OBJ recién importados podían conservar materiales Standard aunque la escena usara materiales URP nativos | Remapeo explícito de los siete OBJ a los ocho materiales propios; auditoría final exige URP tanto en escena como en exportaciones importadas |
| H07-L01 | Validación | Teclado y control virtuales; capturas 720p/1080p mediante RenderTexture | No acreditan control físico ni monitor nativo 1080p. Límites heredados, cobertura física en H11 |
| H07-L02 | Presentación | Se reserva una propuesta de cámara especial y corredores de rayos; todavía no hay secuencia final de cámara/VFX/audio | Alcance previsto. H08 interfaz, H09 audio y H10 efectos/secuencia completa; la auditoría no afirma haber probado partículas finales inexistentes |
| H07-L03 | Contacto | Interpolación de las caídas H06 conserva penetración máxima muestreada de 3,38 mm entre claves | Tolerancia visual menor que el bisel del suelo; auditada con límite de 4 mm. Poses terminales estables, sin desplazamiento de anclajes |
| H07-L04 | Build | Una advertencia de truncamiento implícito en DebugOccluder de Unity Core/d3d11 | Build finalizada con 0 errores; entorno del player inspeccionado sin materiales faltantes |
| H07-L05 | Player oculto | Curl error 6 y avisos de liberación de buffers/asignaciones al apagar el proceso con capturas explícitas, como en H00–H06 | Smoke aprobado, acción animada verificada y salida 0. No se observó excepción de lógica; revisión prolongada y causa en H12 |

No hay correcciones abiertas del usuario. H07 aprobado por el usuario el 21 de noviembre de 2026 mediante «Bien. Queda aprobado. Dame nombre en ingles para el commit tambien.». H08 no iniciado. Los presupuestos geométricos y de luces no sustituyen una medición de rendimiento del producto final en H12.

## Seguimiento H08

| ID | Tareas | Descripción y alcance | Estado / comprobación |
| --- | --- | --- | --- |
| H08-C01 | T12 | Marcadores del suelo orientados hacia abajo | Corregidos; rombo y aros inspeccionados en capturas finales |
| H08-C02 | T06/T23 | El fixture de coexistencia esperaba componentes y textos provisionales | Adaptado a los sprites y contadores finales; suite completa 76/76 |
| H08-C03 | Validación | Windows impedía sobrescribir una captura abierta/mapeada | Evidencia separada por ejecución; suite final y repetición afectada aprobadas |
| H08-C04 | T12/T13/T23 | Etiquetas largas de objetivos y nombres 3D se superponían con selección grupal/números | Nombres de objetivo breves; etiquetas ceden espacio a números manteniendo identidades en HUD. Siete pruebas afectadas aprobadas e inspección visual |
| H08-C05 | Smoke Development | La expectativa heredada buscaba píxeles blancos, incompatibles con el texto dorado del menú | Verificador actualizado; build recompilada y smoke aprobado, salida 0; capturas inspeccionadas y primer intento fallido conservado |
| H08-C06 | T01/T23 | El usuario señaló textos de menús pixelados en comparación con el HUD | Resuelta: glifos al doble de resolución con uGUI nativo y escala compensada, 16 etiquetas y tamaños visuales conservados. Cinco pruebas de interfaz aprobadas, capturas 720p/1080p inspeccionadas, build nueva 0 errores/1 advertencia heredada y smoke del player aprobado/salida 0 |
| H08-L01 | Validación | Teclado y Gamepad virtuales; capturas RenderTexture a 720p/1080p | No acredita hardware ni desconexión físicos; H11 conserva esa verificación |
| H08-L02 | Presentación | Audio y VFX/cámara especial todavía pendientes | Alcance de H09/H10; volumen general H08 ya implementado |
| H08-L03 | Player oculto | Avisos de liberación de GraphicsBuffer/ComputeBuffer y asignaciones JobTempAlloc al cerrar con capturas explícitas, como en entregas anteriores | Smoke aprobado y salida 0, sin excepción de lógica observada; causa y evaluación prolongada pendientes H12 |
| H08-L04 | Build | Una advertencia de truncamiento implícito en DebugOccluder de Unity Core/d3d11 | Build final aprobada, 0 errores; materiales y UI inspeccionados |

Sin cambios de balance ni correcciones abiertas del usuario. H08 y su corrección H08-C06 fueron aprobados por el usuario el 21 de noviembre de 2026; evidencia registrada en el GDD. Las pruebas automáticas y los fixtures de recursos no son mediciones de balance ni de duración objetivo.
