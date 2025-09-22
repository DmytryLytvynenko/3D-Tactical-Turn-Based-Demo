using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillParameters
{
    public List<Character> Targets = new List<Character>();
    public Transform shootPoint;
    public Transform VFXPos;

    public SkillParameters(List<Character> targets, Transform shootPoint = null, Transform VFXPos = null) 
    {
        Targets = targets;
        this.shootPoint = shootPoint;
        this.VFXPos = VFXPos;
    }
    public SkillParameters(Character target, Transform shootPoint = null, Transform VFXPos = null)
    {
        Targets.Add(target);
        this.shootPoint = shootPoint;
        this.VFXPos = VFXPos;
    }
    public SkillParameters()
    {

    }
}
