using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DifficultySelect : ScriptableObject
{
    public Ghost difficultyGhost;
    public List<Ghost> ghosts;

    [HideInInspector] public int ghostNumber;

    void Start()
    {
        difficultyGhost = ghosts[1];
    }

    public void SetDifficulty(int difficultyNum)
    {
        difficultyGhost = ghosts[difficultyNum];

        ghostNumber = difficultyNum;
    }
}
