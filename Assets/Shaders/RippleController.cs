using UnityEngine;

public class RippleEffect : MonoBehaviour
{
    [Header("Material & Settings")]
    public Material rippleMaterial;
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
            rippleMaterial.SetFloat("_Radius", 0f);
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
            rippleMaterial.SetFloat("_Radius", radius);

            if (currentTime >= duration)
            {
                isActive = false;
            }
        }
        else
        {
            currentTime -= Time.deltaTime;
            float radius = Mathf.Clamp01(currentTime / duration) *2;
            rippleMaterial.SetFloat("_Radius", radius);

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
            
            rippleMaterial.SetFloat("_Radius", currentTime);

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