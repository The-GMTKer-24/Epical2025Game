using System;
using System.Collections.Generic;
using System.Linq;
using Factory_Elements;
using Factory_Elements.Settings;
using Unity.Mathematics;
using UnityEngine;

namespace Factory_Elements.Blocks
{
    public class ConveyorBelt : Block
    {
        [SerializeField] public int capacity = 4;

        [SerializeField] public float speed = 1.5f;

        private ConveyorBelt aheadBelt;
        private IFactoryElement aheadNeighbor;
        protected ElementSettings<Direction> directionSetting;

        private LinkedList<BeltItem>
            items; // First is most recently added (last to leave the belt), Last is least recently added (first to leave the belt)

        private float minDistance;

        private void Awake()
        {
            minDistance = 1.0f / capacity;
            items = new LinkedList<BeltItem>();
            aheadBelt = null;
            aheadNeighbor = null;
            directionSetting =
                new ElementSettings<Direction>(Direction.North, "Direction", "The direction of the conveyor.");
        }

        private void Update()
        {
            // TODO: Move all belt items on this conveyor (child objects) to where their progress would indicate
        }

        private void FixedUpdate()
        {
            var aheadProgress = 1.0f;
            if (aheadBelt && aheadBelt.items.Count != 0) aheadProgress = aheadBelt.items.First.Value.Progress;

            var deltaDistance = Time.fixedDeltaTime * speed;

            var itemList = items.ToList();
            var markedForRemoval = new List<BeltItem>();
            for (var i = itemList.Count - 1; i >= 0; i--)
            {
                if (i == itemList.Count - 1)
                {
                    var distance = 1 - itemList[i].Progress + aheadProgress;
                    if (distance > minDistance) itemList[i].Progress += deltaDistance; // Otherwise gets stuck
                }
                else
                {
                    var farthestAllowableProgress = itemList[i + 1].Progress - minDistance;
                    var farthestReachableProgress = itemList[i].Progress + deltaDistance;
                    var newProgress = Mathf.Min(farthestAllowableProgress, farthestReachableProgress);
                    itemList[i].Progress = newProgress;
                }

                if (itemList[i].Progress > 1.0f)
                {
                    if (aheadNeighbor.TryInsertResource(this, itemList[i].Item))
                        markedForRemoval.Add(itemList[i]);
                    else
                        itemList[i].Progress = 1.0f;
                }
            }

            items = new LinkedList<BeltItem>(itemList);

            foreach (var beltItem in markedForRemoval) items.Remove(beltItem);
            // TODO: Kill item game object
        }

        public override void OnNeighborUpdate(IFactoryElement newNeighbor, bool added)
        {
            base.OnNeighborUpdate(newNeighbor, added);

            int2 direction;
            switch (directionSetting.Value)
            {
                case Direction.North: direction = new int2(0, 1); break;
                case Direction.East: direction = new int2(1, 0); break;
                case Direction.South: direction = new int2(0, -1); break;
                case Direction.West: direction = new int2(-1, 0); break;
                default: throw new Exception("Invalid direction");
            }

            aheadNeighbor = Factory.Instance.FromLocation(Position + direction);
            if (aheadNeighbor == null)
                aheadBelt = null;
            else if (aheadNeighbor is ConveyorBelt belt)
                aheadBelt = belt;
            else
                aheadBelt = null;
        }

        public override bool AcceptsResource(IFactoryElement sender, Resource resource)
        {
            if (items.Count == 0)
            {
                return true;
            }
            float room = items.First.Value.Progress;
            return room >= minDistance;
        }

        public override bool TryInsertResource(IFactoryElement sender, Resource resource)
        {
            if (!AcceptsResource(sender, resource)) return false;
            if (resource is not Item) return false;
            items.AddFirst(new BeltItem((Item)resource, 0.0f));
            // TODO: Create item game object (possibly modify BeltItem constructor)
            return true;
        }

        public override ISetting[] GetSettings()
        {
            return new ISetting[] { directionSetting };
        }
    }
}

internal class BeltItem
{
    public Item Item;

    public float Progress; // 0-1
    // TODO: Paired with a child object of the conveyor belt

    public BeltItem(Item item, float progress)
    {
        Item = item;
        Progress = progress;
    }
}