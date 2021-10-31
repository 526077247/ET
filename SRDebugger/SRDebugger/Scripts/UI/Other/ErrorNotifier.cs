using UnityEngine;

namespace SRDebugger.UI.Other
{
    public class ErrorNotifier : MonoBehaviour
    {
        public bool IsVisible
        {
            get { return enabled; }
        }

        private const float DisplayTime = 6;

        [SerializeField]
        private Animator _animator = null;

        private int _triggerHash;

        private float _hideTime;

        void Awake()
        {
            _triggerHash = Animator.StringToHash("Display");
            enabled = false;
        }

        public void ShowErrorWarning()
        {
            _hideTime = Time.realtimeSinceStartup + DisplayTime;

            if (!enabled)
            {
                enabled = true;
                _animator.SetBool(_triggerHash, true);
            }
        }

        void Update()
        {
            if (Time.realtimeSinceStartup > _hideTime)
            {
                enabled = false;
                _animator.SetBool(_triggerHash, false);
            }
        }
    }
}