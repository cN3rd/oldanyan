using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;

[Obsolete("Obsolete")]
public class csDemoSceneCode : MonoBehaviour
{
    public string[] EffectNames;
    public string[] Effect2Names;
    public Transform[] Effect;
    public Text Text1;

    private int i = 0;
    private int a = 0;

    // Reference to Input Actions asset
    private InputAction previousEffectAction;
    private InputAction nextEffectAction;
    private InputAction spawnCurrentEffectAction;

    private void Awake()
    {
        // Create input actions
        previousEffectAction = new InputAction("PreviousEffect", binding: "<Keyboard>/z");
        nextEffectAction = new InputAction("NextEffect", binding: "<Keyboard>/x");
        spawnCurrentEffectAction = new InputAction("SpawnCurrentEffect", binding: "<Keyboard>/c");

        // Register callbacks
        previousEffectAction.performed += ctx => CycleToPreviousEffect();
        nextEffectAction.performed += ctx => CycleToNextEffect();
        spawnCurrentEffectAction.performed += ctx => SpawnCurrentEffect();

        // Enable the actions
        previousEffectAction.Enable();
        nextEffectAction.Enable();
        spawnCurrentEffectAction.Enable();
    }

    private void OnDisable()
    {
        // Disable and dispose actions when the component is disabled
        previousEffectAction.Disable();
        nextEffectAction.Disable();
        spawnCurrentEffectAction.Disable();

        previousEffectAction.Dispose();
        nextEffectAction.Dispose();
        spawnCurrentEffectAction.Dispose();
    }

    void Start()
    {
        Instantiate(Effect[i], new Vector3(0, 5, 0), Quaternion.identity);
        UpdateUI();
    }

    void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        Text1.text = (i + 1) + ":" + EffectNames[i];
    }

    private void CycleToPreviousEffect()
    {
        if (i <= 0)
            i = 99;
        else
            i--;

        SpawnCurrentEffect();
    }

    private void CycleToNextEffect()
    {
        if (i < 99)
            i++;
        else
            i = 0;

        SpawnCurrentEffect();
    }

    private void SpawnCurrentEffect()
    {
        bool isEffect2 = false;

        for (a = 0; a < Effect2Names.Length; a++)
        {
            if (EffectNames[i] == Effect2Names[a])
            {
                Instantiate(Effect[i], new Vector3(0, 0.2f, 0), Quaternion.identity);
                isEffect2 = true;
                break;
            }
        }

        if (!isEffect2)
            Instantiate(Effect[i], new Vector3(0, 5, 0), Quaternion.identity);
    }
}
