
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using KSP.Localization;

namespace Hire
{
    // http://forum.kerbalspaceprogram.com/index.php?/topic/147576-modders-notes-for-ksp-12/#comment-2754813
    // search for "Mod integration into Stock Settings

    public class HireSettings : GameParameters.CustomParameterNode
    {
        public override string Title { get { return Localizer.Format("#TRPHire_Settings_PanelTitle"); } }
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "TRP Hire"; } }
        public override string DisplaySection { get { return "TRP Hire"; } }
        public override int SectionOrder { get { return 1; } }
        public override bool HasPresets { get { return true; } }

        [GameParameters.CustomParameterUI("#TRPHire_Settings_Default",
            toolTip = "#TRPHire_Settings_DefaultTooltip")]
        public bool DefaultSettings = false;

        [GameParameters.CustomParameterUI("#TRPHire_Settings_StockCenter",
            toolTip = "#TRPHire_Settings_StockCenterTooltip")]
        public bool KStockCost = false;


        [GameParameters.CustomIntParameterUI("#TRPHire_Settings_PandaCenter", minValue = 12500, maxValue = 100000, stepSize = 500,
              toolTip = "#TRPHire_Settings_PandaCenterTooltip")]
        public int const_cost = 25000;


        [GameParameters.CustomParameterUI("#TRPHire_Settings_DisableAdvanced",
            toolTip = "#TRPHire_Settings_DisableAdvancedTooltip")]
        public bool disableAllModifiers = false;

        [GameParameters.CustomFloatParameterUI("#TRPHire_Settings_Quality", minValue = 1f, maxValue = 2f, displayFormat = "N2",
toolTip = "#TRPHire_Settings_QualityTooltip")]
        public double quality_coef = 1.25f;

        [GameParameters.CustomFloatParameterUI("#TRPHire_Settings_LowQuality", minValue = 0.25f, maxValue = 1f, displayFormat = "N2",
            toolTip = "#TRPHire_Settings_LowQualityTooltip")]
        public double low_quality = 0.5f;

        [GameParameters.CustomFloatParameterUI("#TRPHire_Settings_HighQuality", minValue = 1f, maxValue = 4f, displayFormat = "N2",
            toolTip = "#TRPHire_Settings_HighQualityTooltip")]
        public double high_quality = 2f;

        public override void SetDifficultyPreset(GameParameters.Preset preset)
        {
            disableAllModifiers = false;
            const_cost = 25000;
            KStockCost = false;

            low_quality = 0.5f;
            high_quality = 2f;
            DefaultSettings = false;
        }

        public override bool Enabled(MemberInfo member, GameParameters parameters)
        {
            if (KStockCost && member.Name == "const_cost")
                return false;
            if (member.Name == "KStockCost" || member.Name == "const_cost")
                return true;

            return true;
        }

        public override bool Interactible(MemberInfo member, GameParameters parameters)
        {
            if (member.Name == "DefaultSettings" && DefaultSettings)
                SetDifficultyPreset(parameters.preset);

            HireSettings2.disabled = disableAllModifiers;
            HireSettings3.disabled = disableAllModifiers;
            return true;
        }

        public override IList ValidValues(MemberInfo member)
        {
            return null;
        }
    }


    public class HireSettings2 : GameParameters.CustomParameterNode
    {
        public override string Title { get { return Localizer.Format("#TRPHire_Settings_Panel2Title"); } }
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "TRP Hire"; } }
        public override string DisplaySection { get { return "TRP Hire"; } }
        public override int SectionOrder { get { return 2; } }
        public override bool HasPresets { get { return true; } }

        internal static bool disabled = false;

        [GameParameters.CustomParameterUI("#TRPHire_Settings_Default",
            toolTip = "#TRPHire_Settings_DefaultTooltip")]
        public bool DefaultSettings = false;

        [GameParameters.CustomFloatParameterUI("#TRPHire_Settings_Gender", minValue = 1f, maxValue = 2f, displayFormat = "N2",
            toolTip = "#TRPHire_Settings_GenderTooltip")]
        public double gender_coef = 1.25f;

        [GameParameters.CustomFloatParameterUI("#TRPHire_Settings_Fearless", minValue = 1.0f, maxValue = 4f, displayFormat = "N2",
            toolTip = "#TRPHire_Settings_FearlessTooltip")]
        public double fearless_coef = 2f;

        [GameParameters.CustomFloatParameterUI("#TRPHire_Settings_Veteran", minValue = 5.0f, maxValue = 15f, displayFormat = "N1",
    toolTip = "#TRPHire_Settings_VeteranTooltip")]
        public double veteran_coef = 10f;

        [GameParameters.CustomFloatParameterUI("#TRPHire_Settings_LevelUp", minValue = 0.0f, maxValue = 3f, displayFormat = "N2",
            toolTip = "#TRPHire_Settings_LevelUpTooltip")]
        public double levelup_coef = 1f;


        public override void SetDifficultyPreset(GameParameters.Preset preset)
        {
            DefaultSettings = false;
            fearless_coef = 2f;
            veteran_coef = 10f;
            gender_coef = 1.25f;
            levelup_coef = 1f;
        }

        public override bool Enabled(MemberInfo member, GameParameters parameters)
        {
            if (disabled)
                return false;

            return true;
        }

        public override bool Interactible(MemberInfo member, GameParameters parameters)
        {
            if (member.Name == "DefaultSettings" && DefaultSettings)
                SetDifficultyPreset(parameters.preset);
            return true;
        }

        public override IList ValidValues(MemberInfo member)
        {
            return null;
        }
    }


    public class HireSettings3 : GameParameters.CustomParameterNode
    {
        public override string Title { get { return Localizer.Format("#TRPHire_Settings_Panel3Title"); } }
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "TRP Hire"; } }
        public override string DisplaySection { get { return "TRP Hire"; } }
        public override int SectionOrder { get { return 3; } }
        public override bool HasPresets { get { return true; } }

        internal static bool disabled = false;

        [GameParameters.CustomParameterUI("#TRPHire_Settings_Default",
                    toolTip = "#TRPHire_Settings_DefaultTooltip")]
        public bool DefaultSettings = false;

        [GameParameters.CustomFloatParameterUI("#TRPHire_Settings_Bulk5", minValue = 0f, maxValue = 40f,
            toolTip = "#TRPHire_Settings_Bulk5Tooltip")]
        public double bulk_discount1 = 15f;

        [GameParameters.CustomFloatParameterUI("#TRPHire_Settings_Bulk10", minValue = 0.0f, maxValue = 40f,
             toolTip = "#TRPHire_Settings_Bulk10Tooltip")]
        public double bulk_discount2 = 30f;

        [GameParameters.CustomFloatParameterUI("#TRPHire_Settings_Black", minValue = 0f, maxValue = 40f,
            toolTip = "#TRPHire_Settings_BlackTooltip")]
        public double black_discount = 15f;

        [GameParameters.CustomFloatParameterUI("#TRPHire_Settings_NewYear", minValue = 0f, maxValue = 80f,
            toolTip = "#TRPHire_Settings_NewYearTooltip")]
        public double new_year_discount = 40f;

        [GameParameters.CustomFloatParameterUI("#TRPHire_Settings_Maximum", minValue = 25f, maxValue = 90f,
            toolTip = "#TRPHire_Settings_MaximumTooltip")]
        public double max_discount = 90f;


        public override void SetDifficultyPreset(GameParameters.Preset preset)
        {
            DefaultSettings = false;

            bulk_discount1 = 15f;
            bulk_discount2 = 30f;
            black_discount = 10f;
            new_year_discount = 50f;
            max_discount = 90f;
        }

        public override bool Enabled(MemberInfo member, GameParameters parameters)
        {
            if (disabled)
                return false;

            return true;
        }

        public override bool Interactible(MemberInfo member, GameParameters parameters)
        {
            if (bulk_discount2 < bulk_discount1)
                bulk_discount2 = bulk_discount1;
            if (member.Name == "DefaultSettings" && DefaultSettings)
                SetDifficultyPreset(parameters.preset);
            return true;
        }

        public override IList ValidValues(MemberInfo member)
        {
            return null;
        }
    }
}
