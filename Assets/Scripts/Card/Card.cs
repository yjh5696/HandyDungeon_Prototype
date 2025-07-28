using UnityEngine;

public class Card : MonoBehaviour
{
    private string _name;
    private string _description;
    [SerializeField] private Sprite sprite;

    public void SetCard(string name, string description)
    {
        _name = name;
        _description = description;
    }
}
