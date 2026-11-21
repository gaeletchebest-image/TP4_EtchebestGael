# Última Guardia

Prototipo de combate 3D por turnos con ATB para PC, un jugador, en español.
Versión **0.0.8-H07**. H00–H07 aprobados por el usuario. H07 aprobado el 21 de noviembre de 2026. H08 no iniciado.

## Abrir y jugar

- Unity **6000.3.11f1** (`3000ef702840`), URP **17.3.0**, calidad PC, Windows x86_64, Mono.
- Abrir `Assets/Scenes/MainMenu.unity`, pulsar Play y elegir Jugar. También funciona `Assets/Scenes/Combat.unity` directamente.
- Ejecutable de revisión H07: `Builds/H07/UltimaGuardia.exe`, junto con toda su carpeta. Se conservan las builds H00–H06.
- Ventana inicial redimensionable de **1280 × 720**. uGUI usa esa resolución de referencia y correspondencia ancho/alto 0,5. Para solicitar otra resolución: `-screen-fullscreen 0 -screen-width 1920 -screen-height 1080`.

Una arena enfrenta a guerrero y maga contra ogro y chamán. No hay exploración ni movimiento libre. Todos empiezan con recursos completos, sin estados, ATB 0 y sin recuperación. Los cuatro modelos H05 tienen 24 clips H06 de reposo, acciones, objetos, reacción y muerte. H07 incorpora la fortaleza final: pavimento, muros, almenas, portón con arco, contrafuertes, estandartes y braseros. Interfaz final, audio y acabado de efectos corresponden a H08–H10.

## Reglas del combate

ATB sube de 0 a 100 según velocidad: Maga 28, Guerrero 22, Chamán 20 y Ogro 16. Al quedar alguien listo se detiene el reloj durante decisiones y ejecución. Se resuelve una acción por vez y se vacía la cola antes de reanudar. El orden usa instante de cruce, velocidad efectiva y desempate Maga → Guerrero → Chamán → Ogro.

Después de actuar, ATB vuelve a cero y se consume la recuperación fija antes de llenarlo. El sobrante de una actualización se usa para ATB. Decisión, ejecución, pausa y resultado congelan reloj y recuperación. No hay regeneración pasiva ni resurrección.

El guerrero tiene básico gratuito (potencia 1, recuperación 0,6 s) y Golpe pesado (8 de maná, potencia 1,8, 2,2 s). La maga tiene Proyectil mágico gratuito (potencia 1, 0,6 s), Curación (36 de vida, 10 de maná, 1,2 s) y Tormenta arcana (potencia 1,6 a todos los enemigos vivos, un único costo de 18 de maná, 2,8 s). Tormenta reproduce una pose de carga de 2,5 s con impacto a 1,2 s; cámara, ocultamiento de HUD, VFX y sonido finales corresponden a H10.

El inventario es compartido: **3 Pociones** (50 de vida), **2 Éteres** (20 de maná), **1 Tónico de rapidez**, **1 Frasco corrosivo** y **2 Antídotos**. Cada objeto cuesta cero maná, una unidad y recuperación de 0,8 s. Poción y Éter requieren recurso faltante; Antídoto exige un aliado vivo envenenado. Tónico admite aliados vivos y Frasco enemigos vivos, incluyendo renovación de un efecto activo. Cancelar no consume recursos; confirmar valida y cobra una sola vez.

El ogro alterna intentos de Golpe y Machetazo (8 de maná, potencia 1,5, 2,2 s), usando Golpe si no puede pagar. El chamán prioriza Curación oscura al ogro vivo por debajo del 50 % (40 de vida, 10 de maná, 1,5 s); después Veneno sobre un aliado vivo sin ese estado (6 de maná, 1,2 s); finalmente Descarga gratuita (potencia 1, 0,6 s). Elige una sola vez entre los objetivos válidos, con igual probabilidad. No se cura a sí mismo.

## Estados de H04

| Estado | Efecto | Duración |
| --- | --- | --- |
| Veneno | 8 de daño al comienzo del turno, ignorando defensa | 3 activaciones |
| Rapidez | Velocidad base ×1,5, conservando decimales, ATB acumulado y recuperación fija | Próximas 2 acciones del afectado |
| Corrosión | Defensa base ×0,5; redondeo solo al calcular el daño final | Próximas 2 acciones del afectado |

Veneno se procesa antes del menú o la IA. Si mata, se cancela el turno, se eliminan efectos y entrada en cola y se comprueba resultado. Antídoto elimina Veneno al confirmar, sin devolver el daño ya recibido. Veneno no causa daño directo al aplicarse y la muerte del chamán no elimina lo que ya aplicó.

Estados diferentes coexisten. Reaplicar renueva duración sin sumar potencia. Solo las aplicaciones presentes al inicio del turno pueden perder una carga al finalizar su acción; una aplicación renovada tiene identidad nueva. Por eso usar Tónico sobre uno mismo termina con **dos cargas**. Al morir o reiniciar desaparecen los efectos; velocidad y defensa se calculan desde sus bases.

El HUD muestra nombre, contador e icono vectorial provisional: gota verde, flecha azul y escudo roto naranja. Los fondos propios evitan superposición con nombres 3D. Las filas se compactan según estados activos. Arte final en H08.

## Controles

| Acción | Teclado | Control |
| --- | --- | --- |
| Elegir comando o lista | Arriba/abajo | Cruceta o stick izquierdo |
| Elegir objetivo | Izquierda/derecha | Cruceta o stick izquierdo |
| Confirmar | Enter o Espacio | Botón inferior (A/cruz) |
| Volver un nivel | Escape | Botón derecho (B/círculo) |
| Pausar | Escape desde raíz, avance o ejecución | Botón de menú (Start/Options), desde cualquier fase |
| Cerrar pausa | Escape o Continuar | Botón derecho, menú o Continuar |

El círculo amarillo marca al actor y el blanco al objetivo. Pausa y resultado permiten reiniciar o volver al menú. Ayuda por páginas, volumen y confirmaciones de abandono completos corresponden a H08.

## Cómo revisar H07

1. Ejecutar la build H07 y elegir Jugar, o abrir MainMenu en Unity y pulsar Play. Observar portón, arco, piedra, estandartes, braseros, sombras y apoyo de los cuatro personajes. El blockout anterior ya no está visible.
2. Usar Proyectil mágico: la maga eleva el bastón, el proyectil viaja al objetivo confirmado y aplica el daño una vez. Usar básico y Golpe pesado del guerrero para comparar movimientos; observar Golpe/Machetazo del ogro.
3. Probar Curación propia y sobre el compañero, los cinco objetos y los conjuros del chamán. Los objetos muestran un pequeño recurso en la mano izquierda. Tormenta eleva y mantiene el bastón, impacta a 1,2 s y termina a 2,5 s.
4. Pausar durante una acción o vuelo, esperar y reanudar: pose, proyectil y efecto deben continuar desde el mismo punto. Reiniciar desde pausa y volver al menú deben retirar la secuencia anterior.
5. Ganar o perder: los derrotados conservan su caída. Reiniciar restaura pose y recursos. Los movimientos no desplazan los anclajes.
6. Inspeccionar `Arena_Fortress_H07` en Combat y los siete prefabs de `Assets/Prefabs/H07`. Consultar [fuentes y reconstrucción](Documentation/H07_FORTRESS.md), [auditoría](Documentation/Evidence/H07/environment-audit.json) y [evidencia H07](Documentation/Evidence/H07/). La captura de cámara especial es una reserva de espacio; el movimiento final de cámara se realiza en H10. Animator y CharacterAnimationPlayer siguen en `Visual` de los prefabs H05; [documentación de animaciones](Documentation/H06_ANIMATIONS.md).

Fuentes editables en `CharacterModeling.cs`, mallas en `Assets/Models/H05/Meshes`, exportaciones OBJ/MTL/jerarquía JSON en `Assets/Models/H05/Exports` y 20 materiales URP propios en `Assets/Materials/H05`. No se descargaron ni compraron recursos. Se usa un rig de piezas rígidas, alternativa admitida por el GDD, con túnicas segmentadas y sin simulación de tela.

## Comprobación de reglas H04 (regresión)

1. Entrar a Objetos con la maga. Consultar Tónico, Frasco y Antídoto: ya no indican pendientes. Sin veneno, Antídoto explica que no hay objetivos válidos.
2. Elegir Tónico, cancelar y comprobar que conserva su unidad. Confirmarlo sobre la propia maga: queda Rapidez con 2 acciones y velocidad efectiva 42. Tras sus próximas acciones pasa a 1 y desaparece. Recuperación del objeto: 0,8 s.
3. Usar Frasco sobre el ogro: defensa efectiva 3 durante sus próximas dos acciones. El básico del guerrero causa 25 mientras está corroído y 22 al expirar.
4. Dejar actuar al chamán: si el ogro está sano, envenena a un aliado sin veneno. La aplicación no quita vida. Al inicio de cada turno del afectado pierde 8 y baja el contador, hasta tres activaciones.
5. Usar Antídoto sobre un aliado envenenado. Desaparece el icono al confirmar, baja una unidad y no devuelve vida. Un aliado sano no se ofrece como objetivo.
6. Observar prioridad de Curación oscura: ogro con menos de 120 de vida y maná suficiente. A 120 exactos no cura. Si ambos aliados tienen veneno, usa Descarga salvo que corresponda curar.
7. Pausar antes/después de aplicar un estado: ATB, recuperación y contadores se conservan. Reiniciar desde pausa o resultado restaura todos los recursos, inventario, estados, marcadores y cámara.
8. Los casos preparados de reaplicación, coexistencia y muerte por veneno antes de actuar están en las pruebas PlayMode; no se agregaron comandos de depuración al producto.

## Compilar y comprobar

Build Profiles: Windows, Intel 64-bit, Mono. Scene List: MainMenu y Combat, en ese orden; SampleScene queda fuera. Build H07 terminada: **166,62 MB**, **0 errores / 1 advertencia** del shader DebugOccluder de Unity Core (d3d11).

Test Runner → PlayMode → asamblea `UltimaGuardia.PlayModeTests`: **71/71 aprobadas** en el escenario H07, incluidas las regresiones de reglas, input y presentación. Auditoría del entorno aprobada: 24 clips en 216 poses, 648 líneas de visión, 21 muestras de aproximación especial, contactos y márgenes de cámara. La matriz cubre las diez acciones y los cinco objetos con cada aliado. Capturas de HUD y player inspeccionadas. [Resultados y métodos](Documentation/TEST_REPORT.md), [evidencia H07](Documentation/Evidence/H07/).

Las pruebas usan Keyboard/Gamepad virtuales y restauran las opciones de entrada y tiempo. El enrutamiento de entrada del editor se ajusta temporalmente para recibir eventos sin foco. No acredita un control físico. Las capturas 720p/1080p usan RenderTexture; no acreditan monitor físico 1080p. Las peleas aceleradas ×4 verifican flujo, sin medir balance o duración objetivo.

La build Development admite `-foundation-smoke`: prueba menú, carga de Combat, una acción animada de la maga con pausa/reanudación e impacto, retorno y salida mediante Salir; conserva JSON y capturas en `Builds/H07/Validation`. Esta prueba no juega una pelea completa. Resultado H07 aprobado: versión `0.0.8-flow`, inicio MainMenu, `passed = true`, `animatedActionVerified = true`, salida desde Salir y código 0. Se conservan avisos de red y liberación de buffers/asignaciones al cerrar el player oculto; seguimiento en KNOWN_ISSUES. El arnés se excluye de builds sin Development Build.

## Estructura y mantenimiento

`CombatSession` contiene reglas, reloj, cola, IA y acciones pendientes. `CombatantState` mantiene recursos y estados; `EffectDefinition` conserva snapshots de datos y `StatusState` la identidad y duración de cada aplicación. `CombatPrototype` controla input, fases y presentación; `StatusHud`/`StatusIcon` muestran los estados provisionales; `CombatantView` vincula figuras y sockets.

Los datos siguen en `Assets/Data/H02/Resources/Encounters/` para conservar GUID y referencias. Los ScriptableObjects configuran el encuentro; cada intento crea estado independiente. Los datos inválidos impiden iniciar y explican el campo que debe corregirse. Modelos y animaciones están conectados; audio y VFX finales pueden seguir sin referencias hasta sus hitos.

`CharacterRig` conserva articulaciones, pose inicial, arma y sockets. `CharacterAnimationAuthoring` genera explícitamente los clips H06 y conecta los prefabs existentes. `CharacterAnimationValidation` audita y captura secuencias. `CharacterAnimationPlayer` reproduce los clips sin reloj automático, y `CombatPresentation` sincroniza pose, proyectil e impacto desde la acción confirmada. Detalle en [H06_ANIMATIONS.md](Documentation/H06_ANIMATIONS.md).

La utilidad H05 regenera modelos y reemplaza instancias; no ejecutarla sobre H06 sin conservar e integrar después los cambios de animación. Las utilidades H06 también reemplazan sus clips/controladores y requieren conservar antes ediciones manuales.

`FortressAuthoring` conserva la fuente editable de los siete módulos H07, las mallas, OBJ/MTL y ocho materiales URP. La arena usa 56 renderers y 25.650 triángulos, con una sola luz de sombras. La reconstrucción explícita reemplaza recursos H07 y el entorno; conservar ediciones manuales antes de ejecutarla. `FortressValidation` audita geometría, exportaciones importadas, contacto, encuadre y espacio reservado para la especial. H07 corrige además la inclinación del pie derecho en los cuatro clips de ataque físico H06, sin cambiar tiempos ni reglas.

Se conservan Input System 1.19.0, uGUI 2.0.0, Test Framework 1.6.0, ProBuilder 6.0.9 y Timeline 1.8.11. Unity MCP es herramienta de desarrollo, sin dependencia del player. No se compraron recursos ni se añadieron servicios pagos.

Las utilidades de H00/H01 reemplazan escenas y no deben ejecutarse sobre esta entrega. Las utilidades H02/H03/H04 son migraciones explícitas de integración. **H04/Integrar estados temporales** limpia descripciones pendientes y prepara el fondo del HUD sobre Combat existente, conservando estadísticas, inventario, arena, prefabs y MainMenu; ya ejecutada.

Versionar Assets con sus `.meta`, Packages, ProjectSettings y Documentation. Excluir Library, Temp, Logs, UserSettings y Builds.

- [GDD, 245 tareas y estado autorizado](Documentation/Ultima_Guardia_GDD_y_Plan_de_Desarrollo.md)
- [Pruebas](Documentation/TEST_REPORT.md)
- [Recursos y licencias](Documentation/ASSET_REGISTER.md)
- [Balance](Documentation/BALANCE_LOG.md)
- [Incidencias y límites](Documentation/KNOWN_ISSUES.md)

Esta entrega realiza únicamente H07. La revisión final corresponde al usuario; H08 se solicita en otro chat.
