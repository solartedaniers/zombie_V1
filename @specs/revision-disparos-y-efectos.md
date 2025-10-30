## Revisión: impactos de bala en enemigos y efectos de impacto

### Resumen del problema
- Los enemigos no registran impactos al disparar.
- El efecto de impacto se muestra solo en algunos objetos (suelo, Enemy, Casa) y no en otros que también reciben disparos.

### Hallazgos en código
- `Assets/Scripts/Bullet.cs` usa `OnCollisionEnter` y restringe el efecto de impacto solo a tags `Ground`, `Enemy`, `Casa`. Además, instancia el efecto en `transform.position` (no en el punto real de contacto) y asume que el objeto colisionado tiene el tag `Enemy` y el componente `EnemyFollow` en el mismo GameObject.
- `Assets/Scripts/PlayerShooting.cs` vuelve a asignar velocidad a la bala con `Rigidbody.velocity`, mientras que `Bullet.cs` también asigna velocidad en `Start()`. Esto no rompe, pero es redundante y puede producir inconsistencias si se usan valores distintos.
- `Assets/Scripts/EnemyFollow.cs` expone `TakeHit()` correctamente, por lo que si la bala lo invoca debería descontar vida y destruir al zombi cuando llega al umbral.

### Causas probables
1. Colisión no detectada porque la bala o el enemigo usan colliders como Trigger y `Bullet.cs` no implementa `OnTriggerEnter`.
2. La colisión impacta en un hijo del enemigo sin el tag `Enemy` ni el componente `EnemyFollow` (p. ej., collider en un hijo); al consultar `collision.gameObject` el `GetComponent<EnemyFollow>()` devuelve `null` y se pierde el hit.
3. El efecto de impacto no aparece en otros objetos por el filtro de tags (solo `Ground`, `Enemy`, `Casa`).
4. Túnel por alta velocidad: sin `collisionDetection = Continuous` en el `Rigidbody` de la bala, puede atravesar colliders finos.
5. Matriz de capas (Project Settings → Physics) bloqueando colisiones entre la capa de la bala y la del enemigo/otros objetos.

### Verificaciones y pasos en Unity (Editor)
1. Prefab de bala:
   - Asegurar que tiene `Rigidbody` (no cinemático) y un `Collider` (SphereCollider/CapsuleCollider). Activar `Collision Detection = Continuous Dynamic`.
   - Si el Collider está con `Is Trigger = true`, se debe manejar `OnTriggerEnter` en el script de la bala (ver propuesta abajo). Si es `false`, `OnCollisionEnter` es suficiente.
2. Enemigo:
   - Confirmar que el GameObject raíz del enemigo (o al menos uno de sus padres) tiene el tag `Enemy` y que el `EnemyFollow` está en el mismo objeto que tiene el tag o en un padre accesible.
   - Si los colliders están en hijos, validar que el código busque el componente en el padre (`GetComponentInParent<EnemyFollow>()`).
3. Layers y matriz de colisiones:
   - Revisar `Edit → Project Settings → Physics` y habilitar colisión entre la capa de la bala y las capas del enemigo y del entorno.
4. Efecto de impacto:
   - Permitir que se instancie para cualquier colisión, y posicionarlo en el punto de contacto real (`GetContact(0).point` o `other.ClosestPoint`). Orientar con la normal del impacto.
5. Sonidos/feedback:
   - Verificar que el `hitEffect` del prefab de bala está asignado. Si se usa VFX/ParticleSystem, habilitar `Stop Action = Destroy` o destruir manualmente la instancia tras 1–2 s.

### Propuestas de cambios de código

1) Unificar manejo de colisiones y triggers en `Bullet.cs`, usar punto de contacto y buscar el enemigo en el padre del collider:

```csharp
// Bullet.cs (propuesta)
private void HandleHit(GameObject other, Vector3 hitPoint, Vector3 hitNormal)
{
    // Efecto de impacto para cualquier objeto
    if (hitEffect != null)
    {
        Quaternion rot = hitNormal != Vector3.zero ? Quaternion.LookRotation(hitNormal) : Quaternion.identity;
        GameObject fx = Instantiate(hitEffect, hitPoint, rot);
        Destroy(fx, 1.5f);
    }

    // Aplicar daño si es enemigo (buscar en el padre por si el collider está en un hijo)
    var enemy = other.GetComponentInParent<EnemyFollow>();
    if (enemy != null)
    {
        enemy.TakeHit();
    }

    Destroy(gameObject);
}

void OnCollisionEnter(Collision collision)
{
    Vector3 point = collision.contactCount > 0 ? collision.GetContact(0).point : transform.position;
    Vector3 normal = collision.contactCount > 0 ? collision.GetContact(0).normal : Vector3.up;
    HandleHit(collision.collider.gameObject, point, normal);
}

void OnTriggerEnter(Collider other)
{
    Vector3 point = other.ClosestPoint(transform.position);
    Vector3 normal = (transform.position - point).sqrMagnitude > 0.0001f ? (transform.position - point).normalized : Vector3.up;
    HandleHit(other.gameObject, point, normal);
}
```

2) Evitar doble asignación de velocidad. Opciones:
   - Dejar que `Bullet.cs` asigne velocidad desde su `speed` y eliminar en `PlayerShooting.cs` la escritura a `rb.velocity`.
   - O bien, eliminar la asignación en `Bullet.Start()` y manejar todo desde `PlayerShooting` con `bulletSpeed`. Elegir una sola fuente de verdad.

3) Robustecer el spawn del efecto para cualquier objeto (eliminar filtro de tags en `Bullet.cs`). Si aún se desea filtrar, usar capas en vez de tags o un `LayerMask` configurable.

4) Configurar físicamente la bala para minimizar túnel:
   - `Rigidbody.collisionDetection = Continuous Dynamic`
   - Activar `Interpolate = Interpolate` y no exagerar `Fixed Timestep`.

### Validación rápida (pruebas manuales)
1. Disparar a un `Cube` con collider en la escena: verificar que el efecto aparece en el punto de impacto.
2. Disparar a un enemigo con colliders en hijos: verificar que recibe daño (log en consola y UI de vida actualiza) y que el efecto aparece.
3. Disparar a larga distancia/objetos finos: confirmar que no hay túnel (si lo hay, aumentar tamaño del collider de la bala ligeramente o considerar raycast hitscan).

### Opcional: enfoque raycast/hitscan
Para armas instantáneas, considerar reemplazar la bala física por un `Physics.Raycast` desde `firePoint` hasta `maxDistance`, instanciando el `hitEffect` donde impacte y aplicando `TakeHit()` si golpea a un `EnemyFollow` en el `collider.transform` o su padre.

---
Si quieres, puedo aplicar estos cambios directamente en los scripts y ajustar el prefab de la bala para dejarlo funcionando.


