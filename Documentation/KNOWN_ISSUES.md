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

La entrega H00 contenía únicamente el arranque vacío y la pausa mínima. H01 incorpora arena y combate básico terminable, con básicos, Golpe pesado, Poción, enemigos, victoria, derrota y reinicio. La UI y los modelos siguen siendo provisionales; ATB, acciones completas, estados, arte, sonido y efectos se realizan en sus hitos posteriores.

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
| H03-L01 | H03-T09/T15/T16 | Tónico, Frasco, Antídoto y Veneno tienen datos y equipo preparados, pero no instancias ni contadores activos de estados. Permanecen bloqueados con motivo explícito, sin gasto. La IA prioriza curación, consulta Veneno y usa Descarga al no ser ejecutable | Alcance previsto; completar en H04-T04/T05/T08/T15. No se acredita selección según presencia/ausencia de veneno todavía |
| H03-L02 | H03-T04 | Tormenta tiene daño de área, confirmación grupal, costo único y recuperación; usa presentación provisional 0,35/0,85 s | Cámara, HUD oculto, animación, VFX y sonido finales en H10 |
| H03-L03 | Validación | Teclado/control virtuales y capturas RenderTexture; sin hardware físico ni monitor nativo 1080p | Limitaciones heredadas, prueba física en H11 |
| H03-L04 | Build | 1 advertencia por truncamiento implícito de DebugOccluder (Unity Core/d3d11) | Build aprobada, 0 errores; sin modificación de paquetes |
| H03-L05 | Player oculto de validación | Curl error 6 y avisos al liberar buffers/asignaciones persistentes/JobTempAlloc al apagar el proceso con capturas explícitas, como en H00–H02 | Sin excepción de lógica, smoke aprobado y salida 0. Evaluación interactiva/prolongada en H12; no se atribuye una causa no demostrada |

No hay correcciones abiertas del usuario. La implementación H03 y sus 49 pruebas no acreditan estados completos ni balance final.
