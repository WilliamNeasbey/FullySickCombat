using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// goal, most re-used methods so dont need to re-write things
/// </summary>
namespace GWLPXL.ActionCharacter
{

    /// <summary>
    /// wrapper class for commonly used movement functions 
    /// </summary>
    public static class MovementHelpers
    {

      
        public static float GetNewRandomDuration(float min, float max)
        {
            float newtimetomove = UnityEngine.Random.Range(min, max);
            return newtimetomove;
        }
        public static Vector3 GetNewRandomTetheredDestination(Vector3 tetherpoint, float radius)
        {
            Vector3 newPoint = tetherpoint + (UnityEngine.Random.insideUnitSphere * radius);
            newPoint.y = tetherpoint.y;
            return newPoint;
        }
        public static Quaternion GetLookRotation(Transform transform, Vector3 input, float speed, float dt, RotateSmoothType type)
        {
            Quaternion current = transform.rotation;
            Quaternion rot = Quaternion.LookRotation(input, Vector3.up);
            Quaternion rotatet = Quaternion.RotateTowards(current, rot, speed);
            Quaternion slerped = Quaternion.Slerp(current, rotatet, dt * speed);//gives more control
            Quaternion applied = Quaternion.identity;

            switch (type)
            {
                case RotateSmoothType.Instant:
                    applied = rotatet;
                    break;
                case RotateSmoothType.Slerp:
                    applied = slerped;
                    break;
            }

            return applied;
        }
       
        public static Vector3 TranslateToCamera(Vector3 newMove)
        {
            Vector3 translatedInput;

            Camera camera = Camera.main;
            //we take into account the camera's rotation
            Vector3 movedirH = camera.transform.right * newMove.x;
            movedirH.y = 0;//we 0 out y otherwise we'll face the camera like a selfie
            Vector3 movedirV = camera.transform.forward * newMove.z;
            movedirV.y = 0;
            translatedInput = movedirH + movedirV;
            return translatedInput;
        }
        public static Vector3 TranslateToLocal(Transform local, Vector3 newMove)
        {
            Vector3 translatedInput = local.TransformDirection(newMove);
            return translatedInput;
        }

    }


   
    public enum InputReference
    {
        World = 0,
        Camera = 1,
        Local = 2
    }
    public enum RotateSmoothType
    {
        Instant = 0,
        Slerp = 1
    }
   
   
   
    public enum PhysicsType
    {
        Physics3D = 0,
        Physics2D = 1
    }

   

 

   



}