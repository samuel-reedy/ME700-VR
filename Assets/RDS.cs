using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RDS", menuName = "ScriptableObjects/RDS", order = 1)]
public class RDS : ScriptableObject
{
    [SerializeField] Texture _texA;
    [SerializeField] Texture _texB;

    [SerializeField] int _percentage_shift;
    [SerializeField] bool _left_shifted;

    public Texture textA
    {
        get { return _texA; }
        set { _texA = value; }
    }

    public Texture textB
    {
        get { return _texB; }
        set { _texB = value; }
    }

    public bool left_shifted
    {
        get { return _left_shifted; }
        set { _left_shifted = value; }
    }

    public int percentage_shift
    {
        get { return _percentage_shift; }
        set { _percentage_shift = value; }
    }
}
