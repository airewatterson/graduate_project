using System.ComponentModel;
using Cinemachine;
using General;
using UnityEngine;

namespace Player.Input
{
    public class CameraShake : SingletonMonoBehavior<CameraShake>
    {
        [SerializeField] private CinemachineVirtualCamera cam1;

        [SerializeField] private CinemachineVirtualCamera cam2;

        private float _shakeTimer;
        private float _shakeTimerTotal;
        private float _startingIntensity;
        // Start is called before the first frame update
        public override void Awake()
        {
            cam1 = GameObject.FindWithTag("cam1").GetComponent<CinemachineVirtualCamera>();
            cam2 = GameObject.FindWithTag("cam2").GetComponent<CinemachineVirtualCamera>();
        }

        // Update is called once per frame
        void Update()
        {
            if (_shakeTimer>0)
            {
                _shakeTimer -= Time.deltaTime;
                
                    CinemachineBasicMultiChannelPerlin cam1CinemachineBasicMultiChannelPerlin =
                        cam1.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                    CinemachineBasicMultiChannelPerlin cam2CinemachineBasicMultiChannelPerlin =
                        cam2.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

                    cam1CinemachineBasicMultiChannelPerlin.m_AmplitudeGain =
                        Mathf.Lerp(_startingIntensity, 0f, _shakeTimer / _shakeTimerTotal);
                    cam2CinemachineBasicMultiChannelPerlin.m_AmplitudeGain =
                        Mathf.Lerp(_startingIntensity, 0f, _shakeTimer / _shakeTimerTotal);


            }
        }

        public void ShakeCamera(float intensity, float time)
        {
            CinemachineBasicMultiChannelPerlin cam1CinemachineBasicMultiChannelPerlin =
                cam1.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            CinemachineBasicMultiChannelPerlin cam2CinemachineBasicMultiChannelPerlin =
                cam2.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            cam1CinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
            cam2CinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
            
            
            _shakeTimer = time;
            _shakeTimerTotal = time;
            _startingIntensity = intensity;
        }
    }
}
