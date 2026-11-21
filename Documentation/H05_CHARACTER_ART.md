# H05 · Dirección visual y fuentes de personajes

Estado: H05 aprobado por el usuario el 21 de noviembre de 2026. Autoría propia para Última Guardia, realizada con Codex. No se incorporan recursos externos ni servicios pagos.

## Hoja de dirección

Fantasía de fortaleza, superficies facetadas y colores planos. Cabezas y manos algo grandes, ojos contrastados, formas geométricas recortadas y detalles concentrados en torso, cabeza y arma. Sin texturas, transparencia ni simulación de tela. Metal frío y latón relacionan a los aliados; cuero, hueso y telas cálidas relacionan a los enemigos. La identidad de equipo continúa indicada también en el HUD.

| Personaje | Silueta y referencias de forma | Materiales y lectura | Producción |
| --- | --- | --- | --- |
| Guerrero | Hombros anchos, coraza, hombreras, casco con cresta, espada de hoja recta y guarda | Azul de guardia, acero frío, latón, cuero oscuro; emblema de cometa en pecho | Geometría propia, lofts poligonales, piezas articuladas |
| Maga | Cuerpo más estrecho, túnica partida, manto, sombrero cónico, bastón de cristal | Violeta oscuro, cabello claro, latón y foco cian | Geometría propia, paneles de tela rígidos ligados a cadera/muslos |
| Ogro | Torso y brazos mucho más voluminosos, mandíbula, colmillos, orejas, cresta, hombrera asimétrica y machete ancho | Piel oliva, tela óxido, cuero y hierro recuperado | Geometría propia con proporciones específicas y articulaciones amplias |
| Chamán | Menor que el ogro, máscara de hueso con cuernos, manto, colgantes y bastón ritual | Ocre, terracota y hueso; verde reservado al pequeño foco de veneno | Geometría propia; máscara, cuernos y foco unidos a sus articulaciones |

Las formas son decisiones propias; no se utilizan imágenes o modelos de referencia externos. Los renders de los modelos y las hojas de poses constituyen las referencias visuales reproducibles del hito.

## Escala, presupuesto y convenciones

- Una unidad Unity equivale a un metro; eje Y arriba y frente local +Z. Raíz a nivel del suelo, escala 1. Anclajes de combate existentes conservados. `Visual` incorpora una pequeña orientación de tres cuartos hacia la cámara.
- Tamaño efectivo en reposo: guerrero 2,52 m; maga 2,71 m incluyendo sombrero; ogro 3 m; chamán 2,37 m incluyendo cuernos. Mínimo Y = 0 en los cuatro. Las medidas incluyen todo el modelo y se registran en `Evidence/H05/model-audit.json`.
- Presupuesto inicial: menos de 10.000 triángulos y 65 renderers por personaje; comprobar cifras reales en el informe. No representa un objetivo de FPS ni una medición de rendimiento.
- Triángulos con normales planas, materiales URP/Lit opacos y colores sin texturas. Las coordenadas UV auxiliares no necesitan atlas; si se añaden texturas en una revisión, habrá que diseñar UV específicas.
- Nombres estables: `Hips/Chest/Head`, `UpperArm_R/L`, `Forearm_R/L`, `Hand_R/L`, `Thigh_R/L`, `Shin_R/L`, `Foot_R/L`. `WeaponGrip_R/Weapon/CastOrigin`, `ItemOrigin` y `HitAnchor` son anclajes explícitos.
- Se emplea la alternativa de piezas rígidas admitida por H05-T22. Cada vértice pertenece por completo a la articulación padre; no hay piel continua ni pesos interpolados. Las uniones se solapan con mangas, guantes y placas para permitir flexión. No se presenta este recurso como un rig Humanoid ni como un SkinnedMeshRenderer.
- Las túnicas se dividen entre cadera y muslos; no requieren simulación de tela. Se comprueban poses de conjuro, ataque, reacción, uso de objeto y caída antes de cerrar H05. Los clips y controladores jugables corresponden a H06.

## Archivos editables y exportaciones

- `Assets/Scripts/Editor/CharacterModeling.cs`: fuente editable de geometría, proporciones, colores, rig, agarres e integración; no se ejecuta por importación o al abrir el proyecto.
- `Assets/Models/H05/Meshes/*.asset`: mallas Unity finales, agrupadas por articulación y material para reducir renderers.
- `Assets/Models/H05/Exports/{warrior,mage,ogre,shaman}.obj` y `.mtl`: exportaciones estáticas por pieza, con materiales, en coordenadas del modelo. No transportan el rig; este se conserva en el prefab y en `.rig.json`.
- `Assets/Models/H05/Exports/*.rig.json`: jerarquía completa con transformaciones de reposo para intercambio e inspección.
- `Assets/Prefabs/H05/*.prefab`: representación animable integrada, mallas y materiales referenciados, identidad, datos y sockets. `CharacterRig` solo conserva presentación; vida, maná y estados siguen en `CombatSession`.

Para reconstruir: detener Play, abrir y guardar Combat y ejecutar `Ultima Guardia/H05/Crear modelos e integrar personajes`. Es una operación explícita de autoría: reemplaza los cuatro prefabs H05 y sus vistas en Combat, no restaura estadísticas ni reconstruye arena o menús. Conservar cambios manuales de esos modelos antes de regenerarlos. Las fuentes y exportaciones se entregan con sus `.meta`.

## Referencias producidas y lectura de las hojas

- [Vista frontal](Evidence/H05/character-lineup-front.png) y [trasera](Evidence/H05/character-lineup-back.png). Orden en pantalla: chamán, ogro, maga, guerrero. Se usa cámara de autoría para inspeccionar los recursos; el encuadre de juego se comprueba por separado en las capturas de Combat.
- [Guerrero](Evidence/H05/warrior-pose-sheet.png), [maga](Evidence/H05/mage-pose-sheet.png), [ogro](Evidence/H05/ogre-pose-sheet.png) y [chamán](Evidence/H05/shaman-pose-sheet.png): seis columnas de izquierda a derecha: reposo, ataque, conjuro, reacción, objeto y caída. Las cámaras se ajustan a cada pose, por lo que el tamaño aparente entre columnas no indica un cambio de escala.
- Las poses son ejemplos estáticos de rango articular, con arma sujeta y ropa segmentada; no son clips ni coreografías terminadas. Conjuro/objeto en enemigos y conjuro en guerrero se incluyen para inspeccionar la representación, sin añadir acciones al juego.
