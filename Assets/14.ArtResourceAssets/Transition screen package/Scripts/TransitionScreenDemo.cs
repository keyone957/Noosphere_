using System.Collections.Generic;
using TransitionScreenPackage.Data;
using UnityEngine;

namespace TransitionScreenPackage.Demo
{
    public class TransitionScreenDemo : MonoBehaviour
    {
        [SerializeField] private TransitionScreenType _selectedType;
        [SerializeField] private TransitionScreenVersion _selectedVersion;
        
        [Space]
        [Header("DO NOT CHANGE THESE!")]
        [SerializeField] private List<TransitionScreenObject> _transitionScreens;

        private TransitionScreenManager _currentTransitionScreen;

        private void Awake()
        {
            SpawnSelectedTransitionScreen();
        }

        private void SpawnSelectedTransitionScreen()
        {
            foreach (TransitionScreenObject transition in _transitionScreens)
            {
                if (transition.Type.Equals(_selectedType))
                {
                    // Clear previous data (if there was any)
                    if (_currentTransitionScreen != null)
                    {
                        _currentTransitionScreen.FinishedRevealEvent -= OnTransitionScreenRevealed;
                        _currentTransitionScreen.FinishedHideEvent -= OnTransitionScreenHidden;
                        Destroy(_currentTransitionScreen.gameObject);
                    }
                    
                    // Instantiate new transition screen prefab
                    GameObject instantiatedTransitionScreen = Instantiate(transition.GetVersion(_selectedVersion).PrefabObject, transform);
                    
                    // Subscribe to it's events
                    _currentTransitionScreen = instantiatedTransitionScreen.GetComponent<TransitionScreenManager>();
                    _currentTransitionScreen.FinishedRevealEvent += OnTransitionScreenRevealed;
                    _currentTransitionScreen.FinishedHideEvent += OnTransitionScreenHidden;
                    
                    // Reveal it
                    _currentTransitionScreen.Reveal();
                    break;
                }
            }
        }

        private void OnTransitionScreenRevealed()
        {
            // Start hide animation, after it's fully revealed
            _currentTransitionScreen.Hide();
        }
        
        private void OnTransitionScreenHidden()
        {
            // Start reveal animation, after it's fully hidden
            _currentTransitionScreen.Reveal();
        }

        private void OnValidate()
        {
            // Runs when you change the selected type via Inspector
            if (Application.isPlaying)
                SpawnSelectedTransitionScreen();
        }
    }
}

