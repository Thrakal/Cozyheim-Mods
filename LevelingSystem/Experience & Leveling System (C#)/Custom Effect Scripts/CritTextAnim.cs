using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Cozyheim.LevelingSystem
{
    public class CritTextAnim : MonoBehaviour
    {
        private AnimationCurve animCurve;
        private float fullAnimTime = 3f;
        private float maxCritSize = 1f;
        private float damageSizeScale = 1f;
        private float spread = 0.5f;

        void Update()
        {
            transform.eulerAngles = Camera.main.transform.eulerAngles;
        }

        IEnumerator Start()
        {
            Vector3 spawnSpread = Vector3.zero;
            spawnSpread += Random.Range(-spread, spread) * Camera.main.transform.right; // Randomize x
            spawnSpread += Random.Range(0f, spread) * Camera.main.transform.up; // Randomize y

            transform.position += spawnSpread;


            Keyframe[] keyFrames = new Keyframe[]
            {
            new Keyframe(0f, 0f, 0f, 1f),
            new Keyframe(0.08f, 1.2f, 0f, 2f),
            new Keyframe(0.15f, 1f, -2f, 0f),
            new Keyframe(0.75f, 1f, 0f, -0.5f),
            new Keyframe(1f, 0f, 0f, 0f)
            };
            animCurve = new AnimationCurve(keyFrames);

            Vector3 startSize = transform.localScale;

            for (float f = 0f; f < fullAnimTime; f += Time.deltaTime)
            {
                // Movement
                float perc = f / fullAnimTime;
                transform.localScale = startSize * damageSizeScale * animCurve.Evaluate(perc);

                yield return null;
            }

            Destroy(gameObject);
        }

        private void SetColorAndScale(float damage)
        {
            Text textComp = GetComponentInChildren<Text>();
            Color color = new Color(0.8f, 0.6f, 0.15f, 1f);

            // Fixed intervals of scale and color
            if(damage < 30f)
            {
                damageSizeScale = 1f;
                color.g = 0.6f;
            } else if(damage < 100f)
            {
                damageSizeScale = 1.2f;
                color.g = 0.45f;
            } else if (damage < 200f)
            {
                damageSizeScale = 1.4f;
                color.g = 0.3f;
            } else if (damage < 300f)
            {
                damageSizeScale = 1.6f;
                color.g = 0.15f;
            } else
            {
                damageSizeScale = 1.8f;
                color.g = 0f;
            }

            textComp.color = color;

            // Gradient scale and color
            // ------------------------
            /*
            float value = Mathf.InverseLerp(30f, 300f, damage);
            color.g = (1 - value) * 0.6f;

            textComp.color = color;
            damageSizeScale = 1 + (value * maxCritSize);
            */
        }

            public void SetText(string value)
        {
            GetComponentInChildren<Text>().text = value.ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US"));
        }

        public void SetText(float value, int decimals = 0)
        {
            decimals = Mathf.Max(0, decimals);
            string decimalsFormat = "N" + decimals.ToString();
            SetText(value.ToString(decimalsFormat));
            SetColorAndScale(value);
        }

        public void SetText(int value)
        {
            SetText(value.ToString());
        }
    }
}