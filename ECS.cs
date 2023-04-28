using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace ECS
{
    public abstract class ComponentSystem<T>
    {
        public PackedArray<Entity> currentEntities = new PackedArray<Entity>();
        public abstract void Execute();
        public abstract void Collect();
    }
    public static class EntityManager
    {

        public static BitTracker[] activeEntityBits = new BitTracker[Sets.MAX_ENTITY_COUNT];
        public static UInt16[] allEntityVersion = new UInt16[Sets.MAX_ENTITY_COUNT];
        public static PackedArray<Entity> deadEntities = new PackedArray<Entity>();
        private static UInt16 entityCount = 0;
        public static Entity CreateEntity()
        {
            UInt16 id = entityCount;
            UInt16 version = ++allEntityVersion[id];
            entityCount++;
            return new Entity(id, version);
        }
        public static void AddComponentKey(Entity entity, int index) => activeEntityBits[entity.id].Add(index);
        public static void RemoveComponentKey(Entity entity, int index) => activeEntityBits[entity.id].Remove(index);
        public static void Destroy(Entity entity)
        {
            deadEntities.Add(entity);
            BitTracker bits = activeEntityBits[entity.id];
            for (int i = 0; i < BitTracker.TOTAL_BITS; i++)
            {
                if (bits.Has(i))
                {
                    EntityManager.RemoveComponentKey(entity, i);
                    ComponentManager.allComponentRemoves[i](entity);
                }
            }
        }
    }
    delegate void RemoveEntity(Entity entity);
    public static class ComponentManager
    {
        internal static PackedArray<Sets> allComponentSets = new PackedArray<Sets>();
        internal static PackedArray<RemoveEntity> allComponentRemoves = new PackedArray<RemoveEntity>();
        public static int componentCount = 0;
        public static ComponentSet<T> CreateComponentSet<T>() => CreateComponentSet<T>(5);

        public static ComponentSet<T> CreateComponentSet<T>(int size)
        {
            ComponentSet<T> instanceOfComponentSet = new ComponentSet<T>(size);
            allComponentSets.Add(instanceOfComponentSet);
            allComponentRemoves.Add(instanceOfComponentSet.RemoveEntity);
            int key = ComponentKey<T>.key;
            componentCount++;
            return instanceOfComponentSet;
        }
        public static ComponentSet<T> GetComponentSet<T>() => (ComponentSet<T>)allComponentSets[ComponentKey<T>.key];
    }
    public abstract class Sets
    {
        public const int MAX_ENTITY_COUNT = 5000;
    }
    internal static class ComponentKey<T>
    {
        public static int key = 0;
        static ComponentKey()
        {
            key = ComponentManager.componentCount;
        }
    }
    public class ComponentSet<T> : Sets
    {
        private UInt16[] sparse;
        private PackedArray<Entity> entities;
        private PackedArray<T> components;
        public int count = 0;

        public ComponentSet(int size)
        {
            sparse = new UInt16[MAX_ENTITY_COUNT];
            //fill sparse with empty
            for (int i = 0; i < sparse.Length; i++)
                sparse[i] = UInt16.MaxValue;

            entities = new PackedArray<Entity>(size);
            components = new PackedArray<T>(size);
        }


        public ref T this[Entity entity]
        {
            get
            {
                System.Diagnostics.Debug.Assert(HasEntity(entity), "Entity not Found");
                return ref components[sparse[entity.id]];
            }
        }
        // Check
        public bool HasEntity(Entity entity) => entity.id < sparse.Length && sparse[entity.id] != UInt16.MaxValue &&
            entities.Count > 0 && entities[sparse[entity.id]] == entity;

        // Add
        public void AddEntity(Entity newEntity, T newComponent)
        {
            //UnityEngine.Assertions.Assert.IsFalse(HasEntity(newEntity), "Tried to add an entity that already is in");
            //if pass this point then entity not in ComponentSet
            if (!HasEntity(newEntity))
            {
                EntityManager.AddComponentKey(newEntity, ComponentKey<T>.key);
                sparse[newEntity.id] = (ushort)entities.Count;
                entities.Add(newEntity);
                components.Add(newComponent);
                count = entities.Count;
            }
        }
        // Remove
        public void RemoveEntity(Entity entity)
        {
            UInt16 pindex = sparse[entity.id];
            if (HasEntity(entity))
            {
                EntityManager.RemoveComponentKey(entity, ComponentKey<T>.key);
                int swapID = entities[entities.Count - 1].id;
                entities.Remove(pindex);
                components.Remove(pindex);
                sparse[swapID] = pindex;
                sparse[entity.id] = UInt16.MaxValue;
                count = entities.Count;
            }
        }
        //Get
        public T GetComponent(Entity entity) => components[sparse[entity.id]];


    }
    public struct BitTracker
    {
        public const int TOTAL_BITS = 64;
        public UInt64 bits;
        public bool Has(int index) => (bits & (1UL << index)) == (1UL << index);
        public void Add(int index) { if (!Has(index)) bits = bits | (1UL << index); }
        public void Remove(int index) { if (Has(index)) bits = bits ^ (1UL << index); }
    }
    public struct Entity : System.IEquatable<Entity>
    {
        public UInt16 id;
        public UInt16 version;
        internal Entity(UInt16 newId, UInt16 newVersion)
        {
            this.id = newId;
            this.version = newVersion;
        }

        public bool HasComponent<T>() => ComponentManager.GetComponentSet<T>().HasEntity(this);
        public void AddComponent<T>(T newComponent) => ComponentManager.GetComponentSet<T>().AddEntity(this, newComponent);
        public void RemoveComponent<T>() => ComponentManager.GetComponentSet<T>().RemoveEntity(this);
        public void GetComponent<T>() => ComponentManager.GetComponentSet<T>().GetComponent(this);
        public void Destroy() => EntityManager.Destroy(this);

        public bool Equals(Entity other) => other.id == this.id && other.version == this.version;
        public static bool operator ==(Entity lhs, Entity rhs) => lhs.Equals(rhs);
        public static bool operator !=(Entity lhs, Entity rhs) => !(lhs == rhs);

        public override bool Equals(object? obj) => false;

        public override int GetHashCode() => base.GetHashCode();
    }
    public class PackedArray<T>
    {
        public int Count { private set; get; }
        private T[] arr;

        public PackedArray(int size)
        {
            this.Count = 0;
            if (size <= 0)
            {
                size = 1;
            }
            arr = new T[size];
        }

        public PackedArray() : this(5) { }

        public ref T this[int i]
        {
            get
            {
                return ref arr[i];
            }
        }

        private void Resize() => System.Array.Resize(ref arr, arr.Length * 2);
        public void Add(T item)
        {
            if (Count + 1 > arr.Length) Resize();
            arr[Count++] = item;
        }

        public void Remove(int index)
        {
            T temp = arr[Count - 1];
            arr[index] = temp;
            arr[--Count] = default;
        }
    }
}
