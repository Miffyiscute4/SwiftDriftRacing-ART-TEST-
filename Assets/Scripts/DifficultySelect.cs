using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DifficultySelect : ScriptableObject
{
    public Ghost difficultyGhost;
    public List<Ghost> ghosts;

    //[HideInInspector] public int ghostNumber;

    public void SetDifficulty(int difficultyNum)
    {
        difficultyGhost = ghosts[difficultyNum];

        //ghostNumber = difficultyNum;
    }
}
