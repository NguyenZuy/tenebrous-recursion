using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Zuy.TenebrousRecursion.Component;

namespace Zuy.TenebrousRecursion.Job
{
    [BurstCompile]
    public partial struct GetInsideAgentsJob : IJobEntity
    {
        [ReadOnly][DeallocateOnJobCompletion] public NativeArray<Enemy> enemies;
        [ReadOnly][DeallocateOnJobCompletion] public NativeArray<Entity> enemyEntities;
        [ReadOnly] public float elapsedTime;

        void Execute(ref Cell cell, ref DynamicBuffer<CellMember> cellMembers, in Entity entity)
        {
            if (!(elapsedTime - cell.lastTimeToExecuteJob >= 2f) && cell.lastTimeToExecuteJob != 0f)
            {
                return;
            }

            cell.lastTimeToExecuteJob = elapsedTime;

            cellMembers.Clear();
            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i].curCell == entity)
                {
                    cellMembers.Add(new CellMember()
                    {
                        memberEntity = enemyEntities[i]
                    });
                }
            }
        }
    }
}
