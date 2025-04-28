using UnityEngine;

namespace TransitionScreenPackage
{
   public class TransitionScreenManager : MonoBehaviour
   {
      [SerializeField] private Animator _animator;
      
      public delegate void FinishedReveal();
      public FinishedReveal FinishedRevealEvent;
      
      public delegate void FinishedHide();
      public FinishedHide FinishedHideEvent;

      public void Reveal()
      {
         _animator.SetTrigger("Reveal");
      }

      public void Hide()
      {
         _animator.SetTrigger("Hide");
      }

      public void OnFinishedHideAnimation()
      {
         // Subscribe to this event, if you'd like to know when it gets hidden
         FinishedHideEvent?.Invoke();
      }
      
      public void OnFinishedRevealAnimation()
      {
         // Subscribe to this event, if you'd like to know when it's revealed
         FinishedRevealEvent?.Invoke();
      }
   }
}
