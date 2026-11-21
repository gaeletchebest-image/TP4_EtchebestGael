# Última Guardia

Prototipo de combate 3D por turnos con ATB para PC, un jugador, en español.
Versión **0.0.6-H05**. H00–H05 aprobados por el usuario. H05 aprobado el 21 de noviembre de 2026; H06 no iniciado.

## Abrir y jugar

- Unity **6000.3.11f1** (`3000ef702840`), URP **17.3.0**, calidad PC, Windows x86_64, Mono.
- Abrir `Assets/Scenes/MainMenu.unity`, pulsar Play y elegir Jugar. También funciona `Assets/Scenes/Combat.unity` directamente.
- Ejecutable de revisión: `Builds/H05/UltimaGuardia.exe`, junto con toda su carpeta. Se conservan las builds H00–H04.
- Ventana inicial redimensionable de **1280 × 720**. uGUI usa esa resolución de referencia y correspondencia ancho/alto 0,5. Para solicitar otra resolución: `-screen-fullscreen 0 -screen-width 1920 -screen-height 1080`.

Una arena enfrenta a guerrero y maga contra ogro y chamán. No hay exploración ni movimiento libre. Todos empiezan con recursos completos, sin estados, ATB 0 y sin recuperación. Los cuatro modelos y sus armas son los recursos H05 propios del prototipo. Arena, interfaz y presentación animada siguen siendo provisionales.

## Reglas del combate

ATB sube de 0 a 100 según velocidad: Maga 28, Guerrero 22, Chamán 20 y Ogro 16. Al quedar alguien listo se detiene el reloj durante decisiones y ejecución. Se resuelve una acción por vez y se vacía la cola antes de reanudar. El orden usa instante de cruce, velocidad efectiva y desempate Maga → Guerrero → Chamán → Ogro.

Después de actuar, ATB vuelve a cero y se consume la recuperación fija antes de llenarlo. El sobrante de una actualización se usa para ATB. Decisión, ejecución, pausa y resultado congelan reloj y recuperación. No hay regeneración pasiva ni resurrección.

El guerrero tiene básico gratuito (potencia 1, recuperación 0,6 s) y Golpe pesado (8 de maná, potencia 1,8, 2,2 s). La maga tiene Proyectil mágico gratuito (potencia 1, 0,6 s), Curación (36 de vida, 10 de maná, 1,2 s) y Tormenta arcana (potencia 1,6 a todos los enemigos vivos, un único costo de 18 de maná, 2,8 s). Tormenta conserva presentación provisional de impacto/cierre a 0,35/0,85 s hasta H10.

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

## Cómo revisar H05

1. Ejecutar `Builds/H05/UltimaGuardia.exe` y elegir Jugar, o abrir MainMenu en Unity y pulsar Play. Revisar espada/armadura del guerrero, túnica/sombrero/bastón de la maga, volumen/colmillos/machete del ogro y máscara/cuernos/bastón del chamán.
2. Seleccionar objetivos y habilidades. Las cuatro siluetas, armas y apoyos deben permanecer visibles, con los modelos ubicados en los anclajes originales. La cámara se ajustó para reservar espacio a los contadores de estados.
3. Atacar, usar objetos, pausar y reiniciar. Se conservan las reglas y la presentación provisional de impactos y caída; los clips completos corresponden a H06.
4. Inspeccionar `Assets/Prefabs/H05/`: cada `Visual` contiene `CharacterRig`, 16 articulaciones, arma en la mano derecha y sockets de conjuro, objeto e impacto. Solo guarda referencias visuales y datos, sin duplicar vida o inventario.
5. Consultar [dirección visual y fuentes](Documentation/H05_CHARACTER_ART.md), [vista frontal](Documentation/Evidence/H05/character-lineup-front.png) y [hojas de poses](Documentation/H05_CHARACTER_ART.md#referencias-producidas-y-lectura-de-las-hojas). Las seis poses son comprobaciones de articulación, no animaciones del juego.

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

Build Profiles: Windows, Intel 64-bit, Mono. Scene List: MainMenu y Combat, en ese orden; SampleScene queda fuera. Build H05 terminada: **164,21 MB**, **0 errores / 1 advertencia** del shader DebugOccluder de Unity Core (d3d11).

Test Runner → PlayMode → asamblea `UltimaGuardia.PlayModeTests`: **68/68 aprobadas** y **4/4 regresiones de flujo** después del ajuste de cámara. Auditoría de los cuatro modelos aprobada y seis poses estáticas por personaje inspeccionadas. [Resultados y métodos](Documentation/TEST_REPORT.md), [evidencia H05](Documentation/Evidence/H05/).

Las pruebas usan Keyboard/Gamepad virtuales y restauran las opciones de entrada y tiempo. El enrutamiento de entrada del editor se ajusta temporalmente para recibir eventos sin foco. No acredita un control físico. Las capturas 720p/1080p usan RenderTexture; no acreditan monitor físico 1080p. Las peleas aceleradas ×4 verifican flujo, sin medir balance o duración objetivo.

La build Development admite `-foundation-smoke`: prueba menú, carga de Combat, pausa, retorno y salida mediante Salir; conserva JSON y capturas en `Builds/H05/Validation`. Esta prueba no juega una pelea completa. Resultado H05 aprobado: versión `0.0.6-flow`, inicio MainMenu, `passed = true`, salida desde Salir y código 0. Se conservan avisos de red y liberación de buffers/asignaciones al cerrar el player oculto; seguimiento en KNOWN_ISSUES. El arnés se excluye de builds sin Development Build.

## Estructura y mantenimiento

`CombatSession` contiene reglas, reloj, cola, IA y acciones pendientes. `CombatantState` mantiene recursos y estados; `EffectDefinition` conserva snapshots de datos y `StatusState` la identidad y duración de cada aplicación. `CombatPrototype` controla input, fases y presentación; `StatusHud`/`StatusIcon` muestran los estados provisionales; `CombatantView` vincula figuras y sockets.

Los datos siguen en `Assets/Data/H02/Resources/Encounters/` para conservar GUID y referencias. Los ScriptableObjects configuran el encuentro; cada intento crea estado independiente. Los datos inválidos impiden iniciar y explican el campo que debe corregirse. Las referencias de arte, audio y animación pueden seguir vacías hasta sus hitos.

`CharacterRig` conserva articulaciones, pose inicial, arma y sockets de cada modelo. `CharacterModeling` es la fuente de autoría H05 y `CharacterArtValidation` genera auditoría y hojas de poses desde el editor. No se ejecutan automáticamente. La utilidad `Ultima Guardia/H05/Crear modelos e integrar personajes` regenera los modelos H05 y reemplaza sus instancias; conservar cambios manuales de esos recursos antes de usarla. Las estadísticas y la arena no se regeneran. Detalle en la dirección visual enlazada arriba.

Se conservan Input System 1.19.0, uGUI 2.0.0, Test Framework 1.6.0, ProBuilder 6.0.9 y Timeline 1.8.11. Unity MCP es herramienta de desarrollo, sin dependencia del player. No se compraron recursos ni se añadieron servicios pagos.

Las utilidades de H00/H01 reemplazan escenas y no deben ejecutarse sobre esta entrega. Las utilidades H02/H03/H04 son migraciones explícitas de integración. **H04/Integrar estados temporales** limpia descripciones pendientes y prepara el fondo del HUD sobre Combat existente, conservando estadísticas, inventario, arena, prefabs y MainMenu; ya ejecutada.

Versionar Assets con sus `.meta`, Packages, ProjectSettings y Documentation. Excluir Library, Temp, Logs, UserSettings y Builds.

- [GDD, 245 tareas y estado autorizado](Documentation/Ultima_Guardia_GDD_y_Plan_de_Desarrollo.md)
- [Pruebas](Documentation/TEST_REPORT.md)
- [Recursos y licencias](Documentation/ASSET_REGISTER.md)
- [Balance](Documentation/BALANCE_LOG.md)
- [Incidencias y límites](Documentation/KNOWN_ISSUES.md)

Este chat realiza únicamente H05. La revisión final corresponde al usuario; H06 se solicita en otro chat.
