using UI.Grid;
using UnityEngine;

public class RippleEffect : MonoBehaviour
{
    private static readonly int Radius = Shader.PropertyToID("_Radius");
    private static readonly int TintColor = Shader.PropertyToID("_TintColor");

    [Header("Material & Settings")]
    public Material rippleMaterial;
    public Color buildColor;
    public Color deleteColor;
    public float duration = 1.0f;
    [SerializeField]
    private AudioSource soundWhenSwitching;
    
    private float currentTime = 0f;
    private bool isActive = false;
    private bool expanding = true;

    private void Start()
    {
        if (rippleMaterial != null)
        {
            rippleMaterial.SetFloat(Radius, 0f);
        }
        
        soundWhenSwitching = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!isActive || rippleMaterial is null) return;
        
        if (expanding)
        {
            currentTime += Time.deltaTime;
            float radius = Mathf.Clamp01(currentTime / duration) *2;
            rippleMaterial.SetFloat(Radius, radius);

            if (currentTime >= duration)
            {
                isActive = false;
            }
        }
        else
        {
            currentTime -= Time.deltaTime;
            float radius = Mathf.Clamp01(currentTime / duration) *2;
            rippleMaterial.SetFloat(Radius, radius);

            if (currentTime <= 0)
            {
                isActive = false;
            }
            
        }

    }

    public void SetActive(bool active)
    {
        soundWhenSwitching.Play();
        
        if (active)
        {
            if (currentTime >= duration)
            {
                return;
            }
            currentTime = 0f;
            expanding = true;
            
            rippleMaterial.SetFloat(Radius, currentTime);
            switch (GridSystem.Instance.buildMode)
            {
                case BuildMode.Building:
                    rippleMaterial.SetColor(TintColor, buildColor);
                    break;
                case BuildMode.Removing:
                    rippleMaterial.SetColor(TintColor, deleteColor);
                    break;
            }
            // You can change the center dynamically if needed:
            // rippleMaterial.SetVector("_RippleCenter", new Vector2(0.5f, 0.5f));

            isActive = true;
        }
        else
        {
            if (currentTime <= 0f)
            {
                return;
            }
            expanding = false;
            currentTime = duration;
            isActive = true;
        }
    }
}