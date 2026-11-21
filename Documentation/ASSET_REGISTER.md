# Registro de recursos

## Criterio de producción

Usar recursos propios o gratuitos, con licencia verificable y estilo 3D simple coherente. No comprar recursos ni contratar servicios pagos. Conservar fuentes editables, exportaciones, metadatos de Unity y créditos. Adaptar un recurso permitido puede sustituir producirlo desde cero; su integración y comprobación siguen siendo obligatorias.

Para cada incorporación registrar: ID, archivo/fuente, URL, autor, licencia y versión, atribución requerida, modificaciones, hito, ruta importada y evidencia de integración. Un recurso sin licencia identificada no se integra como recurso final.

## Recursos presentes en H00

| ID | Recurso / ruta | Procedencia y licencia | Cambios / estado |
| --- | --- | --- | --- |
| BASE-01 | Escenas, scripts y GuardiaInput en Assets | Elaborados para este proyecto; sin recurso externo adicional | Base H00; UI provisional |
| BASE-02 | `LegacyRuntime.ttf`, recurso integrado de Unity | Distribuido con Unity; fuente integrada, sin archivo externo incorporado | Texto provisional de uGUI; tipografía final en H08 |
| BASE-03 | `Assets/Settings/*`, SampleScene y InputSystem_Actions | Plantilla Unity existente en el proyecto recibido | Conservados. SampleScene fuera de build. Gráficos PC/Mobile tenían cambios previos a H00 |
| BASE-04 | Paquetes de Packages | Unity Registry y Unity MCP, licencias incluidas en cada paquete | Versiones y lock conservados; no son assets finales del juego |

## Inventario pendiente de producción e integración

Las filas **CHAR-01 a CHAR-04 y WEAP-01 a WEAP-04 se produjeron e integraron en H05**, según el registro de incorporaciones al final de este archivo. Las filas ANIM, ENV, UI, AUDIO y VFX siguen pendientes; no acreditan adquisiciones ni recursos ya creados.

| ID | Recurso necesario | Producción / preparación | Hito |
| --- | --- | --- | --- |
| CHAR-01 | Guerrero con cuerpo y armadura | Modelar/adaptar, materiales, rig/pesos, fuente, exportación y prefab | H05 |
| CHAR-02 | Maga con vestimenta | Modelar/adaptar, materiales, rig/pesos, fuente, exportación y prefab | H05 |
| CHAR-03 | Ogro | Modelar/adaptar, materiales, rig/pesos, fuente, exportación y prefab | H05 |
| CHAR-04 | Chamán con vestimenta | Modelar/adaptar, materiales, rig/pesos, fuente, exportación y prefab | H05 |
| WEAP-01 | Espada del guerrero | Modelo/materiales, agarre y fuente | H05 |
| WEAP-02 | Bastón y foco de la maga | Modelo/materiales, agarre y emisor | H05 |
| WEAP-03 | Machete o arma pesada del ogro | Modelo/materiales y agarre | H05 |
| WEAP-04 | Bastón/tótem del chamán | Modelo/materiales, agarre y emisor | H05 |
| ANIM-01 | Reposo, reacción y muerte de los cuatro | Clips, controladores, pausa y limpieza | H06 |
| ANIM-02 | Básico y Golpe pesado del guerrero | Clips y sincronización de impacto | H06 |
| ANIM-03 | Proyectil, Curación y pose especial de la maga | Clips, emisores y sincronización | H06 |
| ANIM-04 | Golpe y Machetazo del ogro | Clips e impacto | H06 |
| ANIM-05 | Descarga, Veneno y Curación oscura del chamán | Clips e impacto | H06 |
| ANIM-06 | Uso de objetos de ambos aliados | Gesto reutilizable, regreso al anclaje | H06 |
| ENV-01 | Suelo de piedra | Módulo, materiales, contactos y fuente | H07 |
| ENV-02 | Muros y remates | Kit modular con pivotes coherentes | H07 |
| ENV-03 | Portón y marco | Modelo, materiales y fuente | H07 |
| ENV-04 | Columnas o contrafuertes | Módulos laterales | H07 |
| ENV-05 | Estandarte y/o antorcha | Una o dos decoraciones reutilizables | H07 |
| ENV-06 | Materiales de piedra, metal, madera y tela | UV/texturas si hacen falta, iluminación y cámaras | H07 |
| UI-01 | Tipografía, paleta y paneles finales | Licencia, legibilidad y escalado | H08 |
| UI-02 | Cuatro retratos/identificadores y equipos | Iconos o retratos | H08 |
| UI-03 | Vida, maná, ATB y recuperación | Iconos y estados de barra | H08 |
| UI-04 | Básicos, golpe fuerte, curación, tormenta, descarga y veneno | Iconos de acciones | H08 |
| UI-05 | Poción, Éter, Tónico, Frasco y Antídoto | Cinco iconos de consumibles | H08 |
| UI-06 | Gota verde, flecha azul y escudo roto naranja | Tres iconos de estados con contadores | H08 |
| UI-07 | Ayudas de teclado y control | Iconos por posición de botón, foco y objetivos | H08 |
| AUDIO-01 | Música de combate | Pista gratuita/propia, bucle, edición, mezcla e integración | H09 |
| AUDIO-02 | Navegar, confirmar, cancelar, inválido | Clips breves de interfaz | H09 |
| AUDIO-03 | Espada, golpe e impacto pesado | Movimiento e impactos físicos | H09 |
| AUDIO-04 | Proyectil y Descarga | Emisiones e impactos mágicos | H09 |
| AUDIO-05 | Vida, maná y uso de consumible | Recuperaciones | H09 |
| AUDIO-06 | Veneno, rapidez, corrosión y limpieza | Aplicación y activación | H09 |
| AUDIO-07 | Carga y rayos de Tormenta arcana | Edición a la secuencia de 2,5 s | H09 |
| AUDIO-08 | Muertes, victoria y derrota | Clips/jingles diferenciados | H09 |
| VFX-01 | Impactos físicos | Efectos breves | H10 |
| VFX-02 | Proyectil aliado y Descarga | Emisión, trayectoria e impacto | H10 |
| VFX-03 | Curación, Poción y Éter | Recuperación legible | H10 |
| VFX-04 | Veneno, Rapidez, Corrosión y Antídoto | Aplicación, activación y limpieza | H10 |
| VFX-05 | Círculo, carga, partículas azules y rayos | Secuencia especial integrada con cámara/audio | H10 |

H01 usa representaciones provisionales, H04 iconos provisionales. No sustituyen las filas de recursos finales.

## Incorporaciones H01

| ID | Recurso / ruta | Procedencia y licencia | Integración |
| --- | --- | --- | --- |
| H01-ENV | Suelo, muros, portón y marco en Combat | Geometría propia generada con ProBuilder 6.0.9 ya instalado; sin recurso externo incorporado | 7 volúmenes editables en `Arena_Blockout_Provisional`; fuente en escena y CombatPrototypeSetup.cs; escenario final pendiente H07 |
| H01-CHAR | `Assets/Prefabs/H01/{warrior,mage,ogre,shaman}.prefab` | Primitivas incluidas en Unity, composición propia | Cuatro volúmenes provisionales con nombre, identidad, visual, marcadores y sockets; instancias en Combat; modelos finales pendientes H05 |
| H01-MAT | `Assets/Materials/H01/*.mat` | Materiales propios, shader URP/Lit existente | Piedra, suelo, portón y colores de participantes/marcadores; sin texturas externas |
| H01-CODE | `Assets/Scripts/Combat/*`, CombatPrototypeSetup y pruebas | Elaborados para este proyecto | Núcleo, input/presentación, utilidad explícita y comprobaciones integrados |
| H01-UI | Canvas de Combat, Text y TextMesh | uGUI y fuente integrada LegacyRuntime.ttf de Unity | UI provisional; recursos finales UI-01 a UI-07 siguen pendientes H08 |

No se descargaron recursos ni se compraron assets. Las filas CHAR, ENV y demás recursos finales del inventario anterior conservan su estado pendiente: estos placeholders no los sustituyen como entregables finales.

## Incorporaciones H02

| ID | Recurso / ruta | Procedencia y licencia | Integración |
| --- | --- | --- | --- |
| H02-DATA | `Assets/Data/H02/Resources/Encounters/*.asset` | Configuración propia basada en las tablas del GDD; sin recurso externo | 18 ScriptableObjects: 4 combatientes, 5 acciones existentes, 5 objetos, 3 efectos y 1 encuentro. Solo básicos, Golpe pesado y Poción tienen ejecución jugable en H02 |
| H02-CODE | Cinco tipos de datos, CombatSession, CombatPrototype, AtbSetup y pruebas | Elaborados para este proyecto | Datos, validación, snapshots, inventario de runtime, reloj, cola, recuperación e integración de UI |
| H02-UI | Cuatro barras uGUI de Combat | Componentes uGUI y sprite `UI/Skin/UISprite.psd` integrados en Unity; sin archivo externo | Barras provisionales y texto Llenando/Recuperando/Listo; UI final pendiente H08 |

Los datos de efectos no representan estados activos implementados. Las referencias de iconos, animación, sonido y VFX pueden estar vacías hasta sus hitos; no se marcaron como completos los recursos finales correspondientes. Se conservaron arena, materiales y prefabs de H01.

## Incorporaciones H03

| ID | Recurso / ruta | Procedencia | Integración |
| --- | --- | --- | --- |
| H03-DATA | `Assets/Data/H02/Resources/Encounters/{heal,arcane_storm,cleave,poison_spell,dark_heal}.asset` | Configuración propia del GDD; sin archivos externos | Cinco acciones añadidas sobre el encuentro existente, diez en total. Se conservan rutas/GUID de H02; objetos con comando explícito, descripciones y cantidades originales |
| H03-CODE | CombatSession, CombatPrototype, ItemData, EncounterData, ActionsSetup y pruebas | Elaborados para el proyecto | Resolución de curación/maná/área, IA, inventario y menús; configuración separada del estado. Estados activos pendientes H04 |
| H03-UI | Panel de decisión de Combat | uGUI y fuente integrada existentes | Listas de cinco objetos y confirmación grupal legibles; arte y UI final siguen pendientes H08 |

No se descargaron ni compraron recursos. Se conservan modelos, materiales, arena, motor y paquetes; esta entrega no completa recursos de arte, animación, audio o VFX posteriores.


## Incorporaciones H04

| ID | Recurso / ruta | Procedencia | Integración |
| --- | --- | --- | --- |
| H04-CODE | `StatusState.cs`, `CombatSession.cs`, `EncounterData.cs`, `StatusSetup.cs` | Elaborados para este proyecto; reglas del GDD | Snapshots de efectos, instancias con identidad, duración, estadísticas efectivas, objetos, IA, validación y migración explícita |
| H04-UI | `StatusHud.cs`, `StatusIcon.cs`, fondo de Header en `Combat.unity` | Geometría vectorial propia generada con uGUI; fuente LegacyRuntime existente | Gota verde, flecha azul y escudo roto naranja con nombre y contador. Filas compactadas con fondos opacos; sin texturas externas |
| H04-TEST | `StatusRulesTests.cs`, ampliaciones de pruebas de reglas/flujo/input | Elaboradas para este proyecto | Casos de duración, reaplicación, muerte, coexistencia, objetos e IA, con capturas de la UI real |

No se descargaron ni compraron recursos. Se mantienen modelos, arena, materiales y paquetes existentes. Los iconos de H04 son provisionales y no completan UI-06 ni otras tareas de arte final de H08. Las referencias `EffectData.icon` siguen disponibles para ese hito; H04 dibuja sus formas provisionales mediante `StatusIcon`.

## Incorporaciones H05

Todos los recursos de esta sección son **originales para Última Guardia, elaborados con Codex el 21 de noviembre de 2026**. No derivan de recursos externos, no tienen URL de adquisición ni exigen atribución de terceros. La fuente editable y los metadatos acompañan la entrega; no se afirma una licencia comercial de terceros ni se incorporan servicios pagos.

| ID | Recurso y archivos | Producción / integración |
| --- | --- | --- |
| CHAR-01 / WEAP-01 | `Assets/Prefabs/H05/warrior.prefab`, `Assets/Models/H05/Exports/warrior.{obj,mtl,rig.json}` | Guerrero con cuerpo, cara, armadura, casco, espada con guarda y empuñadura. 1.812 triángulos, 37 renderers, 16 articulaciones |
| CHAR-02 / WEAP-02 | `Assets/Prefabs/H05/mage.prefab`, `Assets/Models/H05/Exports/mage.{obj,mtl,rig.json}` | Maga con túnica partida, manto, cabello, sombrero y bastón con cristal cian. 2.132 triángulos, 41 renderers, 16 articulaciones |
| CHAR-03 / WEAP-03 | `Assets/Prefabs/H05/ogre.prefab`, `Assets/Models/H05/Exports/ogre.{obj,mtl,rig.json}` | Ogro de mayor volumen, mandíbula, colmillos, hombrera asimétrica, correaje y machete. 1.734 triángulos, 32 renderers, 16 articulaciones |
| CHAR-04 / WEAP-04 | `Assets/Prefabs/H05/shaman.prefab`, `Assets/Models/H05/Exports/shaman.{obj,mtl,rig.json}` | Chamán con túnica, máscara de hueso, cuernos, colgantes y bastón ritual con foco verde. 1.920 triángulos, 32 renderers, 16 articulaciones |
| H05-MAT | `Assets/Materials/H05/*.mat` | 20 materiales propios URP/Lit, opacos, colores planos y variantes de metal. Sin texturas externas. Exportaciones OBJ remapeadas a esos materiales |
| H05-SOURCE | `Assets/Scripts/Editor/CharacterModeling.cs`, `CharacterArtValidation.cs`, `Assets/Scripts/Combat/CharacterRig.cs` | Fuente paramétrica editable, mallas guardadas en `Assets/Models/H05/Meshes/`, exportador OBJ/MTL/jerarquía JSON y validación. Rig por piezas rígidas; no es Humanoid ni piel deformable |

Los cuatro prefabs sustituyen los placeholders dentro de Combat y están vinculados a los datos H02 conservando identidad, estadísticas y acciones. Las armas son hijas del agarre de la mano derecha; `CastOrigin`, `HitAnchor` e `ItemOrigin` quedan disponibles para presentación posterior. Se conservan los prefabs H01 como fuentes históricas, sin instancias visibles en el encuentro.

Dirección visual, convenciones y reconstrucción en [H05_CHARACTER_ART.md](H05_CHARACTER_ART.md). Auditoría, vistas frontal/trasera y seis poses estáticas por personaje en [Evidence/H05](Evidence/H05/). Estas poses comprueban articulación; los clips/controladores jugables se realizan en H06. El escenario, iconos y marcadores conservan sus recursos provisionales.
