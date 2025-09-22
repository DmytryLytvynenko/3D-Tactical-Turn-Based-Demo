using System;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerMovement : CharacterMovement
{
    public static event Action MadeStep;

    [SerializeField] private Player player;
    public override async Task MoveAlongPath(Path path)
    {
        character.Moving = true;
        int currentStep = 0;
        int pathLength = path.tiles.Count - 1;

        while (currentStep < pathLength)
        {
            if (characterStats.ActionPoints == 0) return;

            character.characterTile.OnTileDeoccupiedByCharater();
            Vector3 startPosition = transform.position;
            Vector3 moveVector = path.tiles[currentStep + 1].transform.position - path.tiles[currentStep].transform.position;
            Vector3 nextTilePosition = path.tiles[currentStep + 1].transform.position;
            Vector3 currentTilePosition = path.tiles[currentStep].transform.position;
            float expiredTime = 0f;
            float progress = 0f;

            if (path.tiles[currentStep + 1].name.Contains("road"))
            {
                nextTilePosition.y += .25f;
            }

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
            character.InvokeActionMade();
            character.characterTile = path.GetTile(currentStep);
            character.characterTile.OnTileOccupiedByCharater(character);
            MadeStep?.Invoke();
            if (currentStep == pathLength)
            {
                FinalizePosition(path.tiles[pathLength]);
            }
            await Task.Delay((int)(delayBetweenSteps * 1000));
        }
        InvokeCharacterArrived();
        character.Moving = false;
    }
}
