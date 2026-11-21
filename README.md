# Última Guardia

Prototipo de combate 3D por turnos para PC, un jugador, en español.
Versión actual: **0.0.3-H02**, datos configurables, ATB y recuperación. H00, H01 y H02 aprobados por el usuario. H03 pendiente.

## Abrir y jugar

- Unity **6000.3.11f1** (revisión `3000ef702840`), URP **17.3.0**, calidad **PC**, Windows x86_64, Mono.
- Abrir `Assets/Scenes/MainMenu.unity` y pulsar Play; elegir Jugar. También se puede abrir `Assets/Scenes/Combat.unity` directamente.
- Ejecutable de esta entrega: `Builds/H02/UltimaGuardia.exe`; mantener juntos el ejecutable y todos los archivos de su carpeta. Las builds anteriores se conservan en `Builds/H00` y `Builds/H01`.
- Ventana inicial redimensionable de **1280 × 720**, uGUI con resolución de referencia 1280 × 720 y correspondencia ancho/alto 0,5. Se mantienen los assets gráficos PC/Mobile recibidos.

H02 reemplaza el orden fijo por **ATB de 0 a 100 según velocidad**: Maga 28, Guerrero 22, Chamán 20 y Ogro 16. Todos comienzan en cero; la maga queda lista primero, aproximadamente a los 3,57 segundos activos. Al quedar alguien listo se detiene el reloj durante decisión y ejecución. Se ejecuta una acción a la vez; una cola de listos se vacía antes de reanudar el tiempo. Se ordena por instante de cruce, luego velocidad y luego Maga → Guerrero → Chamán → Ogro.

Después de actuar, ATB vuelve a cero y se consume recuperación antes de volver a llenarse: básicos **0,6 s**, Golpe pesado **2,2 s** y Poción **0,8 s**. La recuperación usa el mismo reloj y también se congela en decisiones, acciones, pausa y resultado. Los paneles muestran Llenando, Recuperando y Listo; el sobrante de una actualización se aplica al llenado.

El guerrero puede usar Ataque básico, Golpe pesado (8 de maná, potencia 1,8) y Poción. La maga usa Proyectil mágico gratuito y Poción. Los dos enemigos usan sus básicos gratuitos contra un aliado vivo elegido al azar. Cada intento comienza con las estadísticas del GDD y **3 Pociones compartidas** de 50 de vida. No se permite curar a muertos ni a quien tiene vida completa. Cancelar la selección no gasta recursos. No hay regeneración pasiva.

## Controles

| Acción | Teclado | Control |
| --- | --- | --- |
| Elegir comando o menú | Flechas arriba/abajo | Cruceta o stick izquierdo |
| Elegir objetivo | Flechas izquierda/derecha | Cruceta o stick izquierdo |
| Confirmar | Enter o Espacio | Botón inferior (A/cruz) |
| Cancelar objetivo | Escape | Botón derecho (B/círculo) |
| Pausar | Escape desde comandos o ejecución | Botón de menú (Start/Options) |
| Cerrar pausa | Escape o Continuar | Botón derecho, menú o Continuar |

El círculo amarillo identifica al actor; el blanco al objetivo, acompañado por su nombre y equipo en el panel. La pausa permite continuar, reiniciar o volver al menú. Victoria y derrota permiten reiniciar o volver. Reiniciar restaura recursos, inventario, turno, cámara y figuras. Los menús completos de ayuda, volumen y confirmaciones se realizan en H08.

## Cómo revisar H02

1. Elegir Jugar; comprobar que cargan las cuatro barras y que la maga actúa primero. Esperar en su menú: ninguna barra ni recuperación debe avanzar.
2. Resolver el básico de la maga. Cuando llegue el guerrero, elegir Golpe pesado, cambiar objetivo y cancelar: conserva 30 de maná. Confirmarlo gasta 8 una sola vez. Contra el ogro causa **44**; el básico causa **22**. Después del cierre comienza su recuperación de 2,2 segundos activos.
3. Usar una Poción cuando alguien esté herido: recupera hasta 50 y reduce la cantidad compartida. Con vida completa queda bloqueada.
4. Pausar antes o después de un impacto y continuar: no se repite daño ni gasto. Reiniciar desde pausa restaura el intento.
5. Derrotar a ambos enemigos, comprobar Victoria y reiniciar. Si caen ambos aliados aparece Derrota.
6. En `Assets/Data/H02/Resources/Encounters`, inspeccionar `MainEncounter.asset`, los cuatro combatientes, cinco acciones, cinco objetos y tres efectos. Cambiar un valor fuera de Play, iniciar y observar el nuevo valor. Restaurar la configuración de referencia al terminar. El estado de la pelea no modifica estos assets.

Los cinco objetos tienen datos y cantidades iniciales separados del estado runtime; **solo Poción es utilizable todavía**. Los efectos tienen configuración, sin aplicación ni cargas activas hasta H04. La IA sigue usando básicos; las demás acciones y sus decisiones se implementan en H03/H04. Las referencias de arte, iconos, audio y animación pueden estar vacías hasta sus hitos de producción.

Los modelos y la arena son provisionales. La configuración no constituye un balance final ni una medición de la duración objetivo.

## Compilar y comprobar

- Build Profiles: Windows, Intel 64-bit, Mono. Scene List: MainMenu y Combat, en ese orden; SampleScene queda fuera.
- Build H02 en `Builds/H02/UltimaGuardia.exe`. Para otra resolución: `-screen-fullscreen 0 -screen-width 1920 -screen-height 1080`.
- Test Runner → PlayMode → asamblea `UltimaGuardia.PlayModeTests`: **27/27 aprobadas**, reglas, reloj, cola, recuperación, datos editables, flujo e input. Después del ajuste visual de pausa se repitieron las **4 pruebas de flujo**, también aprobadas. Resultados y métodos en `Documentation/TEST_REPORT.md`.
- Las pruebas de dispositivos utilizan Keyboard y Gamepad virtuales del Input System; no acreditan un control físico.
- La build Development acepta `-foundation-smoke`: comprueba menú, carga de Combat, pausa, retorno y salida real mediante Salir; guarda capturas y JSON en `Builds/H02/Validation`. No juega una pelea completa: esa comprobación está en PlayMode. El arnés queda excluido de builds sin Development Build.
- Las capturas de pruebas a 720p/1080p usan RenderTexture; no acreditan un monitor físico 1080p. Métodos, resultados y avisos se registran en el informe.

## Sistemas y estructura

Input System **1.19.0** (`activeInputHandler = 1`) y **uGUI 2.0.0**. Cada escena usa una copia de ejecución de `GuardiaInput.inputactions`. MainMenu conserva su EventSystem; Combat procesa las mismas acciones y muestra el foco en texto, sin un segundo controlador de confirmación. El asset de entrada de la plantilla se conserva.

`CombatSession` contiene reglas, recursos, reloj activo, cola y acción pendiente; `CombatPrototype` controla fases, input y tiempo de presentación; `CombatantView` vincula figuras y sockets. `CombatantData`, `ActionData`, `ItemData`, `EffectData` y `EncounterData` son ScriptableObjects de configuración. El estado mutable y los snapshots de valores se crean de nuevo en cada intento. La configuración del encuentro referencia participantes, prefabs, posiciones e inventario. Los datos inválidos impiden crear una sesión y explican el campo que debe corregirse. La IA completa, estados y recursos finales corresponden a hitos posteriores.

Test Framework **1.6.0**, ProBuilder **6.0.9** para el blockout editable y Timeline **1.8.11** disponibles. Se conservan manifiesto y lock; Unity MCP es una herramienta de desarrollo, no una dependencia del player. No se compraron assets ni se añadieron servicios pagos.

Las utilidades **Regenerar escenas base (H00)** y **Crear arena y combate provisional (H01)** reemplazan escenas. Son herramientas de preparación explícitas, nunca automáticas: no usarlas sobre escenas con trabajo posterior. La de H01 crea solo Combat y conserva MainMenu.

La utilidad explícita **H02/Integrar datos y ATB en Combat** agrega la configuración y barras sobre Combat existente, sin regenerar arena ni MainMenu. Conserva los assets H02 si ya existen y requiere la escena guardada y Play detenido; no es necesaria para jugar la entrega integrada.

Assets separa Scenes, Scripts, Data, Prefabs, Models, Animations, Materials, Textures, UI, VFX, Audio y Tests. Versionar Assets con sus `.meta`, Packages, ProjectSettings y Documentation; excluir Library, Temp, Logs, UserSettings y Builds.

- [GDD, 245 tareas y estado autorizado](Documentation/Ultima_Guardia_GDD_y_Plan_de_Desarrollo.md)
- [Comprobaciones y evidencia](Documentation/TEST_REPORT.md)
- [Recursos, procedencia y licencias](Documentation/ASSET_REGISTER.md)
- [Balance](Documentation/BALANCE_LOG.md)
- [Incidencias y límites](Documentation/KNOWN_ISSUES.md)

La aprobación final de cada hito corresponde al usuario. Este chat realiza únicamente H02; H03 queda para una conversación posterior.
