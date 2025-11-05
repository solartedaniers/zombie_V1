using UnityEngine;

public class EnemyScene2 : EnemyFollowBase
{
    public float speedIncrease = 1.2f;

    protected override void Start()
    {
        hitsToDie = 4;
        speed = 4f;
        detectionRange = 20f;
        base.Start();
    }

    public override void TakeBulletHit(Collider hitCollider)
    {
        if (agent != null)
            agent.speed *= speedIncrease;

        TakeHit();
    }
}
