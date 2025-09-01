using UnityEngine;
using UnityEngine.EventSystems;

public class ImmediateButtonPress : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] AudioSource Press;

    public void OnPointerDown(PointerEventData eventData)
    { 
        Press.Play();
        EventSystem.current.SetSelectedGameObject(null);
    }
}