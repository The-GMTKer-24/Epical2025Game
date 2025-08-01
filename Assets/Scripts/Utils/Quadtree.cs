using System.Collections.Generic;
using System.Linq;

namespace Utils
{
    public class Quadtree<T>
    {
        private readonly Dictionary<T, IntRect> itemToRectMap = new();
        private readonly int maxDepth;

        private readonly int maxItemsPerNode;
        private readonly QuadNode root;

        public Quadtree(IntRect bounds, int maxItemsPerNode = 4, int maxDepth = 8)
        {
            this.maxItemsPerNode = maxItemsPerNode;
            this.maxDepth = maxDepth;
            root = new QuadNode(bounds);
        }

        public void Insert(T item, IntRect rect)
        {
            if (itemToRectMap.ContainsKey(item)) Remove(item);

            itemToRectMap[item] = rect;
            InsertRecursive(root, new QuadtreeItem { Item = item, Rect = rect }, 0);
        }

        private void InsertRecursive(QuadNode node, QuadtreeItem item, int depth)
        {
            if (!node.Bounds.Overlaps(item.Rect)) return;

            if (node.IsLeaf)
            {
                node.Items.Add(item);
                if (node.Items.Count > maxItemsPerNode && depth < maxDepth)
                    node.Split(depth, maxDepth, maxItemsPerNode);
            }
            else
            {
                foreach (var child in node.Children)
                    if (child.Bounds.Overlaps(item.Rect))
                        InsertRecursive(child, item, depth + 1);
            }
        }

        public void Remove(T item)
        {
            if (!itemToRectMap.TryGetValue(item, out var rect)) return;

            RemoveRecursive(root, item, rect);
            itemToRectMap.Remove(item);
        }

        private void RemoveRecursive(QuadNode node, T item, IntRect rect)
        {
            if (!node.Bounds.Overlaps(rect)) return;

            if (node.IsLeaf)
                node.Items.RemoveAll(i => EqualityComparer<T>.Default.Equals(i.Item, item));
            else
                foreach (var child in node.Children)
                    if (child.Bounds.Overlaps(rect))
                        RemoveRecursive(child, item, rect);
        }

        public bool Overlaps(IntRect area)
        {
            return IsOccupiedRecursive(root, area);
        }

        private bool IsOccupiedRecursive(QuadNode node, IntRect area)
        {
            if (!node.Bounds.Overlaps(area)) return false;

            if (node.IsLeaf) return node.Items.Any(item => item.Rect.Overlaps(area));

            foreach (var child in node.Children)
                if (IsOccupiedRecursive(child, area))
                    return true;
            return false;
        }

        public List<T> ItemsInArea(IntRect area)
        {
            var results = new HashSet<T>();
            GetItemsRecursive(root, area, results);
            return results.ToList();
        }

        private void GetItemsRecursive(QuadNode node, IntRect area, HashSet<T> results)
        {
            if (!node.Bounds.Overlaps(area)) return;

            if (node.IsLeaf)
            {
                foreach (var item in node.Items)
                    if (item.Rect.Overlaps(area))
                        results.Add(item.Item);
            }
            else
            {
                foreach (var child in node.Children) GetItemsRecursive(child, area, results);
            }
        }

        private class QuadNode
        {
            public QuadNode(IntRect bounds)
            {
                Bounds = bounds;
                Items = new List<QuadtreeItem>();
            }

            public IntRect Bounds { get; }
            public List<QuadtreeItem> Items { get; }
            public QuadNode[] Children { get; private set; }
            public bool IsLeaf => Children == null;

            public void Split(int depth, int maxDepth, int maxItems)
            {
                if (depth >= maxDepth) return;

                var left = Bounds.Left;
                var bottom = Bounds.Bottom;
                var width = Bounds.Width;
                var height = Bounds.Height;

                var halfWidth = width / 2;
                var halfHeight = height / 2;
                var remWidth = width - halfWidth;
                var remHeight = height - halfHeight;

                Children = new QuadNode[4];
                // NW (top-left in bottom-left system)
                Children[0] = new QuadNode(new IntRect(left, bottom + halfHeight, halfWidth, remHeight));
                // NE (top-right)
                Children[1] = new QuadNode(new IntRect(left + halfWidth, bottom + halfHeight, remWidth, remHeight));
                // SW (bottom-left)
                Children[2] = new QuadNode(new IntRect(left, bottom, halfWidth, halfHeight));
                // SE (bottom-right)
                Children[3] = new QuadNode(new IntRect(left + halfWidth, bottom, remWidth, halfHeight));

                foreach (var item in Items)
                    for (var i = 0; i < 4; i++)
                        if (Children[i].Bounds.Overlaps(item.Rect))
                            Children[i].Items.Add(item);

                Items.Clear();
            }
        }

        private struct QuadtreeItem
        {
            public T Item;
            public IntRect Rect;
        }
    }

    public struct IntRect
    {
        public int Left;
        public int Bottom;
        public int Width;
        public int Height;

        public int Right => Left + Width;
        public int Top => Bottom + Height;

        public IntRect(int left, int bottom, int width, int height)
        {
            Left = left;
            Bottom = bottom;
            Width = width;
            Height = height;
        }

        public bool Overlaps(IntRect other)
        {
            return Left < other.Right &&
                   Right > other.Left &&
                   Bottom < other.Top &&
                   Top > other.Bottom;
        }
    }
}