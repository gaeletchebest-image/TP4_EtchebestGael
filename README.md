# Última Guardia

Prototipo de combate 3D por turnos con ATB para PC, un jugador, en español.
Estado de esta versión: **H00, base del proyecto**. La escena Combat todavía no tiene combate.

## Abrir y ejecutar

- Unity **6000.3.11f1** (revisión `3000ef702840`). Abrir esta carpeta desde Unity Hub.
- Plataforma: **Windows x86_64**. Backend inicial: **Mono**. Módulo Windows disponible en el editor inspeccionado.
- Renderer: **URP 17.3.0**, calidad **PC**, asset `Assets/Settings/PC_RPAsset.asset` seleccionado por Quality Settings. Se conservan los assets gráficos existentes.
- Abrir `Assets/Scenes/MainMenu.unity` y pulsar Play. Jugar carga `Assets/Scenes/Combat.unity`; Volver al menú regresa al inicio.
- Build de H00: `Builds/H00/UltimaGuardia.exe`, junto a toda su carpeta de datos. Ejecutar desde esa carpeta. Salir cierra la aplicación; dentro del editor detiene Play.
- Ventana inicial redimensionable de **1280 × 720**; interfaz uGUI escalada desde esa resolución, correspondencia ancho/alto 0,5. Calidad PC, sin cambios de balance ni contenido del juego.

## Controles iniciales

| Acción | Teclado | Control |
| --- | --- | --- |
| Navegar | Flechas arriba/abajo | Cruceta o stick izquierdo |
| Confirmar | Enter o Espacio | Botón inferior (A/cruz) |
| Volver | Escape | Botón derecho (B/círculo) |
| Pausa en Combat | Escape | Botón de menú (Start/Options) |

H00 tiene un menú de controles, una escena vacía identificada y una pausa mínima para comprobar entrada y flujo. Ayuda por páginas, volumen, confirmaciones de abandono y menús finales corresponden a H08. Las reglas de combate comienzan en H01.

## Compilar y comprobar

1. En Build Profiles usar Windows, arquitectura Intel 64-bit, backend Mono.
2. Scene List debe contener MainMenu y Combat, en ese orden; SampleScene queda fuera.
3. Build en `Builds/H00/UltimaGuardia.exe`. Mantener juntos ejecutable, UnityPlayer y datos.
4. Para otra resolución, ejecutar con `-screen-fullscreen 0 -screen-width 1920 -screen-height 1080`.
5. En Test Runner, PlayMode, ejecutar `UltimaGuardia.Tests.FoundationInputTests`. Las pruebas usan dispositivos virtuales del Input System; no acreditan un control físico.

La build Development admite `-foundation-smoke` para una comprobación automatizada del ejecutable: carga real de MainMenu, entrada virtual de teclado/control, capturas de menús/Combat/pausa, retorno y salida mediante Salir. Guarda resultados y capturas en `Builds/H00/Validation/`. Combinar con `-screen-width 1280 -screen-height 720 -screen-fullscreen 0` o con 1920 y 1080. Esta comprobación es optativa y su código queda excluido de las builds sin Development Build.

La validación ejecuta el player oculto y renderiza las capturas fuera de pantalla. Los resultados distinguen resolución de captura y tamaño de ventana nativa: en el equipo disponible, Windows limita la ventana de 1080p a 1366 × 768. Se verificó el escalado a 1080p en RenderTexture. Los avisos de apagado de URP y red del entorno de pruebas se detallan en el informe y en las incidencias.

La herramienta de editor `Ultima Guardia/H00/Regenerar escenas base` reconstruye las dos escenas y reemplaza su contenido. Es una utilidad de preparación de H00: **no usar después de comenzar a editar escenas en H01**. No se ejecuta automáticamente al abrir o importar el proyecto.

## Sistemas y dependencias

Un único sistema de entrada: **Input System 1.19.0**, `activeInputHandler = 1`. Única UI del juego: **uGUI 2.0.0**. `GuardiaInput.inputactions` define Navigate, Confirm, Cancel y Pause; cada escena usa una copia de ejecución para aislar su ciclo de vida. El asset `InputSystem_Actions` de la plantilla se conserva, pero el menú utiliza explícitamente GuardiaInput.

`Packages/manifest.json` y `Packages/packages-lock.json` se conservan versionados. Test Framework 1.6.0 valida integración. ProBuilder 6.0.9 y Timeline 1.8.11 están disponibles para etapas posteriores. AI Navigation, Multiplayer Center, Visual Scripting e integraciones IDE provienen de la plantilla; no se añadieron sistemas jugables con ellos.

Unity MCP es una herramienta de desarrollo, no una dependencia del ejecutable. Su manifiesto referencia `main`; el lock actual resuelve el commit `81542a7e86292b5c4c21b1782c5af23b567f45ce`. Conservar el lock y evitar actualizar paquetes durante la revisión del hito. No se compraron assets ni se añadieron servicios pagos.

## Estructura y seguimiento

Assets separa Scenes, Scripts, Data, Prefabs, Models, Animations, Materials, Textures, UI, VFX, Audio y Tests. Las carpetas vacías conservan sus `.meta`. Documentation está en la raíz para alojar el documento vigente y evidencias sin importarlas al juego.

- [GDD, tareas y estado autorizado](Documentation/Ultima_Guardia_GDD_y_Plan_de_Desarrollo.md)
- [Recursos, procedencia y licencias](Documentation/ASSET_REGISTER.md)
- [Comprobaciones y evidencia](Documentation/TEST_REPORT.md)
- [Balance](Documentation/BALANCE_LOG.md)
- [Incidencias y límites](Documentation/KNOWN_ISSUES.md)

Git usa serialización de texto y Visible Meta Files. Versionar Assets con sus `.meta`, Packages, ProjectSettings y Documentation. Library, Temp, Logs, UserSettings, soluciones generadas y Builds quedan excluidos. La aprobación final de cada hito corresponde al usuario.
