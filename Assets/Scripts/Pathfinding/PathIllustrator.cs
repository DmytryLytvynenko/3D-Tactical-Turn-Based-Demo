using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PathIllustrator : MonoBehaviour
{
    [SerializeField] private float LineHeightOffset;
    LineRenderer line;

    private void OnEnable()
    {
        PlayerMovement.CharacterArrived += OnCharacterArrived;
    }
    private void OnDisable()
    {
        PlayerMovement.CharacterArrived -= OnCharacterArrived;
    }
    private void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    public void IllustratePath(Path path)
    {
        line.positionCount = path.tiles.Count;

        for (int i = 0; i < path.tiles.Count; i++)
        {
            Transform tileTransform = path.tiles[i].transform;
            line.SetPosition(i, tileTransform.position.With(y: tileTransform.position.y + LineHeightOffset));
        }
    }
    public void EraseLine()
    {
        line.positionCount = 0;
    }
    private void OnCharacterArrived()
    {
        EraseLine();
    }
}
