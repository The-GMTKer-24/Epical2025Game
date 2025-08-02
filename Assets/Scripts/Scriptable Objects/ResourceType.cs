using UnityEngine;

namespace Scriptable_Objects
{
    public class ResourceType : ScriptableObject
    {
        [SerializeField] private Sprite icon;
        protected static Sprite missingPlaceholder;
        
        public virtual void Reset()
        {
            LoadPlaceholder();
            if (missingPlaceholder != null)
                icon = missingPlaceholder;
        }

        public virtual void OnEnable()
        {
            if (icon == null)
                icon = missingPlaceholder;
        }

        private static void LoadPlaceholder()
        {
            if (missingPlaceholder == null)
                missingPlaceholder = Resources.Load<Sprite>("Placeholders/MissingSprite");
        }
        
        public Sprite Icon => icon;
    }
}