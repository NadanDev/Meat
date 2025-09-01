using UnityEngine;

public class ScrollTexture : MonoBehaviour
{
    private float scrollSpeedX = 0f;
    private float scrollSpeedY = 0f;

    private MeshRenderer meshRenderer;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if (EventHandler.i.DayIsGoing)
        {
            meshRenderer.material.mainTextureOffset = meshRenderer.material.mainTextureOffset + new Vector2(scrollSpeedX, scrollSpeedY / 10000);
            scrollSpeedY = -5f;
        }
        else
        {
            if (scrollSpeedY < -0.01f)
            {
                scrollSpeedY = Mathf.Lerp(scrollSpeedY, 0, Time.deltaTime * 0.5f);
                meshRenderer.material.mainTextureOffset = meshRenderer.material.mainTextureOffset + new Vector2(scrollSpeedX, scrollSpeedY / 10000);
            }
        }
    }
}