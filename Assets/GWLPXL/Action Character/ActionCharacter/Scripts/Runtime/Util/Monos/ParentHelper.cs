using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace GWLPXL.ActionCharacter
{
    public interface IParentPoint
    {
        Transform Transform { get; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class ParentHelper : MonoBehaviour, IParentPoint
    {
        public GameObject RigRoot;


        public ParentConstraint ParentConstraint;

        public Transform Transform => this.transform;
    }
}
