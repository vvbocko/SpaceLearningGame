using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WearAstronautSuit : MonoBehaviour
{
    [SerializeField] private PickUpController pickupController;
    [SerializeField] private Transform maleHoldPoint;
    [SerializeField] private Transform astronautHoldPoint;

    [SerializeField] private CameraRotation playerCamera;
    [SerializeField] private GameObject suitModel;
    [SerializeField] private GameObject maleModel;
    [SerializeField] private GameObject astronautModel;
    [SerializeField] private Material invisibleMaterial;

    [SerializeField] private Animator maleAnimator;
    [SerializeField] private Animator astronautAnimator;

    private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();
    private bool isWearingSuit = false;
    public Animator CurrentAnimator { get; private set; }
    void Start()
    {
        CurrentAnimator = maleAnimator;

        suitModel.SetActive(true);
        maleModel.SetActive(true);
        astronautModel.SetActive(false);

        MakeSuitInvisible(suitModel, false);

        pickupController.SetHoldPoint(maleHoldPoint);
    }
    public void WearSuit()
    {
        isWearingSuit = !isWearingSuit;

        if (isWearingSuit)
        {
            playerCamera.maxRotationX = 65f;
            
            MakeSuitInvisible(suitModel, true);
            maleModel.SetActive(false);
            astronautModel.SetActive(true);

            pickupController.SetHoldPoint(astronautHoldPoint);

            CurrentAnimator = astronautAnimator;
            pickupController.SetAnimator(CurrentAnimator);

        }
        else
        {
            playerCamera.maxRotationX = 80f;

            MakeSuitInvisible(suitModel, false);
            maleModel.SetActive(true);
            astronautModel.SetActive(false);

            pickupController.SetHoldPoint(maleHoldPoint);   

            CurrentAnimator = maleAnimator;
            pickupController.SetAnimator(CurrentAnimator);
        }
    }

    private void MakeSuitInvisible(GameObject parent, bool invisible)
    {
        Renderer[] renderers = parent.GetComponentsInChildren<Renderer>(true);

        foreach (Renderer rend in renderers)
        {
            if (invisible)
            {
                // Save original materials if not already saved
                if (!originalMaterials.ContainsKey(rend))
                    originalMaterials[rend] = rend.sharedMaterials;

                // Apply invisible material to all slots
                Material[] newMats = new Material[rend.sharedMaterials.Length];
                for (int i = 0; i < newMats.Length; i++)
                    newMats[i] = invisibleMaterial;

                rend.materials = newMats;
            }
            else
            {
                // Restore original materials
                if (originalMaterials.ContainsKey(rend))
                    rend.materials = originalMaterials[rend];
            }
        }
    }
}
