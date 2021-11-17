using System;
using System.Linq;
using KSP.UI;
using KSP.UI.Screens;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using KSP.Localization;

namespace Hire
{

    /// <summary>
    /// This bit draws the GUI using the legacy GUI. You could easily use the new GUI instead if you prefer
    /// </summary>
    class CustomAstronautComplexUI : MonoBehaviour
    {
        const int MAX_HIRE_COUNT = 20; // Max limit on number of crew hires at one time
        private Rect _areaRect = new Rect(-500f, -500f, 200f, 200f);
        private Vector2 _guiScalar = Vector2.one;
        private Vector2 _guiPivot = Vector2.zero;
        //private GUIStyle _backgroundStyle; // the background of the whole area our GUI will cover
        //private GUIStyle _scrollviewStyle; // style of the whole scrollview
        //private GUIStyle _nameStyle; // style used for kerbal names
        //private GUIStyle _listItemEntryStyle; // style used for background of each kerbal entry
        private float KBulk = 1;
        private int KBulki = 1;
        private int crewWeCanHire = MAX_HIRE_COUNT;
        private static float KStupidity = 50;
        private static float KCourage = 50;
        private static bool KSpecifyQuality = true;
        private static bool KFearless = false;
        private static bool KVeteran = false;

        private static double KDiscount = 0;
        private static bool KDiscountOverFlow = false;
        private static bool KBlackMundayDiscount = false;
        private static bool KNewYearDiscount = false;
        private static bool KBulkDiscount1 = false;
        private static bool KBulkDiscount2 = false;
        private static int KCareer = 0;

        private static int KLevel = 0;
        private float Krep = Reputation.CurrentRep;
        private static string Level = Localizer.Format("#autoLOC_6002246");

        private string[] KLevelStringsZero = new string[1] { Level + " 0" };
        private string[] KLevelStringsOne = new string[2] { Level + " 0", Level + " 1" };
        private string[] KLevelStringsTwo = new string[3] { Level + " 0", Level + " 1", Level + " 2" };
        private string[] KLevelStringsAll = new string[6] { Level + " 0", Level + " 1", Level + " 2", Level + " 3", Level + " 4", Level + " 5" };

        private string[,] KNames = new string[3, MAX_HIRE_COUNT];
        private ProtoCrewMember.Gender[] KNames2Gender = new ProtoCrewMember.Gender[MAX_HIRE_COUNT];

        private static string Male = Localizer.Format("#autoLOC_900434");
        private static string Female = Localizer.Format("#autoLOC_900444");
        private static string Random = Localizer.Format("#autoLOC_900432");

        private static string Courage = Localizer.Format("#autoLOC_900436");
        private static string Stupidity = Localizer.Format("#autoLOC_900438");
        private static string Badass = Localizer.Format("#autoLOC_900440");
        private static string Veteran = Localizer.Format("#autoLOC_900437");

        private static int KGender = 2;  // default Random 
        private GUIContent KMale = new GUIContent(Male, AssetBase.GetTexture("kerbalicon_recruit"), "Male Kerbal");
        private GUIContent KFemale = new GUIContent(Female, AssetBase.GetTexture("kerbalicon_recruit_female"), "Female Kerbal");
        private GUIContent KGRandom = new GUIContent(Random, Localizer.Format("#TRPHire_RandomTooltip"));
        Color basecolor; // = GUI.color;
        private float ACLevel = 0;
        private double KDead;
        private double DCost = 1;
        KerbalRoster roster = HighLogic.CurrentGame.CrewRoster;
        private bool hTest = true;
        private bool hasKredits = true;
        private bool kerExp = HighLogic.CurrentGame.Parameters.CustomParams<GameParameters.AdvancedParams>().KerbalExperienceEnabled(HighLogic.CurrentGame.Mode);
        private Traits traits = null;

        private HireSettings settings1 = HighLogic.CurrentGame.Parameters.CustomParams<HireSettings>();
        private HireSettings2 settings2 = HighLogic.CurrentGame.Parameters.CustomParams<HireSettings2>();
        private HireSettings3 settings3 = HighLogic.CurrentGame.Parameters.CustomParams<HireSettings3>();

        void Awake()
        {
            basecolor = GUI.color;
        }

        public void Initialize(Rect guiRect)
        {
            // the supplied rect will have the UI scalar already factored in
            var correctedRect = new Rect(guiRect.x, guiRect.y, guiRect.width, guiRect.height);

            _areaRect = correctedRect;

            _guiPivot = new Vector2(_areaRect.x, _areaRect.y);

            //var applicants = HighLogic.CurrentGame.CrewRoster.Applicants.ToList();
            var rand = new System.Random();
            for (int i = 0; i < MAX_HIRE_COUNT; i++)
            {
                KNames[0, i] = CrewGenerator.GetRandomName(ProtoCrewMember.Gender.Male, rand);
                KNames[1, i] = CrewGenerator.GetRandomName(ProtoCrewMember.Gender.Female, rand);

                KNames2Gender[i] = (rand.Next() % 2 == 0 ? ProtoCrewMember.Gender.Male : ProtoCrewMember.Gender.Female);
                KNames[2, i] = (KNames2Gender[i] == ProtoCrewMember.Gender.Male ? KNames[0, i] : KNames[1, i]);
            }

            enabled = true;

            traits = new Traits();

        }

        private void kHire()
        {
            System.Random rand = new System.Random();

            if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER)
            {
                double myFunds = Funding.Instance.Funds;
                Funding.Instance.AddFunds(-costMath(), TransactionReasons.CrewRecruited);
                Hire.Log.Info("KSI :: Total Funds removed " + costMath());
            }

            for (int i = 0; i < KBulki; i++)
            {
                ProtoCrewMember newKerb = HighLogic.CurrentGame.CrewRoster.GetNewKerbal(ProtoCrewMember.KerbalType.Crew);

                newKerb.ChangeName(KNames[KGender, i]);

                switch (KGender) // Sets gender
                {
                    case 0: newKerb.gender = ProtoCrewMember.Gender.Male; break;
                    case 1: newKerb.gender = ProtoCrewMember.Gender.Female; break;
                    case 2: newKerb.gender = KNames2Gender[i]; break;
                    default: break;
                }

                string career = traits.traitTitles[KCareer].name;
                // Sets the kerbal's career based on the KCareer switch.
                KerbalRoster.SetExperienceTrait(newKerb, career);

                // Hire.Log.Info("KSI :: KIA MIA Stat is: " + KDead);
                // Hire.Log.Info("KSI :: " + newKerb.experienceTrait.Config.Name + " " + newKerb.name + " has been created in: " + loopcount.ToString() + " loops.");
                newKerb.rosterStatus = ProtoCrewMember.RosterStatus.Available;
                newKerb.experience = 0;
                newKerb.experienceLevel = 0;
                if (KBulki > 1) // Bulk hires get random stats
                {
                    // The equation gives 60% of results within +/-10% of GUI setting
                    if (KSpecifyQuality)
                    {
                        newKerb.courage = (float)Math.Min(1, Math.Max(0, (Math.Pow(2 * rand.NextDouble() - 1, 3) / 2) + KCourage / 100));
                        newKerb.stupidity = (float)Math.Min(1, Math.Max(0, (Math.Pow(2 * rand.NextDouble() - 1, 3) / 2) + KStupidity / 100));
                    }
                    else
                    {
                        newKerb.courage = (float)rand.NextDouble();
                        newKerb.stupidity = (float)rand.NextDouble();
                    }

                    // 5% chance of Badass
                    newKerb.isBadass = rand.NextDouble() > .95;
                    // No chance of vets in bulk hires.
                    newKerb.veteran = false;
                }
                else // use GUI values
                {
                    newKerb.courage = KCourage / 100;
                    newKerb.stupidity = KStupidity / 100;
                    if (KFearless)
                    {
                        newKerb.isBadass = true;
                    }

                    if (KVeteran)
                    {
                        newKerb.veteran = true;
                    }
                }

                // Hire.Log.Info("PSH :: Status set to Available, courage and stupidity set, fearless trait set.");

                if (kerExp == false)
                {
                    newKerb.experience = 9999;
                    newKerb.experienceLevel = 5;
                    Hire.Log.Info("KSI :: Level set to 5 - Kerbal Experince disabled.");
                }
                else switch (KLevel)
                    {
                        case 1:
                            newKerb.flightLog.AddEntry("Training1," + FlightGlobals.GetHomeBodyName());
                            newKerb.ArchiveFlightLog();
                            newKerb.experience = 2;
                            newKerb.experienceLevel = 1;
                            // Hire.Log.Info("KSI :: Level set to 1.");
                            break;
                        case 2:
                            newKerb.flightLog.AddEntry("Training2," + FlightGlobals.GetHomeBodyName());
                            newKerb.ArchiveFlightLog();
                            newKerb.experience = 8;
                            newKerb.experienceLevel = 2;
                            // Hire.Log.Info("KSI :: Level set to 2.");
                            break;
                        case 3:
                            newKerb.flightLog.AddEntry("Training3," + FlightGlobals.GetHomeBodyName());
                            newKerb.ArchiveFlightLog();
                            newKerb.experience = 16;
                            newKerb.experienceLevel = 3;
                            // Hire.Log.Info("KSI :: Level set to 3.");
                            break;
                        case 4:
                            newKerb.flightLog.AddEntry("Training4," + FlightGlobals.GetHomeBodyName());
                            newKerb.ArchiveFlightLog();
                            newKerb.experience = 32;
                            newKerb.experienceLevel = 4;
                            // Hire.Log.Info("KSI :: Level set to 4.");
                            break;
                        case 5:
                            newKerb.flightLog.AddEntry("Training5," + FlightGlobals.GetHomeBodyName());
                            newKerb.ArchiveFlightLog();
                            newKerb.experience = 64;
                            newKerb.experienceLevel = 5;
                            break;
                    }
                GameEvents.onKerbalAdded.Fire(newKerb); // old gameevent most likely to be used by other mods
                GameEvents.onKerbalAddComplete.Fire(newKerb); // new gameevent that seems relevant
            }
            // Refreshes the AC so that new kerbal shows on the available roster.
            Hire.Log.Info("PSH :: Hiring Function Completed.");


            GameEvents.onGUIAstronautComplexDespawn.Fire();
            GameEvents.onGUIAstronautComplexSpawn.Fire();


        }


        private int costMath()
        {
            dCheck();



            double cost;
            if (settings1.KStockCost)
                cost = GameVariables.Instance.GetRecruitHireCost(HighLogic.CurrentGame.CrewRoster.GetActiveCrewCount());
            else
                cost = (double)settings1.const_cost;

            if (KSpecifyQuality)
            {
                cost *= settings1.quality_coef;

                // Kerbal Quality Cost Modifier
                double low = settings1.low_quality;
                double mid = 1;
                double high = settings1.high_quality;
                double kerbal_quality = (100 - KStupidity + KCourage) / 200;
                double quality_coef;

                if (kerbal_quality < 0.5)
                    quality_coef = 2 * (mid - low) * kerbal_quality + low;
                else
                    quality_coef = 2 * (high - mid) * kerbal_quality - high + 2 * mid;

                cost *= quality_coef;
            }

            KDiscount = 0;
            KDiscountOverFlow = false;
            KBlackMundayDiscount = false;
            KNewYearDiscount = false;
            KBulkDiscount1 = false;
            KBulkDiscount2 = false;


            if (!settings1.disableAllModifiers)
            {

                if (KFearless == true && KBulki <= 1) // disable on bulk hires
                    cost *= settings2.fearless_coef;

                if (KVeteran == true && KBulki <= 1) // disable on bulk hires
                    cost *= settings2.veteran_coef;


                if (KGender != 2)
                    cost *= settings2.gender_coef;

                DCost = 1 + (KDead * 0.1f);
                float difficulty_setting_coef = HighLogic.CurrentGame.Parameters.Career.FundsLossMultiplier;
                double levelup_coef = settings2.levelup_coef;

                cost *= DCost * difficulty_setting_coef * KBulki * (1 + levelup_coef * KLevel);

                // DISCOUNTS

                //  discounts for bulk purchases
                if (KBulki >= 10)
                {
                    KDiscount += settings3.bulk_discount2 / 100;
                    KBulkDiscount2 = true;
                }
                else if (KBulki >= 5)
                {
                    KDiscount += settings3.bulk_discount1 / 100;
                    KBulkDiscount1 = true;
                }
                //  discounts: BlackMunday is day of eclipse, NewYear is last days of year
                if (Planetarium.fetch != null)
                {
                    double seconds = Planetarium.GetUniversalTime();

                    int sec_of_year = (int)seconds % KSPUtil.dateTimeFormatter.Year;
                    int day_of_year = sec_of_year / KSPUtil.dateTimeFormatter.Day + 1;
#if false
                    double eclipse_interval = 141115.4;
                    double first_eclipse = 103000;
                    
                   

                    double pass_simce_eclipse = (seconds - first_eclipse + eclipse_interval) % eclipse_interval;
                    double before_eclipse = eclipse_interval - pass_simce_eclipse;

                    double seconds_nearest_eclipse;

                    if (pass_simce_eclipse < before_eclipse)
                        seconds_nearest_eclipse = seconds - pass_simce_eclipse;
                    else
                        seconds_nearest_eclipse = seconds + before_eclipse;

                    int eclipse_sec_of_year = (int)seconds_nearest_eclipse % KSPUtil.dateTimeFormatter.Year;
                    int eclipse_day_of_year = eclipse_sec_of_year / KSPUtil.dateTimeFormatter.Day + 1;

                    //var isEclipse = Orbital.HasClearPath(FlightGlobals.GetHomeBody(), Planetarium.fetch.Sun);

                    if (day_of_year == eclipse_day_of_year)
                    {
                        Hire.Log.Info("Eclipse, eclipseToday: " + Orbital.eclipseToday);
                        KBlackMunday = true;
                        KDiscount += settings3.black_discount / 100;
                    }
                    else
                        Hire.Log.Info("Not Eclipse, eclipseToday: " + Orbital.eclipseToday);
#else
                    if (Orbital.eclipseToday)
                    {
                        KBlackMundayDiscount = true;
                        KDiscount += settings3.black_discount / 100;
                    }
#endif


                    int days_in_year = KSPUtil.dateTimeFormatter.Year / KSPUtil.dateTimeFormatter.Day;
                    // 2 days on the start and 2 days on the end of the year
                    if (days_in_year - day_of_year < 2 || day_of_year < 3)
                    {
                        KNewYearDiscount = true;
                        KDiscount += settings3.new_year_discount / 100;
                    }
                }
                if (KDiscount > settings3.max_discount / 100)
                {
                    KDiscount = settings3.max_discount / 100;
                    KDiscountOverFlow = true;
                }

                cost -= cost * KDiscount;
            }
            return Convert.ToInt32(cost);
        }

        // these slightly reduce garbage created by avoiding array allocations which is one reason OnGUI
        // is so terrible
        private static readonly GUILayoutOption[] DefaultLayoutOptions = new GUILayoutOption[0];
        private static readonly GUILayoutOption[] PortraitOptions = { GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false) };
        private static readonly GUILayoutOption[] FixedWidth = { GUILayout.Width(220f) };
        private static readonly GUILayoutOption[] HireButtonOptions = { GUILayout.ExpandHeight(true), GUILayout.MaxHeight(40f), GUILayout.MinWidth(40f) };
        private static readonly GUILayoutOption[] StatOptions = { GUILayout.MaxWidth(100f) };
        private static readonly GUILayoutOption[] FlavorTextOptions = { GUILayout.MaxWidth(200f) };

        private bool isNameConflict()
        {
            //checking for all Crew: Missing, Dead, Available, Assigned

            foreach (ProtoCrewMember kerbal in roster.Crew)
            {
                for (int i = 0; i < KBulki; i++)
                {
                    if (kerbal.name == KNames[KGender, i])
                        return true;
                }
            }
            return false;
        }


        private string hireStatus(out bool hTest)
        {

            string bText = Localizer.Format("#TRPHire_Button_Hire");
            hTest = true;
            if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER)
            {
                double kredits = Funding.Instance.Funds;
                if (costMath() > kredits)
                {
                    bText = Localizer.Format("#TRPHire_Button_NotEnoughFunds");
                    hTest = false;
                }
                else if (HighLogic.CurrentGame.CrewRoster.GetActiveCrewCount() >= GameVariables.Instance.GetActiveCrewLimit(ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.AstronautComplex)))
                {
                    bText = Localizer.Format("#TRPHire_Button_RosterFull");
                    hTest = false;
                }
                else if (isNameConflict())
                {
                    bText = Localizer.Format("#TRPHire_Button_NameConflict");
                    hTest = false;
                }
                else
                {
                    hTest = true;
                }
            }
            return bText;
        }

        private int cbulktest()
        {
            if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER)
            {
                crewWeCanHire = Mathf.Clamp(GameVariables.Instance.GetActiveCrewLimit(ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.AstronautComplex)) - HighLogic.CurrentGame.CrewRoster.GetActiveCrewCount(), 0, MAX_HIRE_COUNT);
            }
            return crewWeCanHire;
        }

        private void dCheck()
        {
            KDead = 0;
            // 10 percent for dead and 5 percent for missing, note can only have dead in some career modes.
            foreach (ProtoCrewMember kerbal in roster.Crew)
            {
                if (kerbal.rosterStatus == ProtoCrewMember.RosterStatus.Dead)
                {
                    if (kerbal.experienceTrait.Config.Name == traits.traitTitles[KCareer].name)
                    {
                        KDead += 1;
                    }
                }
                if (kerbal.rosterStatus == ProtoCrewMember.RosterStatus.Missing)
                {
                    if (kerbal.experienceTrait.Config.Name == traits.traitTitles[KCareer].name)
                    {
                        KDead += 0.5;
                    }
                }
            }
        }

        private void OnGUI()
        {
            GUI.skin = HighLogic.Skin;
            var roster = HighLogic.CurrentGame.CrewRoster;
            GUIContent[] KGendArray = new GUIContent[3] { KMale, KFemale, KGRandom };
            if (HighLogic.CurrentGame.Mode == Game.Modes.SANDBOX)
            {
                hasKredits = false;
                ACLevel = 5;
            }
            if (HighLogic.CurrentGame.Mode == Game.Modes.SCIENCE_SANDBOX)
            {
                hasKredits = false;
                ACLevel = 5;
            }
            if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER)
            {
                hasKredits = true;
                ACLevel = ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.AstronautComplex);
            }

            GUILayout.BeginArea(_areaRect);
            {
                GUILayout.Label(Localizer.Format("#TRPHire_Title"));

                // Gender selection 
                GUILayout.BeginHorizontal("box");
                KGender = GUILayout.Toolbar(KGender, KGendArray);
                GUILayout.EndHorizontal();

                // Career selection
                GUILayout.BeginVertical("box");
                KCareer = GUILayout.SelectionGrid(KCareer, traits.KCareerGrid, traits.KCareerPerRow);
                // Adding a section for 'number/bulk hire' here using the int array kBulk 
                if (cbulktest() < 1)
                {
                    GUILayout.Label(Localizer.Format("#TRPHire_BulkHireNo"));
                    KBulk = 0;
                    KBulki = 0;
                }
                else
                {
                    GUILayout.Label(Localizer.Format("#TRPHire_BulkHireSelector") + ": " + KBulki);
                    KBulk = GUILayout.HorizontalSlider(KBulk, 1, cbulktest());
                    KBulki = Convert.ToInt32(KBulk);
                }



                GUI.contentColor = basecolor;
                

                if (KBulki > 0)
                {
                    GUIStyle style = new GUIStyle(HighLogic.Skin.label);
                    style.normal.textColor = Color.white;
                    style.active.textColor = Color.white;
                    style.focused.background = null;

                    GUILayout.BeginHorizontal();

                    GUILayout.BeginVertical(GUILayout.Width(_areaRect.width * 0.48f));
                    for (int i = 0; i < KBulki; i += 2)
                    {
                        KNames[KGender, i] = GUILayout.TextField(KNames[KGender, i], style);
                    }
                    GUILayout.EndVertical();

                    if (KBulki > 1)
                    {
                        GUILayout.BeginVertical(GUILayout.Width(_areaRect.width * 0.48f));
                        for (int i = 1; i < KBulki; i += 2)
                        {
                            KNames[KGender, i] = GUILayout.TextArea(KNames[KGender, i], style);
                        }
                        GUILayout.EndVertical();
                    }

                    GUILayout.EndHorizontal();
                }

                GUILayout.EndVertical();

                // Courage Brains and BadS flag selections
                GUILayout.BeginVertical("box");

                GUILayout.BeginHorizontal();
                GUILayout.Label(Localizer.Format("#TRPHire_IsQuality"));
                KSpecifyQuality = GUILayout.Toggle(KSpecifyQuality, Localizer.Format("#TRPHire_Quality"));
                GUILayout.EndHorizontal();

                if (!KSpecifyQuality)
                    GUI.enabled = false;

                string courage_label = Courage + ":  " + (KSpecifyQuality ? KCourage.ToString("F0") :
                    (KBulki > 1 ? Localizer.Format("#TRPHire_RandomForEach") : Localizer.Format("#TRPHire_Random")));
                GUILayout.Label(courage_label);
                KCourage = GUILayout.HorizontalSlider(KCourage, 0, 100);

                string stupidity_label = Stupidity + ":  " + (KSpecifyQuality ? KStupidity.ToString("F0") :
                    (KBulki > 1 ? Localizer.Format("#TRPHire_RandomForEach") : Localizer.Format("#TRPHire_Random")));
                GUILayout.Label(stupidity_label);
                KStupidity = GUILayout.HorizontalSlider(KStupidity, 0, 100);
                GUI.enabled = true;

                GUILayout.EndVertical();

                if (KBulki > 1)
                {
                    GUI.enabled = false;
                    KFearless = false;
                    KVeteran = false;
                }
                GUILayout.BeginVertical("box");
    
                GUILayout.BeginHorizontal();
                GUILayout.Label(Localizer.Format("#TRPHire_IsFearless"));
                KFearless = GUILayout.Toggle(KFearless, Badass);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label(Localizer.Format("#TRPHire_IsVeteran"));
                KVeteran = GUILayout.Toggle(KVeteran, Veteran);
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();

                GUI.enabled = true;
                // Level selection
                GUILayout.BeginVertical("box");
                GUILayout.Label(Localizer.Format("#TRPHire_SelectLevel"));

                // If statements for level options
                if (kerExp == false)
                {
                    if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER)
                        GUILayout.Label(Localizer.Format("#TRPHire_Level5CareerNoExp"));
                    else
                        GUILayout.Label(Localizer.Format("#TRPHire_Level5SandboxOrScience"));
                    KLevel = 5;
                }
                else
                {
                    if (ACLevel == 0) { KLevel = GUILayout.Toolbar(KLevel, KLevelStringsZero); }
                    else if (ACLevel == 0.5) { KLevel = GUILayout.Toolbar(KLevel, KLevelStringsOne); }
                    else if (ACLevel == 1) { KLevel = GUILayout.Toolbar(KLevel, KLevelStringsTwo); }
                    else { KLevel = GUILayout.Toolbar(KLevel, KLevelStringsAll); }
                }
                GUILayout.EndVertical();

                if (hasKredits == true)
                {
                    GUILayout.BeginHorizontal("window");
                    GUILayout.BeginVertical();
                    int cost = costMath();
                    //GUILayout.FlexibleSpace();
                    string rock = "";
                    if (Orbital.eclipseToday != null)
                    {
                        rock = Orbital.eclipseToday.displayName.Split().Last();
                        rock = Localizer.Format("<<1>>", rock);
                    }

                    string msg = "";

                    if (KDiscount != 0)
                    {
                        string MaxDiscountText = Localizer.Format("#TRPHire_MaxDiscount", settings3.max_discount) + "\n";
                        string NewYearText = Localizer.Format("#TRPHire_NewYear", settings3.new_year_discount) + "\n";
                        string BlackMundayText = Localizer.Format("#TRPHire_BlackMunday", rock, settings3.black_discount) + "\n";
                        string BulkDiscountText1 = Localizer.Format("#TRPHire_BulkDiscount", settings3.bulk_discount1) + "\n";
                        string BulkDiscountText2 = Localizer.Format("#TRPHire_BulkDiscount", settings3.bulk_discount2) + "\n";
                        string YourDiscountText = Localizer.Format("#TRPHire_YourDiscount", KDiscount * 100) + "\n";

                        int discountCount = 0;

                        if (KNewYearDiscount)     { msg += NewYearText;      discountCount++; }
                        if (KBlackMundayDiscount) { msg += BlackMundayText;  discountCount++; }
                        if (KBulkDiscount1)        { msg += BulkDiscountText1; discountCount++; }
                        if (KBulkDiscount2) { msg += BulkDiscountText2; discountCount++; }

                        if (KDiscountOverFlow)
                            msg += MaxDiscountText;

                        if (discountCount > 1)
                            msg += YourDiscountText;
                    }

                    string TotalCostText = Localizer.Format("#TRPHire_TotalCost", cost);
                    msg += TotalCostText;


                    if (cost <= Funding.Instance.Funds)
                    {
                        GUILayout.Label(msg, HighLogic.Skin.textField);
                    }
                    else
                    {
                        GUI.color = Color.red;
                        GUILayout.Label(msg, HighLogic.Skin.textField);
                        GUI.color = basecolor;
                    }

                    // GUILayout.FlexibleSpace();
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (hTest)
                {
                    if (GUILayout.Button(hireStatus(out hTest), GUILayout.Width(200f)))
                    {
                        if (hTest)
                            kHire();
                    }
                }
                if (!hTest)
                {
                    GUILayout.Button(hireStatus(out hTest), GUILayout.Width(250f));
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.EndArea();
            }
        }
        void Update()
        {
            AstronautComplex ac = GameObject.FindObjectOfType<AstronautComplex>();
            if (ac != null)
            {
                if (ac.ScrollListApplicants.Count > 0)
                {
                    Hire.Log.Info("TRP: Clearing Applicant List");
                    ac.ScrollListApplicants.Clear(true);
                }
            }
        }
    }
}
