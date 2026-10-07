using UnityEngine;

public abstract class SpawnableObject : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private Renderer model;
    [SerializeField] private Mesh dirtyMesh;

    [Header("Settings")]
    [SerializeField] private float cleanYOffsetOnPlateau;
    [SerializeField] private float dirtyYOffsetOnPlateau;

    protected bool isDirty;

    public float CleanYOffsetOnPlateau => cleanYOffsetOnPlateau;
    public float DirtyYOffsetOnPlateau => dirtyYOffsetOnPlateau;
    public bool IsDirty => isDirty;
    public bool IsVisible => model.enabled;

    public void MarkAsDirty()
    {
        isDirty = true;
        meshFilter.mesh = dirtyMesh;
    }

    public void Display()
    {
        model.enabled = true;
    }

    public void Hide()
    {
        model.enabled = false;
    }
}