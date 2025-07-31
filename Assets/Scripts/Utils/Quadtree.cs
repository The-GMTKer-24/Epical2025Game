using System.Collections.Generic;
using JetBrains.Annotations;
using NUnit.Framework;
using UnityEngine;

namespace Utils
{
    public class Quadtree<T>
    {

        private List<T> items;
        private QuadNode<T> root;
        
        public Quadtree(int subdivideAt, int maxDepth, RectInt bounding)
        {
            items = new List<T>();
            root = new QuadNode<T>(null, bounding, 0, maxDepth, subdivideAt);
        }
        private List<QuadNode<T>> FindLeaves(RectInt target)
        {
            Queue<QuadNode<T>> ToProccess = new Queue<QuadNode<T>>();
            List<QuadNode<T>> Leaves = new List<QuadNode<T>>();
            ToProccess.Enqueue(root);
            while (ToProccess.TryDequeue(out QuadNode<T> result))
            {
                if (result.Count != -1)
                {
                    Leaves.Add(result);
                }
                else
                {
                    foreach (QuadNode<T> child in result.Children)
                    {
                        if (child.Bounding.Overlaps(target))
                        {
                            ToProccess.Enqueue(child);
                        }
                    }
                }
            }
            return Leaves;
        }

        public bool Overlaps(RectInt area)
        {
            foreach (QuadNode<T> quadNode in FindLeaves(area))
            {
                QuadData<T> current = quadNode.Data;
                while (current != null)
                {
                    if (current.Data.Rect.Overlaps(area))
                    {
                        return true;
                    }
                    current = current.Next;
                }
            }
            return false;
        }
        public List<T> ItemsInArea(RectInt area)
        {
            List<T> overlaps = new List<T>();
            foreach (QuadNode<T> quadNode in FindLeaves(area))
            {
                QuadData<T> current = quadNode.Data;
                while (current != null)
                {
                    if (current.Data.Rect.Overlaps(area))
                    {
                        overlaps.Add(current.Data.Value);
                    }
                    current = current.Next;
                }
            }
            return overlaps;
        }
        public void Insert(T item, RectInt area)
        {
            items.Add(item);
            foreach (QuadNode<T> quadNode in FindLeaves(area))
            {
                quadNode.Put(item, area);
            }
        }
    }

    class QuadNode<T>
    {
        public QuadNode<T>[] Children;
        public int Count { get; private set; }
        public int Depth { get;}
        
        public RectInt Bounding;
        [CanBeNull] public QuadData<T> Data;

        private readonly int maxDepth;
        private readonly int subdivideAt;
        
        public QuadNode(QuadNode<T>[] children,RectInt bounding, int depth, int maxDepth, int subdivideAt)
        {
            Children = children;
            Count = 0;
            Data = null;
            Bounding = bounding;
            Depth = depth;
            this.maxDepth = maxDepth;
            this.subdivideAt = subdivideAt;
        }

        public void Put(T item, RectInt bounds)
        {
            Data = new QuadData<T>(new Data<T>(item, bounds), Data);
            Count++;
            if (Count >= subdivideAt && Depth < maxDepth)
            {
                Subdivide();
            }
        }
        
        private void Subdivide()
        {
            /**
            Children = new[]
            {
                
                new QuadNode<T>(null, new RectInt(Bounding.x,Bounding.y,Bounding.center, Bounding.height/2), Depth + 1, maxDepth, subdivideAt),
                new QuadNode<T>(null, new RectInt(Bounding.x,Bounding.center.y,Bounding.width/2, Bounding.height/2), Depth + 1, maxDepth, subdivideAt),
                new QuadNode<T>(null, new RectInt(Bounding.center.x,Bounding.y,Bounding.width/2, Bounding.height/2), Depth + 1, maxDepth, subdivideAt),
                new QuadNode<T>(null, new RectInt(Bounding.center.x,Bounding.center.y,Bounding.width/2, Bounding.height/2), Depth + 1, maxDepth, subdivideAt)
            };**/
            while (Data != null)
            {
                foreach (QuadNode<T> child in Children)
                {
                    if (child.Bounding.Overlaps(Data.Data.Rect))
                    {
                        child.Put(Data.Data.Value, Data.Data.Rect);
                    }
                }
                Data = Data.Next;
            }
            Count = 0;
        }
    }

    class QuadData<T>
    {
        public QuadData(Data<T> data, [CanBeNull] QuadData<T> next)
        {
            this.Data = data;
            this.Next = next;
        }

        public Data<T> Data { get; private set; }

        [CanBeNull]
        public QuadData<T> Next { get; private set; }
    }
    
    struct Data<T>
    {
        public T Value { get; }

        public RectInt Rect { get; }

        public Data(T value, RectInt rect)
        {
            Value = value;
            Rect = rect;
        }
    }
    
}