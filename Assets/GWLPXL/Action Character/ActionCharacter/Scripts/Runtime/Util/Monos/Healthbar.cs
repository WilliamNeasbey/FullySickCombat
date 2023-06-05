using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GWLPXL.ActionCharacter
{


    public class Healthbar : MonoBehaviour
    {
        public ActionCharacter Character;
        MaterialPropertyBlock matBlock;
        MeshRenderer meshRenderer;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            matBlock = new MaterialPropertyBlock();
            if (Character == null)
            {
                Character = transform.root.gameObject.GetComponent<ActionCharacter>();
            }



        }


        private void Start()
        {
            if (Character != null && Character.DamageController != null)
            {
                Character.DamageController.OnDamaged += UpdateParams;
                Character.DamageController.OnDied += HideBar;
                Character.Events.OnSpawned += ShowBar;
            }
            else
            {
                Debug.Log("Using Health Bar but no Damage Controller set on action character. Can't use healthbar", this.gameObject);
            }


        }

        private void OnDestroy()
        {
            if (Character != null && Character.DamageController != null)
            {
                Character.DamageController.OnDamaged -= UpdateParams;
                Character.DamageController.OnDied -= HideBar;
                Character.Events.OnSpawned -= ShowBar;
            }

        }

        void ShowBar(CharacterArgs args)
        {
            gameObject.SetActive(true);
        }
        void HideBar(ActionCharacter ac)
        {
            gameObject.SetActive(false);
        }
        void UpdateParams(float percent)
        {

            meshRenderer.GetPropertyBlock(matBlock);
            matBlock.SetFloat("_Fill", percent);
            meshRenderer.SetPropertyBlock(matBlock);
        }
    }
}