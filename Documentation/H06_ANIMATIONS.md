# H06 · Animaciones y presentación

Fuente editable: `Assets/Scripts/Editor/CharacterAnimationAuthoring.cs`. Los 24 clips son originales para Última Guardia, elaborados con Codex el 21 de noviembre de 2026. Sin descargas, compras ni recursos de terceros. Se conserva la representación de piezas rígidas de H05: 16 articulaciones, armas sujetas a mano y ropa segmentada.

## Recursos y tiempos

Los `.anim`, cuatro `.controller` y cuatro `CharacterAnimationSet` están en `Assets/Animations/H06`. Los controladores se integran en `Visual` de los prefabs H05 existentes; las diez referencias `ActionData.animationClip` apuntan a sus clips correspondientes.

| Personaje | Clips |
| --- | --- |
| Guerrero | Idle, Basic, HeavyStrike, Item, Reaction, Death |
| Maga | Idle, Basic, Heal, ArcaneStorm, Item, Reaction, Death |
| Ogro | Idle, Basic, Cleave, Reaction, Death |
| Chamán | Idle, Basic, Poison, DarkHeal, Reaction, Death |

| Presentación | Duración | Impacto |
| --- | --- | --- |
| Básicos físicos | 0,85 s | 0,35 s |
| Golpe pesado / Machetazo | 1 s | 0,35 s |
| Básicos mágicos y conjuros ordinarios | 0,95 s | 0,35 s |
| Uso de cualquier objeto | 0,85 s | 0,35 s; Antídoto limpia al confirmar, como exige H04 |
| Pose de Tormenta arcana | 2,5 s | 1,2 s |
| Reacción | 0,38 s | Responde a la pérdida real de vida |
| Caída | 0,45 s | Estado terminal; mantiene la pose final |
| Reposo | Ciclo de 2,4 s | Sin desplazamiento acumulado |

La pose de Tormenta eleva el bastón entre 0 y 0,4 s, mantiene la carga hasta 1,2 s y vuelve a reposo al terminar. H06 conecta esa animación y su impacto, pero la secuencia final con cámara, HUD oculto, rayos, partículas y sonido corresponde a H10. Los proyectiles básicos de H06 son geometría sencilla funcional; su acabado visual corresponde a H10.

## Ejecución y limpieza

`CombatPresentation` conserva la acción confirmada y su destinatario. Su reloj sincroniza el clip del actor y envía una única señal de impacto a `CombatSession.ApplyImpact()`, que conserva además su protección contra repetición. Los clips no contienen eventos de gameplay. El maná y las unidades se siguen cobrando una sola vez al confirmar.

`CharacterAnimationPlayer` muestrea los clips guardados mediante Animator, con avance automático y root motion desactivados. Idle, reacciones y caída usan avance explícito; las acciones usan el mismo tiempo que el impacto. Los controladores tienen parámetros Dead/Executing, retorno a Idle y muerte sin salida. Una muerte tiene prioridad sobre el resto de la presentación y no puede convertirse en reposo por el cierre de una acción.

Los ataques físicos incluyen inclinación y desplazamiento local corto de cadera. Los comandos orientan al actor hacia el objetivo confirmado, permitiendo curación propia sin orientación indefinida. El anclaje de escena no se mueve; al acabar se restablece la orientación inicial. La caída se ajusta contra el suelo usando los vértices reales durante la autoría.

Proyectil mágico y Descarga salen de CastOrigin a 0,15 s y llegan a HitAnchor a 0,35 s. La trayectoria conserva el destinatario y sigue su socket, sin volver a decidir el objetivo. Ambos aliados muestran un pequeño recurso en ItemOrigin durante el gesto de objetos.

Pausa congela clips, trayectoria, impacto y cierre. Reiniciar cancela el controlador anterior, retira proyectiles, oculta el objeto, restaura articulaciones y crea una sesión nueva. Volver al menú y destruir el controlador cancelan igualmente la presentación. No hay corutinas ni callbacks de animación que sobrevivan a la pelea.

ATB, cola, costos, inventario, estados, IA y recuperación mantienen las reglas de H04. La recuperación comienza al finalizar la ejecución y no se mide usando la duración del clip.

## Autoría y revisión

Con Combat guardada y Play detenido, `Ultima Guardia/H06/Crear clips y conectar animaciones` regenera explícitamente los recursos H06 y modifica los cuatro prefabs H05 y las referencias de animación. Conservar ediciones manuales de esos recursos antes de usarlo. No ejecutar las utilidades H00/H01 ni regenerar H05 sobre esta entrega.

`Ultima Guardia/H06/Auditar clips y capturar secuencias` verifica curvas, controladores, ausencia de eventos duplicables, retorno a pose inicial, duración y contacto de caída. Produce `Evidence/H06/animation-audit.json` y una hoja por personaje: cada fila corresponde al orden documentado en el JSON, cada columna avanza de 0 al final del clip. La cámara se mantiene fija dentro de cada fila.

Para revisar en juego, abrir MainMenu o ejecutar la build H06 y elegir Jugar. Observar reposos; probar básico de maga, básico/Golpe pesado de guerrero, curación propia y a otro aliado, objetos, ambas acciones del ogro y los conjuros del chamán. Pausar durante preparación/vuelo y después del impacto, reanudar y reiniciar. Los muertos deben conservar la caída y volver a estar de pie al reiniciar.

Resultados y límites de pruebas en [TEST_REPORT.md](TEST_REPORT.md). Evidencia de la matriz de diez acciones y cinco objetos por cada aliado en `Evidence/H06/action-matrix.txt`.
