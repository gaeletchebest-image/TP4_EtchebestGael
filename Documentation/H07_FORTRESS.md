# H07 · Fortaleza modular

La arena representa la entrada defendida de una fortaleza, con aliados a la izquierda, enemigos a la derecha y portón al fondo. Se conserva el estilo de superficies planas y proporciones sencillas de H05. Piedra gris cálida, madera oscura, hierro, bronce y tela azul; el suelo tiene menos contraste que los personajes. Los braseros usan ascuas estáticas emisivas, sin luces, partículas ni audio propios. Los estandartes tienen pliegue geométrico y escudo del portón, sin simulación de tela.

## Kit y convenciones

| Prefab | Medidas y uso | Pivote |
| --- | --- | --- |
| Paving | Baldosa modular de 3 × 3 m, nueve losas biseladas, superficie superior Y = 0 | Centro en el plano de apoyo |
| CurtainWall | Paño de 2,4 m de ancho × 4,8 m de alto, hiladas alternadas y zócalo | Centro inferior |
| Battlement | Remate de 2,4 m de ancho, cornisa y tres merlones | Centro inferior, sobre muro |
| Gatehouse | Portón cerrado de doce tablones, arco de quince dovelas, jambas, herrajes y tiradores; radio interior 2,1 m | Centro inferior |
| Buttress | Contrafuerte de 5,45 m de alto, base y coronamiento | Centro inferior |
| Standard | Estandarte azul de dos caras, soporte y emblema de bronce | Centro de montaje en pared |
| Brazier | Brasero de 1,375 m de alto, pedestal, cuenco metálico y ascuas | Centro inferior |

Metros, eje Y arriba, fachada hacia −Z, escalas de instancia (1,1,1). Los módulos están en `Assets/Prefabs/H07`; siete mallas persistentes en `Assets/Models/H07/Meshes`; exportaciones OBJ con normales/UV y MTL en `Assets/Models/H07/Exports`; ocho materiales URP/Lit en `Assets/Materials/H07`.

El ensamblaje contiene 56 instancias: 35 pavimentos, seis muros, seis remates, cuatro contrafuertes, dos estandartes, dos braseros y un portón. Total del entorno: **25.650 triángulos, 56 renderers y ocho materiales compartidos**. Cada módulo combina sus superficies en una malla con submallas por material. Los OBJ son exportaciones para intercambio; la escena referencia las mallas nativas. No se construyen modelos durante la ejecución.

Las UV son proyección paramétrica, suficientes para los materiales de color plano entregados. No son un atlas para texturas pintadas ni UV de lightmap: se usan luces en tiempo real y no hay texturas externas. Los materiales metálicos tienen metalicidad moderada; piedra, madera y tela tienen baja suavidad. Un collider plano cubre el suelo; no agrega movimiento ni mecánicas de posición.

## Integración y reconstrucción

Fuente editable: `Assets/Scripts/Editor/FortressAuthoring.cs`. Las formas finales son mallas poligonales propias, con biseles, dovelas y tela plegada; no son los cubos ProBuilder provisionales de H01. La geometría se guarda para poder editarla o sustituirla independientemente de la utilidad.

1. Guardar la escena Combat y detener Play.
2. Para reconstruir explícitamente, usar `Ultima Guardia/H07/Construir fortaleza e integrar` o `FortressAuthoring.BuildAndIntegrate()`.
3. Esta utilidad reemplaza los recursos H07 y el entorno; conservar antes las ediciones manuales de esos recursos. Mantiene personajes H05, sus animaciones, datos, Canvas, anclajes y MainMenu.
4. Para auditar, usar `Ultima Guardia/H07/Auditar escenario y capturar cámaras` o `FortressValidation.AuditAndCapture()`.

La importación de scripts no ejecuta reconstrucciones. No usar utilidades de H00/H01/H05 para regenerar esta entrega, porque reemplazan escenas o personajes. La auditoría muestrea poses y recarga Combat guardada al terminar para no guardar poses de inspección.

## Iluminación y cámaras

Luz principal cálida con sombras suaves, una luz de relleno fría sin sombras y ambiente de tres colores. Una sola luz proyecta sombras; ningún brasero agrega una luz en tiempo real. No se cambia el renderer ni los niveles de calidad recibidos.

Se conserva el encuadre H05: cámara en (0; 7,8; −14), FOV 50, mirada a (0; 0,1; 2). Los cuatro anclajes permanecen exactamente en sus posiciones anteriores. La fachada cierra el fondo y sus soportes laterales cubren las juntas exteriores. El margen inferior corresponde al HUD existente; su diseño final se realiza en H08.

La auditoría guarda una **propuesta de cámara especial** y comprueba su aproximación con 21 muestras, la vista de la maga y los corredores verticales de rayos sobre ambos enemigos. Solo reserva espacio: no agrega movimiento de cámara al juego ni implementa la secuencia de H10. Coordenadas finales y comprobaciones en `Evidence/H07/environment-audit.json`.

## Corrección de contacto heredada

La inspección H07 encontró la punta del pie derecho bajo el plano del suelo durante los ataques físicos H06: aproximadamente −1,54 cm en el guerrero y −2,17 cm en el ogro. Se compensa en el tobillo la inclinación neta de muslo y rodilla (−5° durante el gesto), manteniendo los anclajes y el paso corto. Se regeneran exclusivamente los cuatro clips físicos mediante `CharacterAnimationAuthoring.CorrectPhysicalFootContact()`. Los otros veinte clips, controladores y datos se conservan.

Las caídas conservan una tolerancia de interpolación inferior a 4 mm entre claves; la muestra más baja registrada es −3,38 mm. No se desplaza ningún personaje para ocultar el contacto. La regresión de presentación comprueba retorno al anclaje, impactos únicos, pausa y reinicio.

## Revisión práctica

Abrir `Assets/Scenes/MainMenu.unity`, pulsar Play y elegir Jugar; también se puede abrir Combat directamente. Observar portón, módulos de piedra, estandartes, braseros y sombras. Seleccionar ambos enemigos, usar ataques físicos y proyectiles, dejar actuar al chamán y probar las caídas. Pausar durante una acción, reanudar y reiniciar: la arena y los anclajes deben permanecer estables.

Consultar `Evidence/H07` para las vistas de entorno, HUD a 720p/1080p, poses de acciones y cámara especial reservada. Los métodos, resultados y límites de las pruebas se registran en `TEST_REPORT.md`; recursos y autoría en `ASSET_REGISTER.md`.
