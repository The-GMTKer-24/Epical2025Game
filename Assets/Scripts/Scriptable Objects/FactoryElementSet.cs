using UnityEngine;

namespace Scriptable_Objects
{
    [CreateAssetMenu(fileName = "New Factory Set", menuName = "Sets/Factory Set", order = 0)]
    public class FactoryElementSet : ScriptableObject
    {
        [SerializeField] private FactoryElementType[] elements;

        public FactoryElementType[] Elements => elements;
    }
}