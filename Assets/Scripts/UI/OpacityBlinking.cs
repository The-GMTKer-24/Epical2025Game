using Unity.Mathematics.Geometry;
using UnityEngine;

public class OpacityBlinking : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private float duration=2;
    private float time;
    private float direction = 1;
    private SpriteRenderer renderer;
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        float alpha = Mathf.Abs(Mathf.Sin(time))/2f + 0.3f;
        renderer.color = new Color(1f, 1f, 1f, alpha);
    }
}
