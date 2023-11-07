using Menu.Remix.MixedUI;
using UnityEngine;

/*
namespace Pearlventory
{
    public class PearlventoryOptionsMenu : OptionInterface
    {
        public PearlventoryOptionsMenu(PearlventoryMain plugin)
        {
            MaxPearlsSlider = this.config.Bind<int>("Pearlventory_Int_MaxPearlsSlider", 1);
        }
        public override void Initialize()
        {
            var opTab1 = new OpTab(this, "Pearlventory");
            this.Tabs = new[] { opTab1}; // Add the tabs into your list of tabs. If there is only a single tab, it will not show the flap on the side because there is not need to.

            // Tab 1
            OpContainer tab1Container = new OpContainer(new Vector2(0, 0));
            opTab1.AddItems(tab1Container);


            UIelement[] UIArrayElements = new UIelement[] // Labels in a fixed box size + alignment
            {
                new OpSlider(MaxPearlsSlider, new Vector2(10, 10), 100, false),

                //new OpCheckBox(doubleScugpupCheckBox, 50, 300),
            };
            opTab1.AddItems(UIArrayElements);

            UIelement[] UIArrayElements2 = new UIelement[] //create an array of ui elements
            {
                new OpLabel(0f, 550f, "Pearlventory options", true),

                new OpLabel(80f, 500f, "How many pearls do you want to be able to hold?", false)
                //new OpLabel(80f, 300f, "Enable dual-wielding for Scugpups(or something)", false),
            };
            opTab1.AddItems(UIArrayElements2);
        }
        public override void Update()
        {
            base.Update();
            sprite1.rotation++;
            sprite2.rotation++;
            numberGoUp++;
            //AwriBlush.rotation = Mathf.Sin(numberGoUp / 10) * 15;
        }

        // Configurable values. They are bound to the config in constructor, and then passed to UI elements.
        // They will contain values set in the menu. And to fetch them in your code use their NAME.Value. For example to get the boolean testCheckBox.Value, to get the integer testSlider.Value
        //public readonly Configurable<TYPE> NAME;        
        public static Configurable<int> MaxPearlsSlider;

    }
}
*/

namespace Pearlventory;

//from https://github.com/Dual-Iron/osha-compliant-shelters/blob/master/src/Options.cs
sealed class PearlventoryOptions : OptionInterface
{
    public static Configurable<int> MaxPearls;
    public static Configurable<bool> GainFood;
    public static Configurable<int> FoodGained;
    //public static Configurable<KeyCode> IntakePearlKey;
    //public static Configurable<KeyCode> OuttakePearlKey;

    public PearlventoryOptions()
    {
        MaxPearls = config.Bind("PearlLimit", 10, new ConfigAcceptableRange<int>(1, 100));
        //IntakePearlKey = config.Bind<KeyCode>("IntakePearlKey", KeyCode.LeftShift);
        //OuttakePearlKey = config.Bind<KeyCode>("OuttakePearlKey", KeyCode.LeftControl);
        GainFood = config.Bind("GainFood", true);
        FoodGained = config.Bind("FoodGained", 3, new ConfigAcceptableRange<int>(1, 30));
    }

    //OpCheckBox GainFood;
    //OpCheckBox coopHoldDown;

    public override void Initialize()
    {
        base.Initialize();

        Tabs = new OpTab[] { new OpTab(this) };

        var modname = new OpLabel(20, 600 - 40, "Pearlventory", true);
        var modauthor = new OpLabel(50, 600 - 40, "by Nuclear Pasta", false);
        var github = new OpLabel(20, 600 - 40 - 40, "https://github.com/voidwyrm-2/Pearlventory");

        float y = 340;

        var a = new OpLabel(new(20, y), Vector2.zero, "The storage can hold this many pearls", FLabelAlignment.Left);
        var a2 = new OpSlider(MaxPearls, new Vector2(24, y - 48), 300, vertical: false);

        var b = new OpLabel(new(50, y), Vector2.zero, "Check this to be able to gain food from pearls", FLabelAlignment.Left);
        var b2 = new OpCheckBox(GainFood, new(40, y - 2));

        var c = new OpLabel(new(70, y), Vector2.zero, "The storage can hold this many pearls", FLabelAlignment.Left);
        var c2 = new OpSlider(FoodGained, new Vector2(80, y - 48), 300, vertical: false);

        //var b = new OpLabel(new(50, y), Vector2.zero, "Button for storing pearls", FLabelAlignment.Left);
        //var b2 = new OpKeyBinder(IntakePearlKey, new Vector2(10f, 420f), new Vector2(150f, 30f), true, OpKeyBinder.BindController.AnyController); //from https://github.com/NoirCatto/NoirCatto/blob/master/NoirCattoOptions.cs

        //var c = new OpLabel(new(70, y), Vector2.zero, "Button for un-storing pearls", FLabelAlignment.Left);
        //var c2 = new OpKeyBinder(OuttakePearlKey, new Vector2(10f, 420f), new Vector2(150f, 30f), true, OpKeyBinder.BindController.AnyController); //from https://github.com/NoirCatto/NoirCatto/blob/master/NoirCattoOptions.cs

        Tabs[0].AddItems(modname, modauthor, github, a, a2, b, b2, c, c2);
    }

    /*
    public override void Update()
    {
        base.Update();

        if (holdDown != null)
        {
            bool greyed = holdDown.value != "true";

            coopHoldDown.greyedOut = greyed;
        }
    }
    */
}