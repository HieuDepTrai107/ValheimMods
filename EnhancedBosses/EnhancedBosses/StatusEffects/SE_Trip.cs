using Jotunn.Utils;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.PostProcessing;
using Log = Jotunn.Logger;

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
            name = "EB_Trip";
            m_name = "Hallucinations";
            m_ttl = baseTTL;
            m_icon = AssetUtils.LoadSpriteFromFile("EnhancedBosses/Assets/trip.png");
        }

        public override void Setup(Character character)
        {
            component = GameCamera.instance.gameObject.GetComponent<PostProcessingBehaviour>();
            component.m_Vignette.model.enabled = true;
            component.m_ColorGrading.model.isDirty = true;
            base.Setup(character);
        }

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

            Helpers.Lerp(ref component.m_Vignette.model.m_Settings.intensity, NewVignetteIntensity, DeltaVignetteIntensity);
            Helpers.Lerp(ref component.m_ChromaticAberration.model.m_Settings.intensity, NewChromaticAberrationIntensity, DeltaChromaticAberrationIntensity);
            Helpers.Lerp(ref component.m_ColorGrading.model.m_Settings.basic.saturation, NewColorGradingSaturation, DeltaColorGradingSaturation);

            base.UpdateStatusEffect(dt);
        }

        public override bool CanAdd(Character character)
        {
            return Main.BonemassTripEffect.Value;
        }

        public override void Stop()
        {
            if (Main.BonemassTripEffect.Value)
            {
                RemoveTripEffect();
            }

            base.Stop();
        }

        async public void RemoveTripEffect()
        {          
            var Ticks = 100;

            var OldVignetteIntensity = 0.2f;
            var OldChromaticAberrationIntensity = 20f;
            var OldColorGradingSaturation = 2f;

            var NewVignetteIntensity = 0.45f;
            var NewChromaticAberrationIntensity = 0.15f;
            var NewColorGradingSaturation = 1f;

            var DeltaVignetteIntensity = Mathf.Abs(OldVignetteIntensity - NewVignetteIntensity) / Ticks;
            var DeltaChromaticAberrationIntensity = Mathf.Abs(OldChromaticAberrationIntensity - NewChromaticAberrationIntensity) / Ticks;
            var DeltaColorGradingSaturation = Mathf.Abs(OldColorGradingSaturation - NewColorGradingSaturation) / Ticks;

            float ratio = NewVignetteIntensity / DeltaVignetteIntensity;
            while (ratio != 1)
            {
                ratio = Helpers.Lerp(ref component.m_Vignette.model.m_Settings.intensity, NewVignetteIntensity, DeltaVignetteIntensity);
                Helpers.Lerp(ref component.m_ChromaticAberration.model.m_Settings.intensity, NewChromaticAberrationIntensity, DeltaChromaticAberrationIntensity);
                Helpers.Lerp(ref component.m_ColorGrading.model.m_Settings.basic.saturation, NewColorGradingSaturation, DeltaColorGradingSaturation);

                component.m_Vignette.model.m_Settings.color.r *= 1 - ratio;
                component.m_Vignette.model.m_Settings.color.g *= 1 - ratio;
                component.m_Vignette.model.m_Settings.color.b *= 1 - ratio;

                await Task.Delay(10);
            }

            if (ratio == 1)
            {
                component.m_Vignette.model.enabled = false;
                component.m_ColorGrading.model.isDirty = false;
            }
        }
    }
}
