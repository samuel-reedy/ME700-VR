using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Score", menuName = "ScriptableObjects/Score", order = 2)]
public class SCORE : ScriptableObject
{
    [SerializeField] int _percentage_shift;
    [SerializeField] int _count_left;
    [SerializeField] int _count_right;
    [SerializeField] int _correct_left;
    [SerializeField] int _correct_right;

    public int percentage_shift
    {
        get { return _percentage_shift; }
        set { _percentage_shift = value; }
    }

    public int count_left
    {
        get { return _count_left; }
        set { _count_left = value; }
    }

    public int count_right
    {
        get { return _count_right; }
        set { _count_right = value; }
    }

    public int correct_left
    {
        get { return _correct_left; }
        set { _correct_left = value; }
    }

    public int correct_right
    {
        get { return _correct_right; }
        set { _correct_right = value; }
    }

    public float getScoreLeft()
    {
        return _correct_left / (float)_count_left;
    }

    public float getScoreRight()
    {
        return _correct_right / (float)_count_right;
    }

    

    public float getScoreTotal()
    {
        return (_correct_left + _correct_right) / (float)(_count_left + _count_right);
    }


}
