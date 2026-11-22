# H11 · Entrada y robustez del flujo

## Alcance revisado

La auditoría de H11 cubre MainMenu, ayuda, volumen, comandos, habilidades, objetos, objetivos, pausa, confirmaciones y resultados con las rutas de teclado y Gamepad del mapa `Assets/Data/Input/GuardiaInput.inputactions`.

| Pantalla o caso | Teclado | Control | Comprobación |
| --- | --- | --- | --- |
| Menú, ayuda y volumen | Flechas, Enter/Espacio, Escape | Cruceta/stick, botón inferior/derecho | Navegación, foco, retorno y ayuda por dispositivo |
| Comandos, habilidades, objetos y objetivos | Flechas, Enter, Escape | Cruceta/stick, botones inferiores/derechos | Cancelación y confirmación; listas circulares y objetivos vivos |
| Pausa y confirmaciones | Escape, flechas, Enter | Start, cruceta, botones | Reanudar, ayuda, volumen, reiniciar/salir y cancelar |
| Resultado | Flechas, Enter | Cruceta/stick, botón inferior | Reiniciar y volver al menú |
| Cambio de dispositivo | Alternar teclado/control | Alternar control/teclado | Ayuda e icono se actualizan; el foco se conserva |
| Desconexión y reconexión | Continuar o navegar el aviso | Continuar o esperar/reanudar | Pausa sin perder acción; el impacto no se repite |

Escape cumple estas prioridades: vuelve de objetivo, habilidad u objeto; desde la raíz abre la pausa; durante avance, impacto o Tormenta pausa. El control usa acciones genéricas de Input System (`buttonSouth`, `buttonEast`, `start`) sin condicionar marcas. La repetición comienza después de 0,4 s y continúa cada 0,12 s; el stick aplica `stickDeadzone(min=0.25,max=0.95)` antes del umbral de navegación. Confirmar requiere una pulsación nueva.

`CombatPrototype` escucha `InputSystem.onDeviceChange` mientras está activo y quita el listener al desactivarse. Si se retira o desconecta un Gamepad durante la pelea, conserva la sesión en pausa y abre un aviso navegable con las opciones «Continuar ahora» y «Esperar el control». Escape permite continuar con teclado. Al agregar o reconectar un control, el aviso informa que está disponible. Se conserva la pausa previa si la desconexión ocurrió mientras ya estaba abierta.

## Evidencia

- [Resultados PlayMode de H11](Evidence/H11/playmode-results.json): suite consolidada **14/14 aprobada**, con teclado/control virtuales, navegación, foco, confirmación mantenida, zona muerta/repetición, desconexión durante Tormenta, reconexión, confirmaciones de abandono y objetivo que muere. Job `1b591236fee54949b6529b9901d51182`.
- [Aviso de desconexión a 1280 × 720](Evidence/H11/controller-disconnected-1280x720.png).
- [Smoke del player](Evidence/H11/PlayerSmoke/result-1280x720.json) y sus capturas: pasó una revisión breve con dispositivos virtuales. Informa versión `0.0.11-flow`; no representa una partida completa ni tiene la versión vigente 0.0.12.
- [Build de revisión anterior](Evidence/H11/build-summary-0.0.11.json): Windows x86_64 Development, 169,32 MB, 0 errores y 1 advertencia; versión `0.0.11-flow`. No es la build final de H11.

Los recorridos completos de PlayMode terminan la pelea de victoria: teclado virtual, 22 acciones; Gamepad virtual, 22 acciones. El Gamepad también recorre una derrota de fixture con vida baja y reinicio. Estos tiempos acelerados son verificaciones de flujo, no mediciones de duración o balance.

## Pendientes de comprobación física

Al revisar la lista de dispositivos de Unity no había ningún Gamepad conectado. Por eso el recorrido real desde el ejecutable, el recorrido con control físico y su desconexión/reconexión física quedan sin acreditar. Las pruebas virtuales verifican el código y las transiciones, pero no certifican controladores, drivers, posiciones de botones ni extracción/reconexión de hardware. Además, el intento de regenerar la build con versión 0.0.12 no inició: Unity cerró tras un rechazo de Licensing IPC; [log](../Builds/H11/Validation/unity-h11-build.log). H11 queda en curso hasta completar T09, T11 y los recorridos físicos T13–T15 con una build vigente.

La fuente de `KeyboardOnlySessionDoesNotRequireAController` recibió después del pase una aserción adicional para congelar ATB durante la pausa y el `using System.Linq` correspondiente. Unity se cerró antes de volver a compilar o ejecutar esa versión de pruebas; el resultado 14/14 identifica el pase anterior y no verifica esa última aserción.

## Cómo revisar

Para revisar el artefacto de pruebas disponible, ejecutar `Builds/H11/UltimaGuardia.exe` (versión informada: `0.0.11-flow`). Recorrer ayuda, volumen, combate y una partida hasta victoria o derrota; reiniciar desde resultado. Para control, confirmar con el botón inferior, cancelar con el botón derecho y pausar con Start/Options. Para revisar desconexión, retirar el control durante selección y durante Tormenta: la partida debe pausarse, mostrar el aviso y permitir seguir con teclado o reanudar después de reconectar. En todos los casos, comprobar que no se aplica dos veces el impacto ni se pierde el foco.
