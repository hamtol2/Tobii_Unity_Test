using System.Collections;
using System.Collections.Generic;
using Tobii.Gaming;
using UnityEngine;

namespace RonnieJ
{
    public class GazeAwareTester : MonoBehaviour
    {
        public Color normalColor = Color.white;
        public Color focusedColor = Color.red;

        public GazeAware gazeAware;
        public Renderer targetRenderer;

        private bool hasFocus = false;

        void Awake()
        {
            if (gazeAware == null)
                gazeAware = GetComponent<GazeAware>();

            if (targetRenderer == null)
                targetRenderer = GetComponent<Renderer>();
        }

        void Update()
        {
            UpdateState();
        }

        void UpdateState()
        {
            if (gazeAware.HasGazeFocus != hasFocus)
            {
                hasFocus = gazeAware.HasGazeFocus;
                ChangeColor(hasFocus);
            }
        }

        void ChangeColor(bool state)
        {
            if (state) targetRenderer.material.color = focusedColor;
            else targetRenderer.material.color = normalColor;
        }
    }
}