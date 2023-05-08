using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Score", menuName = "ScriptableObjects/Score", order = 2)]
public class SCORE : ScriptableObject
{
    [SerializeField] int _percentage_shift;
    [SerializeField] int _count;
    [SerializeField] int _correct;

    public int percentage_shift
    {
        get { return _percentage_shift; }
        set { _percentage_shift = value; }
    }

    public int count
    {
        get { return _count; }
        set { _count = value; }
    }

    public int correct
    {
        get { return _correct; }
        set { _correct = value; }
    }

    public float getScore()
    {
        return _correct / (float)_count;
    }

   
}
