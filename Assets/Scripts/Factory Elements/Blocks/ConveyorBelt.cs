using System;
using System.Collections.Generic;
using System.Linq;
using Factory_Elements;
using Factory_Elements.Settings;
using Scriptable_Objects;
using Unity.Mathematics;
using UnityEngine;

namespace Factory_Elements.Blocks
{
    // TODO: Implement running belts into the side of other belts (Low priority input)
    public class ConveyorBelt : Block
    {
        [SerializeField] public int capacity = 4;

        [SerializeField] public float speed = 1.5f;
        [SerializeField] public GameObject bottleAsset;

        private IFactoryElement aheadNeighbor;
        protected ElementSettings<Direction> directionSetting;

        private LinkedList<BeltItem>
            items; // First is most recently added (last to leave the belt), Last is least recently added (first to leave the belt)

        private float minDistance;

        private void Awake()
        {
            minDistance = 1.0f / capacity;
            items = new LinkedList<BeltItem>();
            aheadNeighbor = null;
            directionSetting =
                new ElementSettings<Direction>(Direction.North, "Direction", "The direction of the conveyor.");
        }

        private void Update()
        {
            foreach (BeltItem beltItem in items)
            {
                if (directionSetting.Value is Direction.North or Direction.South)
                {
                    beltItem.LinkedObject.transform.position = new Vector3(transform.position.x + .5f,
                        transform.position.y + beltItem.Progress, 0);
                }
                else
                {
                    beltItem.LinkedObject.transform.position = new Vector3(transform.position.x + beltItem.Progress, transform.position.y + .5f, 0);
                }
            }
        }

        private void FixedUpdate()
        {
            var aheadProgress = 1.0f;

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
                    if (aheadNeighbor is not null && aheadNeighbor.TryInsertResource(this, itemList[i].Item))
                        markedForRemoval.Add(itemList[i]);
                    else
                        itemList[i].Progress = 1.0f;
                }
            }

            items = new LinkedList<BeltItem>(itemList);

            foreach (var beltItem in markedForRemoval)
            {
                Destroy(beltItem.LinkedObject);
                items.Remove(beltItem);
                
            }
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

            print(position);
            aheadNeighbor = Factory.Instance.FromLocation(Position + direction);
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
            items.AddFirst(new BeltItem((Item)resource, 0.0f, bottleAsset));
            
            return true;
        }

        public override ISetting[] GetSettings()
        {
            return new ISetting[] { directionSetting };
        }

        public override Dictionary<ResourceType, int> GetHeldResources()
        {
            Dictionary<ResourceType, int> heldResources = new();
            foreach (BeltItem item in items)
            {
                ResourceType resourceType = item.Item.ResourceType;
                heldResources.TryAdd(resourceType, 0);
                heldResources[resourceType]++;
            }
            return heldResources;
        }
    }
}

internal class BeltItem
{
    public Item Item;
    public GameObject LinkedObject;

    public float Progress; // 0-1

    public BeltItem(Item item, float progress, GameObject asset)
    {
        Item = item;
        Progress = progress;
        LinkedObject = GameObject.Instantiate(asset);
    }
}