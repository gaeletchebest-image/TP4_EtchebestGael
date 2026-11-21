# Registro de balance

Configuración inicial: tablas del GDD vigente. **No se realizaron ajustes de balance en H00.** Los tres a cinco minutos por intento son una meta pendiente de pruebas.

Registrar cada ajuste con fecha, ID, dato/asset afectado, valor anterior, valor nuevo, motivo, pruebas previas, resultados posteriores e impacto en estrategias. Cambiar números con evidencia y sincronizar el GDD; conservar las mecánicas y el alcance autorizado.

| ID / fecha | Dato / ruta | Antes → después | Evidencia y motivo | Regresión | Estado |
| --- | --- | --- | --- | --- | --- |
| — | — | — | Sin ajustes todavía | — | Pendiente de playtesting |

## H01 · Configuración provisional

Se conservan las estadísticas y fórmulas del GDD: guerrero 160/30, maga 110/45, ogro 240/24 y chamán 130/36; básico potencia 1, Golpe pesado potencia 1,8/costo 8, Poción 50 de vida con 3 unidades compartidas. **Sin ajustes de balance.**

Orden provisional: Guerrero → Maga → Ogro → Chamán, omitiendo muertos. Presentación por acción: impacto a 0,35 s y cierre a 0,85 s, congelados por pausa. Estos tiempos no implementan recuperación ni ATB; H02 sustituye el orden temporal.

Las victorias automatizadas de H01 prueban que la pelea puede terminar. Las decisiones automatizadas son casi inmediatas y aún faltan acciones, IA, ATB y presentación finales, por lo que no se usan para evaluar la meta de 3 a 5 minutos. La derrota preparada a 1 de vida solo comprueba el flujo de resultado. El balance de H12 sigue pendiente.

## H02 · ATB y recuperación

Sin ajustes de balance: se trasladan los valores del GDD a assets. Velocidades: guerrero 22, maga 28, ogro 16, chamán 20. Recuperaciones: básicos 0,6 s, Golpe pesado 2,2 s y Poción 0,8 s. Todos comienzan con ATB 0 y sin recuperación. El costo temporal usa únicamente el reloj activo, separado de los 0,35 s hasta impacto y 0,85 s de presentación provisional.

Las pruebas de reglas verificaron 5 s activos a velocidad 20 y 100/28 s a velocidad 28, empates, recuperación y sobrante. Las peleas PlayMode se aceleran a `Time.timeScale = 4` para comprobar flujo con input virtual: no miden duración de experiencia ni hardware. Victoria con teclado: 26 acciones, 0 Pociones restantes. Victoria con control virtual: 23 acciones, 2 Pociones restantes; este segundo recorrido prepara una herida de 1 de vida en el guerrero para comprobar cancelación de Poción antes de jugar. Son muestras de validación, no resultados de balance. H12 mantiene el playtesting y la meta de tres a cinco minutos pendientes.

## H03 · Acciones e IA

**Sin ajustes de balance.** Se conservan estadísticas, fórmula, velocidades e inventario del GDD. Se agregan Curación (36/10/1,2 s), Tormenta arcana (potencia 1,6/18/2,8 s), Machetazo (1,5/8/2,2 s), Curación oscura (40/10/1,5 s) y Éter (20 de maná/0,8 s). Tormenta causa 32 al ogro y 34 al chamán. Los costos temporales utilizan el reloj activo; impacto/cierre de presentación provisional siguen en 0,35/0,85 s.

El ogro alterna intentos de Golpe/Machetazo; el chamán cura solo al ogro por debajo de 50 %. Veneno y los tres objetos de estados siguen bloqueados hasta H04, por lo que la dificultad todavía no representa el sistema completo. No se modifican números para compensarlo.

Victorias automatizadas desde el encuentro normal: teclado virtual 19 acciones y 2 Pociones restantes; control virtual 22 acciones y 0 Pociones. Decisiones inmediatas y reloj acelerado ×4 exclusivamente en pruebas; derrota con aliados preparados a 1 de vida. No son mediciones de duración, balance, dificultad ni comparación de estrategias de H12.


## H04 · Estados completos

**Sin ajustes de balance.** Veneno: 8 de daño fijo que ignora defensa durante tres inicios de turno. Rapidez: velocidad base ×1,5 durante las próximas dos acciones del afectado. Corrosión: defensa base ×0,5 durante las próximas dos acciones. Tónico/Frasco/Antídoto conservan 1/1/2 unidades iniciales y recuperación de 0,8 s. Veneno del chamán conserva costo 6 y recuperación 1,2 s. El Antídoto elimina el estado al confirmar, sin recuperar vida.

La defensa 6 del ogro queda en 3 con Corrosión: básico del guerrero 25 en vez de 22. La velocidad 28 de la maga queda en 42 con Rapidez. Los decimales se conservan en estadísticas efectivas y se redondea solo el daño final. Menús, animación y pausa no consumen cargas. No se modificaron los tiempos provisionales de presentación ni los costos temporales de acciones anteriores.

Victorias automatizadas desde el encuentro normal: teclado virtual **22 acciones, 0 Pociones**; control virtual **22 acciones, 0 Pociones**. Prioridad de eliminar al chamán y uso de básicos, pesado, Tormenta, Curación y Poción según recursos/heridas. Son dos muestras de flujo con decisiones inmediatas y reloj acelerado ×4. No acreditan duración objetivo, balance validado, equivalencia estadística de dispositivos ni hardware físico. La derrota por veneno y casos de estados coexistentes usan fixtures explícitos, sin modificar el producto. Balance y rendimiento siguen pendientes de H12.

## H05 · Modelos integrados

**Sin ajustes de balance.** Los cuatro datos de combatiente solo cambian su referencia de prefab; estadísticas, acciones, inventario, efectos y costos se conservan. Cámara ajustada para la lectura de modelos y estados; ningún cambio del reloj o de las reglas.

Regresión de flujo con modelos finales: victoria con teclado virtual en **21 acciones y 1 Poción restante**; control virtual en **22 acciones y 0 Pociones**. Son muestras nuevas de la suite, con selección enemiga aleatoria y reloj ×4. No prueban una diferencia de dificultad ni se comparan estadísticamente con H04. Balance, duración objetivo y rendimiento continúan pendientes de H12.

## H06 · Animaciones e impactos

**Sin ajustes de balance.** Estadísticas, potencias, maná, inventario, IA, estados, ATB y recuperaciones conservados. Las diez acciones solo agregan referencias de animación. Cambian los tiempos de presentación: básicos físicos 0,85 s, pesados 1 s, magia ordinaria 0,95 s, objetos 0,85 s y pose de Tormenta 2,5 s con impacto a 1,2 s. El reloj ATB y las recuperaciones permanecen congelados durante la ejecución; estos tiempos visuales no sustituyen la recuperación de cada acción.

Regresión automatizada: victoria con teclado virtual y con control virtual, ambas de **22 acciones y 0 Pociones restantes**. Decisiones inmediatas y reloj ×4 verifican flujo; no miden la duración real de una partida con animaciones ni validan balance. Los veinte casos preparados de presentación no se usan como partidas naturales. Duración objetivo, balance y rendimiento siguen pendientes de H12.
