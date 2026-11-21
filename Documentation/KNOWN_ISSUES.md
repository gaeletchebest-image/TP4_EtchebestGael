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

Combat contiene únicamente el arranque vacío y una pausa mínima para comprobar flujo. El combate comienza en H01. La UI es provisional; faltan intencionalmente el arte, ATB, habilidades, estados, sonido y el resto de los hitos. No son funciones entregadas en H00.

La utilidad Regenerar escenas base reemplaza MainMenu y Combat y solo corresponde a H00. No ejecutarla sobre escenas con trabajo posterior.

Para nuevas incidencias registrar gravedad, pasos, resultado real, resultado esperado, archivo/escena, tarea, prueba para cerrarla y regresiones. Las correcciones del usuario se reflejan además en el registro autorizado del GDD.

