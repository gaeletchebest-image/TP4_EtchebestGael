# H08 · Interfaz y menús finales

## Dirección e integración

Interfaz de fortaleza: fondo azul oscuro, paneles de pizarra, acento dorado para selección y títulos. Verde para vida y azul para maná, siempre acompañados por nombre, valor e icono. La tipografía es `LegacyRuntime.ttf`, recurso incorporado de Unity usado mediante `Resources.GetBuiltinResource`; no se descarga ni redistribuye una fuente externa.

Corrección H08-C06: los textos de menús mantienen el componente nativo uGUI Text y generan los glifos al doble de resolución. Se duplican el tamaño de fuente y las dimensiones del rectángulo, con escala local 0,5 para conservar tamaño visual, posiciones y colores. El renderizador nativo conserva la gestión de la textura de glifos y las medidas de texto. Se aplica a los 13 textos de MainMenu y a título/opciones/ayuda de las pantallas superpuestas de Combat (pausa, ayuda, volumen, confirmaciones y resultados). Los textos del HUD de combate conservan su renderizado anterior. La fábrica `InterfaceTheme.MenuText` y la autoría conservan la corrección al reconstruir.

Referencia de Canvas: 1280 × 720, Scale With Screen Size, ajuste ancho/alto 0,5. Títulos de 22–64 px; información de acción de 17–20 px; ayuda de 22–24 px; recursos de 16 px; estados de 13 px con forma, nombre y cargas. El HUD conserva libres la zona central y las cuatro siluetas. Los paneles identifican explícitamente ALIADO y ENEMIGO; un caído muestra DERROTADO.

La fuente editable es `Assets/Scripts/Core/InterfaceTheme.cs`, `Assets/Scripts/Combat/FinalCombatHud.cs` y `Assets/Scripts/Editor/InterfaceAuthoring.cs`. Las escenas serializan el Canvas y sus referencias; el player no necesita utilidades de editor. El menú H08 de Unity reconstruye únicamente los Canvas de MainMenu y Combat, reasigna iconos y regenera marcadores. Reemplaza ediciones manuales de esos recursos: conservarlas antes de ejecutarlo. No ejecutar las utilidades antiguas H00/H01 sobre esta entrega.

## Recursos propios

26 PNG RGBA de 128 × 128 en `Assets/Resources/Interface`, con cobertura 2 × 2 por píxel desde primitivas vectoriales editables en `InterfaceAuthoring.Pixel`. Sprites Single, alpha transparente, bilinear, sin mipmaps, sin compresión, máximo 128 y 128 píxeles por unidad. No se usan imágenes externas, servicios pagos ni recursos descargados.

| Grupo | Recursos |
| --- | --- |
| Participantes | warrior, mage, ogre, shaman |
| Recursos y reloj | life, mana, atb, recovery |
| Acciones | sword, heavy, magic, heal, storm, poison |
| Consumibles | potion, ether, tonic, flask, antidote |
| Estados | poison, haste, corrosion |
| Navegación y composición | target, keyboard, gamepad, fortress, bar |

Veneno reutiliza el mismo significado entre habilidad y estado. Curación oscura reutiliza el símbolo de curación; la descripción y el destinatario indican el bando. Básicos físicos usan espada y básicos mágicos, estrella. La recuperación usa reloj de arena y ATB un reloj. Los datos de combatiente, acción, ítem y efecto conservan referencias a los recursos correspondientes.

Marcadores propios en `Assets/UI/H08`: rombo dorado para actor y aro blanco para objetivo; la etiqueta del actor añade ACTÚA. Los objetivos conservan nombres breves para no superponerse en una selección grupal. La selección grupal marca ambos enemigos vivos y los identifica en el panel. Durante los números flotantes se ocultan brevemente las etiquetas 3D, conservando los cuatro nombres en el HUD. No se modifican anclajes, alcance ni reglas de objetivo.

## Comportamiento

- Los cuatro paneles muestran recursos actuales/máximos, barras, ATB, recuperación pendiente y estados con cargas.
- La lista mantiene consultables las opciones bloqueadas. Un motivo visible impide confirmar sin consumir recursos.
- Descripción y selección informan efecto, costo, recuperación, equipo y destinatario antes de confirmar.
- El registro conserva los últimos cuatro mensajes emitidos por las reglas, incluyendo preparación, impacto y fin de estado. No deduce daño a partir del texto.
- Los números flotantes reciben la cantidad efectiva de daño, curación o maná desde `CombatSession.ResourceFeedback`, con signo y nombre del recurso. El gasto de maná no se presenta como recuperación. Veneno también emite su daño efectivo. No se duplica el impacto.
- Los números usan un desplazamiento vertical separado y se congelan durante pausa; al reiniciar se limpian y se sustituye la suscripción al encuentro anterior.
- MainMenu: Jugar, Cómo jugar, Volumen, Salir. Ayuda circular de cuatro páginas: objetivo, ATB, recursos/estados, controles.
- Pausa: Continuar, Cómo jugar, Volumen, Reiniciar, Volver al menú. Ayuda y volumen mantienen el combate detenido. Volver recupera la opción de origen.
- Reiniciar y volver al menú desde pausa abren una confirmación con Cancelar seleccionado inicialmente. Escape cancela; no se consume ni altera el intento. Resultado ofrece Reiniciar y Volver al menú sin confirmación adicional.
- Volumen general: 0–100, pasos de 10 con izquierda/derecha, aplicado a `AudioListener.volume` y conservado entre escenas y reinicios durante la sesión. La música y los efectos se integran en H09.
- Un solo propietario procesa input de cada pantalla. Las confirmaciones necesitan una pulsación nueva; navegación repite tras 0,4 s y cada 0,12 s, con umbral de stick 0,5. El foco de MainMenu usa EventSystem y el de combate usa el índice seleccionado visible. Ayudas e icono cambian con el último dispositivo utilizado.

## Comprobación

`FinalInterfaceTests` recorre menús con teclado y Gamepad virtual, extremos de volumen, las cuatro páginas, cambio de dispositivo, confirmación mantenida, pausa anidada, cancelación y aceptación de abandono, reinicio, recursos efectivos, restricciones y selección grupal. Captura a 720p y 1080p y exige que el alto de texto visible quepa en cada panel y que todos los iconos usados estén asignados.

`CombatFlowTests` y `AnimationPresentationTests` conservan las regresiones anteriores y guardan la evidencia nueva en `Documentation/Evidence/H08`, sin sobrescribir H07. Se actualizó el recorrido de pausa para exigir las nuevas confirmaciones. La prueba Development `-foundation-smoke` también usa ese recorrido y la cuarta opción Salir.

Los resultados finales, build, inspección visual y límites se registran en `TEST_REPORT.md` y en el registro H08 del GDD. Las capturas por RenderTexture y los controles virtuales no acreditan monitor ni control físicos. H11 conserva su auditoría de hardware y desconexión; H09/H10 conservan audio y VFX finales.

`InterfaceBuild.BuildReview` agrega el comando **H08/Compilar build de revisión**. Compila MainMenu y Combat para Windows x86_64 en Development, conserva el backend del proyecto y escribe `Documentation/Evidence/H08/build-summary.json` con los datos y mensajes reales de BuildReport. No reconstruye escenas ni recursos.
