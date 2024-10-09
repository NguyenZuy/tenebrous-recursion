using UnityEngine;
using Zuy.TenebrousRecursion.Authoring;

public class Test : MonoBehaviour
{
    public uint a;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("### " + transform.parent.GetComponent<GridAuthoring>().IsEntityInsideCell(transform.localPosition, a));
    }
}