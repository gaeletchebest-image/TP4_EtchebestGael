# Última Guardia

Prototipo de combate 3D por turnos con ATB para PC, un jugador, en español.
Versión fuente **0.0.12-H11 (en curso)**. H00–H10 aprobados por el usuario. Falta validar T09/T11 y los recorridos con dispositivos físicos para cerrar H11.

## Abrir y jugar

- Unity **6000.3.11f1** (`3000ef702840`), URP **17.3.0**, Windows x86_64, Mono.
- Abrir `Assets/Scenes/MainMenu.unity`, pulsar Play y elegir Jugar. También funciona Combat directamente.
- Build H10 de referencia: `Builds/H10/UltimaGuardia.exe`, junto con toda su carpeta. La build H11 disponible en `Builds/H11/UltimaGuardia.exe` informa `0.0.11-flow`; no corresponde a la versión fuente 0.0.12.
- Ventana inicial redimensionable de **1280 × 720**. Canvas con esa resolución de referencia y ajuste ancho/alto 0,5. Para solicitar otra resolución: `-screen-fullscreen 0 -screen-width 1920 -screen-height 1080`.

Una sola pelea enfrenta a guerrero y maga contra ogro y chamán frente a una fortaleza. Los modelos H05, los 24 clips H06 y la arena H07 están integrados. H08 incorpora HUD, iconos y menús finales. H09 incorpora 28 clips de audio originales. H10 incorpora VFX y la presentación completa de Tormenta.

## Interfaz H08

Los cuatro paneles muestran equipo, vida/maná actuales y máximos, barras, ATB, recuperación pendiente y estados con cargas. Un rombo dorado identifica al actor; un aro blanco marca cada objetivo. La descripción informa destinatario, efecto, costo, recuperación y restricciones antes de confirmar. Las opciones bloqueadas siguen siendo consultables.

El registro conserva cuatro eventos. Los números flotantes muestran daño, vida o maná efectivamente modificados, con signo y nombre del recurso. Las etiquetas 3D ceden espacio durante esos números; los cuatro nombres permanecen en el HUD.

MainMenu ofrece Jugar, Cómo jugar, Volumen y Salir. La ayuda tiene cuatro páginas. Volumen va de 0 a 100 en pasos de 10 y se conserva durante la sesión, incluyendo cambios de escena y reinicios. La pausa ofrece Continuar, Cómo jugar, Volumen, Reiniciar y Volver al menú. Reiniciar/abandonar desde pausa exigen confirmar, con Cancelar seleccionado inicialmente. Victoria y derrota permiten reiniciar o volver sin otra confirmación.

Fuentes, estilo, 26 sprites propios, tipografía incorporada de Unity y reconstrucción: [H08_INTERFACE.md](Documentation/H08_INTERFACE.md). No se descargaron ni compraron recursos.

Corrección **H08-C06**: textos de menús con glifos al doble de resolución, conservando el tamaño visual. Incluye principal, ayuda, volumen, pausa, confirmaciones y resultados. El HUD de combate conserva su configuración.

## Audio H09

La música de combate y 27 efectos originales cubren navegación, acciones físicas y mágicas, recuperación, estados, Tormenta, muerte y resultado. El volumen general de 0 a 100 afecta a todas las fuentes. La pausa detiene música y sonidos de combate, mientras la interfaz de pausa conserva sus sonidos. [Mapa, autoría y reconstrucción](Documentation/H09_AUDIO.md).

Para revisar el audio, recorrer menús y ayuda, probar volumen 0 y 100, jugar ataques, curación, objetos y estados, pausar durante un sonido y reanudar, usar Tormenta, reiniciar y volver al menú.

## VFX y especial H10

Golpes, magia, recuperaciones, estados y Veneno tienen efectos transitorios. Tormenta acerca la cámara a la maga, oculta el HUD, presenta círculos y partículas azules, lanza rayos a los enemigos vivos en 1,2 s y devuelve cámara e interfaz antes de cerrar la secuencia de 2,5 s. La pausa detiene también esta presentación. [Autoría, capturas y pasos de revisión](Documentation/H10_VFX.md).

## Controles

| Acción | Teclado | Control |
| --- | --- | --- |
| Listas | Arriba/abajo | Cruceta o stick izquierdo |
| Objetivos, páginas y volumen | Izquierda/derecha | Cruceta o stick izquierdo |
| Confirmar | Enter o Espacio | Botón inferior (A/cruz) |
| Volver un nivel | Escape | Botón derecho (B/círculo) |
| Pausar | Escape desde raíz, avance o ejecución | Botón de menú (Start/Options) |
| Cerrar pausa | Escape o Continuar | Botón derecho, menú o Continuar |

La navegación es circular. Las ayudas y el icono de dispositivo cambian con el último utilizado. Una pulsación mantenida no confirma una pantalla nueva.

Si se desconecta un control durante el combate, el juego pausa y abre un aviso navegable. Se puede continuar con teclado o esperar a reconectarlo; la pausa previa se conserva y reanudar no repite impactos. Este comportamiento está cubierto con Gamepad virtual. La desconexión física todavía requiere validación.

## Reglas del combate

ATB va de 0 a 100 según velocidad: Maga 28, Guerrero 22, Chamán 20, Ogro 16. Cuando alguien queda listo, el reloj se detiene para decidir y ejecutar una acción por vez. Se vacía la cola antes de reanudar. Desempate: instante de cruce, velocidad efectiva y luego Maga → Guerrero → Chamán → Ogro.

Tras actuar, ATB vuelve a cero y se cumple la recuperación fija antes de llenarlo. El sobrante de una actualización alimenta ATB. Menús, ejecución, pausa y resultado congelan reloj y recuperación. No hay regeneración pasiva ni resurrección.

Guerrero: básico gratuito (potencia 1; 0,6 s) y Golpe pesado (8 de maná; potencia 1,8; 2,2 s). Maga: Proyectil mágico gratuito (potencia 1; 0,6 s), Curación (36 de vida; 10 de maná; 1,2 s) y Tormenta arcana (potencia 1,6 a enemigos vivos; costo único de 18; 2,8 s). La pose de Tormenta dura 2,5 s e impacta a 1,2 s.

Inventario compartido: **3 Pociones** (50 de vida), **2 Éteres** (20 de maná), **1 Tónico de rapidez**, **1 Frasco corrosivo**, **2 Antídotos**. Cada objeto cuesta una unidad, cero maná y recuperación de 0,8 s. Poción/Éter requieren recurso faltante; Antídoto requiere un aliado vivo envenenado. Cancelar no consume recursos; confirmar valida y cobra una sola vez.

Ogro: alterna intentos de Golpe y Machetazo (8 de maná; potencia 1,5; 2,2 s), con básico si no puede pagar. Chamán: cura al ogro vivo por debajo del 50 % (40 de vida; 10 de maná; 1,5 s), luego aplica Veneno a un aliado sin ese estado (6 de maná; 1,2 s), o usa Descarga gratuita (potencia 1; 0,6 s). No se cura a sí mismo.

| Estado | Efecto | Duración |
| --- | --- | --- |
| Veneno | 8 de daño al inicio del turno, ignorando defensa | 3 activaciones |
| Rapidez | Velocidad base ×1,5; conserva ATB y recuperación | Próximas 2 acciones |
| Corrosión | Defensa base ×0,5; redondeo al calcular daño final | Próximas 2 acciones |

Veneno se procesa antes del menú o IA y puede matar. Antídoto elimina el estado al confirmar sin devolver daño. Estados distintos coexisten; reaplicar renueva duración sin sumar potencia. Solo aplicaciones presentes al inicio del turno pierden carga al terminar su acción; un Tónico usado sobre uno mismo termina con dos cargas. Muerte y reinicio limpian estados.

## Cómo revisar H08

1. Abrir MainMenu o la build H08. Recorrer las cuatro opciones con flechas/control; comprobar foco y navegación circular.
2. Abrir Cómo jugar, recorrer cuatro páginas y volver. Abrir Volumen, probar extremos y dejar otro valor.
3. Jugar. Revisar los cuatro paneles, ATB, recuperación, descripción de habilidades/objetos y motivos de bloqueo. Cancelar una selección y comprobar que conserva recursos.
4. Elegir Tormenta para ver ambos objetivos marcados; cancelar o confirmar. Usar curación/objetos, observar signos y recursos de los números y el registro de cuatro eventos.
5. Pausar durante una acción. Consultar ayuda/volumen y volver a la misma pausa. Cancelar un reinicio o abandono: la pelea se conserva. Aceptar Reiniciar: recursos y estados se restauran, volumen se conserva.
6. Ganar o perder, reiniciar desde resultado y volver al menú. Salir cierra el ejecutable.
7. Inspeccionar las capturas finales y los resultados enlazados en [TEST_REPORT.md](Documentation/TEST_REPORT.md).

## Comprobaciones

H10: suite PlayMode completa **78/78 aprobadas** antes de la corrección de material para player; después, pruebas de presentación afectadas aprobadas. Las capturas finales de Tormenta muestran carga, impacto y HUD restaurado. [Evidencia](Documentation/Evidence/H10/).

Las pruebas usan teclado y Gamepad virtuales. Las capturas usan RenderTexture y no acreditan un monitor físico 1080p. Las peleas de regresión a ×4 verifican flujo, sin medir duración objetivo ni balance. Hardware y desconexión física corresponden a H11; balance/rendimiento a H12.

La build Development admite `-foundation-smoke` para comprobar menú, carga de Combat, música, básico animado, pausa/reanudación sonora, impacto, retorno confirmado y Salir. Guarda resultado y capturas en `Builds/H10/Validation`. El arnés queda fuera de builds sin Development.

Build H10: **169,32 MB**, **0 errores / 0 advertencias**. Smoke del player **aprobado**, música y pausa verificadas, salida desde Salir y código **0**. La especial se comprobó en PlayMode y capturas del editor; el smoke no recorre Tormenta.

## Fuentes y mantenimiento

`CombatSession` contiene reglas, reloj, cola, IA y acciones. `CombatantState` mantiene recursos/estados; ScriptableObjects configuran el encuentro. `CombatPrototype` dirige input/fases; `FinalCombatHud`, `FoundationMenu` e `InterfaceTheme` presentan información y menús. `ResourceFeedback` emite cantidades efectivas para números, sin duplicar impacto.

Los datos permanecen en `Assets/Data/H02/Resources/Encounters` para conservar GUID. H08 solo agrega referencias visuales; no cambia valores de juego. Modelos, clips, materiales y módulos de fortaleza conservan sus fuentes y exportaciones.

- [Modelos H05](Documentation/H05_CHARACTER_ART.md)
- [Animaciones H06](Documentation/H06_ANIMATIONS.md)
- [Fortaleza H07](Documentation/H07_FORTRESS.md)
- [Interfaz H08](Documentation/H08_INTERFACE.md)
- [Audio H09](Documentation/H09_AUDIO.md)
- [VFX y Tormenta H10](Documentation/H10_VFX.md)

Las utilidades H00/H01 reemplazan escenas: **no ejecutarlas sobre esta entrega**. Las de H05/H06/H07 reemplazan sus recursos y requieren conservar ediciones manuales. H08 reconstruye los Canvas de ambas escenas y los recursos propios de interfaz; conservar ediciones manuales antes de usarla. El comando **H08/Compilar build de revisión** compila las dos escenas y guarda el BuildReport resumido.

Versionar Assets con sus `.meta`, Packages, ProjectSettings y Documentation. Excluir Library, Temp, Logs, UserSettings y Builds. Unity MCP es herramienta de desarrollo y no depende de ella el player. Paquetes y renderer conservados.

- [GDD, 245 tareas y estado autorizado](Documentation/Ultima_Guardia_GDD_y_Plan_de_Desarrollo.md)
- [Pruebas](Documentation/TEST_REPORT.md)
- [Recursos y licencias](Documentation/ASSET_REGISTER.md)
- [Balance](Documentation/BALANCE_LOG.md)
- [Incidencias y límites](Documentation/KNOWN_ISSUES.md)

Esta entrega realiza únicamente H10, aprobado por el usuario. H11 se solicita en otro chat.
