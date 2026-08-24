using System;
using System.Collections.Generic;

namespace Portfolio.SampleCode.Gameplay
{
    public interface IRoomDoor
    {
        void Open();
        void Close();
    }

    public interface IRoomEnemy
    {
        event Action<IRoomEnemy> Defeated;
        void Spawn();
    }

    public enum RoomState
    {
        Waiting,
        Fighting,
        Cleared,
        Disposed
    }

    /// <summary>
    /// Coordinates a combat room without depending on scene singletons.
    ///
    /// Adapted from:
    /// 2023/Burbird/SceneGame/Room/Room.cs
    /// 2023/Burbird/SceneGame/Room/BossRoom.cs
    /// </summary>
    public sealed class RoomLifecycle : IDisposable
    {
        private readonly IRoomDoor entranceDoor;
        private readonly IRoomDoor exitDoor;
        private readonly List<IRoomEnemy> configuredEnemies;
        private readonly HashSet<IRoomEnemy> remainingEnemies = new HashSet<IRoomEnemy>();
        private readonly int clearExperience;

        public event Action<int> Cleared;

        public RoomState State { get; private set; } = RoomState.Waiting;
        public int RemainingEnemyCount => remainingEnemies.Count;

        public RoomLifecycle(
            IRoomDoor entranceDoor,
            IRoomDoor exitDoor,
            IEnumerable<IRoomEnemy> enemies,
            int clearExperience)
        {
            this.entranceDoor = entranceDoor ?? throw new ArgumentNullException(nameof(entranceDoor));
            this.exitDoor = exitDoor ?? throw new ArgumentNullException(nameof(exitDoor));
            configuredEnemies = enemies == null
                ? throw new ArgumentNullException(nameof(enemies))
                : new List<IRoomEnemy>(enemies);
            this.clearExperience = Math.Max(0, clearExperience);
        }

        public bool Enter()
        {
            if (State != RoomState.Waiting)
            {
                return false;
            }

            State = RoomState.Fighting;
            entranceDoor.Close();
            exitDoor.Close();

            for (int i = 0; i < configuredEnemies.Count; i++)
            {
                IRoomEnemy enemy = configuredEnemies[i];
                if (enemy == null || !remainingEnemies.Add(enemy))
                {
                    continue;
                }

                enemy.Defeated += OnEnemyDefeated;
            }

            if (remainingEnemies.Count == 0)
            {
                Complete();
                return true;
            }

            // Spawn callbacks are allowed to report defeat immediately, so iterate
            // over a snapshot instead of the set modified by OnEnemyDefeated.
            var enemiesToSpawn = new List<IRoomEnemy>(remainingEnemies);
            for (int i = 0; i < enemiesToSpawn.Count; i++)
            {
                enemiesToSpawn[i].Spawn();
            }

            return true;
        }

        public void Dispose()
        {
            if (State == RoomState.Disposed)
            {
                return;
            }

            UnsubscribeFromRemainingEnemies();
            State = RoomState.Disposed;
            Cleared = null;
        }

        private void OnEnemyDefeated(IRoomEnemy enemy)
        {
            if (State != RoomState.Fighting || enemy == null || !remainingEnemies.Remove(enemy))
            {
                return;
            }

            enemy.Defeated -= OnEnemyDefeated;

            if (remainingEnemies.Count == 0)
            {
                Complete();
            }
        }

        private void Complete()
        {
            if (State != RoomState.Fighting)
            {
                return;
            }

            State = RoomState.Cleared;
            exitDoor.Open();
            Cleared?.Invoke(clearExperience);
        }

        private void UnsubscribeFromRemainingEnemies()
        {
            foreach (IRoomEnemy enemy in remainingEnemies)
            {
                enemy.Defeated -= OnEnemyDefeated;
            }

            remainingEnemies.Clear();
        }
    }
}
