using Jotunn.Utils;
using UnityEngine;
using UnityEngine.PostProcessing;

namespace EnhancedBosses.StatusEffects
{
    public class SE_Trip : SE_Stats
    {
        public float baseTTL = 1f;

        public static int rainbowcurrent = 0;

        public static int rainbownext = 1;

        public static float rainbowtimer = 0f;

        public static float rainbowchangeevery = 0.5f;

        public static Color[] rainbowcolors = new Color[]
        {
            new Color(5f, 0f, 0f, 1f),
            new Color(5f, 2f, 0f, 1f),
            new Color(5f, 5f, 0f, 1f),
            new Color(0f, 5f, 0f, 1f),
            new Color(0f, 3f, 2f, 1f),
            new Color(0f, 0f, 5f, 1f),
            new Color(3f, 0f, 5f, 1f),
            new Color(4f, 0f, 2f, 1f)
        };

        public static PostProcessingBehaviour component;

        public static float Ticks = 50f;

        public static float OldVignetteIntensity = 0.45f;
        public static float OldChromaticAberrationIntensity = 0.15f;
        public static float OldColorGradingSaturation = 1f;

        public static float NewVignetteIntensity = 0.2f;
        public static float NewChromaticAberrationIntensity = 20f;
        public static float NewColorGradingSaturation = 2f;

        public static float DeltaVignetteIntensity = Mathf.Abs(OldVignetteIntensity - NewVignetteIntensity) / Ticks;
        public static float DeltaChromaticAberrationIntensity = Mathf.Abs(OldChromaticAberrationIntensity - NewChromaticAberrationIntensity) / Ticks;
        public static float DeltaColorGradingSaturation = Mathf.Abs(OldColorGradingSaturation - NewColorGradingSaturation) / Ticks;


        public SE_Trip()
        {
            m_name = "Галлюцинации";
            name = "EB_Trip";
            m_ttl = baseTTL;
            m_icon = AssetUtils.LoadSpriteFromFile("EnhancedBosses/Assets/trip.png");
            m_flashIcon = true;
        }

        public override void Setup(Character character)
        {
            component = GameCamera.instance.gameObject.GetComponent<PostProcessingBehaviour>();
            component.m_Vignette.model.enabled = true;
            component.m_ColorGrading.model.isDirty = true;
            base.Setup(character);
        }

        /*
        public override bool IsDone()
        {
            if (GetRemaningTime() < 0)Log.LogWarning("SE_Trip OnDestroy");
            return base.IsDone();
        }
        */

        public override void UpdateStatusEffect(float dt)
        {
            rainbowtimer += Time.deltaTime;
            if (rainbowtimer > rainbowchangeevery)
            {
                rainbowcurrent = (rainbowcurrent + 1) % rainbowcolors.Length;
                rainbownext = (rainbowcurrent + 1) % rainbowcolors.Length;
                rainbowtimer = 0f;
            }
            component.m_Vignette.model.m_Settings.color = Color.Lerp(rainbowcolors[rainbowcurrent], rainbowcolors[rainbownext], rainbowtimer / rainbowchangeevery);

            Utils.Lerp(ref component.m_Vignette.model.m_Settings.intensity, NewVignetteIntensity, DeltaVignetteIntensity);
            Utils.Lerp(ref component.m_ChromaticAberration.model.m_Settings.intensity, NewChromaticAberrationIntensity, DeltaChromaticAberrationIntensity);
            Utils.Lerp(ref component.m_ColorGrading.model.m_Settings.basic.saturation, NewColorGradingSaturation, DeltaColorGradingSaturation);

            base.UpdateStatusEffect(dt);
        }
    }
}
