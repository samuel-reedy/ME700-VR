using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RDSObjectGenerator : MonoBehaviour
{
    [SerializeField] private List<Texture> textures;

    public List<RDS> RDS_List;


    public void GenerateList()
    {
        for (int i = 0; i < (textures.Count); i=i+2)
        {
            string name_A = textures[i].name;
            string[] name_A_split = name_A.Split('_');

            RDS rds = ScriptableObject.CreateInstance<RDS>();
            rds.left_shifted = name_A_split[2] == "L" ? true: false;
            int percent;
            int.TryParse(name_A_split[1], out percent);
            rds.percentage_shift = percent;
            rds.textA = textures[i];
            rds.textB = textures[i + 1];

            RDS_List.Add(rds);


        }
    }

}
