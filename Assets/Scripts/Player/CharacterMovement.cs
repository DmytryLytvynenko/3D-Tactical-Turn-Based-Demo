using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public CharacterStats characterStats;

    [SerializeField] protected AnimationCurve jumpCurve;
    [SerializeField] protected Character character;
    [SerializeField, Range(0.05f, 1.5f)] protected float delayBetweenSteps;
    [SerializeField] protected float stepDuration = 0.65f;
    [SerializeField] protected LayerMask tileMask;
    [SerializeField] protected float rotationLerpRate;
    [SerializeField] protected GameObject teleportationEffect;

    #region Events
    public static event Action CharacterArrived;
    #endregion
    public void StartMove(Path _path)
    {
        Task task = MoveAlongPath(_path);
    }
    public virtual async Task MoveAlongPath(Path path)
    {
        character.Moving = true;
        int currentStep = 0;
        int pathLength = path.tiles.Count - 1;

        while (currentStep < pathLength)
        {
            if (characterStats.ActionPoints == 0) return;

            await Task.Delay((int)(delayBetweenSteps * 1000));
            character.characterTile.OnTileDeoccupiedByCharater();

            //Debug.Log($"Current Tile: {path.tiles[currentStep]}, next tile: {path.tiles[currentStep + 1]}");
            Vector3 startPosition = transform.position;
            Vector3 moveVector = path.tiles[currentStep + 1].transform.position - path.tiles[currentStep].transform.position;
            Vector3 nextTilePosition = path.tiles[currentStep + 1].transform.position;
            Vector3 currentTilePosition = path.tiles[currentStep].transform.position;
            float expiredTime = 0f;
            float progress = 0f;

            MakeCurve(startPosition, nextTilePosition);
            character.animationController.SetTrigger(CharacterAnimParameters.Jump);
            while (progress < 1)
            {
                expiredTime += Time.deltaTime;
                progress = expiredTime / stepDuration;

                Vector3 newPos = (new Vector3(startPosition.x + moveVector.x * progress,
                                           startPosition.y + jumpCurve.Evaluate(progress),
                                           startPosition.z + moveVector.z * progress) - transform.position);
                transform.Translate(newPos);
                RotateWhileMove(currentTilePosition, nextTilePosition);

                await Task.Yield();
            }
            currentStep++;
            characterStats.UseActionPoints(1);
            //characterStats.ActionPoints--;
            character.characterTile = path.GetTile(currentStep);
            character.characterTile.OnTileOccupiedByCharater(character);
            bool needToChangeState = false;
            character.OnStep(ref needToChangeState);
            if (needToChangeState) 
            {
                FinalizePosition(path.tiles[currentStep]);
                CharacterArrived?.Invoke();
                character.Moving = false;
                character.CancelCurentStateTask();
                return;
            }
        }
        FinalizePosition(path.tiles[pathLength]);
        CharacterArrived?.Invoke();
        character.Moving = false;
    }
    public virtual async Task MoveToTile(Tile nextTile, float duration)
    {
        character.Moving = true;


        await Task.Delay((int)(delayBetweenSteps * 1000));
        character.characterTile.OnTileDeoccupiedByCharater();

        Vector3 startPosition = transform.position;
        Vector3 moveVector = nextTile.transform.position - startPosition;
        Vector3 nextTilePosition = nextTile.transform.position;
        float expiredTime = 0f;
        float progress = 0f;

        MakeCurve(startPosition, nextTilePosition);

        while (progress < 1)
        {
            expiredTime += Time.deltaTime;
            progress = expiredTime / duration;

            Vector3 newPos = (new Vector3(startPosition.x + moveVector.x * progress,
                                       startPosition.y + jumpCurve.Evaluate(progress),
                                       startPosition.z + moveVector.z * progress) - transform.position);
            transform.Translate(newPos);

            await Task.Yield();
        }
        character.characterTile = nextTile;
        character.characterTile.OnTileOccupiedByCharater(character);
        bool needToChangeState = false;
        character.OnStep(ref needToChangeState);
        FinalizePosition(nextTile);
        CharacterArrived?.Invoke();
        character.Moving = false;
    }
    public void FindTileAtStart()
    {
        if (character.characterTile != null)
        {
            character.characterTile.OnTileOccupiedByCharater(character);
            FinalizePosition(character.characterTile);
            return;
        }
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 50f, tileMask))
        {
            Tile tile = hit.transform.GetComponent<Tile>();
            tile.OnTileOccupiedByCharater(character);
            FinalizePosition(tile);
            return;
        }

        Debug.Log("Unable to find a start position");
    }
    public async Task Teleport(Tile destination)
    {
        Instantiate(teleportationEffect, character.characterCenter.position, Quaternion.identity);
        character.HideModel();
        await Task.Delay(200);
        character.characterTile.OnTileDeoccupiedByCharater();
        await Task.Delay(1100);
        destination.OnTileOccupiedByCharater(character);
        transform.position = destination.transform.position;
        FinalizePosition(destination);
        Instantiate(teleportationEffect, character.characterCenter.position, Quaternion.identity);
        character.ShowModel();
    }
    public void FinalizePosition(Tile tile)
    {
/*        if (character.characterTile != null)
        {
            character.characterTile.Occupied = false;
        }*/
        Vector3 rayPos = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        Debug.DrawRay(rayPos, -Vector3.up * 200f, Color.green, 10f);
        Physics.Raycast(rayPos, -Vector3.up, out RaycastHit hit, 200f, tileMask);
        if (hit.collider != null)
        {
            transform.position = new Vector3(tile.transform.position.x, hit.point.y, tile.transform.position.z);
        }
        character.characterTile = tile;
        character.characterTile.Occupied = true;
        tile.OnTileOccupiedByCharater(character);
    }
    protected void RotateWhileMove(Vector3 origin, Vector3 destination)
    {
        Quaternion targetRotation = Quaternion.LookRotation(origin.DirectionTo(destination).Flat(), Vector3.up);
        float t = Mathf.Clamp(Time.deltaTime * rotationLerpRate, 0f, 0.99f);
        character.characterVisual.rotation = Quaternion.Lerp(character.characterVisual.rotation, targetRotation, t);
        character.characterVisual.rotation = Quaternion.Euler(0, character.characterVisual.localEulerAngles.y, 0f);
    }
    protected void MakeCurve(Vector3 startPosition, Vector3 nextTilePosition)
    {
        float tungent = 1.8f;
        float jumpOffset = 0.35f;
        float topKeyPosition;
        float firstKeyPosition = 0f;
        float lastKeyPosition = nextTilePosition.y - startPosition.y;
        if (startPosition.y > nextTilePosition.y)
            topKeyPosition = firstKeyPosition + jumpOffset;
        else
            topKeyPosition = lastKeyPosition + jumpOffset;

        jumpCurve.ClearKeys();
        jumpCurve.AddKey(0f, 0);
        jumpCurve.AddKey(0.5f, 0.5f);
        jumpCurve.AddKey(1f, 0);

        Keyframe keyframe = jumpCurve.keys[0];
        keyframe.outTangent = tungent;
        keyframe.value = firstKeyPosition;
        keyframe.time = 0f;
        jumpCurve.MoveKey(0, keyframe);

        keyframe = jumpCurve.keys[1];
        keyframe.outTangent = 0;
        keyframe.inTangent = 0;
        keyframe.value = topKeyPosition;
        keyframe.time = 0.5f;
        jumpCurve.MoveKey(1, keyframe);

        keyframe = jumpCurve.keys[2];
        keyframe.inTangent = -tungent;
        keyframe.value = lastKeyPosition;
        keyframe.time = 1f;
        jumpCurve.MoveKey(2, keyframe);
    }
    protected void InvokeCharacterArrived()
    {
        CharacterArrived?.Invoke();
    }

}
