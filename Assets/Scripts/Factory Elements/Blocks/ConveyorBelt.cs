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
        [SerializeField] public float equalizationRate = 0.15f;
        [SerializeField] public GameObject beltItemPrefab;

        private IFactoryElement aheadNeighbor;
        protected ElementSettings<Direction> directionSetting;

        private List<BeltItem>
            items; // First is most recently added (last to leave the belt), Last is least recently added (first to leave the belt)

        private float minDistance;

        private void Awake()
        {
            minDistance = 1.0f / capacity;
            items = new List<BeltItem>(capacity);
            aheadNeighbor = null;
            directionSetting =
                new ElementSettings<Direction>(Direction.North, "Direction", "The direction of the conveyor.");
        }

        private void Update()
        {
            foreach (BeltItem beltItem in items)
            {
                    beltItem.LinkedObject.transform.localPosition = new Vector3(0,
                        beltItem.Progress-.5f, 0);
            }
        }

        private void FixedUpdate()
        {
            var aheadProgress = 1.0f;

            if (aheadNeighbor != null && aheadNeighbor is ConveyorBelt belt)
            {
                aheadProgress = belt.items[0].Progress;
            }

            var deltaDistance = Time.fixedDeltaTime * speed;

            var markedForRemoval = new BeltItem[items.Count];
            for (var i = items.Count - 1; i >= 0; i--)
            {
                if (i == items.Count - 1)
                {
                    var distance = 1 - items[i].Progress + aheadProgress;
                    if (distance > minDistance) items[i].Progress += deltaDistance; // Otherwise gets stuck
                }
                else
                {
                    var farthestAllowableProgress = items[i + 1].Progress - minDistance;
                    var farthestReachableProgress = items[i].Progress + deltaDistance;
                    var newProgress = Mathf.Min(farthestAllowableProgress, farthestReachableProgress);
                    items[i].Progress = newProgress;
                }

                if (items[i].Progress > 1.0f)
                {
                    if (aheadNeighbor is not null && aheadNeighbor.TryInsertResource(this, items[i].Item))
                        markedForRemoval[i] = items[i];
                    else
                        items[i].Progress = 1.0f;
                }
            }


            foreach (var beltItem in markedForRemoval)
            {
                if (beltItem != null)
                {
                    Destroy(beltItem.LinkedObject);
                    items.Remove(beltItem);
                }
            }
        }

        public override Direction? Rotation
        {
            get => directionSetting.Value;
            set
            {
                if (value != null) directionSetting.Value = (Direction)value;
            }
        }

        public override bool Rotate(Direction direction)
        {
            directionSetting.Value = direction;
            gameObject.transform.rotation = Quaternion.Euler(0,0,(int)direction * 90);
            return true;
        }

        public override bool SupportsRotation => true;

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
            float room = items[0].Progress;
            return room >= minDistance;
        }

        public override bool TryInsertResource(IFactoryElement sender, Resource resource)
        {
            if (!AcceptsResource(sender, resource)) return false;
            if (resource is Item item)
            {
                item.EqualizationRate = equalizationRate;
                items.Insert(0,new BeltItem(item, 0.0f, beltItemPrefab, (ItemType)item.ResourceType, transform));
                return true;
            }

            return false;
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

    public BeltItem(Item item, float progress, GameObject asset, ItemType itemType, Transform parent = null)
    {
        Item = item;
        Progress = progress;
        LinkedObject = GameObject.Instantiate(asset,parent);
        LinkedObject.name = itemType.name;
        LinkedObject.GetComponent<SpriteRenderer>().sprite = itemType.InWorldSprite;
    }
}