# Evidencia H08

La corrección de nitidez solicitada por el usuario es **H08-C06**. Sus resultados actuales están en `C06/`: XML de cinco pruebas aprobadas, auditoría de 16 etiquetas, BuildReport actualizado y capturas/resultado del player. Las capturas finales 720p/1080p están en `Run-20261121-232425-443-3746e2/`. Las imágenes previas conservan la entrega original de H08.

- `playmode-results.json`: suite completa 76/76, antes del último ajuste exclusivamente visual.
- `final-ui-results.json`: siete pruebas afectadas repetidas después del ajuste, 7/7.
- `Run-20261121-200346-448-bee60d/`: evidencia de la suite completa, incluida matriz de 20 acciones/objetos.
- `Run-20261121-200944-516-0dcacd/`: capturas canónicas de la interfaz final. Incluye ayuda/volumen por dispositivo, selección grupal, estados, números efectivos y resultados.
- `build-summary.json`: BuildReport real de la build final, 0.0.9-H08, 168,33 MB, 0 errores y 1 advertencia.
- `player-result-1280x720.json`: smoke final aprobado, acción animada verificada y salida por menú/código 0. Sus capturas son `main-menu-1280x720.png`, `animated-action-complete-1280x720.png` y `pause-1280x720.png`.
- `resource-audit.json`: 26 sprites y conservación de valores de los 23 assets de encuentro.
- `scene-preservation.json`: comparación con H07 de bloques serializados de Combat.
- `fingerprints.json`: huellas de archivos de la entrega.

Las otras imágenes sueltas de esta carpeta y `first-full-results.json`/`player-first-attempt.json` conservan iteraciones previas y resultados fallidos. No son la evidencia de cierre. Los métodos, correcciones y límites se explican en `Documentation/TEST_REPORT.md`.
