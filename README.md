# Última Guardia

Prototipo de combate 3D por turnos para PC, un jugador, en español.
Versión actual: **0.0.4-H03**, acciones, inventario e inteligencia enemiga. H00–H03 aprobados por el usuario. H04 pendiente.

## Abrir y jugar

- Unity **6000.3.11f1** (revisión `3000ef702840`), URP **17.3.0**, calidad **PC**, Windows x86_64, Mono.
- Abrir `Assets/Scenes/MainMenu.unity` y pulsar Play; elegir Jugar. También se puede abrir `Assets/Scenes/Combat.unity` directamente.
- Ejecutable de esta entrega: `Builds/H03/UltimaGuardia.exe`; mantener juntos el ejecutable y todos los archivos de su carpeta. Las builds anteriores se conservan en `Builds/H00`, `Builds/H01` y `Builds/H02`.
- Ventana inicial redimensionable de **1280 × 720**, uGUI con resolución de referencia 1280 × 720 y correspondencia ancho/alto 0,5. Se mantienen los assets gráficos PC/Mobile recibidos.

H02 reemplaza el orden fijo por **ATB de 0 a 100 según velocidad**: Maga 28, Guerrero 22, Chamán 20 y Ogro 16. Todos comienzan en cero; la maga queda lista primero, aproximadamente a los 3,57 segundos activos. Al quedar alguien listo se detiene el reloj durante decisión y ejecución. Se ejecuta una acción a la vez; una cola de listos se vacía antes de reanudar el tiempo. Se ordena por instante de cruce, luego velocidad y luego Maga → Guerrero → Chamán → Ogro.

Después de actuar, ATB vuelve a cero y se consume recuperación antes de volver a llenarse: básicos **0,6 s**, Golpe pesado **2,2 s** y Poción **0,8 s**. La recuperación usa el mismo reloj y también se congela en decisiones, acciones, pausa y resultado. Los paneles muestran Llenando, Recuperando y Listo; el sobrante de una actualización se aplica al llenado.

El menú raíz ofrece **Atacar, Habilidades y Objetos**. El guerrero dispone de Ataque básico y Golpe pesado (8 de maná, potencia 1,8, recuperación 2,2 s). La maga dispone de Proyectil mágico, Curación (36 de vida, 10 de maná, 1,2 s) y Tormenta arcana (potencia 1,6 a todos los enemigos vivos, un único costo de 18 de maná, 2,8 s). Los básicos son gratuitos y recuperan en 0,6 s. Tormenta requiere confirmar el grupo completo; su presentación sigue siendo provisional hasta H10.

Ambos aliados comparten **3 Pociones, 2 Éteres, 1 Tónico, 1 Frasco y 2 Antídotos**. Poción recupera 50 de vida y Éter 20 de maná, limitados al máximo; no pueden usarse sobre muertos ni sobre el recurso completo. Todos los objetos tienen recuperación de 0,8 s y costo de maná cero. Tónico, Frasco y Antídoto permiten consultar su descripción y equipo previsto, pero permanecen bloqueados y señalados como pendientes de H04. No gastan unidades ni turnos.

El ogro comienza con Golpe y alterna intentos de Machetazo (8 de maná, potencia 1,5, 2,2 s); si no puede pagarlo usa Golpe y mantiene la alternancia. El chamán cura 40 al ogro vivo por 10 de maná y recuperación de 1,5 s cuando está **por debajo del 50 %**. La segunda prioridad, Veneno (6 de maná, 1,2 s, sin daño directo), tiene datos pero sigue bloqueada hasta H04; mientras tanto usa Descarga gratuita. Los ataques eligen una sola vez entre aliados vivos con igual probabilidad. El chamán no se cura a sí mismo.

Las opciones bloqueadas siguen siendo consultables y explican el motivo. Cancelar no gasta recursos; confirmar valida actor, recursos y objetivos y cobra una sola vez. No hay regeneración pasiva ni ajustes de balance.

## Controles

| Acción | Teclado | Control |
| --- | --- | --- |
| Elegir comando o menú | Flechas arriba/abajo | Cruceta o stick izquierdo |
| Elegir objetivo | Flechas izquierda/derecha | Cruceta o stick izquierdo |
| Confirmar | Enter o Espacio | Botón inferior (A/cruz) |
| Volver un nivel desde objetivo, habilidad u objeto | Escape | Botón derecho (B/círculo) |
| Pausar | Escape desde raíz, avance o ejecución | Botón de menú (Start/Options), desde cualquier fase |
| Cerrar pausa | Escape o Continuar | Botón derecho, menú o Continuar |

El círculo amarillo identifica al actor; el blanco al objetivo, acompañado por su nombre y equipo en el panel. La pausa permite continuar, reiniciar o volver al menú. Victoria y derrota permiten reiniciar o volver. Reiniciar restaura recursos, inventario, turno, cámara y figuras. Los menús completos de ayuda, volumen y confirmaciones se realizan en H08.

## Cómo revisar H03

1. Elegir Jugar. La maga actúa primero. Abrir Habilidades: Curación está bloqueada con ambos aliados sanos. Esperar: ATB y recuperación no avanzan.
2. Seleccionar Tormenta arcana: deben marcarse ambos enemigos y verse costo 18 y recuperación 2,8 s. Cancelar conserva el maná y vuelve a la misma habilidad. Confirmar causa **32 al ogro y 34 al chamán**, con un único gasto de 18. Con un solo enemigo vivo afecta solamente a él.
3. Con el guerrero abrir Habilidades → Golpe pesado. Cancelar objetivos conserva 30 de maná; confirmar descuenta 8 y causa **44 al ogro**. El básico causa **22**.
4. Cuando un aliado esté herido, usar Curación o Poción, incluso sobre el propio actor. Con maná faltante, usar Éter desde cualquiera de los aliados: recupera hasta 20 y reduce las unidades compartidas. Cancelar no gasta.
5. En Objetos consultar las cinco entradas; las tres dependientes de estados indican que están pendientes de H04 y no se pueden ejecutar. Volver conserva el foco. Escape retrocede un nivel; desde raíz abre pausa. El botón de menú pausa también dentro de listas.
6. Observar Golpe → Machetazo del ogro. Dejarlo por debajo de 120 de vida para que el chamán lo cure en su turno si puede pagar 10. Con 120 exactos no lo cura. Veneno no se ejecuta todavía.
7. Pausar antes/después de un impacto y reiniciar, ganar o perder y reiniciar: recursos, inventario, contadores, barras, marcadores y cámara deben volver al inicio.
8. Los datos siguen en `Assets/Data/H02/Resources/Encounters` para conservar rutas y referencias: 4 combatientes, **10 acciones**, 5 objetos, 3 efectos y encuentro. Cambiar un valor fuera de Play se refleja en el siguiente intento; el estado runtime no modifica estos assets.

Los efectos tienen configuración, sin aplicación ni cargas activas hasta H04. La prioridad de Veneno está preparada y condicionada a que la acción sea ejecutable; H04 completará selección por ausencia de veneno, aplicación y duración. Las referencias de arte, iconos, audio y animación pueden estar vacías hasta sus hitos de producción.

Los modelos y la arena son provisionales. La configuración no constituye un balance final ni una medición de la duración objetivo.

## Compilar y comprobar

- Build Profiles: Windows, Intel 64-bit, Mono. Scene List: MainMenu y Combat, en ese orden; SampleScene queda fuera.
- Build H03 en `Builds/H03/UltimaGuardia.exe`. Para otra resolución: `-screen-fullscreen 0 -screen-width 1920 -screen-height 1080`.
- Test Runner → PlayMode → asamblea `UltimaGuardia.PlayModeTests`: **49/49 aprobadas**, reglas nuevas, regresiones, reloj, cola, recuperación, datos editables, flujo e input. Resultados y métodos en `Documentation/TEST_REPORT.md`.
- Las pruebas de dispositivos utilizan Keyboard y Gamepad virtuales del Input System; no acreditan un control físico.
- La build Development acepta `-foundation-smoke`: comprueba menú, carga de Combat, pausa, retorno y salida real mediante Salir; guarda capturas y JSON en `Builds/H03/Validation`. No juega una pelea completa: esa comprobación está en PlayMode. El arnés queda excluido de builds sin Development Build.
- Las capturas de pruebas a 720p/1080p usan RenderTexture; no acreditan un monitor físico 1080p. Métodos, resultados y avisos se registran en el informe.

## Sistemas y estructura

Input System **1.19.0** (`activeInputHandler = 1`) y **uGUI 2.0.0**. Cada escena usa una copia de ejecución de `GuardiaInput.inputactions`. MainMenu conserva su EventSystem; Combat procesa las mismas acciones y muestra el foco en texto, sin un segundo controlador de confirmación. El asset de entrada de la plantilla se conserva.

`CombatSession` contiene reglas, recursos, reloj activo, cola, IA y acción pendiente con colección de objetivos/resultados; `CombatPrototype` controla fases, menús, input y tiempo de presentación; `CombatantView` vincula figuras y sockets. `CombatantData`, `ActionData`, `ItemData`, `EffectData` y `EncounterData` son ScriptableObjects de configuración. El estado mutable y los snapshots de valores se crean de nuevo en cada intento. La configuración del encuentro referencia participantes, prefabs, posiciones e inventario. Los datos inválidos impiden crear una sesión y explican el campo que debe corregirse. Los estados y su integración completa con IA corresponden a H04.

Test Framework **1.6.0**, ProBuilder **6.0.9** para el blockout editable y Timeline **1.8.11** disponibles. Se conservan manifiesto y lock; Unity MCP es una herramienta de desarrollo, no una dependencia del player. No se compraron assets ni se añadieron servicios pagos.

Las utilidades **Regenerar escenas base (H00)** y **Crear arena y combate provisional (H01)** reemplazan escenas. Son herramientas de preparación explícitas, nunca automáticas: no usarlas sobre escenas con trabajo posterior. La de H01 crea solo Combat y conserva MainMenu.

La utilidad explícita **H02/Integrar datos y ATB en Combat** agrega la configuración y barras sobre Combat existente, sin regenerar arena ni MainMenu. Conserva los assets H02 si ya existen y requiere la escena guardada y Play detenido; no es necesaria para jugar la entrega integrada.

La utilidad explícita **H03/Integrar acciones e inventario** agrega los datos faltantes al encuentro existente y ajusta la UI provisional; conserva estadísticas, cantidades y assets de acciones existentes. No regenera geometría, prefabs ni MainMenu. Ya fue ejecutada para esta entrega.

Assets separa Scenes, Scripts, Data, Prefabs, Models, Animations, Materials, Textures, UI, VFX, Audio y Tests. Versionar Assets con sus `.meta`, Packages, ProjectSettings y Documentation; excluir Library, Temp, Logs, UserSettings y Builds.

- [GDD, 245 tareas y estado autorizado](Documentation/Ultima_Guardia_GDD_y_Plan_de_Desarrollo.md)
- [Comprobaciones y evidencia](Documentation/TEST_REPORT.md)
- [Recursos, procedencia y licencias](Documentation/ASSET_REGISTER.md)
- [Balance](Documentation/BALANCE_LOG.md)
- [Incidencias y límites](Documentation/KNOWN_ISSUES.md)

La aprobación final de cada hito corresponde al usuario. Este chat realiza únicamente H03; H04 queda para otra conversación después de la revisión.
