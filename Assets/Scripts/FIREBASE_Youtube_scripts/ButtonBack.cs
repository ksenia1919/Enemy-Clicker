using UnityEngine;
using UnityEngine.EventSystems;
public class ButtonBack : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject activate;
    [SerializeField] private GameObject disactiavate;
    public void OnPointerClick(PointerEventData eventData)
    {
        activate.SetActive(true);
        disactiavate.SetActive(false);
    }
}
